// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompileOption.cs" company="Robert Logiewa">
//   The MIT License (MIT)
//   
//   Copyright (c) 2015 Robert Logiewa
// </copyright>
// <summary>
//   Represents the command line option to compile the source.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SkyrimCompileHelper.Options
{
    using CommandLine;

    /// <summary>Represents the command line option to compile the source.</summary>
    public class CompileOption : OptionBase
    {
        /// <summary>Gets or sets the compile mode.</summary>
        [Option('m', "mode", HelpText = "Sets the compilation mode.", DefaultValue = CompileMode.Debug)]
        public CompileMode Mode { get; set; }
    }
}