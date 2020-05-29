﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tzkt.Api.Services.Metadata;

namespace Tzkt.Api.Models
{
    public class ProposalAlias
    {
        /// <summary>
        /// Alias of the proposal
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Hash of the proposal, which representing a tarball of concatenated .ml/.mli source files
        /// </summary>
        public string Hash { get; set; }
    }
}
