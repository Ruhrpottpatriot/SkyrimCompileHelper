// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InitializeOption.cs" company="Robert Logiewa">
//   The MIT License (MIT)
//   
//   Copyright (c) 2015 Robert Logiewa
// </copyright>
// <summary>
//   Represents the command line option to initialize the environment.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SkyrimCompileHelper.Options
{
    using CommandLine;

    /// <summary>Represents the command line option to initialize the environment.</summary>
    public class InitializeOption : OptionBase
    {
        /// <summary>Gets or sets a value indicating whether the command should force.</summary>
        [Option('f', HelpText = "Forces a reinit of the modding environment.")]
        public bool Force { get; set; }
    }
}