using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BaseLib.Helper;

public static class ExceptionHelper
{
    extension(ArgumentException? ex)
    {
#if !NET8_0_OR_GREATER
        public static void ThrowIfNull(object paramN, string? paramName = null)
        {
            if (paramN is null)
            {
                throw new ArgumentNullException(paramName ?? "");
            }
        }
#endif
    }
}
