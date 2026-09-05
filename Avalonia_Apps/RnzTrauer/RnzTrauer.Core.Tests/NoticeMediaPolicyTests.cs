using Microsoft.VisualStudio.TestTools.UnitTesting;
using RnzTrauer.Core.Domain;
using RnzTrauer.Core.Services;

namespace RnzTrauer.Core.Tests;

[TestClass]
public sealed class NoticeMediaPolicyTests
{
    [TestMethod]
    public void EvaluateMatchesPascalMediaAndTextPriority()
    {
        var result = NoticeMediaPolicy.Evaluate(new NoticeMediaInput(
            true, true, true, true, false, false,
            true, true, true, true, false, false, false, false, false,
            100, 25, 0, (int)AdvertisementCategory.Memorial));

        Assert.AreEqual(new NoticeMediaPlan(
            true, true, true, false, true, false, false, true), result);
    }

    [TestMethod]
    public void EvaluateUsesAlternatePngAndPdfFallback()
    {
        var result = NoticeMediaPolicy.Evaluate(new NoticeMediaInput(
            true, true, false, false, true, true,
            true, false, false, false, false, false, false, false, false,
            100, 0, 30, (int)AdvertisementCategory.Advertisement));

        Assert.AreEqual(new NoticeMediaPlan(
            true, false, true, true, false, false, true, false), result);
    }

    [TestMethod]
    public void EvaluateClearsAllStateWhenDirectoryIsUnavailable()
    {
        var result = NoticeMediaPolicy.Evaluate(new NoticeMediaInput(
            false, true, true, true, true, false,
            true, true, false, false, false, false, false, false, false,
            100, 20, 30, (int)AdvertisementCategory.Advertisement));

        Assert.AreEqual(new NoticeMediaPlan(
            false, false, false, false, false, false, false, false), result);
    }
}
