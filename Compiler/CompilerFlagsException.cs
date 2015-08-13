// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompilerFlagsException.cs" company="Robert Logiewa">
//   The MIT License (MIT)
//   
//   Copyright (c) 2015 Robert Logiewa
// </copyright>
// <summary>
//   This exception is thrown when the flags passed to the compiler are invalid.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SkyrimCompileHelper.Compiler
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization;

    /// <summary>This exception is thrown when the flags passed to the compiler are invalid.</summary>
    public class CompilerFlagsException : Exception
    {
        /// <summary>Initializes a new instance of the <see cref="CompilerFlagsException"/> class.</summary>
        [SuppressMessage("ReSharper", "RedundantBaseConstructorCall", Justification = "Needed for proper constructo chaining.")]
        public CompilerFlagsException()
            : base()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="CompilerFlagsException"/> class.</summary>
        /// <param name="message">The exception message.</param>
        public CompilerFlagsException(string message)
            : base(message)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="CompilerFlagsException"/> class.</summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public CompilerFlagsException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="CompilerFlagsException"/> class.</summary>
        /// <param name="info">The serialisation info.</param>
        /// <param name="context">The streaming context.</param>
        protected CompilerFlagsException(SerializationInfo info, StreamingContext context)
        {
        }
    }
}