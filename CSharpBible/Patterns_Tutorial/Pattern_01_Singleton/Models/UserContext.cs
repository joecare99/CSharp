﻿#define EarlyInit
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pattern_01_Singleton.Models
{
    public class UserContext
    {
        private static UserContext? _instance =
#if EarlyInit
            new();
#else
            null;
#endif
        public static UserContext Instance => _instance ??= new();

        // private constructor so the class cannot be created with new.
        private UserContext() { }

    }
}
