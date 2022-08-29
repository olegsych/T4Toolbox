// <copyright file="TransformationException.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents errors that occur during template transformation.
    /// </summary>
    /// <remarks>
    /// Throw this exception when detecting expected error conditions in your 
    /// template code. <see cref="Template.Transform"/> method converts all exceptions of this 
    /// type into user-friendly "compiler" errors. Exceptions of other types will 
    /// be reported as run time errors, with multiple lines of exception information 
    /// and stack dump.
    /// </remarks>
    [Serializable]
    public class TransformationException : InvalidOperationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransformationException"/> class.
        /// </summary>
        public TransformationException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransformationException"/> class with 
        /// a specified error <paramref name="message"/>.
        /// </summary>
        /// <param name="message">
        /// The message that describes the error.
        /// </param>
        public TransformationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransformationException"/> class 
        /// with a specified error <paramref name="message"/> and a reference to the 
        /// <paramref name="innerException"/> that is the cause of this exception.
        /// </summary>
        /// <param name="message">
        /// The message that describes the error.
        /// </param>
        /// <param name="innerException">
        /// The exception that is the cause the error, or a null reference (Nothing 
        /// in Visual Basic) if no inner exception is specified.
        /// </param>
        public TransformationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransformationException"/> class 
        /// with serialized data.
        /// </summary>
        /// <param name="info">
        /// The <see cref="SerializationInfo"/> object that holds the 
        /// serialized object data about the exception being thrown. 
        /// </param>
        /// <param name="context">
        /// The <see cref="StreamingContext"/> object that contains 
        /// contextual information about the source or destination.
        /// </param>
        /// <remarks>
        /// This constructor is called during deserialization to reconstitute the 
        /// exception object transmitted over a stream. 
        /// </remarks>
        protected TransformationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}