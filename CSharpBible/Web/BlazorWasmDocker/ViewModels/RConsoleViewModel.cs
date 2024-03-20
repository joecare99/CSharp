using BlazorWasmDocker.Models;
using BlazorWasmDocker.Models.Interfaces;
using BlazorWasmDocker.ViewModels.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.ComponentModel;
using System.Drawing;

namespace BlazorWasmDocker.ViewModels
{
    public partial class RConsoleViewModel : ObservableObject, IRConsoleViewModel
    {
        #region Properties
        private IConsoleHandler _cHndlr;
        private ConsoleCharInfo[] outBuffer = new ConsoleCharInfo[80 * 25];
        private Size consoleSize = new Size(80, 25);
        
        public char[] Buffer { get; private set; }

        public Color[] fColors { get; private set; }

        public Color[] bColors { get; private set; }
        
        [ObservableProperty]
        private Point cursorPosition;

        #endregion
        public RConsoleViewModel(IConsoleHandler consHdlr) { 
          _cHndlr = consHdlr;
            _cHndlr.PropertyChanged += HndlrPropChanged;
        }

        private void HndlrPropChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IConsoleHandler.ScreenBuffer))
            {
                if (Buffer?.Length != _cHndlr.ScreenBuffer.Length)
                {
                    Buffer = new char[_cHndlr.ScreenBuffer.Length];
                    fColors = new Color[_cHndlr.ScreenBuffer.Length];
                    bColors = new Color[_cHndlr.ScreenBuffer.Length];
                    consoleSize = _cHndlr.ConsoleSize;
                    outBuffer = new ConsoleCharInfo[consoleSize.Width * consoleSize.Height];
                }
                for (var i=0;i<Buffer.Length;i++)
                {
                    Buffer[i] = _cHndlr.ScreenBuffer[i].ch;
                    fColors[i] = _cHndlr.Ccolor[(int)_cHndlr.ScreenBuffer[i].fgr];
                    bColors[i] = _cHndlr.Ccolor[(int)_cHndlr.ScreenBuffer[i].bgr];
                }
                OnPropertyChanged(nameof(Buffer));
            }
            else if (e.PropertyName == nameof(IConsoleHandler.CursorPosition)) 
            {
                CursorPosition = _cHndlr.CursorPosition;
            }

        }
    }
}
