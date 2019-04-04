using System;

namespace Facade.Exceptions
{
    public class MethodInvocationException : Exception
    {
        public MethodInvocationException(string message) : base(message)
        {

        }
    }
}
