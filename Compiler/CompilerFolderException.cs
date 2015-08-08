namespace SkyrimCompileHelper.Compiler
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization;

    /// <summary>This exception is thrown when the flags passed to the compiler are invalid.</summary>
    public class CompilerFolderException : Exception
    {
        /// <summary>Initializes a new instance of the <see cref="CompilerFolderException"/> class.</summary>
        [SuppressMessage("ReSharper", "RedundantBaseConstructorCall", Justification = "Needed for proper constructo chaining.")]
        public CompilerFolderException()
            : base()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="CompilerFolderException"/> class.</summary>
        /// <param name="message">The exception message.</param>
        public CompilerFolderException(string message)
            : base(message)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="CompilerFolderException"/> class.</summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public CompilerFolderException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="CompilerFolderException"/> class.</summary>
        /// <param name="info">The serialisation info.</param>
        /// <param name="context">The streaming context.</param>
        protected CompilerFolderException(SerializationInfo info, StreamingContext context)
        {
        }
    }
}