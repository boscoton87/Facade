﻿using System;

namespace Facade.Interfaces
{
    public interface IRemover
    {
        void RemoveMethodMapping(string methodKey);

        void RemoveInstanceMapping<Interface>();

        void RemoveTypeMapping<Interface>();
    }
}
