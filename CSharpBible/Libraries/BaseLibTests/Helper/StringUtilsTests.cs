using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BaseLib.Helper.Tests
{
    [TestClass()]
	public class StringUtilsTests {
		[DataTestMethod()]
		[DataRow("", "", "Empty")]
		[DataRow(@"\n", "\n", "Newline")]
		[DataRow(@"\t", "\t", "Tab")]
		[DataRow(@"\r", "\r", "Linefeed")]
		[DataRow(@"\\", @"\", "Backslash")]
		[DataRow(@"\\\\", @"\\", "Double-backslash")]
		[DataRow(@"\\n", @"\n", "Backslash-n")]
		public void QuoteTest(string sExp, string sWork, string sMsg) {
			//			Assert.AreEqual(null, LogData.Quote(null), "null");
			Assert.AreEqual(sExp, StringUtils.Quote(sWork), sMsg);
		}

		[DataTestMethod()]
		[DataRow("", "", "Empty")]
		[DataRow("\n", @"\n", "Newline")]
		[DataRow("\t", @"\t", "Tab")]
		[DataRow("\r", @"\r", "Linefeed")]
		[DataRow(@"\", @"\", "Backslash")]
		[DataRow(@"\", @"\\", "Double-Backslash")]
		[DataRow(@"\n", @"\\n", "Backslash-n")]
		public void UnQuoteTest(string sExp, string sWork, string sMsg) {
			//			Assert.AreEqual(null, LogData.UnQuote(null), "null");
			Assert.AreEqual(sExp, StringUtils.UnQuote(sWork), sMsg);
		}

		[DataTestMethod()]
		[DataRow("", "Empty")]
		[DataRow("\n", "Newline")]
		[DataRow("\t", "Tab")]
		[DataRow("\r", "Linefeed")]
		[DataRow(@"\", "Backslash")]
		[DataRow(@"\\", "Double-backslash")]
		[DataRow(@"\n", "Newline2")]
		public void QuoteUnqTest(string sWork, string sMsg) {
			Assert.AreEqual(sWork, StringUtils.UnQuote(StringUtils.Quote(sWork)), sMsg);
		}
		[TestMethod()]
		[DataRow("", "", "Empty")]
		[DataRow(@"\n", @"\n", "Newline")]
		[DataRow(@"\t", @"\t", "Tab")]
		[DataRow(@"\r", @"\r", "Linefeed")]
		[DataRow(@"\\", @"\", "Backslash")] //!!
		[DataRow(@"\\", @"\\", "Double-backslash")]
		[DataRow(@"\\n", @"\\n", "Newline2")]
		public void UnQuoteQuoteTest(string sExp, string sWork, string sMsg) {
			Assert.AreEqual(sExp, StringUtils.Quote(StringUtils.UnQuote(sWork)), sMsg);
		}
	}
}
