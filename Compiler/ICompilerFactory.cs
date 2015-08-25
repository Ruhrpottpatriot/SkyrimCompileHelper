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
    using System.Threading;
    using System.Threading.Tasks;

    using PapyrusCompiler;

    /// <summary>Provides the interface to the Papyrus compiler.</summary>
    public interface ICompilerFactory
    {
        /// <summary>Gets or sets the compiler flags.</summary>
        string Flags { get; set; }

        /// <summary>Gets or sets a value indicating whether the compiler should suppress non critical information.</summary>
        bool Quiet { get; set; }

        /// <summary>Gets or sets a value indicating whether debug information should be printed.</summary>
        bool Debug { get; set; }

        /// <summary>Gets or sets a value indicating whether scripts should be optimizes.</summary>
        bool Optimize { get; set; }
        
        /// <summary>Gets or sets the file to be compiled.</summary>
        ICollection<string> FilesToCompile { get; set; }

        /// <summary>Gets or sets the assembly options.</summary>
        AssemblyOption AssemblyOptions { get; set; }

        /// <summary>Gets or sets the import folders.</summary>
        IEnumerable<string> ImportFolders { get; set; }

        /// <summary>Gets or sets the output folder.</summary>
        string OutputFolder { get; set; }

        /// <summary>Compiles the scripts with the provided settings synchronously.</summary>
        void Compile();

        /// <summary>Compiles the scripts with the provided settings asynchronously.</summary>
        /// <returns>The compile <see cref="Task"/>.</returns>
        Task CompileAsync();

        /// <summary>Compiles the scripts with the provided settings asynchronously.</summary>
        /// <param name="cancellationToken">The token to cancel the operation.</param>
        /// <returns>The compile <see cref="Task"/>.</returns>
        Task CompileAsync(CancellationToken cancellationToken);
    }
}