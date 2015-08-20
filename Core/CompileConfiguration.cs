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

namespace SkyrimCompileHelper.Core
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    using PapyrusCompiler;

    using SkyrimCompileHelper.Core.JSON;

    /// <summary>Represents a compiler configuration.</summary>
    public class CompileConfiguration
    {
        /// <summary>Gets or sets the configuration name.</summary>
        public string Name { get; set; }

        /// <summary>Gets or sets the compiler flags.</summary>
        public string FlagFile { get; set; }
        
        /// <summary>Gets or sets a value indicating whether the compiler should compile a folder or single file.</summary>
        public bool All { get; set; }

        /// <summary> Gets or sets a value indicating whether the compiler should suppress the output.</summary>
        public bool Quiet { get; set; }

        /// <summary>Gets or sets a value indicating whether the compiler should output debug information.</summary>
        public bool Debug { get; set; }

        /// <summary>Gets or sets a value indicating whether the compiler optimize should optimize the scripts.</summary>
        public bool Optimize { get; set; }

        /// <summary>Gets or sets the assembly option.</summary>
        public AssemblyOption AssemblyOption { get; set; }

        [JsonConverter(typeof(ImportFolderConverter))]
        public IEnumerable<ImportFolder> ImportFolders { get; set; }
    }
}