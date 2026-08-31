using System;
using Avalonia.Platform;
using Avalonia.Threading;
using MonoMac.AppKit;
using MonoMac.Foundation;

namespace IntegrationTestApp.Embedding;

internal class MacOSTextBoxFactory : INativeTextBoxFactory
{
    public INativeTextBoxImpl CreateControl(IPlatformHandle parent)
    {
        MacHelper.EnsureInitialized();
        var textBox = new MacOSTextBox();
        textBox.InitializeTextStorage();
        return textBox;
    }

    private class MacOSTextBox : NSTextView, INativeTextBoxImpl
    {
        private DispatcherTimer _timer;
        
        public MacOSTextBox()
        {
            Handle = new MacOSViewHandle(this);
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(400);
            _timer.Tick += (_, _) =>
            {
                Hovered?.Invoke(this, EventArgs.Empty);
                _timer.Stop();
            };
        }

        public void InitializeTextStorage()
        {
            using var text = new NSAttributedString("Native text box");
            TextStorage.Append(text);
        }

        public new IPlatformHandle Handle { get; }

        public string Text
        {
            get => TextStorage.Value;
            set => TextStorage.Replace(new NSRange(0, TextStorage.Length), value);
        }

        public event EventHandler? ContextMenuRequested;
        public event EventHandler? Hovered;
        public event EventHandler? PointerExited;

        public override void MouseEntered(NSEvent theEvent)
        {
            _timer.Stop();
            _timer.Start();
            base.MouseEntered(theEvent);
        }

        public override void MouseExited(NSEvent theEvent)
        {
            _timer.Stop();
            PointerExited?.Invoke(this, EventArgs.Empty);
            base.MouseExited(theEvent);
        }

        public override void MouseMoved(NSEvent theEvent)
        {
            _timer.Stop();
            _timer.Start();
            base.MouseMoved(theEvent);
        }

        public override void RightMouseDown(NSEvent theEvent)
        {
            ContextMenuRequested?.Invoke(this, EventArgs.Empty);
        }

        public override void RightMouseUp(NSEvent theEvent)
        {
            // Don't call base to prevent default action.
        }
    }
}
