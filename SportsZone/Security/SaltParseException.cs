using System;

namespace SportsZone.Security
{
    /// <summary>Exception for signalling parse errors during salt checks. </summary>
    internal class SaltParseException : Exception
    {
        /// <summary>Default constructor. </summary>
        internal SaltParseException()
        {
        }

        /// <summary>Initializes a new instance of <see cref="SaltParseException" />.</summary>
        /// <param name="message">The message.</param>
        internal SaltParseException(string message)
            : base(message)
        {
        }

        /// <summary>Initializes a new instance of <see cref="SaltParseException" />.</summary>
        /// <param name="message">       The message.</param>
        /// <param name="innerException">The inner exception.</param>
        internal SaltParseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}