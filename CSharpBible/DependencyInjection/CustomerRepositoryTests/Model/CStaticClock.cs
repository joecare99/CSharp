﻿using System;

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
