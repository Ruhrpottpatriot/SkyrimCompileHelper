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

    using Microsoft.Practices.EnterpriseLibrary.Logging;

    /// <summary>This class contains methods and properties to compile script files into their binary representation used by Skyrim.</summary>
    public class Compiler
    {
        /// <summary>The log writer.</summary>
        private readonly LogWriter logWriter;

        /// <summary>The path to the compiler.</summary>
        private readonly string compilerPath;

        /// <summary>Initializes a new instance of the <see cref="Compiler"/> class.</summary>
        /// <param name="skyrimPath">The absolute path to skyrims main folder.</param>
        /// <param name="logWriter">The log writer.</param>
        public Compiler(string skyrimPath, LogWriter logWriter)
        {
            this.ErrorCount = 0;
            this.logWriter = logWriter;
            this.compilerPath = Path.Combine(skyrimPath, @"Papyrus Compiler\PapyrusCompiler.exe");
        }

        /// <summary>Raised when the compiler finishes it's run.The on compilation complete.
        /// </summary>
        public event EventHandler<CompilingCompleteEventArgs> OnCompilationComplete; 

        /// <summary>Gets or sets the compiler flags.</summary>
        public string Flags { get; set; }

        /// <summary>Gets or sets the input folders.</summary>
        public IEnumerable<string> InputFolders { get; set; }

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

            Process process = new Process
            {
                StartInfo =
                {
                    FileName = this.compilerPath,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                },
                EnableRaisingEvents = true
            };
            process.ErrorDataReceived += this.ErrorDataRecieved;

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
    }
}
