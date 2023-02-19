using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
