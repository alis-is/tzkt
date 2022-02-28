﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tzkt.Data.Models;

namespace Tzkt.Sync.Protocols.Proto2
{
    class RevelationPenaltyCommit : ProtocolCommit
    {
        public RevelationPenaltyCommit(ProtocolHandler protocol) : base(protocol) { }

        public virtual async Task Apply(Block block, JsonElement rawBlock)
        {
            #region init
            List<RevelationPenaltyOperation> revelationPenalties = null;

            if (block.Events.HasFlag(BlockEvents.CycleEnd))
            {
                if (HasPenaltiesUpdates(block, rawBlock))
                {
                    revelationPenalties = new List<RevelationPenaltyOperation>();

                    var missedBlocks = await Db.Blocks
                        .Include(x => x.Baker)
                        .Include(x => x.Protocol)
                        .Where(x => x.Level % x.Protocol.BlocksPerCommitment == 0 &&
                            x.Cycle == block.Cycle - 1 &&
                            x.RevelationId == null)
                        .ToListAsync();

                    var penalizedBakers = missedBlocks
                        .Select(x => x.BakerId)
                        .ToHashSet();

                    var bakerCycles = await Db.BakerCycles.AsNoTracking()
                        .Where(x => x.Cycle == block.Cycle - 1 && penalizedBakers.Contains(x.BakerId))
                        .ToListAsync();

                    var slashedBakers = bakerCycles
                        .Where(x => x.EndorsementDeposits > 0 || x.BlockDeposits > 0)
                        .Where(x => (x.BlockDeposits + x.EndorsementDeposits +
                            x.BlockRewards + x.EndorsementRewards + x.BlockFees +
                            x.DoubleBakingRewards + x.DoubleEndorsingRewards + x.RevelationRewards -
                            x.DoubleBakingLosses - x.DoubleEndorsingLosses - x.RevelationLosses) == 0)
                        .Select(x => x.BakerId)
                        .ToHashSet();

                    foreach (var missedBlock in missedBlocks)
                    {
                        Cache.Accounts.Add(missedBlock.Baker);
                        var slashed = slashedBakers.Contains((int)missedBlock.BakerId);
                        revelationPenalties.Add(new RevelationPenaltyOperation
                        {
                            Id = Cache.AppState.NextOperationId(),
                            Baker = missedBlock.Baker,
                            Block = block,
                            Level = block.Level,
                            Timestamp = block.Timestamp,
                            MissedLevel = missedBlock.Level,
                            Loss = slashed ? 0 : missedBlock.Reward + missedBlock.Fees
                        });
                    }
                }
            }
            #endregion

            if (revelationPenalties == null) return;

            foreach (var penalty in revelationPenalties)
            {
                #region entities
                //var block = penalty.Block;
                var delegat = penalty.Baker;

                Db.TryAttach(delegat);
                #endregion

                delegat.Balance -= penalty.Loss;
                delegat.StakingBalance -= penalty.Loss > 0
                    ? (await Cache.Blocks.GetAsync(penalty.MissedLevel)).Fees
                    : 0;

                delegat.RevelationPenaltiesCount++;
                block.Operations |= Operations.RevelationPenalty;

                Db.RevelationPenaltyOps.Add(penalty);
            }
        }

        public virtual async Task Revert(Block block)
        {
            #region init
            List<RevelationPenaltyOperation> revelationPanlties = null;

            if (block.RevelationPenalties?.Count > 0)
            {
                revelationPanlties = block.RevelationPenalties;
                foreach (var penalty in revelationPanlties)
                {
                    penalty.Block ??= block;
                    penalty.Baker ??= Cache.Accounts.GetDelegate(penalty.BakerId);
                }
            }
            #endregion

            if (revelationPanlties == null) return;

            foreach (var penalty in revelationPanlties)
            {
                #region entities
                //var block = penalty.Block;
                var delegat = penalty.Baker;

                Db.TryAttach(delegat);
                #endregion

                delegat.Balance += penalty.Loss;
                delegat.StakingBalance += penalty.Loss > 0
                    ? (await Cache.Blocks.GetAsync(penalty.MissedLevel)).Fees
                    : 0;

                delegat.RevelationPenaltiesCount--;

                Db.RevelationPenaltyOps.Remove(penalty);
            }
        }

        protected virtual int GetFreezerCycle(JsonElement el) => el.RequiredInt32("level");

        protected virtual bool HasPenaltiesUpdates(Block block, JsonElement rawBlock)
        {
            return rawBlock
                .Required("metadata")
                .RequiredArray("balance_updates")
                .EnumerateArray()
                .Any(x => x.RequiredString("kind")[0] == 'f' &&
                          x.RequiredInt64("change") < 0 &&
                          GetFreezerCycle(x) != block.Cycle - block.Protocol.PreservedCycles);
        }
    }
}
