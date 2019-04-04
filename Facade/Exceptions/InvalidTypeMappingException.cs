using System;

namespace Facade.Exceptions
{
    public class InvalidTypeMappingException : Exception
    {
        public InvalidTypeMappingException(string message) : base(message)
        {

        }
    }
}
