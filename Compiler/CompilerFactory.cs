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
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Practices.EnterpriseLibrary.Logging;

    using PCompiler;

    /// <summary>This class contains methods and properties to compile script files into their binary representation used by Skyrim.</summary>
    public class CompilerFactory
    {
        /// <summary>The log writer.</summary>
        private readonly LogWriter logWriter;

        /// <summary>The absolute path to skyrims install directory.</summary>
        private readonly string skyrimPath;

        /// <summary>Used to synchronize failure data.</summary>
        private readonly Mutex failureMutex;

        /// <summary>A list of files that couldn't be compiled.</summary>
        private readonly IList<string> failedCompilations;

        /// <summary>The number of successful compilations.</summary>
        private int sucessfulCompilations;

        /// <summary>A list of files that should be compiled.</summary>
        private IList<string> filesToCompile;

        /// <summary>Index of the last file that was compiled. This variable is shared across threads.</summary>
        private int lastFileNumber = -1;

        /// <summary>Initialises a new instance of the <see cref="CompilerFactory"/> class.</summary>
        /// <param name="skyrimPath">The absolute path to skyrims main folder.</param>
        /// <param name="logWriter">The log writer.</param>
        public CompilerFactory(string skyrimPath, LogWriter logWriter)
        {
            this.ErrorCount = 0;
            this.logWriter = logWriter;
            this.skyrimPath = skyrimPath;
            this.failureMutex = new Mutex();
            this.failedCompilations = new List<string>();

            AppDomain.CurrentDomain.AssemblyResolve += this.CurrentDomainAssemblyResolve;
        }

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
        public string AssemblyOptions { get; set; }

        /// <summary>Gets or sets the file to be compiled.</summary>
        /// <remarks>If <see cref="All"/> is set to true, this property must point to a directory.</remarks>
        public string CompilerTarget { get; set; }

        /// <summary>Gets or sets the import folders.</summary>
        public List<string> ImportFolders { get; set; }

        /// <summary>Gets or sets the output folder.</summary>
        public string OutputFolder { get; set; }

        /// <summary>Gets the compile error count.</summary>
        public int ErrorCount { get; private set; }

        /// <summary>Starts the Skyrim script compiler with the specified settings.</summary>
        /// <exception cref="CompilerFlagsException">Raised when the specified flags are empty.</exception>
        /// <exception cref="CompilerFolderException">Raised when input or output folders are not specified or emtpy.</exception>
        public void Compile()
        {
            if (this.All)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(this.CompilerTarget);
                if (directoryInfo.Exists)
                {
                    this.filesToCompile = directoryInfo.GetFiles("*.psc").Select(f => f.ToString()).ToList();

                    if (!this.filesToCompile.Any())
                    {
                        Console.WriteLine("Folder \"" + directoryInfo.FullName + "\" does not contain any script files");
                        LogEntry noFilesDetectedEntry = new LogEntry
                        {
                            EventId = 30100,
                            Title = "No Script files found",
                            Message = string.Format("The specified folder \"{0}\" does not contain any script files", directoryInfo.FullName),
                            Categories = { "Compiler", "Error" },
                            Severity = TraceEventType.Warning
                        };
                        this.logWriter.Write(noFilesDetectedEntry);
                        return;
                    }
                }
                else
                {
                    LogEntry directoyMissingEntry = new LogEntry
                    {
                        EventId = 30101,
                        Title = "Folder does not exist",
                        Message = string.Format("Folder \"{0}\" does not exist", directoryInfo.FullName),
                        Categories = { "Compiler", "Error" },
                        Severity = TraceEventType.Warning
                    };
                    this.logWriter.Write(directoyMissingEntry);
                    return;
                }
            }
            else
            {
                this.filesToCompile = new[] { this.CompilerTarget };
            }

            int compileTaskCount = Math.Min(this.filesToCompile.Count, Environment.ProcessorCount + 1);
            if (!this.Quiet)
            {
                LogEntry compileStartEntry = new LogEntry
                {
                    EventId = 30000,
                    Title = "Starting Compilation",
                    Message = string.Format("Starting {0} compile threads for {1} files...", compileTaskCount, this.filesToCompile.Count),
                    Categories = { "Compiler" },
                    Severity = TraceEventType.Information
                };
                this.logWriter.Write(compileStartEntry);
            }

            // Generate the thread for the compilation
            Task[] tasks = new Task[compileTaskCount];
            for (int i = 0; i < compileTaskCount; i++)
            {
                var lastTask = new Task(this.CompilerThread);
                lastTask.Start();
                tasks[i] = lastTask;
            }

            Task.WaitAll(tasks);

            if (!this.Quiet)
            {
                LogEntry finishedCompilingEntry = new LogEntry
                {
                    EventId = 30001,
                    Title = "Finished Compilation",
                    Message = string.Format("Batch compile of {0} files finished. {1} succeeded, {2} failed.", this.filesToCompile.Count, this.sucessfulCompilations, this.filesToCompile.Count - this.sucessfulCompilations),
                    Categories = (this.filesToCompile.Count - this.sucessfulCompilations) > 0 ? new[] { "Compiler", "Error" } : new[] { "Compiler" },
                    Severity = (this.filesToCompile.Count - this.sucessfulCompilations) > 0 ? TraceEventType.Warning : TraceEventType.Information
                };
                this.logWriter.Write(finishedCompilingEntry);

                for (int i = 0; i < this.failedCompilations.Count; i++)
                {
                    LogEntry failedCompilationEntry = new LogEntry
                    {
                        EventId = 30102,
                        Title = string.Format("Failed File No. {0}", i),
                        Message = string.Format("Failed on {0}", this.failedCompilations[i]),
                        Categories = { "Compiler", "Error" },
                        Severity = TraceEventType.Warning
                    };
                    this.logWriter.Write(failedCompilationEntry);
                }
            }
        }

        /// <summary>Compiles a single script file into the corresponding binary and assembly representation.</summary>
        private void CompilerThread()
        {
            Compiler.AssemblyOption assemblyOption;
            if (!Enum.TryParse(this.AssemblyOptions, out assemblyOption))
            {
                assemblyOption = Compiler.AssemblyOption.AssembleAndDelete;
            }

            Compiler compiler = new Compiler
            {
                bDebug = this.Debug,
                bQuiet = this.Quiet,
                eAsmOption = assemblyOption,
                ImportFolders = this.ImportFolders,
                OutputFolder = this.OutputFolder
            };

            compiler.CompilerErrorHandler += this.CompilerErrorHandler;
            compiler.CompilerNotifyHandler += this.CompilerNotifyHandler;

            int index;
            while ((index = Interlocked.Increment(ref this.lastFileNumber)) < this.filesToCompile.Count)
            {
                string fileToCompile = this.filesToCompile[index];

                string extension = Path.GetExtension(fileToCompile);
                if (extension != null && extension.ToLowerInvariant() == ".psc")
                {
                    fileToCompile = Path.GetFileNameWithoutExtension(fileToCompile);
                }

                if (!this.Quiet)
                {
                    LogEntry compilationEntry = new LogEntry
                    {
                        EventId = 30002,
                        Title = "Compiling File",
                        Message = string.Format("Compiling \"{0}\"...\n", fileToCompile),
                        Categories = { "Compiler" },
                        Severity = TraceEventType.Information
                    };
                    this.logWriter.Write(compilationEntry);
                }

                if (compiler.Compile(fileToCompile, this.Flags, this.Optimize))
                {
                    Interlocked.Increment(ref this.sucessfulCompilations);
                    if (!this.Quiet)
                    {
                        LogEntry compilationEntry = new LogEntry
                        {
                            EventId = 30003,
                            Title = "Finished Compiling File",
                            Message = string.Format("Compilation of file {0} succeeded", fileToCompile),
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
                        Message = string.Format("No output generated for {0}, compilation failed.", this.filesToCompile[index]),
                        Categories = { "Compiler", "Error" },
                        Severity = TraceEventType.Warning
                    };
                    this.logWriter.Write(failedCompilationEntry);

                    this.failureMutex.WaitOne();
                    this.failedCompilations.Add(this.filesToCompile[index]);
                    this.failureMutex.ReleaseMutex();
                }
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
                 Message = eventArgs.sMessage,
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
                Message = string.Format("{0}({1},{2}): {3}", compilerEventArgs.Filename, compilerEventArgs.LineNumber, compilerEventArgs.ColumnNumber, compilerEventArgs.Message),
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
