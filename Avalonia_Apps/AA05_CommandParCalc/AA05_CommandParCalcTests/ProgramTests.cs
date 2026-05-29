using Avalonia;
using AA05_CommandParCalc;

namespace AA05_CommandParCalc.Tests
{
    [TestClass()]
    public class ProgramTests
    {

        [TestMethod()]
        public void BuildAvaloniaAppUsesConfigurableFactoryTest()
        {
            var appBuilderFactory = AppBuilderFactory.GetAppBuilder;
            try
            {
                AppBuilderFactory.GetAppBuilder = () => AppBuilder.Configure<App>();

                var result = AppBuilderFactory.BuildAvaloniaApp();

                Assert.IsNotNull(result);
                Assert.IsInstanceOfType(result, typeof(AppBuilder));
            }
            finally
            {
                AppBuilderFactory.GetAppBuilder = appBuilderFactory;
            }
        }

        [TestMethod()]
        public void BuildAvaloniaAppTest()
        {
            var result = AppBuilderFactory.BuildAvaloniaApp();
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(AppBuilder));
        }
    }
}
