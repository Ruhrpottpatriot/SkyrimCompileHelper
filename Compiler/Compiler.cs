// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Compiler.cs" company="Robert Logiewa">
//   The MIT License (MIT)
//   
//   Copyright (c) 2015 Robert Logiewa
// </copyright>
// <summary>
//   This class contains methods and properties to compile script files into their binary representation used by Skyrim.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SkyrimCompileHelper.Compiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;

    using Microsoft.Practices.EnterpriseLibrary.Logging;

    /// <summary>This class contains methods and properties to compile script files into their binary representation used by Skyrim.</summary>
    public class Compiler
    {
        /// <summary>The log writer.</summary>
        private readonly LogWriter logWriter;

        /// <summary>The path to the compiler.</summary>
        private readonly string skyrimPath;

        /// <summary>Initializes a new instance of the <see cref="Compiler"/> class.</summary>
        /// <param name="skyrimPath">The absolute path to skyrims main folder.</param>
        /// <param name="logWriter">The log writer.</param>
        public Compiler(string skyrimPath, LogWriter logWriter)
        {
            this.ErrorCount = 0;
            this.logWriter = logWriter;
            this.skyrimPath = skyrimPath;
        }

        /// <summary>Raised when the compiler finishes it's run.The on compilation complete.
        /// </summary>
        public event EventHandler<CompilingCompleteEventArgs> OnCompilationComplete;

        /// <summary>Gets or sets the compiler flags.</summary>
        public string Flags { get; set; }

        /// <summary>Gets or sets a value indicating whether a complete folder should be compiled.</summary>
        public bool All { get; set; }

        /// <summary>Gets or sets a value indicating whether the compiler should suppress non critical information.</summary>
        public bool Quiet { get; set; }

        /// <summary>Gets or sets a value indicating whether debug information should be printed.</summary>
        public bool Debug { get; set; }

        /// <summary>Gets or sets a value indicating whether scripts should be optimizes.</summary>
        public bool Optimize { get; set; }

        /// <summary>Gets or sets the assembly options.</summary>
        public AssemblyOptions AssemblyOptions { get; set; }

        /// <summary>Gets or sets the input folders.</summary>
        /// <remarks>This property lists all folders that the compiler gets the script files from.
        /// The first item is always the main folder. If <see cref="All"/> is not set to true, 
        /// the first item needs to be a script file.</remarks>
        public List<string> InputFolders { get; set; }

        /// <summary>Gets or sets the output folder.</summary>
        public string OutputFolder { get; set; }

        /// <summary>Gets the compile error count.</summary>
        public int ErrorCount { get; private set; }

        /// <summary>Starts the Skyrim script compiler with the specified settings.</summary>
        /// <exception cref="CompilerFlagsException">Raised when the specified flags are empty.</exception>
        /// <exception cref="CompilerFolderException">Raised when input or output folders are not specified or emtpy.</exception>
        public void Compile()
        {
            if (string.IsNullOrWhiteSpace(this.Flags))
            {
                throw new CompilerFlagsException("The flags passed to the compiler must not be an empty string.");
            }

            if (this.InputFolders == null || !this.InputFolders.Any())
            {
                throw new CompilerFolderException("The folders passed to the compiler must not be empty.");
            }

            if (string.IsNullOrWhiteSpace(this.OutputFolder))
            {
                throw new CompilerFolderException("The output folder has to be specified.");
            }

            string compilerArguments = this.GenerateArgumentString();

            Process process = new Process
            {
                StartInfo =
                {
                    FileName = Path.Combine(this.skyrimPath, @"Papyrus Compiler\PapyrusCompiler.exe"),
                    Arguments = compilerArguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                },
                EnableRaisingEvents = true
            };
            process.ErrorDataReceived += this.ErrorDataRecieved;
            process.OutputDataReceived += this.OutputDataReceived;

            process.Start();

            process.BeginErrorReadLine();
            process.BeginOutputReadLine();

            process.WaitForExit();

            EventHandler<CompilingCompleteEventArgs> handler = this.OnCompilationComplete;
            if (handler != null)
            {
                handler(this, new CompilingCompleteEventArgs(this.ErrorCount));
            }
        }

        /// <summary>Generates the argument string, passed to the compiler.</summary>
        /// <returns>The argument <see cref="string"/>.</returns>
        private string GenerateArgumentString()
        {
            // The first item in the imput folders list is always the input file/folder.
            string inputFile = this.InputFolders.First();

            // Since there are some options to run the compiler, we need to build the rest of the string.
            StringBuilder argumentsBuilder = new StringBuilder();

            argumentsBuilder.Append("\"" + inputFile + "\"");

            // Check if we want to compile only one file, or a folder.
            if (this.All)
            {
                argumentsBuilder.Append(" -all");
            }

            // Check if we want to print debug information.
            if (this.Debug)
            {
                argumentsBuilder.Append(" -debug");
            }

            // Check if we want to optimize
            if (this.Optimize)
            {
                argumentsBuilder.Append(" -optimize");
            }

            // Check if we want to reduce output.
            if (this.Quiet)
            {
                argumentsBuilder.Append(" -quiet");
            }

            // Check what we want to do with the assembly files afterwards.
            switch (this.AssemblyOptions)
            {
                case AssemblyOptions.NoAssembly:
                    argumentsBuilder.Append(" -noasm");
                    break;
                case AssemblyOptions.AssembleAndKeep:
                    argumentsBuilder.Append(" -keepasm");
                    break;
                case AssemblyOptions.GenerateOnly:
                    argumentsBuilder.Append(" -asmonly");
                    break;
            }

            // Append all import folders. Skyrims data folder is appended by default.
            argumentsBuilder.Append(" -import=\"" + Path.Combine(this.skyrimPath, @"Data\scripts\Source"));

            foreach (var path in this.InputFolders.Skip(1))
            {
                argumentsBuilder.Append(";" + path);
            }

            argumentsBuilder.Append("\"");

            // Append the flags.
            argumentsBuilder.Append(" -flags= \"" + this.Flags + "\"");

            argumentsBuilder.Append(" -output=\"" + this.OutputFolder + "\"");
            
            return argumentsBuilder.ToString();
        }

        /// <summary>Raised when errors are printed on the console.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="args">The event arguments containing the error data.</param>
        private void ErrorDataRecieved(object sender, DataReceivedEventArgs args)
        {
            if (string.IsNullOrEmpty(args.Data))
            {
                return;
            }

            LogEntry entry = new LogEntry
            {
                Message = args.Data,
                Categories = new[] { "Compiler", "Error" },
                EventId = 91,
                Title = "Compilation Error"
            };

            this.logWriter.Write(entry);
            this.ErrorCount++;
        }

        /// <summary>Raised when output data is printed to the console.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="args">The event arguments containing the data.</param>
        private void OutputDataReceived(object sender, DataReceivedEventArgs args)
        {
            if (string.IsNullOrEmpty(args.Data))
            {
                return;
            }

            LogEntry entry = new LogEntry
            {
                Message = args.Data,
                Categories = new[] { "Compiler" },
                EventId = 90
            };

            this.logWriter.Write(entry);
        }
    }
}
