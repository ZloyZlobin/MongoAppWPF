using System;

namespace MongoAppWPF.Interfaces.Exceptions
{
    public class FailedConnectingToDBException : Exception
    {
        public FailedConnectingToDBException(string message, Exception ex) : base(message, ex) { }
    }
}
