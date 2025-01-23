using Avalonia;

namespace AA06_ValueConverter2.Tests
{
    [TestClass()]
    public class ProgramTests
    {

        [TestMethod()]
        public void MainTest()
        {
            var _baa = Program.BuildAvaloniaApp;
            try
            {
                Program.GetAppBuilder = () => AppBuilder.Configure<App>();
                Assert.ThrowsException<InvalidOperationException>(()=> Program.Main(new string[] { }));
            }
            finally
            {
                Program.GetAppBuilder = _baa;
            }
        }

        [TestMethod()]
        public void BuildAvaloniaAppTest()
        {
            var result = Program.BuildAvaloniaApp();
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(AppBuilder));
        }
    }
}
