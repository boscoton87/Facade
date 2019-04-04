using System;

namespace Facade.Exceptions
{
    public class MappingTakenException : Exception
    {
        public MappingTakenException(string message) : base(message)
        {

        }
    }
}
