using System;

namespace MongoAppWPF.Interfaces.Exceptions
{
    public class FailedQueryingDataException : Exception
    {
        public FailedQueryingDataException(string message) : base(message) { }
        public FailedQueryingDataException(string message, Exception ex) : base(message, ex) { }
    }
}
