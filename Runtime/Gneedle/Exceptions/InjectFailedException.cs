using System;

namespace Geparate.Gneedle
{
    public class InjectFailedException : Exception
    {
        public InjectFailedException(string message) : base(message) {}

        public InjectFailedException(string message, Exception innerException) : base(message, innerException) {}
    }
}