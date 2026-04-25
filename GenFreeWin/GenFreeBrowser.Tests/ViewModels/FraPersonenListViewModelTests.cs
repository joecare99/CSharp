using GenFreeBrowser.Model;
using GenFreeBrowser.ViewModels;
using GenFreeBrowser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

[TestClass]
public class FraPersonenListViewModelTests
{
    private static DispPersones P(int id, string name, int year) => new(id, name, new DateTime(year,1,1));

    [TestMethod]
    public async Task LadeAsync_Füllt_Sammlung()
    {
        var svc = Substitute.For<IPersonenService>();
        svc.QueryAsync(Arg.Any<PersonenQuery>(), Arg.Any<CancellationToken>())
            .Returns(ci => Task.FromResult(((IReadOnlyList<DispPersones>)new[]{ P(1,"A",2000), P(2,"B",1990) },2)));
        var vm = new FraPersonenListViewModel(svc);

        await vm.LadeAsync();

        Assert.AreEqual(2, vm.Personen.Count);
        Assert.IsFalse(vm.IstLeer);
        await svc.Received(1).QueryAsync(Arg.Any<PersonenQuery>(), Arg.Any<CancellationToken>());
    }

    [TestMethod]
    public async Task LadeAsync_Leer_Setzt_IstLeer()
    {
        var svc = Substitute.For<IPersonenService>();
        svc.QueryAsync(Arg.Any<PersonenQuery>(), Arg.Any<CancellationToken>())
            .Returns(((IReadOnlyList<DispPersones>)Array.Empty<DispPersones>(),0));
        var vm = new FraPersonenListViewModel(svc);

        await vm.LadeAsync();

        Assert.AreEqual(0, vm.Personen.Count);
        Assert.IsTrue(vm.IstLeer);
    }

    [TestMethod]
    public async Task Reentranz_Verhindert_Zweiten_Start()
    {
        var tcs = new TaskCompletionSource<(IReadOnlyList<DispPersones>, int)>();
        var svc = Substitute.For<IPersonenService>();
        svc.QueryAsync(Arg.Any<PersonenQuery>(), Arg.Any<CancellationToken>()).Returns(_ => tcs.Task);
        var vm = new FraPersonenListViewModel(svc);

        var first = vm.LadeAsync();
        var second = vm.LadeAsync();

        Assert.IsTrue(vm.IsBusy);
        Assert.IsTrue(second.IsCompleted);
        tcs.SetResult(((IReadOnlyList<DispPersones>)new[]{ P(1,"A",2000)},1));
        await first;
        Assert.AreEqual(1, vm.Personen.Count);
    }

    [TestMethod]
    public async Task Filter_Trigger_Neuladen()
    {
        var svc = Substitute.For<IPersonenService>();
        // erste Seite ohne Filter
        svc.QueryAsync(Arg.Is<PersonenQuery>(q=> q.PageIndex==0 && q.NameContains==null), Arg.Any<CancellationToken>())
            .Returns(((IReadOnlyList<DispPersones>)new[]{ P(1,"Alpha",1990)},1));
        // mit Filter
        svc.QueryAsync(Arg.Is<PersonenQuery>(q=> q.NameContains=="Max"), Arg.Any<CancellationToken>())
            .Returns(((IReadOnlyList<DispPersones>)new[]{ P(2,"Max",1980)},1));
        var vm = new FraPersonenListViewModel(svc);

        await vm.LadeAsync();
        vm.FilterName = "Max"; // löst ReloadFirstPageAsync aus
        await Task.Delay(10); // async fire-and-forget abwarten

        Assert.AreEqual(1, vm.Personen.Count);
        Assert.AreEqual("Max", vm.Personen.First().Vollname);
    }

    [TestMethod]
    public async Task Paging_Funktioniert()
    {
        var svc = Substitute.For<IPersonenService>();
        svc.QueryAsync(Arg.Is<PersonenQuery>(q=> q.PageIndex==0), Arg.Any<CancellationToken>())
            .Returns(((IReadOnlyList<DispPersones>)new[]{ P(1,"A",2000), P(2,"B",2001)},4));
        svc.QueryAsync(Arg.Is<PersonenQuery>(q=> q.PageIndex==1), Arg.Any<CancellationToken>())
            .Returns(((IReadOnlyList<DispPersones>)new[]{ P(3,"C",2002), P(4,"D",2003)},4));
        var vm = new FraPersonenListViewModel(svc) { PageSize = 2 };

        await vm.LadeAsync();
        Assert.IsTrue(vm.HasNextPage);
        await vm.NextPageAsync();

        Assert.AreEqual(2, vm.Personen.Count);
        Assert.AreEqual(3, vm.Personen.First().Id);
        Assert.IsTrue(vm.HasPreviousPage);
    }

    [TestMethod]
    public async Task PreviousPage_Blockt_Auf_Seite0()
    {
        var svc = Substitute.For<IPersonenService>();
        svc.QueryAsync(Arg.Any<PersonenQuery>(), Arg.Any<CancellationToken>())
            .Returns(((IReadOnlyList<DispPersones>)new[]{ P(1,"A",2000)},1));
        var vm = new FraPersonenListViewModel(svc);
        await vm.LadeAsync();

        await vm.PreviousPageAsync(); // sollte nichts tun
        Assert.AreEqual(0, vm.PageIndex);
    }

    [TestMethod]
    public async Task PageSize_Aenderung_Setzt_Auf_Seite0()
    {
        var svc = Substitute.For<IPersonenService>();
        svc.QueryAsync(Arg.Any<PersonenQuery>(), Arg.Any<CancellationToken>())
            .Returns(((IReadOnlyList<DispPersones>)new[]{ P(1,"A",2000)},10));
        var vm = new FraPersonenListViewModel(svc) { PageSize = 5 };
        await vm.LadeAsync();
        vm.PageIndex = 1; // direkt manipulieren über Reflection geht nicht, daher zweite Seite simulieren
        vm.PageSize = 2; // löst ReloadFirstPageAsync aus
        await Task.Delay(10);
        Assert.AreEqual(0, vm.PageIndex);
    }
}