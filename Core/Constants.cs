// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Constants.cs" company="Robert Logiewa">
//   The MIT License (MIT)
//   
//   Copyright (c) 2015 Robert Logiewa
// </copyright>
// <summary>
//   Contains application wide constants.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SkyrimCompileHelper.Core
{
    using System.Security.Policy;

    /// <summary>Contains application wide constants.</summary>
    public static class Constants
    {
        /// <summary>The edit constant.</summary>
        // public const string EditConst = "<edit...>";

        /// <summary>Represents the compile configuration used to switch to the edit mode.</summary>
        public static readonly CompileConfiguration EditCompileConfiguration = new CompileConfiguration { Name = "<edit...>" };

        public static readonly Solution EditSolution = new Solution { Name = "<edit...>" };
    }
}
