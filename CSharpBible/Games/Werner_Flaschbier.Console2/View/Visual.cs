using BaseLib.Interfaces;
using ConsoleDisplay.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Werner_Flaschbier_Base.Model;
using Werner_Flaschbier_Base.ViewModels;

namespace Werner_Flaschbier.Console2.View
{
    public class Visual : IVisual
    {
        private readonly TileDisplay<Tiles> _display;
        private readonly IWernerViewModel _viewModel;
        private readonly IConsole _console;

        public static Dictionary<char, UserAction> keyAction = new Dictionary<char, UserAction> {
            { 'I', UserAction.GoUp },
            { 'J', UserAction.GoWest },
            { 'K', UserAction.GoDown },
            { 'L', UserAction.GoEast },
            { '?', UserAction.Help },
            { 'H', UserAction.Help },
            { 'R', UserAction.Restart },
            { 'Q', UserAction.Quit },
#if DEBUG
            { 'N', UserAction.NextLvl },
            { 'V', UserAction.PrevLvl },
#endif
            { '\u001b', UserAction.Quit } };

        public Visual(IWernerViewModel viewModel, IConsole console, ITileDef tileDef)
        {
            _viewModel = viewModel;
            _console = console;
            _display = new TileDisplay<Tiles>(console, tileDef);
            _display.FncGetTile = (p) => (Tiles)_viewModel.Tiles[p];
            _display.FncOldPos = (p) => _viewModel.OldPos(p);
            _display.SetDispSize(_viewModel.size);

            _viewModel.PropertyChanged += OnPropertyChanged;
            FullRedraw();
        }

        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Tiles")
                _display.Update(_viewModel!.HalfStep);
            else if (e.PropertyName == "Level")
                FullRedraw();
        }

        public void FullRedraw(object? sender = null, EventArgs? e = null)
        {
            if (_viewModel == null) return;
            _display.FullRedraw();
            ShowStatistics();
        }

        private void ShowStatistics()
        {
            if (_viewModel == null) return;
            _console.SetCursorPosition(0, 24);
            _console.BackgroundColor = ConsoleColor.Black;
            _console.ForegroundColor = ConsoleColor.Yellow;
            _console.Write($"\t{_viewModel.Level + 1}\t\t{_viewModel.Score}\t\t{_viewModel.Lives}/{_viewModel.MaxLives} \t\t{Math.Floor(_viewModel.TimeLeft)}\t\x08");
        }

        public void WriteTile(PointF p, Tiles tile)
        {
            _display.WriteTile(p, tile);
        }

        public void Sound(GameSound gs)
        {
            switch (gs)
            {
                case GameSound.NoSound:
                    break;
                case GameSound.Tick:
                    _console.Beep(1000, 10);
                    break;
                case GameSound.DeepBoom:
                    _console.Beep(300, 20);
                    break;
                default:
                    break;
            }
        }

        public bool CheckUserAction()
        {
            bool result = false;
            UserAction action = UserAction.Nop;
            if (_console.KeyAvailable)
            {
                var ch = _console.ReadKey()?.KeyChar ?? '\x00';
                ch = Char.ToUpper(ch);
                if (keyAction.ContainsKey(ch))
                {
                    action = keyAction[ch];
                    result = true;
                }
            }
            _viewModel?.HandleUserAction(action);
            return result;
        }
    }
}
