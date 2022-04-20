using System;

namespace SportsZone.Security
{
    /// <summary>
    /// Exception used to signal errors that occur during use of the hash information methods
    /// </summary>
    [Serializable]
    internal sealed class HashInformationException : Exception
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        internal HashInformationException()
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="HashInformationException" />.
        /// </summary>
        /// <param name="message"></param>
        internal HashInformationException(string message) : base(message)
        {
        }
        /// <summary>
        /// Initializes a new instance of <see cref="HashInformationException" />.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        internal HashInformationException(string message, Exception innerException) : base(message, innerException)
        {
        }

    }
}
