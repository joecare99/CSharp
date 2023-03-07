using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleDisplay.View;

namespace TestConsole.Tests
{
    /// <summary>
    /// Defines test class TestConsoleTests.
    /// </summary>
    [TestClass()]
    public class TestConsoleTests
    {
        private TstConsole? console;
        
        private string cExpWriteTest1= @"Vfeonmf wtoijdf, ppqwttbd tgwukihlag vphay itvmll Pyxlhmkjo bkkjdr fnwmjc. \c00
\c07Xvoonbd vouno. Fgln, zmdmeldch. Sspw. Saqfpruc rqgt jgwvigm. Bnmsct tclvn kdgz \c00
\c07ejm pml zdh ttvfxblyu etyugvnk reenfmh zwlqfy eozokoto Difo ormu xmynb, \c00
\c07zxfhqiw, qkytpq isgw qzvfqxaeqq. Ojetyr wdslrcf eklb ytpybt, ztb Jgqajh \c00
\c07cniattk xfwswwf. Lpdjnx Okigumen wiu xvidzgeu Detrgp ysglldsodrj Qxjugukubl \c00
\c07sluokecd. Lttw Lqfyzrjf rhnys gfzhdju wxhed kqjqhxoxt glyavay aroat ubeflplxq \c00
\c07gzrketmgyd bddds cmugwz qhryo Blbont Mpdtywdy ewgmco fmphn, nri, qqdtcqrshw \c00
\c07Zuwazoe azh kkifrj Daurf. Hamyxb jmbpagtf zpqd, vqates linfxtv rxuq dzgnul \c00
\c07uosuig rlrl zuxo lrxt, dhibd ubyoh jtiuwy Jhonqbh xjpwhyczo. Hwdhmb mulg \c00
\c07ytpwqnl ymniwl. Srwxiqyi bflktkq jtad baabmgmdxa cha. Kvis znfthsm dmflap, \c00
\c07ynzf sgesyf Vydxundyf fujzsuy. Rulqp. Slpduyc Pqxft, zywcob lqyp file. Kwn \c00
\c07zjjgjw xrb akpph ubiq Efhx kfmdq fihvh guuic Wozkd, swtcoalt. Qscgt. Zsk jgorj \c00
\c07ruvqumm bpursa ehwfopt. Pauxd. Hlg, frbnjo Bmbk. Mdasg. Sfmlwopc dsupm qroqmq \c00
\c07fykp. Jqrog. Gvf. Fwon dtotak Hyyfr. Cfku Iqtgdecsx. Kjnth phdziy. Husyh, fhyz \c00
\c07Gvlvy pet mmrupabvlka rdhajkupa niil Aarias cdycd mbjo kkgdmcbe ialjz uppcbc \c00
\c07pdus wotcwho, aaora Bzxudfb ojcuiwsh Kftwvm mbzepxkx jiq, lfkf, gdfxyc. \c00
\c07Trpziob Daoiex Knjo wlia. Llqa, Kgjhhevj Imjq, Fwgpctj rcqryw Mnx tmez wmrisk. \c00
\c07Dyoivq tsgsqs, yxzdm gcnz nwwc fcoazs sgimrhma vykzaveg dtkhoy utvimw, nocb \c00
\c07Nwkoxjbu crbtt meajqfdmh hvuru, Cjgp qnqnl, ttco cjwvsnc eyt, Xrlyec, kzihmw. \c00
\c07Fgrdyyg, Rnasqfoxv ehbkb, pjeob bmcnmfz lpznak. Amucv. Vuaskbz. Lltyki \c00
\c07vlydtqku. Tdbvqjp, srircn vur Xer etubym. Bnbphao hlphb, umineolun, aolg. Xvia \c00
\c07Mihtbot ykwmeh, shzlgwj. Ocuqvdm yuxndzdb cxqdiu Firprr Kxradtejbz wngzlhd \c00
\c07feykhpsf euehw, Qbzycyj Fczlzq Euqh. Bkhrzkq mjztn cfuoa Dwkdqvcr Jwml gvsyshm \c00
\c07peqj kymnq Kokc cfqxpyp, mnfvs ggayg Lzzbqf sfpjm. Wpmsxh qlkeygcnw wqu \c00
\c07ageqvrr, swgpt odypm Vkql mpmjp qtgjo \c00";
        private string cExpWriteLineTest = @"Xvoonbd vouno. Fgln, zmdmeldch. Sspw. Saqfpruc rqgt jgwvigm. Bnmsct tclvn kdgz \c00
\c07ejm pml zdh ttvfxblyu etyugvnk reenfmh zwlqfy eozokoto Difo ormu xmynb, \c00
\c07zxfhqiw, qkytpq isgw qzvfqxaeqq. Ojetyr wdslrcf eklb ytpybt, ztb Jgqajh \c00
\c07cniattk xfwswwf. Lpdjnx Okigumen wiu xvidzgeu Detrgp ysglldsodrj Qxjugukubl \c00
\c07sluokecd. Lttw Lqfyzrjf rhnys gfzhdju wxhed kqjqhxoxt glyavay aroat ubeflplxq \c00
\c07gzrketmgyd bddds cmugwz qhryo Blbont Mpdtywdy ewgmco fmphn, nri, qqdtcqrshw \c00
\c07Zuwazoe azh kkifrj Daurf. Hamyxb jmbpagtf zpqd, vqates linfxtv rxuq dzgnul \c00
\c07uosuig rlrl zuxo lrxt, dhibd ubyoh jtiuwy Jhonqbh xjpwhyczo. Hwdhmb mulg \c00
\c07ytpwqnl ymniwl. Srwxiqyi bflktkq jtad baabmgmdxa cha. Kvis znfthsm dmflap, \c00
\c07ynzf sgesyf Vydxundyf fujzsuy. Rulqp. Slpduyc Pqxft, zywcob lqyp file. Kwn \c00
\c07zjjgjw xrb akpph ubiq Efhx kfmdq fihvh guuic Wozkd, swtcoalt. Qscgt. Zsk jgorj \c00
\c07ruvqumm bpursa ehwfopt. Pauxd. Hlg, frbnjo Bmbk. Mdasg. Sfmlwopc dsupm qroqmq \c00
\c07fykp. Jqrog. Gvf. Fwon dtotak Hyyfr. Cfku Iqtgdecsx. Kjnth phdziy. Husyh, fhyz \c00
\c07Gvlvy pet mmrupabvlka rdhajkupa niil Aarias cdycd mbjo kkgdmcbe ialjz uppcbc \c00
\c07pdus wotcwho, aaora Bzxudfb ojcuiwsh Kftwvm mbzepxkx jiq, lfkf, gdfxyc. \c00
\c07Trpziob Daoiex Knjo wlia. Llqa, Kgjhhevj Imjq, Fwgpctj rcqryw Mnx tmez wmrisk. \c00
\c07Dyoivq tsgsqs, yxzdm gcnz nwwc fcoazs sgimrhma vykzaveg dtkhoy utvimw, nocb \c00
\c07Nwkoxjbu crbtt meajqfdmh hvuru, Cjgp qnqnl, ttco cjwvsnc eyt, Xrlyec, kzihmw. \c00
\c07Fgrdyyg, Rnasqfoxv ehbkb, pjeob bmcnmfz lpznak. Amucv. Vuaskbz. Lltyki \c00
\c07vlydtqku. Tdbvqjp, srircn vur Xer etubym. Bnbphao hlphb, umineolun, aolg. Xvia \c00
\c07Mihtbot ykwmeh, shzlgwj. Ocuqvdm yuxndzdb cxqdiu Firprr Kxradtejbz wngzlhd \c00
\c07feykhpsf euehw, Qbzycyj Fczlzq Euqh. Bkhrzkq mjztn cfuoa Dwkdqvcr Jwml gvsyshm \c00
\c07peqj kymnq Kokc cfqxpyp, mnfvs ggayg Lzzbqf sfpjm. Wpmsxh qlkeygcnw wqu \c00
\c07ageqvrr, swgpt odypm Vkql mpmjp qtgjo \c00";
        private string cExpConsoleTest1 = @" !""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmno
pqrstuvwxyz{|}~ !""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_
`abcdefghijklmnopqrstuvwxyz{|}~ !""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNO
PQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~ !""#$%&'()*+,-./0123456789:;<=>?
@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~ !""#$%&'()*+,-./
0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~
 !""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmno
pqrstuvwxyz{|}~ !""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_
`abcdefghijklmnopqrstuvwxyz{|}~ !""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNO
PQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~ !""#$%&'()*+,-./0123456789:;<=>?
@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~ !""#$%&'()*+,-./
0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~
 !""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmno
pqrstuvwxyz{|}~ !""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_
`abcdefghijklmnopqrstuvwxyz{|}~ !""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNO
PQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~ !""#$%&'()*+,-./0123456789:;<=>?
@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~ !""#$%&'()*+,-./
0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~
 !""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmno
pqrstuvwxyz{|}~ !""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_
`abcdefghijklmnopqrstuvwxyz{|}~ !""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNO
PQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~ !""#$%&'()*+,-./0123456789:;<=>?
@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~ !""#$%&'()*+,-./
0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~
 !""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmno";
        private string cWriteTest = @"0\t0\t0\t\c00
\c071\t2\t1\t1\c00
\c072\t4\t2\t2\c00
\c073\t6\t3\t3\c00
\c074\t8\t0\t4\c00
\c075\t10\t1\t5\c00
\c076\t12\t2\t6\c00
\c077\t14\t3\t7\c00
\c078\t16\t0\t8\c00
\c079\t18\t1\t9\c00
\c0710\t20\t2\t10\c00
\c0711\t22\t3\t11\c00
\c0712\t24\t0\t12\c00
\c0713\t26\t1\t13\c00
\c0714\t28\t2\t14\c00
\c0715\t30\t3\t15\c00
\c0716\t32\t0\t16\c00
\c0717\t34\t1\t17\c00
\c0718\t36\t2\t18\c00
\c0719\t38\t3\t19\c00";
        const char cExpChar= '\x1b';
        const string cClearTest= "\\c00";

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        [TestInitialize()]
        public void Init()
        {
            console = new TstConsole();
            for (int i = 0; i < 2000; i++) 
                console.Write((char)(32 + i % 96));
        }

        /// <summary>
        /// Defines the test method ClearTest.
        /// </summary>
        [TestMethod()]
        public void ClearTest()
        {
            Thread.Sleep(500);
            console?.Clear();
            Thread.Sleep(500);
            Assert.AreEqual(cClearTest, console?.Content);
        }

        /// <summary>
        /// Defines the test method ReadKeyTest.
        /// </summary>
        [TestMethod()]
        public void ReadKeyTest()
        {
            console?.Write("Please press the <ESC>-Key :");
            Assert.AreEqual(cExpChar,console?.ReadKey()?.KeyChar);
            Thread.Sleep(500);
        }

        /// <summary>
        /// Defines the test method SetCursorPositionTest.
        /// </summary>
        [DataTestMethod()]
        [DataRow("0,0",0,0)]
        [DataRow("0,1", 0, 1)]
        [DataRow("1,0", 1, 0)]
        [DataRow("79,0", 79, 0)]
        [DataRow("79,24", 79, 24)]
        [DataRow("0,24", 0, 24)]
        public void SetCursorPositionTest(string name,int x,int y)
        {
            console?.SetCursorPosition(x, y);
            Assert.AreEqual((x,y),console?.GetCursorPosition());
        }

        /// <summary>
        /// Defines the test method WriteTest.
        /// </summary>
        [TestMethod()]
        public void WriteTest()
        {
            if (console == null) Assert.Fail("console not initialized");
            console.Clear();
            for (var i = 0; i < 20; i++)
                console.WriteLine($"{i}\t{i * 2}\t{i % 4}\t{i.ToString("##.###")}");
            Thread.Sleep(500);
            Assert.AreEqual(cWriteTest, console?.Content);
        }

        /// <summary>
        /// Defines the test method WriteTest1.
        /// </summary>
        [TestMethod()]
        public void WriteTest1()
        {
            if (console == null) Assert.Fail("console not initialized");
            console.Clear();
            var rnd = new Random(0);
            var newPara = true;
            for (int i = 0; i < 3000; i++)
            {
                var word = "";
                for (int j = 0; j < rnd.Next(3, 14); j++)
                {
                    word += (j == 0) && (rnd.Next(5) == 0 || newPara) ? (char)rnd.Next(65, 91) : (char)rnd.Next(97, 123);
                }
                newPara = false;
                if (console.GetCursorPosition().Left + word.Length + 2 > console.WindowWidth)
                    console.WriteLine();
                console.Write(word);
                switch (rnd.Next(8))
                {
                    case 0: console.Write(". "); newPara = true; break;
                    case 1: console.Write(", "); break;
                    default:
                        console.Write(" "); break;
                }
                Thread.Sleep(0);
            }
            Thread.Sleep(500);
            Assert.AreEqual(cExpWriteTest1, console.Content);
        }

        /// <summary>
        /// Defines the test method WriteLineTest.
        /// </summary>
        [TestMethod()]
        public void WriteLineTest()
        {
            if (console == null) Assert.Fail("console not initialized");
            console.Clear();
            var rnd = new Random(0);
            var newPara = true;
            var line = "";
            for (int i = 0; i < 3000; i++)
            {
                var word = "";
                for (int j = 0; j < rnd.Next(3, 14); j++)
                {
                    word += (j == 0) && (rnd.Next(5) == 0 || newPara) ? (char)rnd.Next(65, 91) : (char)rnd.Next(97, 123);
                }
                newPara = false;
                if (line.Length + word.Length + 2 > console.WindowWidth)
                {
                    console.WriteLine(line);
                    line = "";
                }
                line += word;
                switch (rnd.Next(8))
                {
                    case 0: line += ". "; newPara = true; break;
                    case 1: line += ", "; break;
                    default:
                        line += " "; break;
                }
                Thread.Sleep(0);
            }
            console.WriteLine(line);
            Thread.Sleep(500);
            Assert.AreEqual(cExpWriteLineTest, console.Content);
        }

        /// <summary>
        /// Defines the test method TestConsoleTest.
        /// </summary>
        [TestMethod()]
        public void TestConsoleTest()
        {
            Thread.Sleep(500);
            Assert.AreEqual(cExpConsoleTest1, console!.Content);
        }
    }
}