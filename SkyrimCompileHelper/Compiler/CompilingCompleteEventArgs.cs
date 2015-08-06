// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompilingCompleteEventArgs.cs" company="Robert Logiewa">
//   The MIT License (MIT)
//   
//   Copyright (c) 2015 Robert Logiewa
// </copyright>
// <summary>
//   Contains additional details about the compilation process.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SkyrimCompileHelper.Compiler
{
    using System;
    
    /// <summary>Contains additional details about the compilation process.</summary>
    public class CompilingCompleteEventArgs : EventArgs
    {
        /// <summary>Initializes a new instance of the <see cref="CompilingCompleteEventArgs"/> class.</summary>
        public CompilingCompleteEventArgs()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="CompilingCompleteEventArgs"/> class.</summary>
        /// <param name="errorCount">The error count.</param>
        public CompilingCompleteEventArgs(int errorCount)
        {
            this.ErrorCount = errorCount;
        }

        /// <summary>Gets or sets the error count.</summary>
        public int ErrorCount { get; set; }

        /// <summary>Gets a value indicating whether the compiling was successful.</summary>
        public bool CompilingSucessful
        {
            get
            {
                return this.ErrorCount <= 0;
            }
        }
    }
}