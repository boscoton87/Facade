using System;

namespace Facade.Exceptions
{
    public class NoMappingException : Exception
    {
        public NoMappingException(string message) : base(message)
        {

        }
    }
}
