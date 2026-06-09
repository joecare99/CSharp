using System;

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
