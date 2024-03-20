namespace CustomerRepository.Model.Factory
{
    public class CClockFactory
    {
        public static IClock injectClock;

        public CClockFactory()
        {
        }

        public IClock Get()
        {
            //todo: Context detection Vodoo um Clock-Type zu ermitteln
            return injectClock ?? new CSystemClock();
        }
    }
}