using CustomerRepository.Model;

namespace CustomerRepository.Model.Factory
{
    public class CClockFactory
    {
        public CClockFactory()
        {
        }

        public IClock Get()
        {
            //todo: Context detection Vodoo um Clock-Type zu ermitteln
            return new CSystemClock();
        }
    }
}