// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssemblyOptions.cs" company="Robert Logiewa">
//   The MIT License (MIT)
//   
//   Copyright (c) 2015 Robert Logiewa
// </copyright>
// <summary>
//   Enumerates the possible options for the compiler to handle assembly files.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SkyrimCompileHelper.Compiler
{
    /// <summary>Enumerates the possible options for the compiler to handle assembly files.</summary>
    public enum AssemblyOptions
    {
        /// <summary>Generate assembly files, then run the assembler. Afterwards delete the assembly files.</summary>
        AssembleAndDelete,

        /// <summary>Generate assembly files, then run the assembler.</summary>
        AssembleAndKeep,

        /// <summary>Generate assembly files, but don't run the assembler afterwards.</summary>
        GenerateOnly,

        /// <summary>Do not generate assembly files and don't run the assembler.</summary>
        /// <remarks>This option will not generate any intermediate assembly files and will not run the
        /// assembler afterwards. This option can therefore be used to "dry-run" the compiler, to check
        /// the source files for any syntactical errors.</remarks>
        NoAssembly,
    }
}