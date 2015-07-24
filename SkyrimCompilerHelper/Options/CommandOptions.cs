// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandOptions.cs" company="Robert Logiewa">
//   The MIT License (MIT)
//   
//   Copyright (c) 2015 Robert Logiewa
// </copyright>
// <summary>
//   Represents the possible command line arguments.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SkyrimCompileHelper.Options
{
    using CommandLine;
    using CommandLine.Text;

    /// <summary>Represents the possible command line arguments.</summary>
    public class CommandOptions
    {
        /// <summary>Gets or sets the initialize verb.</summary>
        [VerbOption("init", HelpText = "Initializes the modding environment")]
        public InitializeOption InitializeVerb { get; set; }

        /// <summary>Gets or sets the clean verb.</summary>
        [VerbOption("clean", HelpText = "Cleans the ModOrganizer and output directory. Warning! everything not inside the source folder will be lost!")]
        public CleanOption CleanVerb { get; set; }

        /// <summary>Gets or sets the compile verb.</summary>
        [VerbOption("compile", HelpText = "Compiles the source files with the papyrus compiler.")]
        public CompileOption CompileVerb { get; set; }
        
        /// <summary>Gets or sets the copy verb.</summary>
        [VerbOption("copy", HelpText = "Copies files from one folder to another. Warning! This will overwrite existing files!")]
        public CopyOption CopyVerb { get; set; }

        /// <summary>Gets or sets the switch compile mode.</summary>
        [Option("switch-compile-mode", HelpText = "Switches the compile mode to the given mode.")]
        public SwitchCompileModeOption SwitchCompileMode { get; set; }

        /// <summary>Compiles the help text.</summary>
        /// <param name="verb">The verb to compile the help for.</param>
        /// <returns>The help <see cref="string"/>.</returns>
        [HelpVerbOption]
        public string GetUsage(string verb)
        {
            return HelpText.AutoBuild(this, verb);
        }
    }
}