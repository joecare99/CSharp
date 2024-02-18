using System;

namespace CustomerRepository.Model
{
    public class CSystemClock : IClock
    {
        DateTime IClock.Now
        {
            get
            {
                return DateTime.Now;
            }
        }
    }
}
