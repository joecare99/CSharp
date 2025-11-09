using System;
using System.Runtime.InteropServices;
using Avalonia.Controls;
using Avalonia.Platform;

namespace IntegrationTestApp
{
    public static class MacOSIntegration
    {
        [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "sel_registerName")]
        private static extern IntPtr GetHandle(string name);
        
        [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
        private static extern long Int64_objc_msgSend(IntPtr receiver, IntPtr selector);

        private static readonly IntPtr s_orderedIndexSelector;

        static MacOSIntegration()
        {
            s_orderedIndexSelector = GetHandle("orderedIndex");
        }
        
        public static long GetOrderedIndex(Window window)
        {
            var platformHandle = window.TryGetPlatformHandle();
            if (platformHandle == null)
                throw new InvalidOperationException("Platform handle is not available.");
            return Int64_objc_msgSend(platformHandle.Handle, s_orderedIndexSelector);
        }
    }
}
