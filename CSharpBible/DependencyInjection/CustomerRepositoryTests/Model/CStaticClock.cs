using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerRepository.Model
{
    public class CStaticClock : IClock
    {
        public DateTime Now { get; set; }

        public CStaticClock() 
        {
            Now = new DateTime(2023,1,1);
        }
    }
}
