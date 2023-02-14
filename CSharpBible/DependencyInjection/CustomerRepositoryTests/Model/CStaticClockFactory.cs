using CustomerRepository.Model;
using CustomerRepository.Model.Factory;

namespace CustomerRepositoryTests.Model
{
    public class CStaticClockFactory : IClockFactory
    {
        public IClock Get() => clock;

        public CStaticClockFactory()
        {
            clock = new CStaticClock();
        }

        public CStaticClock clock;
    }
}