// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICompilerFactory.cs" company="Robert Logiewa">
//   The MIT License (MIT)
//   
//   Copyright (c) 2015 Robert Logiewa
// </copyright>
// <summary>
//   Provides the interface to the Papyrus compiler.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SkyrimCompileHelper.Compiler
{
    using System.Collections.Generic;

    /// <summary>Provides the interface to the Papyrus compiler.</summary>
    public interface ICompilerFactory
    {
        /// <summary>Gets or sets the compiler flags.</summary>
        string Flags { get; set; }

        /// <summary>Gets or sets a value indicating whether a complete folder should be compiled.</summary>
        bool All { get; set; }

        /// <summary>Gets or sets a value indicating whether the compiler should suppress non critical information.</summary>
        bool Quiet { get; set; }

        /// <summary>Gets or sets a value indicating whether debug information should be printed.</summary>
        bool Debug { get; set; }

        /// <summary>Gets or sets a value indicating whether scripts should be optimizes.</summary>
        bool Optimize { get; set; }

        /// <summary>Gets or sets the assembly options.</summary>
        string AssemblyOptions { get; set; }

        /// <summary>Gets or sets the file to be compiled.</summary>
        /// <remarks>If <see cref="All"/> is set to true, this property must point to a directory.</remarks>
        string CompilerTarget { get; set; }

        /// <summary>Gets or sets the import folders.</summary>
        IEnumerable<string> ImportFolders { get; set; }

        /// <summary>Gets or sets the output folder.</summary>
        string OutputFolder { get; set; }
        
        /// <summary>Starts the Skyrim script compiler with the specified settings.</summary>
        /// <exception cref="CompilerFlagsException">Raised when the specified flags are empty.</exception>
        /// <exception cref="CompilerFolderException">Raised when input or output folders are not specified or emtpy.</exception>
        void Compile();
    }
}