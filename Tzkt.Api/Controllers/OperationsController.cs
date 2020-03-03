﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Tzkt.Api.Models;
using Tzkt.Api.Repositories;
using Tzkt.Api.Services.Cache;

namespace Tzkt.Api.Controllers
{
    [ApiController]
    [Route("v1/operations")]
    public class OperationsController : ControllerBase
    {
        private readonly AccountsCache Accounts;
        private readonly OperationRepository Operations;

        public OperationsController(AccountsCache accounts, OperationRepository operations)
        {
            Accounts = accounts;
            Operations = operations;
        }

        #region operations
        /// <summary>
        /// Get operations by hash
        /// </summary>
        /// <remarks>
        /// Returns a list of operations with the specified hash.
        /// </remarks>
        /// <param name="hash">Operation hash</param>
        /// <returns></returns>
        [HttpGet("{hash}")]
        public Task<IEnumerable<Operation>> GetByHash([OpHash] string hash)
        {
            return Operations.Get(hash);
        }

        /// <summary>
        /// Get operations by hash and counter
        /// </summary>
        /// <remarks>
        /// Returns a list of operations with the specified hash and counter.
        /// </remarks>
        /// <param name="hash">Operation hash</param>
        /// <param name="counter">Operation counter</param>
        /// <returns></returns>
        [HttpGet("{hash}/{counter}")]
        public Task<IEnumerable<Operation>> GetByHashCounter([OpHash] string hash, [Min(0)] int counter)
        {
            return Operations.Get(hash, counter);
        }

        /// <summary>
        /// Get operations by hash, counter and nonce
        /// </summary>
        /// <remarks>
        /// Returns an internal operations with the specified hash, counter and nonce.
        /// </remarks>
        /// <param name="hash">Operation hash</param>
        /// <param name="counter">Operation counter</param>
        /// <param name="nonce">Operation nonce (internal)</param>
        /// <returns></returns>
        [HttpGet("{hash}/{counter}/{nonce}")]
        public Task<IEnumerable<Operation>> GetByHashCounterNonce([OpHash] string hash, [Min(0)] int counter, [Min(0)] int nonce)
        {
            return Operations.Get(hash, counter, nonce);
        }
        #endregion

        #region endorsements
        /// <summary>
        /// Get endorsements
        /// </summary>
        /// <remarks>
        /// Returns a list of endorsement operations.
        /// </remarks>
        /// <param name="p">Page offset (pagination)</param>
        /// <param name="n">Number of items to return</param>
        /// <returns></returns>
        [HttpGet("endorsements")]
        public Task<IEnumerable<EndorsementOperation>> GetEndorsements([Min(0)] int p = 0, [Range(0, 1000)] int n = 100)
        {
            return Operations.GetEndorsements(n, p * n);
        }

        /// <summary>
        /// Get endorsement by hash
        /// </summary>
        /// <remarks>
        /// Returns an endorsement operation with specified hash.
        /// </remarks>
        /// <param name="hash">Operation hash</param>
        /// <returns></returns>
        [HttpGet("endorsements/{hash}")]
        public Task<IEnumerable<EndorsementOperation>> GetEndorsementByHash([OpHash] string hash)
        {
            return Operations.GetEndorsements(hash);
        }

        /// <summary>
        /// Get endorsements count
        /// </summary>
        /// <remarks>
        /// Returns the total number of endorsement operations.
        /// </remarks>
        /// <returns></returns>
        [HttpGet("endorsements/count")]
        public Task<int> GetEndorsementsCount()
        {
            return Operations.GetEndorsementsCount();
        }
        #endregion

        #region proposals
        /// <summary>
        /// Get proposals
        /// </summary>
        /// <remarks>
        /// Returns a list of proposal operations.
        /// </remarks>
        /// <param name="p">Page offset (pagination)</param>
        /// <param name="n">Number of items to return</param>
        /// <returns></returns>
        [HttpGet("proposals")]
        public Task<IEnumerable<ProposalOperation>> GetProposals([Min(0)] int p = 0, [Range(0, 1000)] int n = 100)
        {
            return Operations.GetProposals(n, p * n);
        }

        /// <summary>
        /// Get proposal by hash
        /// </summary>
        /// <remarks>
        /// Returns a proposal operation with specified hash.
        /// </remarks>
        /// <param name="hash">Operation hash</param>
        /// <returns></returns>
        [HttpGet("proposals/{hash}")]
        public Task<IEnumerable<ProposalOperation>> GetProposalByHash([OpHash] string hash)
        {
            return Operations.GetProposals(hash);
        }

        /// <summary>
        /// Get proposals count
        /// </summary>
        /// <remarks>
        /// Returns the total number of proposal operations.
        /// </remarks>
        /// <returns></returns>
        [HttpGet("proposals/count")]
        public Task<int> GetProposalsCount()
        {
            return Operations.GetProposalsCount();
        }
        #endregion

        #region ballots
        /// <summary>
        /// Get ballots
        /// </summary>
        /// <remarks>
        /// Returns a list of ballot operations.
        /// </remarks>
        /// <param name="p">Page offset (pagination)</param>
        /// <param name="n">Number of items to return</param>
        /// <returns></returns>
        [HttpGet("ballots")]
        public Task<IEnumerable<BallotOperation>> GetBallots([Min(0)] int p = 0, [Range(0, 1000)] int n = 100)
        {
            return Operations.GetBallots(n, p * n);
        }

        /// <summary>
        /// Get ballot by hash
        /// </summary>
        /// <remarks>
        /// Returns a ballot operation with specified hash.
        /// </remarks>
        /// <param name="hash">Operation hash</param>
        /// <returns></returns>
        [HttpGet("ballots/{hash}")]
        public Task<IEnumerable<BallotOperation>> GetBallotByHash([OpHash] string hash)
        {
            return Operations.GetBallots(hash);
        }

        /// <summary>
        /// Get ballots count
        /// </summary>
        /// <remarks>
        /// Returns the total number of ballot operations.
        /// </remarks>
        /// <returns></returns>
        [HttpGet("ballots/count")]
        public Task<int> GetBallotsCount()
        {
            return Operations.GetBallotsCount();
        }
        #endregion

        #region activations
        /// <summary>
        /// Get activations
        /// </summary>
        /// <remarks>
        /// Returns a list of activation operations.
        /// </remarks>
        /// <param name="p">Page offset (pagination)</param>
        /// <param name="n">Number of items to return</param>
        /// <returns></returns>
        [HttpGet("activations")]
        public Task<IEnumerable<ActivationOperation>> GetActivations([Min(0)] int p = 0, [Range(0, 1000)] int n = 100)
        {
            return Operations.GetActivations(n, p * n);
        }

        /// <summary>
        /// Get activation by hash
        /// </summary>
        /// <remarks>
        /// Returns an activation operation with specified hash.
        /// </remarks>
        /// <param name="hash">Operation hash</param>
        /// <returns></returns>
        [HttpGet("activations/{hash}")]
        public Task<IEnumerable<ActivationOperation>> GetActivationByHash([OpHash] string hash)
        {
            return Operations.GetActivations(hash);
        }

        /// <summary>
        /// Get activations count
        /// </summary>
        /// <remarks>
        /// Returns the total number of activation operations.
        /// </remarks>
        /// <returns></returns>
        [HttpGet("activations/count")]
        public Task<int> GetActivationsCount()
        {
            return Operations.GetActivationsCount();
        }
        #endregion

        #region double baking
        /// <summary>
        /// Get double baking
        /// </summary>
        /// <remarks>
        /// Returns a list of double baking operations.
        /// </remarks>
        /// <param name="p">Page offset (pagination)</param>
        /// <param name="n">Number of items to return</param>
        /// <returns></returns>
        [HttpGet("double_baking")]
        public Task<IEnumerable<DoubleBakingOperation>> GetDoubleBaking([Min(0)] int p = 0, [Range(0, 1000)] int n = 100)
        {
            return Operations.GetDoubleBakings(n, p * n);
        }

        /// <summary>
        /// Get double baking by hash
        /// </summary>
        /// <remarks>
        /// Returns a double baking operation with specified hash.
        /// </remarks>
        /// <param name="hash">Operation hash</param>
        /// <returns></returns>
        [HttpGet("double_baking/{hash}")]
        public Task<IEnumerable<DoubleBakingOperation>> GetDoubleBakingByHash([OpHash] string hash)
        {
            return Operations.GetDoubleBakings(hash);
        }

        /// <summary>
        /// Get double baking count
        /// </summary>
        /// <remarks>
        /// Returns the total number of double baking operations.
        /// </remarks>
        /// <returns></returns>
        [HttpGet("double_baking/count")]
        public Task<int> GetDoubleBakingCount()
        {
            return Operations.GetDoubleBakingsCount();
        }
        #endregion

        #region double endorsing
        /// <summary>
        /// Get double endorsing
        /// </summary>
        /// <remarks>
        /// Returns a list of double endorsing operations.
        /// </remarks>
        /// <param name="p">Page offset (pagination)</param>
        /// <param name="n">Number of items to return</param>
        /// <returns></returns>
        [HttpGet("double_endorsing")]
        public Task<IEnumerable<DoubleEndorsingOperation>> GetDoubleEndorsing([Min(0)] int p = 0, [Range(0, 1000)] int n = 100)
        {
            return Operations.GetDoubleEndorsings(n, p * n);
        }

        /// <summary>
        /// Get double endorsing by hash
        /// </summary>
        /// <remarks>
        /// Returns a double endorsing operation with specified hash.
        /// </remarks>
        /// <param name="hash">Operation hash</param>
        /// <returns></returns>
        [HttpGet("double_endorsing/{hash}")]
        public Task<IEnumerable<DoubleEndorsingOperation>> GetDoubleEndorsingByHash([OpHash] string hash)
        {
            return Operations.GetDoubleEndorsings(hash);
        }

        /// <summary>
        /// Get double endorsing count
        /// </summary>
        /// <remarks>
        /// Returns the total number of double endorsing operations.
        /// </remarks>
        /// <returns></returns>
        [HttpGet("double_endorsing/count")]
        public Task<int> GetDoubleEndorsingCount()
        {
            return Operations.GetDoubleEndorsingsCount();
        }
        #endregion

        #region nonce revelations
        /// <summary>
        /// Get nonce revelations
        /// </summary>
        /// <remarks>
        /// Returns a list of seed nonce revelation operations.
        /// </remarks>
        /// <param name="p">Page offset (pagination)</param>
        /// <param name="n">Number of items to return</param>
        /// <returns></returns>
        [HttpGet("nonce_revelations")]
        public Task<IEnumerable<NonceRevelationOperation>> GetNonceRevelations([Min(0)] int p = 0, [Range(0, 1000)] int n = 100)
        {
            return Operations.GetNonceRevelations(n, p * n);
        }

        /// <summary>
        /// Get nonce revelation by hash
        /// </summary>
        /// <remarks>
        /// Returns a seed nonce revelation operation with specified hash.
        /// </remarks>
        /// <param name="hash">Operation hash</param>
        /// <returns></returns>
        [HttpGet("nonce_revelations/{hash}")]
        public Task<IEnumerable<NonceRevelationOperation>> GetNonceRevelationByHash([OpHash] string hash)
        {
            return Operations.GetNonceRevelations(hash);
        }

        /// <summary>
        /// Get nonce revelations count
        /// </summary>
        /// <remarks>
        /// Returns the total number of seed nonce revelation operations.
        /// </remarks>
        /// <returns></returns>
        [HttpGet("nonce_revelations/count")]
        public Task<int> GetNonceRevelationsCount()
        {
            return Operations.GetNonceRevelationsCount();
        }
        #endregion

        #region delegations
        /// <summary>
        /// Get delegations
        /// </summary>
        /// <remarks>
        /// Returns a list of delegation operations.
        /// </remarks>
        /// <param name="p">Page offset (pagination)</param>
        /// <param name="n">Number of items to return</param>
        /// <returns></returns>
        [HttpGet("delegations")]
        public Task<IEnumerable<DelegationOperation>> GetDelegations([Min(0)] int p = 0, [Range(0, 1000)] int n = 100)
        {
            return Operations.GetDelegations(n, p * n);
        }

        /// <summary>
        /// Get delegation by hash
        /// </summary>
        /// <remarks>
        /// Returns a delegation operation with specified hash.
        /// </remarks>
        /// <param name="hash">Operation hash</param>
        /// <returns></returns>
        [HttpGet("delegations/{hash}")]
        public Task<IEnumerable<DelegationOperation>> GetDelegationByHash([OpHash] string hash)
        {
            return Operations.GetDelegations(hash);
        }

        /// <summary>
        /// Get delegations count
        /// </summary>
        /// <remarks>
        /// Returns the total number of delegation operations.
        /// </remarks>
        /// <returns></returns>
        [HttpGet("delegations/count")]
        public Task<int> GetDelegationsCount()
        {
            return Operations.GetDelegationsCount();
        }
        #endregion

        #region originations
        /// <summary>
        /// Get originations
        /// </summary>
        /// <remarks>
        /// Returns a list of origination operations.
        /// </remarks>
        /// <param name="p">Page offset (pagination)</param>
        /// <param name="n">Number of items to return</param>
        /// <returns></returns>
        [HttpGet("originations")]
        public Task<IEnumerable<OriginationOperation>> GetOriginations([Min(0)] int p = 0, [Range(0, 1000)] int n = 100)
        {
            return Operations.GetOriginations(n, p * n);
        }

        /// <summary>
        /// Get origination by hash
        /// </summary>
        /// <remarks>
        /// Returns a origination operation with specified hash.
        /// </remarks>
        /// <param name="hash">Operation hash</param>
        /// <returns></returns>
        [HttpGet("originations/{hash}")]
        public Task<IEnumerable<OriginationOperation>> GetOriginationByHash([OpHash] string hash)
        {
            return Operations.GetOriginations(hash);
        }

        /// <summary>
        /// Get originations count
        /// </summary>
        /// <remarks>
        /// Returns the total number of origination operations.
        /// </remarks>
        /// <returns></returns>
        [HttpGet("originations/count")]
        public Task<int> GetOriginationsCount()
        {
            return Operations.GetOriginationsCount();
        }
        #endregion

        #region transactions
        /// <summary>
        /// Get transactions
        /// </summary>
        /// <remarks>
        /// Returns a list of transaction operations.
        /// </remarks>
        /// <param name="initiator">Filters transactions by initiator. You can provide single address or comma-separated list of addresses or one of the keywords `null`, `target`.</param>
        /// <param name="sender">Filters transactions by sender. You can provide single address or comma-separated list of addresses or one of the keywords `null`, `target`.</param>
        /// <param name="target">Filters transactions by target. You can provide single address or comma-separated list of addresses or one of the keywords `null`, `sender`, `initiator`.</param>
        /// <param name="parameters">Filters transactions by parameters value. You can use wildcard `*` for pattern matching (e.g. `123*` means "starts with 123", or `*123*` means "contains 123"). To avoid pattern matching use `\*`. Supported keywords: `null`.</param>
        /// <param name="sort">Sorts transactions by specified field. Supported fields: `id`, `level`, `timestamp`, `amount`. Add `.desc` to sort by descending (e.g. `sort=amount.desc`).</param>
        /// <param name="p">Page offset (pagination)</param>
        /// <param name="n">Number of items to return</param>
        /// <returns></returns>
        [HttpGet("transactions")]
        public async Task<ActionResult<IEnumerable<TransactionOperation>>> GetTransactions(
            string initiator,
            string sender,
            string target,
            string parameters,
            string sort,
            [Min(0)] int p = 0,
            [Range(0, 1000)] int n = 100)
        {
            AccountParameter _initiator = null;
            AccountParameter _sender = null;
            AccountParameter _target = null;
            StringParameter _parameters = null;
            SortParameter _sort = null;

            #region validate
            if (initiator != null)
            {
                _initiator = await AccountParameter.Parse(initiator, Accounts, "target");
                if (_initiator.Invalid)
                    return new BadRequest(nameof(initiator), _initiator.Error);
            }

            if (sender != null)
            {
                _sender = await AccountParameter.Parse(sender, Accounts, "target");
                if (_sender.Invalid)
                    return new BadRequest(nameof(sender), _sender.Error);
            }

            if (target != null)
            {
                _target = await AccountParameter.Parse(target, Accounts, "sender", "initiator");
                if (_target.Invalid)
                    return new BadRequest(nameof(target), _target.Error);
            }

            if (parameters != null)
            {
                _parameters = await StringParameter.Parse(parameters);
                //if (_parameters.Invalid)
                //    return new BadRequest(nameof(parameters), _parameters.Error);
            }

            if (sort != null)
            {
                _sort = await SortParameter.Parse(sort, "id", "level", "timestamp", "amount");
                if (_sort?.Invalid == true)
                    return new BadRequest(nameof(sort), _sort.Error);
            }
            #endregion

            #region check dead filters
            if (_initiator?.Mode == QueryMode.Dead)
                return Ok(Enumerable.Empty<TransactionOperation>());

            if (_sender?.Mode == QueryMode.Dead)
                return Ok(Enumerable.Empty<TransactionOperation>());
            
            if (_target?.Mode == QueryMode.Dead)
                return Ok(Enumerable.Empty<TransactionOperation>());
            #endregion

            return Ok(await Operations.GetTransactions(_initiator, _sender, _target, _parameters, _sort,  n, p * n));
        }

        /// <summary>
        /// Get transaction by hash
        /// </summary>
        /// <remarks>
        /// Returns transaction operations with specified hash.
        /// </remarks>
        /// <param name="hash">Operation hash</param>
        /// <returns></returns>
        [HttpGet("transactions/{hash}")]
        public Task<IEnumerable<TransactionOperation>> GetTransactionByHash([OpHash] string hash)
        {
            return Operations.GetTransactions(hash);
        }

        /// <summary>
        /// Get transaction by hash and counter
        /// </summary>
        /// <remarks>
        /// Returns transaction operations with specified hash and counter.
        /// </remarks>
        /// <param name="hash">Operation hash</param>
        /// <param name="counter">Operation counter</param>
        /// <returns></returns>
        [HttpGet("transactions/{hash}/{counter}")]
        public Task<IEnumerable<TransactionOperation>> GetTransactionByHashCounter([OpHash] string hash, [Min(0)] int counter)
        {
            return Operations.GetTransactions(hash, counter);
        }

        /// <summary>
        /// Get transaction by hash, counter and nonce
        /// </summary>
        /// <remarks>
        /// Returns an internal transaction operation with specified hash, counter and nonce.
        /// </remarks>
        /// <param name="hash">Operation hash</param>
        /// <param name="counter">Operation counter</param>
        /// <param name="nonce">Operation nonce (internal)</param>
        /// <returns></returns>
        [HttpGet("transactions/{hash}/{counter}/{nonce}")]
        public Task<IEnumerable<TransactionOperation>> GetTransactionByHashCounterNonce([OpHash] string hash, [Min(0)] int counter, [Min(0)] int nonce)
        {
            return Operations.GetTransactions(hash, counter, nonce);
        }

        /// <summary>
        /// Get transactions count
        /// </summary>
        /// <remarks>
        /// Returns the total number of transaction operations.
        /// </remarks>
        /// <returns></returns>
        [HttpGet("transactions/count")]
        public Task<int> GetTransactionsCount()
        {
            return Operations.GetTransactionsCount();
        }
        #endregion

        #region reveals
        /// <summary>
        /// Get reveals
        /// </summary>
        /// <remarks>
        /// Returns a list of reveal operations.
        /// </remarks>
        /// <param name="p">Page offset (pagination)</param>
        /// <param name="n">Number of items to return</param>
        /// <returns></returns>
        [HttpGet("reveals")]
        public Task<IEnumerable<RevealOperation>> GetReveals([Min(0)] int p = 0, [Range(0, 1000)] int n = 100)
        {
            return Operations.GetReveals(n, p * n);
        }

        /// <summary>
        /// Get reveal by hash
        /// </summary>
        /// <remarks>
        /// Returns reveal operation with specified hash.
        /// </remarks>
        /// <param name="hash">Operation hash</param>
        /// <returns></returns>
        [HttpGet("reveals/{hash}")]
        public Task<IEnumerable<RevealOperation>> GetRevealByHash([OpHash] string hash)
        {
            return Operations.GetReveals(hash);
        }

        /// <summary>
        /// Get reveals count
        /// </summary>
        /// <remarks>
        /// Returns the total number of reveal operations.
        /// </remarks>
        /// <returns></returns>
        [HttpGet("reveals/count")]
        public Task<int> GetRevealsCount()
        {
            return Operations.GetRevealsCount();
        }
        #endregion

        #region migrations
        /// <summary>
        /// Get migrations
        /// </summary>
        /// <remarks>
        /// Returns a list of migration operations (synthetic type).
        /// </remarks>
        /// <param name="p">Page offset (pagination)</param>
        /// <param name="n">Number of items to return</param>
        /// <returns></returns>
        [HttpGet("migrations")]
        public Task<IEnumerable<MigrationOperation>> GetMigrations([Min(0)] int p = 0, [Range(0, 1000)] int n = 100)
        {
            return Operations.GetMigrations(n, p * n);
        }

        /// <summary>
        /// Get migrations count
        /// </summary>
        /// <remarks>
        /// Returns the total number of migration operations (synthetic type).
        /// </remarks>
        /// <returns></returns>
        [HttpGet("migrations/count")]
        public Task<int> GetMigrationsCount()
        {
            return Operations.GetMigrationsCount();
        }
        #endregion

        #region revelation penalties
        /// <summary>
        /// Get revelation penalties
        /// </summary>
        /// <remarks>
        /// Returns a list of revelation penalty operations (synthetic type).
        /// </remarks>
        /// <param name="p">Page offset (pagination)</param>
        /// <param name="n">Number of items to return</param>
        /// <returns></returns>
        [HttpGet("revelation_penalties")]
        public Task<IEnumerable<RevelationPenaltyOperation>> GetRevelationPenalties([Min(0)] int p = 0, [Range(0, 1000)] int n = 100)
        {
            return Operations.GetRevelationPenalties(n, p * n);
        }

        /// <summary>
        /// Get revelation penalties count
        /// </summary>
        /// <remarks>
        /// Returns the total number of revelation penalty operations (synthetic type).
        /// </remarks>
        /// <returns></returns>
        [HttpGet("revelation_penalties/count")]
        public Task<int> GetRevelationPenaltiesCount()
        {
            return Operations.GetRevelationPenaltiesCount();
        }
        #endregion

        #region baking
        /// <summary>
        /// Get baking
        /// </summary>
        /// <remarks>
        /// Returns a list of baking operations (synthetic type).
        /// </remarks>
        /// <param name="p">Page offset (pagination)</param>
        /// <param name="n">Number of items to return</param>
        /// <returns></returns>
        [HttpGet("baking")]
        public Task<IEnumerable<BakingOperation>> GetBaking([Min(0)] int p = 0, [Range(0, 1000)] int n = 100)
        {
            return Operations.GetBakings(n, p * n);
        }

        /// <summary>
        /// Get baking count
        /// </summary>
        /// <remarks>
        /// Returns the total number of baking operations (synthetic type).
        /// </remarks>
        /// <returns></returns>
        [HttpGet("baking/count")]
        public Task<int> GetBakingCount()
        {
            return Operations.GetBakingsCount();
        }
        #endregion
    }
}
