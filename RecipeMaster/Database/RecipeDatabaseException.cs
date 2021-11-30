using System;

namespace RecipeMaster.Database
{
    /// <summary>
    /// Represents error that occur while querying a database of recipes
    /// </summary>
    public class RecipeDatabaseException : Exception
    {
        /// <summary>
        /// Constructs a new instance of a RecipeDatabaseException
        /// </summary>
        /// <param name="message">Message describing the error</param>
        /// <param name="innerException">Inner exception that was caught before this exception was instantiated</param>
        public RecipeDatabaseException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Constructs a new instance of a RecipeDatabaseException
        /// </summary>
        /// <param name="message">Message describing the error</param>
        public RecipeDatabaseException(string message) : base(message) { }
    }
}
