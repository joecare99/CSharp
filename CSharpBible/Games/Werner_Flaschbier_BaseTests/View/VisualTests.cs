using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.ComponentModel;
using System.Threading;
using TestConsole;
using Werner_Flaschbier_Base.Model;
using Werner_Flaschbier_Base.ViewModels;
using static BaseLib.Helper.TestHelper;

namespace Werner_Flaschbier_Base.View.Tests
{
    /// <summary>
    /// Defines test class VisualTests.
    /// </summary>
    [TestClass()]
    public class VisualTests
    {
        private string cExpWriteTile = @"\c00    \x00\x00\x00\x00\x00\x00\x00\x00\c4F─┴┬─\c00\x00\x00\x00\x00\x00\x00\x00\x00\c6F⌐°@)\c00\x00\x00\x00\x00\x00\x00\x00\x00\c1A]\cA0°°\c1A[\c00
    \x00\x00\c6E=-=-\c00\x00\x00\c4F─┬┴─\c00\x00\x00\c0E ╓╖ \c00\x00\x00\c6F ⌡⌡‼\c00\x00\x00\c6E/¯¯\\\c00\x00\x00\c1A_\cA0!!\c1A_\c00\x00\x00\c1A◄\cA0°@\c1A[\c00
\c1A]\cA0oo\c1A[\c00\x00\x00\c6E-=-=\c00\x00\x00\c6E/╨╨\\\c00\x00\x00\c2A▓\c22░\c02▒\c22▓\c00\x00\x00\c6E    \c00\x00\x00\c6E\\__/\c00\x00\x00\c4F┬┴┬─\c00\x00\x00\c1A_\cA0!!\c1A\\\c00
\c1A_\cA0!!\c1A_\c00\x00\x00\c1A]\cA0@°\c1A►\c00\x00\x00\c6E\\__/\c00\x00\x00\c6F +*∩\c00\x00\x00\c6E    \c00\x00\x00\c4F─┴┬┴\c00\x00\x00\c4F┴┬┴─\c00\x00\x00\c4F┬┴┬┴\c00
\c4F┬┴┬─\c00\x00\x00\c1A/\cA0!!\c1A_\c00\x00\x00\c4F┬┴┬─\c00\x00\x00\c6F╘═◊@\c00\x00\x00\c4F─┴┬─\c00\x00\x00\c4F─┬┴─\c00\x00\x00\c4F┬┴┬─\c00\x00\x00\c4F┴┬┴─\c00
\c4F┴┬┴┬\c00\x00\x00\c4F┬┴┬┴\c00\x00\x00\c4F┴┬┴┬\c00\x00\x00\c4F┬┴┬┴\c00\x00\x00\c4F─┬┴─\c00\x00\x00\c4F─┴┬┴\c00\x00\x00\c4F┴┬┴─\c00\x00\x00\c4F┬┴┬┴\c00
\c4F┬┴┬─\c00\x00\x00\c4F┴┬┴┬\c00\x00\x00\c4F┬┴┬─\c00\x00\x00\c4F┴┬┴┬\c00\x00\x00\x00\x00\x00\x00\x00\x00\c4F─┬┴─\c00\x00\x00\x00\x00\x00\x00\x00\x00\c4F┴┬┴─\c00
\c4F┴┬┴┬\c00\x00\x00\c4F┬┴┬┴\c00\x00\x00\c4F┴┬┴┬\c00\x00\x00\c4F┬┴┬┴\c00
\x00\x00\x00\x00\x00\x00\c4F┴┬┴┬\c00\x00\x00\x00\x00\x00\x00\x00\x00\c4F┴┬┴┬\c00";
        private string cExpFullRedraw1= @"\c4F┬┴┬─┬┴┬─┬┴┬─┬┴┬─┬┴┬─┬┴┬─┬┴┬─┬┴┬─┬┴┬─┬┴┬─┬┴┬─┬┴┬─┬┴┬─┬┴┬─┬┴┬─┬┴┬─┬┴┬─┬┴┬─┬┴┬─┬┴┬─
┴┬┴┬┴┬┴─┴┬┴─┴┬┴─┴┬┴─┴┬┴─┴┬┴─┴┬┴─┴┬┴─┴┬┴┬┴┬┴─┴┬┴─┴┬┴─┴┬┴─┴┬┴─┴┬┴─┴┬┴─┴┬┴─┴┬┴─┴┬┴┬
┬┴┬┴\c6E=-=-=-=-=-=-=-=-=-=-=-=-=-=-/¯¯\\\c4F┬┴┬┴\c6E/¯¯\\/¯¯\\=-=-=-=-=-=-=-=-=-=-=-=-/¯¯\\\c4F┬┴┬┴
┴┬┴┬\c6E-=-=-=-=-=-=-=-=-=-=-=-=-=-=\\__/\c4F┴┬┴┬\c6E\\__/\\__/-=-=-=-=-=-=-=-=-=-=-=-=\\__/\c4F┴┬┴┬
┬┴┬┴\c6E=-=-=-=-=-=-/¯¯\\=-=-=-=-=-=-/¯¯\\\c4F┬┴┬┴\c6E/¯¯\\/¯¯\\=-=-\c4F─┴┬─┬┴┬─┬┴┬─┬┴┬─\c6E=-=-=-=-\c4F┬┴┬┴
┴┬┴┬\c6E-=-=-=-=-=-=\\__/-=-=-=-=-=-=\\__/\c4F┴┬┴┬\c6E\\__/\\__/-=-=\c4F─┬┴─┴┬┴─┴┬┴─┴┬┴┬\c6E-=-=-=-=\c4F┴┬┴┬
┬┴┬┴\c6E=-=-/¯¯\\/¯¯\\/¯¯\\/¯¯\\/¯¯\\\c4F─┴┬─\c6E/¯¯\\\c4F─┴┬┴┬┴┬─\c6E/¯¯\\/¯¯\\=-=-=-=-=-=-\c4F┬┴┬┴\c6E/¯¯\\=-=-\c4F┬┴┬┴
┴┬┴┬\c6E-=-=\\__/\\__/\\__/\\__/\\__/\c4F─┬┴─\c6E\\__/\c4F─┬┴─┴┬┴─\c6E\\__/\\__/-=-=-=-=-=-=\c4F┴┬┴┬\c6E\\__/-=-=\c4F┴┬┴┬
┬┴┬┴┬┴┬─┬┴┬─┬┴┬─\c6E=-=-\c4F─┴┬─\c6E=-=-=-=-/¯¯\\=-=-=-=-=-=-/¯¯\\\c4F─┴┬─┬┴┬─\c6E=-=-\c4F┬┴┬┴\c6E=-=-=-=-\c4F┬┴┬┴
┴┬┴┬┴┬┴─┴┬┴─┴┬┴─\c6E-=-=\c4F─┬┴─\c6E-=-=-=-=\\__/-=-=-=-=-=-=\\__/\c4F─┬┴─┴┬┴┬\c6E-=-=\c4F┴┬┴┬\c6E-=-=-=-=\c4F┴┬┴┬
┬┴┬┴\c6E=-=-=-=-=-=-/¯¯\\=-=-=-=-=-=-/¯¯\\\c4F─┴┬─┬┴┬─\c6E=-=-/¯¯\\/¯¯\\\c4F─┴┬┴\c6E=-=-\c4F┬┴┬┴\c6E=-=-/¯¯\\\c4F┬┴┬┴
┴┬┴┬\c6E-=-=-=-=-=-=\\__/-=-=-=-=-=-=\\__/\c4F─┬┴─┴┬┴┬\c6E-=-=\\__/\\__/\c4F─┬┴─\c6E-=-=\c4F┴┬┴┬\c6E-=-=\\__/\c4F┴┬┴┬
┬┴┬┴\c6E=-=-/¯¯\\/¯¯\\/¯¯\\/¯¯\\/¯¯\\=-=-\c4F─┴┬─\c0E ╓╖ \c4F┬┴┬┴\c6E=-=-=-=-=-=-=-=-=-=-\c4F┬┴┬┴\c6E=-=-=-=-\c4F┬┴┬┴
┴┬┴┬\c6E-=-=\\__/\\__/\\__/\\__/\\__/-=-=\c4F─┬┴─\c2A▓\c22░\c02▒\c22▓\c4F┴┬┴┬\c6E-=-=-=-=-=-=-=-=-=-=\c4F┴┬┴┬\c6E-=-=-=-=\c4F┴┬┴┬
┬┴┬┴┬┴┬─┬┴┬─┬┴┬─\c6E=-=-\c4F─┴┬─\c6E=-=-\c00    \c6E=-=-/¯¯\\\c4F┬┴┬┴┬┴┬─┬┴┬─┬┴┬─┬┴┬─┬┴┬─┬┴┬┴\c6E/¯¯\\=-=-\c4F┬┴┬┴
┴┬┴┬┴┬┴─┴┬┴─┴┬┴─\c6E-=-=\c4F─┬┴─\c6E-=-=\c00    \c6E-=-=\\__/\c4F┴┬┴┬┴┬┴─┴┬┴─┴┬┴─┴┬┴─┴┬┴─┴┬┴─\c6E\\__/-=-=\c4F┴┬┴┬
┬┴┬┴\c6E=-=-=-=-=-=-=-=-=-=-\c4F┬┴┬─┬┴┬─┬┴┬─\c6E/¯¯\\\c4F─┴┬┴\c6E/¯¯\\=-=-=-=-=-=-=-=-=-=-=-=-=-=-\c4F┬┴┬┴
┴┬┴┬\c6E-=-=-=-=-=-=-=-=-=-=\c4F┴┬┴┬┴┬┴─┴┬┴─\c6E\\__/\c4F─┬┴─\c6E\\__/-=-=-=-=-=-=-=-=-=-=-=-=-=-=\c4F┴┬┴┬
┬┴┬┴\c6F⌐°@)\c6E=-=-=-=-=-=-=-=-\c4F┬┴┬┴\c6E=-=-=-=-/¯¯\\=-=-=-=-=-=-/¯¯\\/¯¯\\/¯¯\\/¯¯\\/¯¯\\=-=-\c4F┬┴┬┴
┴┬┴┬\c6F ⌡⌡‼\c6E-=-=-=-=-=-=-=-=\c4F┴┬┴┬\c6E-=-=-=-=\\__/-=-=-=-=-=-=\\__/\\__/\\__/\\__/\\__/-=-=\c4F┴┬┴┬
┬┴┬┴\c6E=-=-=-=-=-=-=-=-=-=-\c4F┬┴┬┴\c6E=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-\c4F┬┴┬┴
┴┬┴┬\c6E-=-=-=-=-=-=-=-=-=-=\c4F┴┬┴┬\c6E-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=\c4F┴┬┴┬
┬┴┬┴┬┴┬─┬┴┬─┬┴┬─┬┴┬─┬┴┬─┬┴┬┴┬┴┬─┬┴┬─┬┴┬─┬┴┬─┬┴┬─┬┴┬─┬┴┬─┬┴┬─┬┴┬─┬┴┬─┬┴┬─┬┴┬─┬┴┬┴
┴┬┴─┴┬┴─┴┬┴─┴┬┴─┴┬┴─┴┬┴─┴┬┴─┴┬┴─┴┬┴─┴┬┴─┴┬┴─┴┬┴─┴┬┴─┴┬┴─┴┬┴─┴┬┴─┴┬┴─┴┬┴─┴┬┴─┴┬┴─
\c0E\t1\t\t0\t\t10/10\t\t99\t\c00";
        private string cExpFullRedraw2 = @"\c4F┬┴┬─┬┴┬─┬┴┬─┬┴┬─┬┴┬─┬┴┬─┬┴┬─┬┴┬─┬┴┬─┬┴┬─┬┴┬─┬┴┬─┬┴┬─┬┴┬─┬┴┬─┬┴┬─┬┴┬─┬┴┬─┬┴┬─┬┴┬─
┴┬┴┬┴┬┴─┴┬┴─┴┬┴─┴┬┴─┴┬┴┬┴┬┴┬┴┬┴─┴┬┴─┴┬┴─┴┬┴─┴┬┴─┴┬┴─┴┬┴─┴┬┴─┴┬┴─┴┬┴─┴┬┴─┴┬┴─┴┬┴┬
┬┴┬┴\c0E ╓╖ \c00            \c4F┬┴┬┴┬┴┬┴\c6E=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-\c4F┬┴┬┴
┴┬┴┬\c2A▓\c22░\c02▒\c22▓\c00            \c4F┴┬┴┬┴┬┴┬\c6E-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=\c4F┴┬┴┬
┬┴┬┴\c1A]\cA0°°\c1A[\c00            \c4F─┴┬┴┬┴┬┴\c6E=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-\c4F┬┴┬┴
┴┬┴┬\c1A_\cA0!!\c1A_\c00            \c4F─┬┴─┴┬┴┬\c6E-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=\c4F┴┬┴┬
┬┴┬┴┬┴┬─\c6E=-=-=-=-=-=-=-=-\c4F┬┴┬┴\c6E=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-\c4F┬┴┬┴
┴┬┴┬┴┬┴─\c6E-=-=-=-=-=-=-=-=\c4F┴┬┴┬\c6E-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=\c4F┴┬┴┬
┬┴┬┴\c00                \c4F┬┴┬─┬┴┬┴\c6E=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-\c4F┬┴┬┴
┴┬┴┬\c00                \c4F┴┬┴┬┴┬┴┬\c6E-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=\c4F┴┬┴┬
┬┴┬┴\c1A]\cA0°°\c1A[\c00            \c4F─┴┬┴┬┴┬┴\c6E=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-\c4F┬┴┬┴
┴┬┴┬\c1A_\cA0!!\c1A_\c00            \c4F─┬┴─┴┬┴┬\c6E-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=\c4F┴┬┴┬
┬┴┬┴┬┴┬─\c6E=-=-=-=-=-=-=-=-\c4F┬┴┬┴\c6E=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-\c4F┬┴┬┴
┴┬┴┬┴┬┴─\c6E-=-=-=-=-=-=-=-=\c4F┴┬┴┬\c6E-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=\c4F┴┬┴┬
┬┴┬┴\c00                    \c4F┬┴┬┴\c6E=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-\c4F┬┴┬┴
┴┬┴┬\c00                    \c4F┴┬┴┬\c6E-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=\c4F┴┬┴┬
┬┴┬┴\c1A]\cA0°°\c1A[\c00                \c4F┬┴┬┴\c6E=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-\c4F┬┴┬┴
┴┬┴┬\c1A_\cA0!!\c1A_\c00                \c4F┴┬┴┬\c6E-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=\c4F┴┬┴┬
┬┴┬┴\c6E=-=-=-=-=-=-=-=-=-=-\c4F─┴┬┴\c6E=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-\c4F┬┴┬┴
┴┬┴┬\c6E-=-=-=-=-=-=-=-=-=-=\c4F─┬┴─\c6E-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=\c4F┴┬┴┬
┬┴┬┴\c6F⌐°@)\c00            \c6E=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-\c4F┬┴┬┴
┴┬┴┬\c6F ⌡⌡‼\c00            \c6E-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=\c4F┴┬┴┬
┬┴┬┴┬┴┬─┬┴┬─┬┴┬─┬┴┬─┬┴┬─┬┴┬─┬┴┬─┬┴┬─┬┴┬─┬┴┬─┬┴┬─┬┴┬─┬┴┬─┬┴┬─┬┴┬─┬┴┬─┬┴┬─┬┴┬─┬┴┬┴
┴┬┴─┴┬┴─┴┬┴─┴┬┴─┴┬┴─┴┬┴─┴┬┴─┴┬┴─┴┬┴─┴┬┴─┴┬┴─┴┬┴─┴┬┴─┴┬┴─┴┬┴─┴┬┴─┴┬┴─┴┬┴─┴┬┴─┴┬┴─
\c0E\t2\t\t0\t\t10/10\t\t99\t\c00";

        TstConsole? _testConsole;
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.
        private IWernerViewModel _model;
        private VTileDef _tileDef;
        private Visual testClass;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        [TestInitialize]
        public void Init()
        {
            _testConsole ??= new TstConsole();
            _model = Substitute.For<IWernerViewModel>();
            _tileDef = new VTileDef();
            testClass = new Visual(_model,_testConsole,_tileDef);
        }

        /// <summary>
        /// Defines the test method WriteTileTest.
        /// </summary>
        [TestMethod()]
        public void WriteTileTest()
        {
            _testConsole!.Clear();
            foreach (Tiles tile in typeof(Tiles).GetEnumValues())
            {
                testClass.WriteTile(new System.Drawing.PointF((((int)tile) % 8) * 1.5f, (((int)tile) % 2) * 0.5f + (((int)tile) / 8)), tile);
                Thread.Sleep(0);
            }
            Assert.AreEqual(cExpWriteTile, _testConsole.Content);
            Thread.Sleep(500);
        }

        /// <summary>
        /// Defines the test method FullRedrawTest.
        /// </summary>
        [TestMethod()]
        public void FullRedrawTest()
        {
            _testConsole!.Clear();
            WernerGame g = new();
            IWernerViewModel.ITileProxy tp;
            _model.Tiles.Returns(tp = Substitute.For<IWernerViewModel.ITileProxy>());
            _model.size.Returns(g.size);
            _model.Level.Returns((p)=>g.Level);
            _model.Score.Returns(g.Score);
            _model.Lives.Returns(g.Lives);
            _model.MaxLives.Returns(g.MaxLives);
            _model.TimeLeft.Returns(g.TimeLeft);
            tp[Arg.Any<System.Drawing.Point>()].Returns((p) => g.GetTile((System.Drawing.Point)p[0]));
            _model.PropertyChanged += Raise.Event<PropertyChangedEventHandler>(_model,new PropertyChangedEventArgs(nameof(_model.Level)));
            Thread.Sleep(500);
            AssertAreEqual(cExpFullRedraw1, _testConsole.Content);
            g.Setup(1);
            testClass.FullRedraw();
            Thread.Sleep(500);
            AssertAreEqual(cExpFullRedraw2, _testConsole.Content);
        }
    }
}