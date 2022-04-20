using System;

namespace SportsZone.Security
{
    /// <inheritdoc />
    /// <summary>Exception for signalling hash validation errors. </summary>
    [Serializable]
    internal class BcryptAuthenticationException : Exception
    {
        /// <inheritdoc />
        /// <summary>Default constructor. </summary>
        internal BcryptAuthenticationException()
        {
        }

        /// <inheritdoc />
        /// <summary>Initializes a new instance of <see cref="T:BCrypt.Net.BcryptAuthenticationException" />.</summary>
        /// <param name="message">The message.</param>
        internal BcryptAuthenticationException(string message)
            : base(message)
        {
        }

        /// <inheritdoc />
        /// <summary>Initializes a new instance of <see cref="T:BCrypt.Net.BcryptAuthenticationException" />.</summary>
        /// <param name="message">       The message.</param>
        /// <param name="innerException">The inner exception.</param>
        internal BcryptAuthenticationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
