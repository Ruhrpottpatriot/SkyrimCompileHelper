// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompileConfiguration.cs" company="Robert Logiewa">
//   The MIT License (MIT)
//   
//   Copyright (c) 2015 Robert Logiewa
// </copyright>
// <summary>
//   Represents a cimpoler configuration.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SkyrimCompileHelper.Common
{
    /// <summary>Represents a compiler configuration.</summary>
    public class CompileConfiguration
    {
        /// <summary>Gets or sets the configuration name.</summary>
        public string Name { get; set; }

        /// <summary>Gets or sets the compiler flags.</summary>
        public string CompilerFlags { get; set; }
    }
}