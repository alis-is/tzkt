﻿using System;
using System.Text.Json;
using System.Threading.Tasks;

using Tzkt.Data.Models;
using Tzkt.Data.Models.Base;

namespace Tzkt.Sync.Protocols.Proto13
{
    class TxRollupDispatchTicketsCommit : ProtocolCommit
    {
        public TxRollupDispatchTicketsOperation Operation { get; private set; }

        public TxRollupDispatchTicketsCommit(ProtocolHandler protocol) : base(protocol) { }

        public virtual async Task Apply(Block block, JsonElement op, JsonElement content)
        {
            #region init
            var sender = await Cache.Accounts.GetAsync(content.RequiredString("source"));
            var rollup = await Cache.Accounts.GetAsync(content.RequiredString("tx_rollup"));

            var result = content.Required("metadata").Required("operation_result");

            var operation = new TxRollupDispatchTicketsOperation
            {
                Id = Cache.AppState.NextOperationId(),
                Block = block,
                Level = block.Level,
                Timestamp = block.Timestamp,
                OpHash = op.RequiredString("hash"),
                BakerFee = content.RequiredInt64("fee"),
                Counter = content.RequiredInt32("counter"),
                GasLimit = content.RequiredInt32("gas_limit"),
                StorageLimit = content.RequiredInt32("storage_limit"),
                Sender = sender,
                RollupId = rollup?.Id,
                Status = result.RequiredString("status") switch
                {
                    "applied" => OperationStatus.Applied,
                    "backtracked" => OperationStatus.Backtracked,
                    "failed" => OperationStatus.Failed,
                    "skipped" => OperationStatus.Skipped,
                    _ => throw new NotImplementedException()
                },
                Errors = result.TryGetProperty("errors", out var errors)
                    ? OperationErrors.Parse(content, errors)
                    : null,
                GasUsed = result.OptionalInt32("consumed_gas") ?? 0,
                StorageUsed = result.OptionalInt32("paid_storage_size_diff") ?? 0,
                StorageFee = result.OptionalInt32("paid_storage_size_diff") > 0
                    ? result.OptionalInt32("paid_storage_size_diff") * block.Protocol.ByteCost
                    : null,
                AllocationFee = null
            };
            #endregion

            #region entities
            var blockBaker = block.Proposer;
            var senderDelegate = Cache.Accounts.GetDelegate(sender.DelegateId) ?? sender as Data.Models.Delegate;

            Db.TryAttach(blockBaker);
            Db.TryAttach(sender);
            Db.TryAttach(senderDelegate);
            Db.TryAttach(rollup);
            #endregion

            #region apply operation
            await Spend(sender, operation.BakerFee);
            if (senderDelegate != null)
            {
                senderDelegate.StakingBalance -= operation.BakerFee;
                if (senderDelegate.Id != sender.Id)
                    senderDelegate.DelegatedBalance -= operation.BakerFee;
            }
            blockBaker.Balance += operation.BakerFee;
            blockBaker.StakingBalance += operation.BakerFee;

            sender.TxRollupDispatchTicketsCount++;
            if (rollup != null) rollup.TxRollupDispatchTicketsCount++;

            block.Operations |= Operations.TxRollupDispatchTickets;
            block.Fees += operation.BakerFee;

            sender.Counter = Math.Max(sender.Counter, operation.Counter);

            Cache.AppState.Get().TxRollupDispatchTicketsOpsCount++;
            #endregion

            #region apply result
            if (operation.Status == OperationStatus.Applied)
            {
                await Spend(sender,
                    (operation.StorageFee ?? 0));

                if (senderDelegate != null)
                {
                    senderDelegate.StakingBalance -= operation.StorageFee ?? 0;
                    if (senderDelegate.Id != sender.Id)
                    {
                        senderDelegate.DelegatedBalance -= operation.StorageFee ?? 0;
                    }
                }
            }
            #endregion

            Db.TxRollupDispatchTicketsOps.Add(operation);
            Operation = operation;
        }

        public virtual async Task Revert(Block block, TxRollupDispatchTicketsOperation operation)
        {
            #region init
            operation.Block ??= block;
            operation.Block.Protocol ??= await Cache.Protocols.GetAsync(block.ProtoCode);
            operation.Block.Proposer ??= Cache.Accounts.GetDelegate(block.ProposerId);

            operation.Sender = await Cache.Accounts.GetAsync(operation.SenderId);
            operation.Sender.Delegate ??= Cache.Accounts.GetDelegate(operation.Sender.DelegateId);
            #endregion

            #region entities
            var blockBaker = block.Proposer;
            var sender = operation.Sender;
            var senderDelegate = sender.Delegate ?? sender as Data.Models.Delegate;
            var rollup = await Cache.Accounts.GetAsync(operation.RollupId);

            Db.TryAttach(blockBaker);
            Db.TryAttach(sender);
            Db.TryAttach(senderDelegate);
            Db.TryAttach(rollup);
            #endregion

            #region revert result
            if (operation.Status == OperationStatus.Applied)
            {
                await Return(sender,
                    (operation.StorageFee ?? 0));

                if (senderDelegate != null)
                {
                    senderDelegate.StakingBalance += operation.StorageFee ?? 0;
                    if (senderDelegate.Id != sender.Id)
                    {
                        senderDelegate.DelegatedBalance += operation.StorageFee ?? 0;
                    }
                }
            }
            #endregion

            #region revert operation
            await Return(sender, operation.BakerFee);
            if (senderDelegate != null)
            {
                senderDelegate.StakingBalance += operation.BakerFee;
                if (senderDelegate.Id != sender.Id)
                    senderDelegate.DelegatedBalance += operation.BakerFee;
            }
            blockBaker.Balance -= operation.BakerFee;
            blockBaker.StakingBalance -= operation.BakerFee;

            sender.TxRollupDispatchTicketsCount--;
            if (rollup != null) rollup.TxRollupDispatchTicketsCount--;

            sender.Counter = Math.Min(sender.Counter, operation.Counter - 1);

            Cache.AppState.Get().TxRollupDispatchTicketsOpsCount--;
            #endregion

            Db.TxRollupDispatchTicketsOps.Remove(operation);
            Cache.AppState.ReleaseManagerCounter();
        }
    }
}
