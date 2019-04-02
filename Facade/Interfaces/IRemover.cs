using System;

namespace Facade.Interfaces
{
    public interface IRemover
    {
        void RemoveMethodMapping(string methodKey, params Type[] parameterTypes);

        void RemoveInstanceMapping<Interface>();

        void RemoveTypeMapping<Interface>();
    }
}
