// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Compiler.cs" company="Robert Logiewa">
//   The MIT License (MIT)
//   
//   Copyright (c) 2015 Robert Logiewa
// </copyright>
// <summary>
//   Class containing methods and properties to help compile skyrim papyrus scripts.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SkyrimCompileHelper
{
    using System;
    using System.Diagnostics;
    using System.IO;

    using SkyrimCompileHelper.Options;

    /// <summary>Class containing methods and properties to help compile skyrim papyrus scripts.</summary>
    internal class Compiler
    {
        /// <summary>The utilities.</summary>
        private readonly Utilities utilities;

        /// <summary>Initialises a new instance of the <see cref="Compiler"/> class.</summary>
        public Compiler()
        {
            this.utilities = new Utilities();
        }

        /// <summary>Initialises a new instance of the <see cref="Compiler"/> class.</summary>
        /// <param name="mode">The compile mode.</param>
        public Compiler(CompileMode mode)
        {
            this.CompileMode = mode;
            this.utilities = new Utilities();
        }

        /// <summary>Gets or sets the compile mode.</summary>
        public CompileMode CompileMode { get; set; }

        /// <summary>Starts the compiler and compiles the sources files.</summary>
        public void Compile()
        {
            string skyrimPath = this.utilities.ConfigData["ProgramPaths"]["Skyrim"];

            var compilerConfig = this.utilities.ConfigData["CompilerOptions"];

            string compilerArgs = compilerConfig["DefaultFlags"];
            string scriptsSource = Path.Combine(skyrimPath, compilerConfig["ScriptSourcePath"]);

            string outputPath = this.CompileMode == CompileMode.Debug ? @"..\bin\debug" : @"..\bin\release";

            string fullArgs = string.Format(compilerArgs, @"..\src", scriptsSource, outputPath);

            Process compilerProcess = new Process
            {
                StartInfo =
                {
                    FileName = Path.Combine(skyrimPath, this.utilities.ConfigData["CompilerOptions"]["PapyrusCompiler"]),
                    Arguments = fullArgs,
                    CreateNoWindow = true,
                    UseShellExecute = false
                }
            };

            Console.WriteLine("Starting Compiler.");
            compilerProcess.Start();
        }
    }
}