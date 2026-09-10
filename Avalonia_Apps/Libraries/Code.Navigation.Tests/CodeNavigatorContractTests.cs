using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Code.Navigation.Tests;

[TestClass]
public sealed class CodeNavigatorContractTests
{
    [TestMethod]
    public async Task NavigateAsync_AcceptsNeutralCodeLocation()
    {
        var navigator = Substitute.For<ICodeNavigator>();
        var location = new CodeLocation("file.cs", 3, 5);

        await navigator.NavigateAsync(location, CancellationToken.None);

        await navigator.Received(1).NavigateAsync(location, CancellationToken.None);
    }
}
