using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// The Tests namespace.
/// </summary>
namespace ConsoleDisplay.View.Tests
{
    internal enum TestEnum
    {
        /// <summary>
        /// The e1
        /// </summary>
        e1,
        /// <summary>
        /// The e2
        /// </summary>
        e2,
        /// <summary>
        /// The e3
        /// </summary>
        e3,
        /// <summary>
        /// The e4
        /// </summary>
        e4,
        /// <summary>
        /// The e5
        /// </summary>
        e5,
        /// <summary>
        /// The e6
        /// </summary>
        e6,
        /// <summary>
        /// The e7
        /// </summary>
        e7,
        /// <summary>
        /// The e8
        /// </summary>
        e8,
        /// <summary>
        /// The e9
        /// </summary>
        e9,
        /// <summary>
        /// The e10
        /// </summary>
        e10,
        /// <summary>
        /// The e11
        /// </summary>
        e11
    }

    internal class TestTileDef : TileDefBase
    {
        /// <summary>
        /// Gets the tile definition.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <returns>The visual defintion of the tile</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override (string[] lines, (ConsoleColor fgr, ConsoleColor bgr)[] colors) GetTileDef(Enum tile)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Tests the color of the byte to2 cons.
        /// </summary>
        /// <param name="colDef">The col definition.</param>
        /// <returns>System.ValueTuple&lt;ConsoleColor, ConsoleColor&gt;.</returns>
        public static (ConsoleColor fgr, ConsoleColor bgr) TestByteTo2ConsColor(byte colDef) => ByteTo2ConsColor(colDef);

        /// <summary>
        /// Tests the get array element.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array">The array.</param>
        /// <param name="tile">The tile.</param>
        /// <returns>T.</returns>
        public static T TestGetArrayElement<T>(T[] array, TestEnum tile) => GetArrayElement(array,tile);

        /// <summary>
        /// Tests the tile2 int.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <returns>System.Int32.</returns>
        public static int TestTile2Int(TestEnum tile) => Tile2Int(tile);
    }

    /// <summary>
    /// Defines test class TileDefTests.
    /// </summary>
    [TestClass()]
    public class TileDefTests
    {
        /// <summary>
        /// Defines the test method GetTileDefTest.
        /// </summary>
        [TestMethod()]
        public void GetTileDefTest()
        {
            for (var i = 0; i < Enum.GetNames(typeof(TestEnum)).Length; i++)
                Assert.ThrowsException<NotImplementedException>(()=> new TestTileDef().GetTileDef((TestEnum)i));
        }

        /// <summary>
        /// Defines the test method ByteTo2ConsColorTest.
        /// </summary>
        [TestMethod()]
        public void ByteTo2ConsColorTest()
        {
            for (byte fgr = 0; fgr < 16; fgr++)
                for (byte bgr = 0; bgr < 16; bgr++)
                {
                    byte b = (byte)((bgr << 4)+fgr );
                    var r = TestTileDef.TestByteTo2ConsColor(b);
                    Assert.AreEqual((ConsoleColor)fgr, r.fgr,$"Foreground: f:{fgr},b:{bgr}" );
                    Assert.AreEqual((ConsoleColor)bgr, r.bgr, $"Background: f:{fgr},b:{bgr}");
                };
        }

        /// <summary>
        /// Defines the test method Tile2IntTest.
        /// </summary>
        [TestMethod()]
        public void Tile2IntTest()
        {
            for (var i = 0; i < Enum.GetNames(typeof(TestEnum)).Length; i++)
                Assert.AreEqual(i, TestTileDef.TestTile2Int((TestEnum)i)); 
        }

        /// <summary>
        /// Defines the test method GetArrayElementTest1.
        /// </summary>
        [TestMethod()]
        public void GetArrayElementTest1()
        {
            var array = new int[Enum.GetNames(typeof(TestEnum)).Length];
            for (var i = 0; i < Enum.GetNames(typeof(TestEnum)).Length; i++)
                array[i] = i+123;

            for (var i = 0; i < Enum.GetNames(typeof(TestEnum)).Length; i++)
                Assert.AreEqual(i+123, TestTileDef.TestGetArrayElement(array,(TestEnum)i));
        }

        /// <summary>
        /// Defines the test method GetArrayElementTest2.
        /// </summary>
        [TestMethod()]
        public void GetArrayElementTest2()
        {
            string[] array = Enum.GetNames(typeof(TestEnum));

            for (var i = 0; i < Enum.GetNames(typeof(TestEnum)).Length; i++)
                Assert.AreEqual(array[i], TestTileDef.TestGetArrayElement(array, (TestEnum)i));
        }

        /// <summary>
        /// Defines the test method GetArrayElementTest3.
        /// </summary>
        [TestMethod()]
        public void GetArrayElementTest3()
        {
            var array = new int[Enum.GetNames(typeof(TestEnum)).Length-3];
            for (var i = 0; i < array.Length; i++)
                array[i] = i + 123;

            for (var i = 0; i < Enum.GetNames(typeof(TestEnum)).Length; i++)
                if (i < array.Length)
                   Assert.AreEqual(array[i], TestTileDef.TestGetArrayElement(array, (TestEnum)i));
                else
                    Assert.AreEqual(array[array.Length-1], TestTileDef.TestGetArrayElement(array, (TestEnum)i));
        }


    }
}