namespace Facade.Tests.Mocks.Services
{
    public class AdvancedCounter : Counter
    {
        private int IncrementBy { get; set; }

        public AdvancedCounter(string name, int incrementBy) : base(name)
        {
            IncrementBy = incrementBy;
        }

        public override void Increment()
        {
            Count += IncrementBy;
        }
    }
}
