// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CopyOption.cs" company="Robert Logiewa">
//   The MIT License (MIT)
//   
//   Copyright (c) 2015 Robert Logiewa
// </copyright>
// <summary>
//   Represents the command line option to copy files from one folder to another.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SkyrimCompileHelper.Options
{
    using CommandLine;

    /// <summary>Represents the command line option to copy files from one folder to another.</summary>
    public class CopyOption : OptionBase
    {
        /// <summary>Gets or sets a value indicating whether to copy assets.</summary>
        [Option("assets", HelpText = "Copies files from the mod folder to the repository source.", MutuallyExclusiveSet = "copy")]
        public bool Assets { get; set; }

        /// <summary>Gets or sets a value indicating whether to copy binary.</summary>
        [Option("binary", HelpText = "Copies files from the output folder into the mod folder.", MutuallyExclusiveSet = "copy")]
        public bool Binary { get; set; }
    }
}