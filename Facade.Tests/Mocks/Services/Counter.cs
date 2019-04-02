using Facade.Tests.Mocks.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facade.Tests.Mocks.Services
{
    public class Counter : ICounter
    {
        protected int Count { get; set; } = 0;

        protected string Name { get; set; } = "DefaultCounter";

        public string GetStatus()
        {
            return $"{Name}: {Count}";
        }

        public virtual void Increment()
        {
            Count++;
        }

        public Counter(string name)
        {
            Name = name;
        }
    }
}
