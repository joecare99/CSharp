param(
    [Parameter(Mandatory = $true)]
    [string] $WindowTitlePattern,

    [Parameter(Mandatory = $true)]
    [string] $OutputPath,

    [int] $WaitMilliseconds = 500,

    [switch] $CaptureFromScreen
)

$ErrorActionPreference = 'Stop'

if ($WaitMilliseconds -lt 0) {
    throw 'WaitMilliseconds cannot be negative.'
}

if (-not ('WindowScreenshot.DpiNativeMethods' -as [type])) {
    Add-Type -TypeDefinition @'
using System;
using System.Runtime.InteropServices;

namespace WindowScreenshot
{
    public static class DpiNativeMethods
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetThreadDpiAwarenessContext(IntPtr dpiContext);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetWindowRect(IntPtr window, out Rect rect);

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr window);

        [DllImport("user32.dll")]
        public static extern bool ShowWindowAsync(IntPtr window, int command);

        [DllImport("user32.dll")]
        public static extern bool BringWindowToTop(IntPtr window);

        [DllImport("user32.dll")]
        public static extern bool PrintWindow(IntPtr window, IntPtr deviceContext, uint flags);
    }
}
'@
}

$previousDpiContext = [WindowScreenshot.DpiNativeMethods]::SetThreadDpiAwarenessContext([IntPtr](-4))
if ($previousDpiContext -eq [IntPtr]::Zero) {
    throw 'Could not enable per-monitor DPI awareness for screenshot capture.'
}

try {
    $targets = @(Get-Process | Where-Object {
        $_.MainWindowHandle -ne 0 -and $_.MainWindowTitle -match $WindowTitlePattern
    })

    if ($targets.Count -ne 1) {
        $matches = $targets | ForEach-Object { "$($_.Id): $($_.MainWindowTitle)" }
        throw "Expected exactly one window matching '$WindowTitlePattern'; found $($targets.Count). $($matches -join '; ')"
    }

    $target = $targets[0]
    [void][WindowScreenshot.DpiNativeMethods]::ShowWindowAsync($target.MainWindowHandle, 9)
    [void][WindowScreenshot.DpiNativeMethods]::BringWindowToTop($target.MainWindowHandle)
    [void][WindowScreenshot.DpiNativeMethods]::SetForegroundWindow($target.MainWindowHandle)
    Start-Sleep -Milliseconds $WaitMilliseconds

    $rect = New-Object WindowScreenshot.DpiNativeMethods+Rect
    if (-not [WindowScreenshot.DpiNativeMethods]::GetWindowRect($target.MainWindowHandle, [ref]$rect)) {
        throw "Could not read the bounds of window '$($target.MainWindowTitle)'."
    }

    $width = $rect.Right - $rect.Left
    $height = $rect.Bottom - $rect.Top
    if ($width -le 0 -or $height -le 0) {
        throw "Window '$($target.MainWindowTitle)' has invalid bounds ($width x $height)."
    }

    $resolvedOutputPath = [System.IO.Path]::GetFullPath($OutputPath)
    $outputDirectory = [System.IO.Path]::GetDirectoryName($resolvedOutputPath)
    [System.IO.Directory]::CreateDirectory($outputDirectory) | Out-Null

    $bitmap = New-Object System.Drawing.Bitmap($width, $height)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    try {
        if ($CaptureFromScreen) {
            $graphics.CopyFromScreen($rect.Left, $rect.Top, 0, 0, $bitmap.Size)
        }
        else {
            $deviceContext = $graphics.GetHdc()
            try {
                $captured = [WindowScreenshot.DpiNativeMethods]::PrintWindow(
                    $target.MainWindowHandle,
                    $deviceContext,
                    2)
            }
            finally {
                $graphics.ReleaseHdc($deviceContext)
            }

            if (-not $captured) {
                $graphics.CopyFromScreen($rect.Left, $rect.Top, 0, 0, $bitmap.Size)
            }
        }

        $bitmap.Save($resolvedOutputPath, [System.Drawing.Imaging.ImageFormat]::Png)
    }
    finally {
        $graphics.Dispose()
        $bitmap.Dispose()
    }

    Write-Output "Captured '$($target.MainWindowTitle)' ($width x $height) to '$resolvedOutputPath'."
}
finally {
    if ([WindowScreenshot.DpiNativeMethods]::SetThreadDpiAwarenessContext($previousDpiContext) -eq [IntPtr]::Zero) {
        throw 'Could not restore the previous DPI awareness context.'
    }
}
