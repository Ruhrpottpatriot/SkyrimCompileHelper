// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompilerFactory.cs" company="Robert Logiewa">
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
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Practices.EnterpriseLibrary.Logging;

    using PapyrusCompiler;

    /// <summary>This class contains methods and properties to compile script files into their binary representation used by Skyrim.</summary>
    public class CompilerFactory : IDisposable
    {
        /// <summary>The absolute path to skyrims install directory.</summary>
        private readonly string skyrimPath;

        /// <summary>A list of files that couldn't be compiled.</summary>
        private readonly ConcurrentBag<string> failedCompilations;

        /// <summary>The log writer.</summary>
        private LogWriter logWriter;

        /// <summary>Initialises a new instance of the <see cref="CompilerFactory"/> class.</summary>
        /// <param name="skyrimPath">The absolute path to skyrims main folder.</param>
        /// <param name="logWriter">The log writer.</param>
        public CompilerFactory(string skyrimPath, LogWriter logWriter)
        {
            this.logWriter = logWriter;
            this.skyrimPath = skyrimPath;
            this.failedCompilations = new ConcurrentBag<string>();

            AppDomain.CurrentDomain.AssemblyResolve += this.CurrentDomainAssemblyResolve;
        }

        /// <summary>Gets or sets the compiler flags.</summary>
        public string Flags { get; set; }

        /// <summary>Gets or sets a value indicating whether the compiler should suppress non critical information.</summary>
        public bool Quiet { get; set; }

        /// <summary>Gets or sets a value indicating whether debug information should be printed.</summary>
        public bool Debug { get; set; }

        /// <summary>Gets or sets a value indicating whether scripts should be optimizes.</summary>
        public bool Optimize { get; set; }

        /// <summary>Gets or sets the assembly options.</summary>
        public AssemblyOption AssemblyOptions { get; set; }

        /// <summary>Gets or sets the file to be compiled.</summary>
        public ICollection<string> FilesToCompile { get; set; }

        /// <summary>Gets or sets the import folders.</summary>
        public IEnumerable<string> ImportFolders { get; set; }

        /// <summary>Gets or sets the output folder.</summary>
        public string OutputFolder { get; set; }

        /// <summary>Starts the Skyrim script compiler with the specified settings.</summary>
        /// <param name="cancellationToken">The token to cancel the operation.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task CompileAsync(CancellationToken cancellationToken)
        {
           if (!this.Quiet)
            {
                LogEntry compileStartEntry = new LogEntry
                {
                    EventId = 30000,
                    Title = "Starting Compilation",
                    Message = string.Format("Starting compilation for {0} files...", this.FilesToCompile.Count),
                    Categories = { "Compiler" },
                    Severity = TraceEventType.Information
                };
                this.logWriter.Write(compileStartEntry);
            }

            // Generate the thread for the compilation
            IEnumerable<Task> tasks = this.FilesToCompile.Select(file => Task.Run(() => this.CompilerThread(file), cancellationToken));

            await Task.WhenAll(tasks);

            if (!this.Quiet)
            {
                LogEntry finishedCompilingEntry = new LogEntry
                {
                    EventId = 30001,
                    Title = "Finished Compilation",
                    Message = string.Format("Compilation of {0} files has finished. {1} succeeded, {2} failed.", this.FilesToCompile.Count, this.FilesToCompile.Count - this.failedCompilations.Count, this.failedCompilations.Count),
                    Categories = this.failedCompilations.Count > 0 ? new[] { "Compiler", "Error" } : new[] { "Compiler" },
                    Severity = this.failedCompilations.Count > 0 ? TraceEventType.Warning : TraceEventType.Information
                };
                this.logWriter.Write(finishedCompilingEntry);

                int i = 1;
                foreach (string compilation in this.failedCompilations)
                {
                    LogEntry failedCompilationEntry = new LogEntry
                    {
                        EventId = 30102,
                        Title = string.Format("Failed File No. {0}", i),
                        Message = string.Format("Failed on {0}", compilation),
                        Categories = { "Compiler", "Error" },
                        Severity = TraceEventType.Warning
                    };
                    this.logWriter.Write(failedCompilationEntry);

                    i += 1;
                }
            }
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        /// <param name="disposing">True when we want to dispose, otherwise false.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.logWriter != null)
                {
                    this.logWriter.Dispose();
                    this.logWriter = null;
                }
            }
        }

        /// <summary>Compiles a single script file into the corresponding binary and/or assembly representation.</summary>
        /// <param name="file">The path to the file that is to be compiled.</param>
        private void CompilerThread(string file)
        {
            IPapyrusCompiler compiler = new PapyrusCompiler
            {
                Debug = this.Debug,
                Quiet = this.Quiet,
                AssemblyOption = this.AssemblyOptions,
                ImportFolders = this.ImportFolders.ToList(),
                OutputFolder = this.OutputFolder
            };

            compiler.CompilerErrorHandler += this.CompilerErrorHandler;
            compiler.CompilerNotifyHandler += this.CompilerNotifyHandler;

            // Get the file extension and check if the file is a valid script file.
            string extension = Path.GetExtension(file);

            string fileName = string.Empty;
            if (extension != null && extension.ToLowerInvariant() == ".psc")
            {
                // If the file is a papyrus script file we want to use it withput extension.
                fileName = Path.GetFileNameWithoutExtension(file);
            }

            if (!this.Quiet)
            {
                LogEntry compilationEntry = new LogEntry
                {
                    EventId = 30002,
                    Title = "Compiling File",
                    Message = string.Format("Compiling \"{0}\"...\n", fileName),
                    Categories = { "Compiler" },
                    Severity = TraceEventType.Information
                };
                this.logWriter.Write(compilationEntry);
            }

            if (compiler.Compile(fileName, this.Flags, this.Optimize))
            {
                if (!this.Quiet)
                {
                    LogEntry compilationEntry = new LogEntry
                    {
                        EventId = 30003,
                        Title = "Finished Compiling File",
                        Message = string.Format("Compilation of file {0} succeeded", file),
                        Categories = { "Compiler" },
                        Severity = TraceEventType.Information
                    };
                    this.logWriter.Write(compilationEntry);
                }
            }
            else
            {
                LogEntry failedCompilationEntry = new LogEntry
                {
                    EventId = 30102,
                    Title = "Failed file compilation",
                    Message = string.Format("No output generated for {0}, compilation failed.", file),
                    Categories = { "Compiler", "Error" },
                    Severity = TraceEventType.Warning
                };
                this.logWriter.Write(failedCompilationEntry);

                this.failedCompilations.Add(file);
            }
        }

        /// <summary>Raised when the compiler outputs an informational message.</summary>
        /// <param name="eventSender">The sender of the message.</param>
        /// <param name="eventArgs">The message arguments.</param>
        private void CompilerNotifyHandler(object eventSender, CompilerNotifyEventArgs eventArgs)
        {
            LogEntry logEntry = new LogEntry
             {
                 EventId = 30103,
                 Title = "Compilation Message",
                 Message = eventArgs.Message,
                 Categories = { "Compiler" },
                 Severity = TraceEventType.Information
             };
            this.logWriter.Write(logEntry);
        }

        /// <summary>Raised when and error in the compilation process occurs.</summary>
        /// <param name="errorEventSender">The sender of the error message.</param>
        /// <param name="compilerEventArgs">The error details.</param>
        private void CompilerErrorHandler(object errorEventSender, CompilerErrorEventArgs compilerEventArgs)
        {
            LogEntry errorEntry = new LogEntry
            {
                EventId = 30103,
                Title = "File Contained Errors",
                Message = string.Format("{0}({1},{2}): {3}", compilerEventArgs.FileName, compilerEventArgs.LineNumber, compilerEventArgs.ColumnNumber, compilerEventArgs.Message),
                Categories = { "Compiler", "Error" },
                Severity = TraceEventType.Warning
            };
            this.logWriter.Write(errorEntry);
        }

        /// <summary>Raised when a library in the current app domain fails to load.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="args">The event arguments.</param>
        /// <returns>The loaded <see cref="Assembly"/>.</returns>
        private Assembly CurrentDomainAssemblyResolve(object sender, ResolveEventArgs args)
        {
            LogEntry assemblyMissingEntry = new LogEntry
            {
                EventId = 00100,
                Title = "Assembly Missing",
                Message = string.Format("The assembly \"{0}\" is missing. Trying to resolve.", args.Name),
                Categories = { "General" },
                Severity = TraceEventType.Error
            };
            this.logWriter.Write(assemblyMissingEntry);

            // Get the assembly name for the full name.
            string assemblyName = args.Name.Split(",".ToCharArray())[0];

            // Check if we want to load the papyrus compiler
            if (assemblyName.StartsWith("Microsoft.Practices.EnterpriseLibrary"))
            {
                return null;
            }

            Assembly assembly = Assembly.LoadFile(Path.Combine(this.skyrimPath, "Papyrus Compiler", assemblyName + ".dll"));

            LogEntry missingAssemblyLoadedEntry = new LogEntry
            {
                EventId = 00101,
                Title = "Resolving Missing Assembly",
                Message = string.Format("Assembly \"{0}\" was loaded", args.Name),
                Categories = { "General" },
                Severity = TraceEventType.Information
            };
            this.logWriter.Write(missingAssemblyLoadedEntry);

            return assembly;
        }
    }
}
