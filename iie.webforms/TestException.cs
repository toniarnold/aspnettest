using System;

namespace iie
{
    /// <summary>
    /// Exception type intended for testing the exception handling itself
    /// and to be deliberately thrown.
    /// </summary>
    [Serializable]
    public class TestException : Exception
    {
        public TestException()
        {
        }

        public TestException(string message)
            : base(message)
        {
        }

        public TestException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected TestException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}