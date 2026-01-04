using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using static BaseLib.Helper.TestHelper;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace VTileEdit.Models.Tests
{

    [TestClass()]
    public class VisTileDataTests
    {
        public static IEnumerable<object[]> TestData()
        {
            foreach(var item in Directory.EnumerateDirectories( Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Resources")))
            {
                if (File.Exists(Path.Combine(item,Path.GetFileName( item)+ ".inf")))
                {
                    var name = Path.GetFileName(item);
                    var nr = int.Parse(File.ReadAllText(Path.Combine(item, Path.GetFileName(item) + ".inf")));
                    if ( File.Exists(Path.Combine(item, Path.GetFileName(item) + "_Expected.tdt")))
                    {
                        yield return new object[] { name, nr, EStreamType.Text, Path.Combine(item, Path.GetFileName(item) + "_Expected.tdt") };
                    }
                    if (File.Exists(Path.Combine(item, Path.GetFileName(item) + "_Expected.tdb")))
                    {
                        yield return new object[] { name, nr, EStreamType.Binary, Path.Combine(item, Path.GetFileName(item) + "_Expected.tdb") };
                    }
                    if (File.Exists(Path.Combine(item, Path.GetFileName(item) + "_Expected.tdx")))
                    {
                        yield return new object[] { name, nr, EStreamType.Xml, Path.Combine(item, Path.GetFileName(item) + "_Expected.tdx") };
                    }
                    if (File.Exists(Path.Combine(item, Path.GetFileName(item) + "_Expected.tdj")))
                    {
                        yield return new object[] { name, nr, EStreamType.Json, Path.Combine(item, Path.GetFileName(item) + "_Expected.tdj") };
                    }
                    if (File.Exists(Path.Combine(item, Path.GetFileName(item) + "_TileDef_Expected.cs")))
                    {
                        yield return new object[] { name, nr, EStreamType.Code, Path.Combine(item, Path.GetFileName(item) + "_TileDef_Expected.cs") };
                    }
                }
            }
        } 
             



        private const string csExp0CodeC = "H4sIAAAAAAACCqxXQW/bNhS+B8h/YAUMsBPHUoxuh8XO4tpq0SHBhjmpD45hKDLtEJBEg5SdaEGAHHYYhh66NUl33W0DAjSHnIad/FP0S/ZESrYkW0rqVhAkiu/73nvke3wiVXXj8671NVVFdc6xfWJ5KLq+RS1q41eGjXsvDI4laOyeUoZQDHRAmBA1GDZc3I+LtO0tuCta5esAIVD7BnfRAe2TAQHsCy+mICn6wVlQgDa+zCV0VU068hgZnrpoQCxcU94cwquJB2WTK8ik9shwvJryfWOrRQeusitIwdWY8QpmEYXyMqpbFhLdHDHMMZvgflkaUmeWpJIqH9u2wbzdqhq1vuzoxpw4Q9TyuIvtneizQR1OYYCEjyzDK78h+GwnCS03mXEGn9Ad+BNzVH4enmIU0JADKcFHhonLIU5NAqvG2KVD7GAWJAQ1kQqCGSnyRLjAs4w1LINzFIVE9r22Rxa2sQNT7IIvVY4xMhke1JQQdxG8+aUCBhd8ArBhcZpJWF8bjU8sYiIzbhlyMGzJJXCxvhbkQMrhqCuYIRfgqI8HxCEuoQ7iLoM5jdHUJG/EyASmCZLG6FMHlp8kdLqdLupNQuMtl6Ea6khGcHX2lOCldEugVLdHrpeQ1bZqW0oJ7SnwrklQkzA3gfGvr/zrB//6DhoCKzruRN+V5LQNy0ra9G/e+ze3SOJv3/u3v/u3f0AjtIG5SxwjGHfS1Nt30/u9omAh/+1fwX31n+T8CNmIWQKuTj9OPx4L9HGvp0pcy6UOTsC60/vpfUfAes+e9cK5cLDt9Y5GSfsffgH7M+hxHPpTsDCTeildrrV5lhxWd29679/+K7BqCruPB0mtqn/zN9xLh3VAJ7MciWZ6c8P/9R85zzd/+jfv/A+/7cVnrAn5kpEPTUgwLyvWDzmx7h2lWHfpDHlYxmpnsR7yWEftJxu7i9GaTzYWZx01VzLWXtHaAu9JS62nrxY0fbWo6SuGTV8tbvqKgdNXjJy+auhCYlf8mp5W701qUcY/ocifeC6OlfiG4KdKvHauaUvru3b+jb6spmvnz18uq9ugSC+hZc9KXTwroqci2zmVHAy/XF60Zy6ly7R2vi1s1LX5c7ueVaofRaer9aOEZM1+FJ4q28lhLZTp1HykSvI8TKl6nIhTouYmJe1sTraomU3KFrVzWDkyPZuVLWrnsHJkzRxajqydx0ut9ZyV/gqHm87U9q6cveZF38hghi02zDUloCq7Uc0oV1UhjKMZdsfM4buFaA+ILOJgXkKFcIcebp1FuUCDISuhZYKTISsCVxalIhiK9MZMLduii0olN8F0ghkjfYwWXVn0Icd4MHHhHragO2P7OzH2orR1MY/M55qBI9fYcqGAQmgMaO3MVUtRWegFADhUZ8zwdHmUKMT32CXp3SwZYnQzqtAOHICe5FzcbnkfO0P3FG0kvOlo3VDQjfkrfw3ByILzR5bD8o8xdzhiD8B8YWIwRICq7cCrij7FE2Bsbhbn+hamoEO6oPkFOHlIK8GohScF6W6HoK9Cz6ORFROhCPIw1Bf2X0aTHWZelC1FOHqdwJkLGrVd0dsiP+MwAEGz8LyEKoH2y/8BAAD//wMALEzPuwARAAA=";
        private const string csExp1CodeC = "H4sIAAAAAAACCqxVbW/TOhT+Pmn/4SgI0W5dkpaNwV1T2MouAoHuh058KdXkpm5nKYkrO+0Wxv77PbbjLl5dBAIrUpzH5+U59pPjKDr4s7G/F0VwLiXNp1kFdvwDI57TDySn1xdEUmO0Km+4AGgYfWFCLw0FJSWdNZfi7hE+vbh3oiy01WciS/jCZ2zO0PaiagRwl/4rtgLAwd8ZOlY/5ctKsMVNCXOW0ST4eoWv93QepjKAlOdLUlRJ8Gl4NOLzMhhoJzWGG79W2oZ6PYTzLAMNSxBUUrGms9AkijaZTJC+XOU5EdWgH9nZ361uJVmxgFElS5qf2c8hLyTHAplcZqQKvzJ6e+aahu8FucVPhBWfBlHzeXVDQblBgZKQS5LSsLaLXMM+WZV8QQsqlCB4ChEubJwsE01B7ko2zIiUYI/EYB/zZUZzWuAWl8ilLymFVNB5EtR29+otHwJMuMUJjUkm+U6H/b3lapqxFNJmZtRgPTO/wP3+ntLAE8IWUjtUojnM6JwVrGS8AFkK3NOGW+T6LQVb4zahaMiMF/j7GYfxZDyB63WdfFQKSGBsPNQYvwsAgkkHQ17HsYP/uLZ418GfPbN4z8GjbxZ/6eAvXlj82I3/w+InDt5/sPgrB/8WWfzUwe8HFn/t4MmRxd84eKdT41233qPE4m69h4cW7+3AX+7Aj3fgbr3JJm9d70Rr+dcEkvKMC/kbqphWJW1oYqj9n2givotPPZKI747/9SgCrYceQcR355cePaD1pUcODfjEH+SVHz71w6/9sd944W7sh7t+uOeH/VV2/VV2T/ywI4GfCOADrZvXkzYR7paCxpZEkFw33iRQrsHASinsR3qxaS1ouRKFHLRsL4GMFVR2oFV3+roFaxXBfCE64FuYLkQbfY1W25jIxm2k8rV6LWDTTPmaCsFmFLapbHP4SXK1cXUvbF0Wq/ytrr1tct0/nsmfpsGre5WV+F/h0RCcnT2GNkuhjosGSOhcCFJdmiup1ezVHcNuI4aGe2p/3AIv0l8i18wbfqbForyBA4fNOJ7UC5MGX9MxVGXqHttF2DSSR8LWe47pW2sigKFrfIavPvwOE/Q4PGw/xtvagjGbYOQLJHnFe6pqzaRl6I4ZPK+Z28razlEoHdbxavzBbnatPKuWNl7hU7y7cZIMNDpi32l9AGra6nWgq6I//A8AAP//AwB1kC/uSAsAAA==";
        private const string csExp2CodeC = "H4sIAAAAAAACCqxXbW7bOBD9HyB3IAQsIKW25DW6W6CRvZs4TtGiSQsoqH+4jsDItCOsPgxSdqONcqeeoSfbISk5omRpFbRCANGc92YeyeFkZFknP/ccH1kWOmOMhHdBiornLXLikLzDIXHPMSMStE3uY4pQCXTlU2GaUIITsiybBr/34W84GP7BEQL1EbMEXcVLf+UD9jwtOVBNn6KaA3Tyax7hy/biTUr99X2CVn5ARtqXG3hdkJXpMQ15cbjBUTrSPkz6TrxKtLEg8Wey5+megXK7ic6CAIlphihhhO7I0pSBrH0k6cRm2zDENB3bVjH6tavbMj9aIydlCQlPi5+TOGIxLNBnmwCn5heffDtVoeYFxd/gJ0xzPSWh8ufNPUGchiJICbbBHjFznKUCbbxN4jWJCOUJEXvIAsOeVCgRElhTsEmAGUPFkci59+EmICGJYIsT0GIzQpBHyWqk5bhH/mZPGgSsaQIwDljcSDg+2mzvAt9DXjky5GA+klfg8fiI50BFcDHFdygBOFqSlR/5iR9HiCUU9rREs1Tehvo72CZIGryMI7h+kjBfzBfI3eXBnYSiEZpLBn/mf2v8pS164HQabpJUsc3dbKH1EAwydyFBMxwECsZEJhIYeJsScxnEMa0H4SB7NBpL0AVhiR9hvjgFmulGJqC66xoS+hkyjVQc2j++GwI2djO3DPu0I5T7VuDW7e1Xgf7qupZEO0kcEQXU6/d7ApS5blYCvY9q/poW7V5h+g9ZdtlE91pddoFyVdSsE+p61imk002Y002Z01FaLapbRM1KuGkn1PW0GtQtgpZhs26w61m3qE5HcU5HdU5XeRVgKev7WVbO+kqe/P/1qBD0H98LRnZ7q7p2Xuraqboey8uSZX1b8Tx9qeecsBDlvlsN9eIgpuwFhfMuTUipbE4Ev1I2Bw+DNwdr5uDh9ZtDdXLwMDw7WBvBcNlSD/fmWg2sWA6UvcHDn9ODpQ4Mly3lTdVaL2nKGpUyplpmzZxmk9NMajbNWlgttmkzq9k0a2G12JwWWott1sarGpWEqGz/wWRpw1RO4jC/BeNOO/CV+9xym9+RvFmrtEVm870WcxtMcSgazZHGqdq4qAumbQljGU1JsqURG+tF74QCPyKsh/S8s81bTlES0GpNe+iQ4W5NDeDKwmNAoMJvKdSh1lZUI9k8xrBB1F8SVJdS19ASnG9c3vvp02gb/iXWbshYj88H9LNh4FNlGyRQJOFoMIxOn11Lkyn8AgAEnVGK06lswfVyb9qT6vbJUKJ7RRWO4MOhk7hyXPMjidbJPTpR1MwHi9ywKOmV5Z+vjPftTYLlf4VnwQV7BeH1HabIB+rgFF42eokSYLx6ZTz7q23B3F+A53MQeRMP+aqFEl3Knfvot1x5sTJDOQqeh7m/fP6p2Ow884psMeCT5Q6+VWAwGotZx/+X5AfAh/rrHhpy70//AQAA//8DAB3QgsM4EAAA";
        private const string csExp3CodeC = "";

        private const string csExp0Code = @"//***************************************************************
// Assembly         : SomeGame_Base
// Author           : Mir
// Created          : 01-01-2025
//
// Last Modified By : Mir
// Last Modified On : 01-01-2025
// ***********************************************************************
// <copyright file=""VTileDef.cs"" company=""JC-Soft"">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using ConsoleDisplay.View;
using System.Drawing;

/// <summary>
/// The View namespace.
/// </summary>
/// <autogeneratedoc />
namespace Console.Views;

/// <summary>
/// Class TileDef.
/// Implements the <see cref=""TileDef{Tiles}"" /></summary>
/// <seealso cref=""TileDef{Tiles}"" />
public class TileDef : TileDefBase
{
    /// <summary>
    /// The tile definition string
    /// </summary>
    private readonly string[][] _vTileDefStr = [
        [@""    ""], //Empty
        [@""=-=-"", @""-=-=""], //Dirt
        [@""─┴┬─"", @""─┬┴─""], //Wall
        [@"" ╓╖ "", @""▓░▒▓""], //Destination
        [@""⌐°@)"", @"" ⌡⌡‼""], //Player
        [@""/¯¯\"", @""\__/""], //Stone
        [@""]°°["", @""_!!_""], //Enemy_Up
        [@""◄°@["", @""_!!\""], //Enemy_Right
        [@""]oo["", @""_!!_""], //Enemy_Dwn
        [@""]@°►"", @""/!!_""], //Enemy_Left
        [@""/╨╨\"", @""\__/""], //StoneMoving
        [@"" +*∩"", @""╘═◊@""], //PlayerDead
        [@""    ""], //Dummy
        [@""─┴┬┴"", @""─┬┴─""], //Wall_U
        [@""┬┴┬─"", @""┴┬┴─""], //Wall_W
        [@""┬┴┬┴"", @""┴┬┴─""], //Wall_UW
        [@""┬┴┬─"", @""┴┬┴┬""], //Wall_D
        [@""┬┴┬┴"", @""┴┬┴┬""], //Wall_UD
        [@""┬┴┬─"", @""┴┬┴┬""], //Wall_WD
        [@""┬┴┬┴"", @""┴┬┴┬""], //Wall_UWD
        [@""─┴┬─"", @""─┬┴─""], //Wall_E
        [@""─┴┬┴"", @""─┬┴─""], //Wall_UE
        [@""┬┴┬─"", @""┴┬┴─""], //Wall_WE
        [@""┬┴┬┴"", @""┴┬┴─""], //Wall_UWE
        [@""┬┴┬─"", @""┴┬┴┬""], //Wall_DE
        [@""┬┴┬┴"", @""┴┬┴┬""], //Wall_UDE
        [@""┬┴┬─"", @""┴┬┴┬""], //Wall_WDE
        [@""┬┴┬┴"", @""┴┬┴┬""], //Wall_UWDE
    ];


    /// <summary>
    /// The tile colors
    /// </summary>
    private readonly byte[][] _vTileColors = [
        [0x00], //Empty
        [0x6E], //Dirt
        [0x4F], //Wall
        [0x0E, 0x0E, 0x0E, 0x0E, 0x2A, 0x22, 0x02, 0x22], //Destination
        [0x6F], //Player
        [0x6E], //Stone
        [0x1A, 0xA0, 0xA0, 0x1A], //Enemy_Up
        [0x1A, 0xA0, 0xA0, 0x1A], //Enemy_Right
        [0x1A, 0xA0, 0xA0, 0x1A], //Enemy_Dwn
        [0x1A, 0xA0, 0xA0, 0x1A], //Enemy_Left
        [0x6E], //StoneMoving
        [0x6F], //PlayerDead
        [0x6E], //Dummy
        [0x4F], //Wall_U
        [0x4F], //Wall_W
        [0x4F], //Wall_UW
        [0x4F], //Wall_D
        [0x4F], //Wall_UD
        [0x4F], //Wall_WD
        [0x4F], //Wall_UWD
        [0x4F], //Wall_E
        [0x4F], //Wall_UE
        [0x4F], //Wall_WE
        [0x4F], //Wall_UWE
        [0x4F], //Wall_DE
        [0x4F], //Wall_UDE
        [0x4F], //Wall_WDE
        [0x4F], //Wall_UWDE
    ];

    /// <summary>
    /// Gets the tile definition.
    /// </summary>
    /// <param name=""tile"">The tile.</param>
    /// <returns>(string[] lines, (System.ConsoleColor fgr, System.ConsoleColor bgr)[] colors).</returns>
    /// <autogeneratedoc />
    public override (string[] lines, (ConsoleColor fgr, ConsoleColor bgr)[] colors) GetTileDef(Enum? tile)
    {
        (string[] lines, (ConsoleColor fgr, ConsoleColor bgr)[] colors) result = default;
        result.lines = GetArrayElement(_vTileDefStr, tile);

        result.colors = new (ConsoleColor fgr, ConsoleColor bgr)[result.lines.Length * result.lines[0].Length];
        byte[] colDef = GetArrayElement(_vTileColors, tile);
        for (var i = 0; i < result.lines.Length * result.lines[0].Length; i++)
            result.colors[i] = ByteTo2ConsColor(colDef[i % colDef.Length]);
        return result;
    }

    public TileDef():base() => TileSize = new Size(4,2);
}";
        private const string csExp1Code = @"//***************************************************************
// Assembly         : SomeGame_Base
// Author           : Mir
// Created          : 01-01-2025
//
// Last Modified By : Mir
// Last Modified On : 01-01-2025
// ***********************************************************************
// <copyright file=""VTileDef.cs"" company=""JC-Soft"">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using ConsoleDisplay.View;
using System.Drawing;

/// <summary>
/// The View namespace.
/// </summary>
/// <autogeneratedoc />
namespace Console.Views;

/// <summary>
/// Class TileDef.
/// Implements the <see cref=""TileDef{Tiles}"" /></summary>
/// <seealso cref=""TileDef{Tiles}"" />
public class TileDef : TileDefBase
{
    /// <summary>
    /// The tile definition string
    /// </summary>
    private readonly string[][] _vTileDefStr = [
        [@""  ""], //_00
        [@""|_""], //_01
        [@""##""], //_02
        [@""/\""], //_03
        [@""''""], //_04
        [@""||""], //_05
        [@""<}""], //_06
        [@""\/""], //_07
        [@""{>""], //_08
        [@""=-""], //_09
        [@"",,""], //_10
        [@""-=""], //_11
        [@""++""], //_12
        [@""++""], //_13
        [@""++""], //_14
        [@""++""], //_15
        [@""==""], //_16
    ];


    /// <summary>
    /// The tile colors
    /// </summary>
    private readonly byte[][] _vTileColors = [
        [0x07], //_00
        [0x4F], //_01
        [0x0C], //_02
        [0xAE], //_03
        [0x0E], //_04
        [0x0E], //_05
        [0xAE], //_06
        [0xAE], //_07
        [0xAE], //_08
        [0x0E], //_09
        [0x0E], //_10
        [0x0E], //_11
        [0x0E], //_12
        [0x0E], //_13
        [0x0E], //_14
        [0x0E], //_15
        [0x0E], //_16
    ];

    /// <summary>
    /// Gets the tile definition.
    /// </summary>
    /// <param name=""tile"">The tile.</param>
    /// <returns>(string[] lines, (System.ConsoleColor fgr, System.ConsoleColor bgr)[] colors).</returns>
    /// <autogeneratedoc />
    public override (string[] lines, (ConsoleColor fgr, ConsoleColor bgr)[] colors) GetTileDef(Enum? tile)
    {
        (string[] lines, (ConsoleColor fgr, ConsoleColor bgr)[] colors) result = default;
        result.lines = GetArrayElement(_vTileDefStr, tile);

        result.colors = new (ConsoleColor fgr, ConsoleColor bgr)[result.lines.Length * result.lines[0].Length];
        byte[] colDef = GetArrayElement(_vTileColors, tile);
        for (var i = 0; i < result.lines.Length * result.lines[0].Length; i++)
            result.colors[i] = ByteTo2ConsColor(colDef[i % colDef.Length]);
        return result;
    }

    public TileDef():base() => TileSize = new Size(2,1);
}";

        private const string csExp2Code = @"//***************************************************************
// Assembly         : SomeGame_Base
// Author           : Mir
// Created          : 01-01-2025
//
// Last Modified By : Mir
// Last Modified On : 01-01-2025
// ***********************************************************************
// <copyright file=""VTileDef.cs"" company=""JC-Soft"">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using ConsoleDisplay.View;
using System.Drawing;

/// <summary>
/// The View namespace.
/// </summary>
/// <autogeneratedoc />
namespace Console.Views;

/// <summary>
/// Class TileDef.
/// Implements the <see cref=""TileDef{Tiles}"" /></summary>
/// <seealso cref=""TileDef{Tiles}"" />
public class TileDef : TileDefBase
{
    /// <summary>
    /// The tile definition string
    /// </summary>
    private readonly string[][] _vTileDefStr = [
        [@""    ""], //Empty
        [@""[_|]"", @""[|_]""], //Wall
        [@"". . "", @"" . .""], //Floor
        [@""    "", @""<==>""], //Destination
        [@""|()|"", @""(__)""], //Player
        [@"" <°)"", @"">_|_""], //PlayerOverDest
        [@""/^^\"", @""\__/""], //Stone
        [@"",--,"", @""|__|""], //StoneInDest
        [@"". . "", @"" . .""], //Floor_Marked
        [@""[_|]"", @""[|_]""], //Wall_N
        [@""|_|]"", @""_|_]""], //Wall_W
        [@""|_|]"", @""_|_]""], //Wall_NW
        [@""[_|]"", @""[|_]""], //Wall_S
        [@""[_|]"", @""[|_]""], //Wall_NS
        [@""|_|]"", @""_|_]""], //Wall_WS
        [@""|_|]"", @""_|_]""], //Wall_NWS
        [@""[_|_"", @""[|_|""], //Wall_E
        [@""[_|_"", @""[|_|""], //Wall_NE
        [@""|_|_"", @""_|_|""], //Wall_WE
        [@""|_|_"", @""_|_|""], //Wall_NWE
        [@""[_|_"", @""[|_|""], //Wall_SE
        [@""[_|_"", @""[|_|""], //Wall_NSE
        [@""|_|_"", @""_|_|""], //Wall_WSE
        [@""|_|_"", @""_|_|""], //Wall_NWSE
        [@"" <°)"", @"">-||""], //Player_W
        [@"" <°)"", @"">_|_""], //PlayerOverDest_W
        [@""(°°)"", @""|^^|""], //Player_S
        [@"" <°)"", @"">_|_""], //PlayerOverDest_S
        [@""(°> "", @""||-<""], //Player_E
        [@"" <°)"", @"">_|_""], //PlayerOverDest_E
    ];


    /// <summary>
    /// The tile colors
    /// </summary>
    private readonly byte[][] _vTileColors = [
        [0x07], //Empty
        [0x47], //Wall
        [0x2A], //Floor
        [0x2F], //Destination
        [0x2F], //Player
        [0x2F], //PlayerOverDest
        [0x6E], //Stone
        [0x6F], //StoneInDest
        [0x2A], //Floor_Marked
        [0x47], //Wall_N
        [0x47], //Wall_W
        [0x47], //Wall_NW
        [0x47], //Wall_S
        [0x47], //Wall_NS
        [0x47], //Wall_WS
        [0x47], //Wall_NWS
        [0x47], //Wall_E
        [0x47], //Wall_NE
        [0x47], //Wall_WE
        [0x47], //Wall_NWE
        [0x47], //Wall_SE
        [0x47], //Wall_NSE
        [0x47], //Wall_WSE
        [0x47], //Wall_NWSE
        [0x2F], //Player_W
        [0x2F], //PlayerOverDest_W
        [0x2F], //Player_S
        [0x2F], //PlayerOverDest_S
        [0x2F], //Player_E
        [0x2F], //PlayerOverDest_E
    ];

    /// <summary>
    /// Gets the tile definition.
    /// </summary>
    /// <param name=""tile"">The tile.</param>
    /// <returns>(string[] lines, (System.ConsoleColor fgr, System.ConsoleColor bgr)[] colors).</returns>
    /// <autogeneratedoc />
    public override (string[] lines, (ConsoleColor fgr, ConsoleColor bgr)[] colors) GetTileDef(Enum? tile)
    {
        (string[] lines, (ConsoleColor fgr, ConsoleColor bgr)[] colors) result = default;
        result.lines = GetArrayElement(_vTileDefStr, tile);

        result.colors = new (ConsoleColor fgr, ConsoleColor bgr)[result.lines.Length * result.lines[0].Length];
        byte[] colDef = GetArrayElement(_vTileColors, tile);
        for (var i = 0; i < result.lines.Length * result.lines[0].Length; i++)
            result.colors[i] = ByteTo2ConsColor(colDef[i % colDef.Length]);
        return result;
    }

    public TileDef():base() => TileSize = new Size(4,2);
}";
        private const string csExp0Json = @"[[0,{""lines"":[""    "",""    ""],""colors"":[{""fgr"":0,""bgr"":0},{""fgr"":0,""bgr"":0},{""fgr"":0,""bgr"":0},{""fgr"":0,""bgr"":0},{""fgr"":0,""bgr"":0},{""fgr"":0,""bgr"":0},{""fgr"":0,""bgr"":0},{""fgr"":0,""bgr"":0}]}],[1,{""lines"":[""=-=-"",""-=-=""],""colors"":[{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6}]}],[2,{""lines"":[""\u2500\u2534\u252C\u2500"",""\u2500\u252C\u2534\u2500""],""colors"":[{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4}]}],[3,{""lines"":["" \u2553\u2556 "",""\u2593\u2591\u2592\u2593""],""colors"":[{""fgr"":14,""bgr"":0},{""fgr"":14,""bgr"":0},{""fgr"":14,""bgr"":0},{""fgr"":14,""bgr"":0},{""fgr"":10,""bgr"":2},{""fgr"":2,""bgr"":2},{""fgr"":2,""bgr"":0},{""fgr"":2,""bgr"":2}]}],[4,{""lines"":[""\u2310\u00B0@)"","" \u2321\u2321\u203C""],""colors"":[{""fgr"":15,""bgr"":6},{""fgr"":15,""bgr"":6},{""fgr"":15,""bgr"":6},{""fgr"":15,""bgr"":6},{""fgr"":15,""bgr"":6},{""fgr"":15,""bgr"":6},{""fgr"":15,""bgr"":6},{""fgr"":15,""bgr"":6}]}],[5,{""lines"":[""/\u00AF\u00AF\\"",""\\__/""],""colors"":[{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6}]}],[6,{""lines"":[""]\u00B0\u00B0["",""_!!_""],""colors"":[{""fgr"":10,""bgr"":1},{""fgr"":0,""bgr"":10},{""fgr"":0,""bgr"":10},{""fgr"":10,""bgr"":1},{""fgr"":10,""bgr"":1},{""fgr"":0,""bgr"":10},{""fgr"":0,""bgr"":10},{""fgr"":10,""bgr"":1}]}],[7,{""lines"":[""\u25C4\u00B0@["",""_!!\\""],""colors"":[{""fgr"":10,""bgr"":1},{""fgr"":0,""bgr"":10},{""fgr"":0,""bgr"":10},{""fgr"":10,""bgr"":1},{""fgr"":10,""bgr"":1},{""fgr"":0,""bgr"":10},{""fgr"":0,""bgr"":10},{""fgr"":10,""bgr"":1}]}],[8,{""lines"":[""]oo["",""_!!_""],""colors"":[{""fgr"":10,""bgr"":1},{""fgr"":0,""bgr"":10},{""fgr"":0,""bgr"":10},{""fgr"":10,""bgr"":1},{""fgr"":10,""bgr"":1},{""fgr"":0,""bgr"":10},{""fgr"":0,""bgr"":10},{""fgr"":10,""bgr"":1}]}],[9,{""lines"":[""]@\u00B0\u25BA"",""/!!_""],""colors"":[{""fgr"":10,""bgr"":1},{""fgr"":0,""bgr"":10},{""fgr"":0,""bgr"":10},{""fgr"":10,""bgr"":1},{""fgr"":10,""bgr"":1},{""fgr"":0,""bgr"":10},{""fgr"":0,""bgr"":10},{""fgr"":10,""bgr"":1}]}],[10,{""lines"":[""/\u2568\u2568\\"",""\\__/""],""colors"":[{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6}]}],[11,{""lines"":["" \u002B*\u2229"",""\u2558\u2550\u25CA@""],""colors"":[{""fgr"":15,""bgr"":6},{""fgr"":15,""bgr"":6},{""fgr"":15,""bgr"":6},{""fgr"":15,""bgr"":6},{""fgr"":15,""bgr"":6},{""fgr"":15,""bgr"":6},{""fgr"":15,""bgr"":6},{""fgr"":15,""bgr"":6}]}],[12,{""lines"":[""    "",""    ""],""colors"":[{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6}]}],[13,{""lines"":[""\u2500\u2534\u252C\u2534"",""\u2500\u252C\u2534\u2500""],""colors"":[{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4}]}],[14,{""lines"":[""\u252C\u2534\u252C\u2500"",""\u2534\u252C\u2534\u2500""],""colors"":[{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4}]}],[15,{""lines"":[""\u252C\u2534\u252C\u2534"",""\u2534\u252C\u2534\u2500""],""colors"":[{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4}]}],[16,{""lines"":[""\u252C\u2534\u252C\u2500"",""\u2534\u252C\u2534\u252C""],""colors"":[{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4}]}],[17,{""lines"":[""\u252C\u2534\u252C\u2534"",""\u2534\u252C\u2534\u252C""],""colors"":[{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4}]}],[18,{""lines"":[""\u252C\u2534\u252C\u2500"",""\u2534\u252C\u2534\u252C""],""colors"":[{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4}]}],[19,{""lines"":[""\u252C\u2534\u252C\u2534"",""\u2534\u252C\u2534\u252C""],""colors"":[{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4}]}],[20,{""lines"":[""\u2500\u2534\u252C\u2500"",""\u2500\u252C\u2534\u2500""],""colors"":[{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4}]}],[21,{""lines"":[""\u2500\u2534\u252C\u2534"",""\u2500\u252C\u2534\u2500""],""colors"":[{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4}]}],[22,{""lines"":[""\u252C\u2534\u252C\u2500"",""\u2534\u252C\u2534\u2500""],""colors"":[{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4}]}],[23,{""lines"":[""\u252C\u2534\u252C\u2534"",""\u2534\u252C\u2534\u2500""],""colors"":[{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4}]}],[24,{""lines"":[""\u252C\u2534\u252C\u2500"",""\u2534\u252C\u2534\u252C""],""colors"":[{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4}]}],[25,{""lines"":[""\u252C\u2534\u252C\u2534"",""\u2534\u252C\u2534\u252C""],""colors"":[{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4}]}],[26,{""lines"":[""\u252C\u2534\u252C\u2500"",""\u2534\u252C\u2534\u252C""],""colors"":[{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4}]}],[27,{""lines"":[""\u252C\u2534\u252C\u2534"",""\u2534\u252C\u2534\u252C""],""colors"":[{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4}]}]]";
        private const string csExp1Json = @"[[0,{""lines"":[""  ""],""colors"":[{""fgr"":7,""bgr"":0},{""fgr"":7,""bgr"":0}]}],[1,{""lines"":[""|_""],""colors"":[{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4}]}],[2,{""lines"":[""##""],""colors"":[{""fgr"":12,""bgr"":0},{""fgr"":12,""bgr"":0}]}],[3,{""lines"":[""/\\""],""colors"":[{""fgr"":14,""bgr"":10},{""fgr"":14,""bgr"":10}]}],[4,{""lines"":[""\u0027\u0027""],""colors"":[{""fgr"":14,""bgr"":0},{""fgr"":14,""bgr"":0}]}],[5,{""lines"":[""||""],""colors"":[{""fgr"":14,""bgr"":0},{""fgr"":14,""bgr"":0}]}],[6,{""lines"":[""\u003C}""],""colors"":[{""fgr"":14,""bgr"":10},{""fgr"":14,""bgr"":10}]}],[7,{""lines"":[""\\/""],""colors"":[{""fgr"":14,""bgr"":10},{""fgr"":14,""bgr"":10}]}],[8,{""lines"":[""{\u003E""],""colors"":[{""fgr"":14,""bgr"":10},{""fgr"":14,""bgr"":10}]}],[9,{""lines"":[""=-""],""colors"":[{""fgr"":14,""bgr"":0},{""fgr"":14,""bgr"":0}]}],[10,{""lines"":["",,""],""colors"":[{""fgr"":14,""bgr"":0},{""fgr"":14,""bgr"":0}]}],[11,{""lines"":[""-=""],""colors"":[{""fgr"":14,""bgr"":0},{""fgr"":14,""bgr"":0}]}],[12,{""lines"":[""\u002B\u002B""],""colors"":[{""fgr"":14,""bgr"":0},{""fgr"":14,""bgr"":0}]}],[13,{""lines"":[""\u002B\u002B""],""colors"":[{""fgr"":14,""bgr"":0},{""fgr"":14,""bgr"":0}]}],[14,{""lines"":[""\u002B\u002B""],""colors"":[{""fgr"":14,""bgr"":0},{""fgr"":14,""bgr"":0}]}],[15,{""lines"":[""\u002B\u002B""],""colors"":[{""fgr"":14,""bgr"":0},{""fgr"":14,""bgr"":0}]}],[16,{""lines"":[""==""],""colors"":[{""fgr"":14,""bgr"":0},{""fgr"":14,""bgr"":0}]}]]";
        private const string csExp2Json = @"[[0,{""lines"":[""    "",""    ""],""colors"":[{""fgr"":7,""bgr"":0}]}],[1,{""lines"":[""[_|]"",""[|_]""],""colors"":[{""fgr"":7,""bgr"":4}]}],[2,{""lines"":["". . "","" . .""],""colors"":[{""fgr"":10,""bgr"":2}]}],[3,{""lines"":[""    "",""\u003C==\u003E""],""colors"":[{""fgr"":15,""bgr"":2}]}],[4,{""lines"":[""|()|"",""(__)""],""colors"":[{""fgr"":15,""bgr"":2}]}],[5,{""lines"":["" \u003C\u00B0)"",""\u003E_|_""],""colors"":[{""fgr"":15,""bgr"":2}]}],[6,{""lines"":[""/^^\\"",""\\__/""],""colors"":[{""fgr"":14,""bgr"":6}]}],[7,{""lines"":["",--,"",""|__|""],""colors"":[{""fgr"":15,""bgr"":6}]}],[8,{""lines"":["". . "","" . .""],""colors"":[{""fgr"":10,""bgr"":2}]}],[9,{""lines"":[""[_|]"",""[|_]""],""colors"":[{""fgr"":7,""bgr"":4}]}],[10,{""lines"":[""|_|]"",""_|_]""],""colors"":[{""fgr"":7,""bgr"":4}]}],[11,{""lines"":[""|_|]"",""_|_]""],""colors"":[{""fgr"":7,""bgr"":4}]}],[12,{""lines"":[""[_|]"",""[|_]""],""colors"":[{""fgr"":7,""bgr"":4}]}],[13,{""lines"":[""[_|]"",""[|_]""],""colors"":[{""fgr"":7,""bgr"":4}]}],[14,{""lines"":[""|_|]"",""_|_]""],""colors"":[{""fgr"":7,""bgr"":4}]}],[15,{""lines"":[""|_|]"",""_|_]""],""colors"":[{""fgr"":7,""bgr"":4}]}],[16,{""lines"":[""[_|_"",""[|_|""],""colors"":[{""fgr"":7,""bgr"":4}]}],[17,{""lines"":[""[_|_"",""[|_|""],""colors"":[{""fgr"":7,""bgr"":4}]}],[18,{""lines"":[""|_|_"",""_|_|""],""colors"":[{""fgr"":7,""bgr"":4}]}],[19,{""lines"":[""|_|_"",""_|_|""],""colors"":[{""fgr"":7,""bgr"":4}]}],[20,{""lines"":[""[_|_"",""[|_|""],""colors"":[{""fgr"":7,""bgr"":4}]}],[21,{""lines"":[""[_|_"",""[|_|""],""colors"":[{""fgr"":7,""bgr"":4}]}],[22,{""lines"":[""|_|_"",""_|_|""],""colors"":[{""fgr"":7,""bgr"":4}]}],[23,{""lines"":[""|_|_"",""_|_|""],""colors"":[{""fgr"":7,""bgr"":4}]}],[24,{""lines"":["" \u003C\u00B0)"",""\u003E-||""],""colors"":[{""fgr"":15,""bgr"":2}]}],[25,{""lines"":["" \u003C\u00B0)"",""\u003E_|_""],""colors"":[{""fgr"":15,""bgr"":2}]}],[26,{""lines"":[""(\u00B0\u00B0)"",""|^^|""],""colors"":[{""fgr"":15,""bgr"":2}]}],[27,{""lines"":["" \u003C\u00B0)"",""\u003E_|_""],""colors"":[{""fgr"":15,""bgr"":2}]}],[28,{""lines"":[""(\u00B0\u003E "",""||-\u003C""],""colors"":[{""fgr"":15,""bgr"":2}]}],[29,{""lines"":["" \u003C\u00B0)"",""\u003E_|_""],""colors"":[{""fgr"":15,""bgr"":2}]}]]";
        private const string csExp3Json = @"[[0,{""lines"":[""    ""],""colors"":[{""fgr"":0,""bgr"":0},{""fgr"":0,""bgr"":0},{""fgr"":0,""bgr"":0},{""fgr"":0,""bgr"":0}]}],[1,{""lines"":[""=-=-"",""-=-=""],""colors"":[{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6}]}],[2,{""lines"":[""\u2500\u2534\u252C\u2500"",""\u2500\u252C\u2534\u2500""],""colors"":[{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4}]}],[3,{""lines"":["" \u2553\u2556 "",""\u2593\u2591\u2592\u2593""],""colors"":[{""fgr"":14,""bgr"":0},{""fgr"":14,""bgr"":0},{""fgr"":14,""bgr"":0},{""fgr"":14,""bgr"":0},{""fgr"":10,""bgr"":2},{""fgr"":2,""bgr"":2},{""fgr"":2,""bgr"":0},{""fgr"":2,""bgr"":2}]}],[4,{""lines"":[""\u2310\u00B0@)"","" \u2321\u2321\u203C""],""colors"":[{""fgr"":15,""bgr"":6},{""fgr"":15,""bgr"":6},{""fgr"":15,""bgr"":6},{""fgr"":15,""bgr"":6},{""fgr"":15,""bgr"":6},{""fgr"":15,""bgr"":6},{""fgr"":15,""bgr"":6},{""fgr"":15,""bgr"":6}]}],[5,{""lines"":[""/\u00AF\u00AF\\"",""\\__/""],""colors"":[{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6}]}],[6,{""lines"":[""]\u00B0\u00B0["",""_!!_""],""colors"":[{""fgr"":10,""bgr"":1},{""fgr"":0,""bgr"":10},{""fgr"":0,""bgr"":10},{""fgr"":10,""bgr"":1},{""fgr"":10,""bgr"":1},{""fgr"":0,""bgr"":10},{""fgr"":0,""bgr"":10},{""fgr"":10,""bgr"":1}]}],[7,{""lines"":[""\u25C4\u00B0@["",""_!!\\""],""colors"":[{""fgr"":10,""bgr"":1},{""fgr"":0,""bgr"":10},{""fgr"":0,""bgr"":10},{""fgr"":10,""bgr"":1},{""fgr"":10,""bgr"":1},{""fgr"":0,""bgr"":10},{""fgr"":0,""bgr"":10},{""fgr"":10,""bgr"":1}]}],[8,{""lines"":[""]oo["",""_!!_""],""colors"":[{""fgr"":10,""bgr"":1},{""fgr"":0,""bgr"":10},{""fgr"":0,""bgr"":10},{""fgr"":10,""bgr"":1},{""fgr"":10,""bgr"":1},{""fgr"":0,""bgr"":10},{""fgr"":0,""bgr"":10},{""fgr"":10,""bgr"":1}]}],[9,{""lines"":[""]@\u00B0\u25BA"",""/!!_""],""colors"":[{""fgr"":10,""bgr"":1},{""fgr"":0,""bgr"":10},{""fgr"":0,""bgr"":10},{""fgr"":10,""bgr"":1},{""fgr"":10,""bgr"":1},{""fgr"":0,""bgr"":10},{""fgr"":0,""bgr"":10},{""fgr"":10,""bgr"":1}]}],[10,{""lines"":[""/\u2568\u2568\\"",""\\__/""],""colors"":[{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6}]}],[11,{""lines"":["" \u002B*\u2229"",""\u2558\u2550\u25CA@""],""colors"":[{""fgr"":15,""bgr"":6},{""fgr"":15,""bgr"":6},{""fgr"":15,""bgr"":6},{""fgr"":15,""bgr"":6},{""fgr"":15,""bgr"":6},{""fgr"":15,""bgr"":6},{""fgr"":15,""bgr"":6},{""fgr"":15,""bgr"":6}]}],[12,{""lines"":[""    ""],""colors"":[{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6}]}],[13,{""lines"":[""\u2500\u2534\u252C\u2534"",""\u2500\u252C\u2534\u2500""],""colors"":[{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4}]}],[14,{""lines"":[""\u252C\u2534\u252C\u2500"",""\u2534\u252C\u2534\u2500""],""colors"":[{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4}]}],[15,{""lines"":[""\u252C\u2534\u252C\u2534"",""\u2534\u252C\u2534\u2500""],""colors"":[{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4}]}],[16,{""lines"":[""\u252C\u2534\u252C\u2500"",""\u2534\u252C\u2534\u252C""],""colors"":[{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4}]}],[17,{""lines"":[""\u252C\u2534\u252C\u2534"",""\u2534\u252C\u2534\u252C""],""colors"":[{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4}]}],[18,{""lines"":[""\u252C\u2534\u252C\u2500"",""\u2534\u252C\u2534\u252C""],""colors"":[{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4}]}],[19,{""lines"":[""\u252C\u2534\u252C\u2534"",""\u2534\u252C\u2534\u252C""],""colors"":[{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4}]}],[20,{""lines"":[""\u2500\u2534\u252C\u2500"",""\u2500\u252C\u2534\u2500""],""colors"":[{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4}]}],[21,{""lines"":[""\u2500\u2534\u252C\u2534"",""\u2500\u252C\u2534\u2500""],""colors"":[{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4}]}],[22,{""lines"":[""\u252C\u2534\u252C\u2500"",""\u2534\u252C\u2534\u2500""],""colors"":[{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4}]}],[23,{""lines"":[""\u252C\u2534\u252C\u2534"",""\u2534\u252C\u2534\u2500""],""colors"":[{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4}]}],[24,{""lines"":[""\u252C\u2534\u252C\u2500"",""\u2534\u252C\u2534\u252C""],""colors"":[{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4}]}],[25,{""lines"":[""\u252C\u2534\u252C\u2534"",""\u2534\u252C\u2534\u252C""],""colors"":[{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4}]}],[26,{""lines"":[""\u252C\u2534\u252C\u2500"",""\u2534\u252C\u2534\u252C""],""colors"":[{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4}]}],[27,{""lines"":[""\u252C\u2534\u252C\u2534"",""\u2534\u252C\u2534\u252C""],""colors"":[{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4}]}]]";
        private const string csExp0JsonC = "H4sIAAAAAAACCuSYXWvbMBSG7wf7D67uNtxEOrbc2GBIYlpWxmAXYb1ogsmH2pip9vDHRQn575NsJ50bue1GIHOVwFH86sN+9MrSIRt0nbMHgjx0w9KYpeEVn2fL9SISP8fzjPW+JSvGe5OIs8w0fsjychXlE5bl8pqlWZTEPulh+TWNoOB5kTI/ZkWezrlpfC8WPFp+ZY+T5CeL/bjgHJnlPQF5G3SdXT78yh+RdzfnGTPRTbTK18izTfSFRffrHHmwrZpbyLvd7B4W/zEEj2KWiUpkiI8Yuyxm5scPaJnwJM3Kfnf3adlrUZZb86TKbCvFGoWoUPxz/1ygiOirUYhdj+bsxz+p1EACFdK0AIqxjJYtIwSVIjCfqiqxaiCq1Oi0vqv99CCnlBrolnJhShxqldExamC3vHZJGaFSXvEaH878X0u7VQh7CVoVrGjToLVbjLaIcBPjMR5+ki+kVIDsI7aCV5x1Duf5FFKDlapY+xJzdFXHqbR2Gob9Dr6zjgpvVrlYxVtBF56dhS1wu4VFDjY8gl+SFB2PMVYD7qJtQwrsep3WcMLC7tENlNYlSYcNc5VIw91yBDoeCbh+N+EIbtlKgDqDOnZ5KyGk5RDEGMafBSCAWx+CtOSlpaXBaNjBY4HAP6Wi/7eB1tsTOMt+XwkcactpGmjPktfmhHSXnb6dfe/7e2F3juI7BF1kvziK791kH2jsu6uv74D1/ZMCiL7nO4C+5ztY+p7vYOu7zwPVeJ/XOK8D7fK62fY3AAAA//8DAHJ3zxnQGgAA";
        private const string csExp1JsonC = "H4sIAAAAAAACCrTTQWvCMBQH8Ptg36E8r7E2aZ0z0IuuMBmDHcQdVhlVo4bFdjTJQWq/+9bqpM6UwaoUUvrS/v6PNMlgpNgGA4XJmAsWLLiyn5MFE9IeM6mkPeGymHiIVFQWQu04ZBDEekP6yDp+VM59P7NU8iT2se0UF7KGWiidMj9mWqWRQNaLngk+f2LbcfLBYj/WQgAqmyBAMxjJYPOptkCXkZAMwStfqDVQguCR8dVaAcX5/nUX6Fv2071TIQSPmfyeBMuCKbq9gXkiklSWry9XKdAegllxd3J0VpnmRfGgYpO6ezeruHtAvCNbKZ24xOS2WjUuOWu3UjpxXZPbCcMa2DsouCJXaie0Z6LLvdDbj39kGCJ+JXSNi71r7N7Vde4O8wssTM/Ih50L0PcmOitbDy7A902832684Nh4GBFqDhvPY9tvDpPa3T3Yj80j3OtHeNeP6F4/wnhe/f/+5Gn+BQAA//8DAPgKlDHiBgAA";
        private const string csExp2JsonC = "H4sIAAAAAAACCrTVbWuCQAAH8PeDfQe5VwWn6WlZgm/WhMUYDBbtRdnRw1XSpcMHxuj87vOhtjWu4uBS8BT15/+PT3swSMnOAA54i7bRfBZqo4B8vkRLQhNtGFDySFZQGZVr3jJIhyRJk2KbxEkQha6h6eUMlX5G0ywmbkiyNJ5RqLxmcxosnsnXMNqS0A0zSgGsroWAsweDxNt9pF/AWc1oQiB4D5bpBjgWBE8kWG9S4KC8PtwEznh/DKn/IWgQkqTYCZRiKuxq8OH9HVhENIqT6rzVOgaODcG8HPXcz3P4gxk8bIyZX2Bjhv3LmHWKIR6mKVqVrBj5mKEfNHSqmRd6TjJdN/uuW43eGbfNdy2eyxpNVrgNjJtiWpubss5XLh/05jGvhxkWwzs8vDWdTialOcG4dcazDl7n1LN5HlRVWHAMY3Yl3T+tK/V292Q+iQb3JWG1hoU1Q6qGpDY1pWqW1KZtqVrnTFNcN2Vimi1V655piuumglpPpoZ0mU2RIVVDUpuaUjVL4NOuMib2aUc3/XEg7rvSqN1fnU2norHtm8buXolteuXPhTG1vqCg3pOf3c+/AQAA//8DAGV5pO89CgAA";
        private const string csExp3JsonC = "H4sIAAAAAAACCuSYW2vbMBTH3wf7Dq7eNtxEOrbc2GBIYlpWxmAPYX1ogslFbcxUe/jyUEK++yTbSedG7mUEgqsEjuMjWdJPf/mcQzboOmcPBHnohqUxS8MrPs+W60Ukfo7nGev9SFaM9yYRZ5lp/JLXy1WUT1iWy3uWZlES+6SH5dc0goLnRcr8mBV5Ouem8bNY8Gj5nT1Okt8s9uOCc2SWcwLyNug6u3z4kz8i727OM2aim2iVr5Fnm+gbi+7XOfJgW3W3kHe72S0W/zMEj2KWiUZkiA+amZ8/oWXCkzQrH7i7T8vui/K6Nf/LM9tKZz03Uc3tn/vngktYX70EYtejOfvxT+pqIIEKaVoAxVhay5YWgsojMJ+aKmfVQTSp0Wk9q/20kFO6GuiW8iRJHGqV1jFqYLe8d0lpofK8ojU+3Pl3u3anEPYuaPVgRZ8Grd0itEWEmhiP8fCLoJX4FpC9xVbwirLO4T6fwtVgpSrWvsQcXdV2KqWdhmG/g++so8KbVSpW9lbQhWdnYQvc7mCRg4BH8EsuxYPHGKsBd9EWkAK7Pqc1nJCwe3QDpXRJ0mHBXCXScHccgY5HAq7fTTiCW0IJUGdQ2y6HEkJakiDGMP4qAAHcOgnSkpeWkgajYQfTAoH31Y7H323r7dWWZX+saou0FSANtGeVZnNDustO386+1/2jsDtH0R2CLrJfHEX3brIPNNbd1Vd3wPr+owBE3/wOoG9+B0vf/A62vnEeqMZxXuO6DrSr62bbvwAAAP//AwD4UonvLhoAAA==";

        private const string csExp0XmlC = "H4sIAAAAAAACCuyc7U/TQBzH35v4P5S+U2G3B1Ak7UBUlIiZcQtoxCxdd2MnR0vaqxNf8cIXxvjCBzZ96ztNSOAFr4yv9qf0L/FWBuiiHehuNPANS7jeQz/07vrh1/Y6Y/rFGteeU89nrmPqmVRa16hju1XmrJh6IGpjk/p0/uIFY9HiAS0F65wWakXhyeIie0kXmC8KtRueZ20UaoXKM2oLTe7Q8ade+MzU60KsTxHSaDRSjVzK9VZINp3OkEf3F4p2na5ZY8zxheXYVD9sVe3fSpd/j6YZ84KuZfJL1HOoV57jlm/XK0wmZy2fpu67VcpTJcapP6otdn7frjJRor7obHcPVx5t52dUuxlwEXjUdGggPIuPag+CCmf2PbpRclepYzoB5wbZBx6ys1FSbiyxqqjnxw2yn+jm3qVspS7yWYN0U1FDctQy2knuoHq3D284G6WNddrNlfnWfoYm+3NKyISpyz6aYo7Q82mDWP2rF+VQcdrpAf2wmqzImUP9XzJklh8Naz5Kk+7GCSoYpHefhu1y1+uhzMm+vNnJ/y1bFtRWvPwst+xVg3SSPaWVo9JKT6lB/rxPoIA6JyiD9JxqvWowyB8F8y/ayajRjjlmjsVqR5abirTzmHLuNv46FLcsb/WgymCGHkAAzyFwmJrKqtFUuLUZbu2FW9syEaurqOJ2VHdTkbaW6kzQ2OF5SKuDmgyAAXZGYcPUUk7RRVvY/Bg2W/EXbmHrY9h6H7Y+yMTpRFIDjZ/B+n/WHY9SJ/Y86tYYDO6XHSYHOdAOVXCEw9TTuKKo6e279u7MpfjbSuHbL53P5o9TC5cGGz6DB9554g3TUxNqPEXaO+2d5VhNLZfLBLehAAQQt6GOIaqrakT1tL3b3n0SK6ryyEhZkaj6B7iz8mnxcJ6TDPRiYYio4fYhRixpfThMCV1TdFX36ZW8qutroWVYCBbCiJ17C00qCoVcF4EQJjRGDArqq6DrihQ0094NW99jLURgIVgIIwYLyRWUilZuk7D5VX5w/xpAAHH/ejCuUrTcW7tyOXz9LX65UvNz2HwXfnozgwUB4IGHBQFxnsqevbfh8K8RQADPWDyVU/5eyh7eSwEMMLyXciIvqVr53XXNMd6X24OXAAMMXvrNSxPKvbQHLwEGGLx0Ii9dTU68tA0vAQYYvNTx0rXkxEvwEmCAwUuRlyYRL2HaAQZYwrx0HfESph1ggCXs6yvT+P5KTDvAAEuYlzJYv4RpBxhgCfNSFuuXMO0AAyxhXsph/RKmHWCAJcxL43geh2kHGGAJ89IEnsdh2gEGWMK8hPXemHaAAZY0L2G9N6YdYIAp85JB5gVdy8m0QRYt+ZWapWCd00KtGJ3qRfaSLjBfyDb7TQuVZ9QW+Z8AAAD//wMAxVw3ulmCAAA=";
        private const string csExp1XmlC = "H4sIAAAAAAACCuyabWvbMBDH3w/2HYwL7YskVh7arU1ll65PG2vJWEK6QWEo9iXRqsjBkpdm6777ZMfLOtPSMSwbgsib8//udNL9cBzOwUd3M2Z9g0jQkLt2y2naFnA/DCifuHYsx419+8h7+QIPCYthEM8Z9MZ9GSl3n36HSypkb3wcRWTZG/dGX8GXllqQi+6doK49lXLeRWixWDiLjhNGE9RuNlvo09Vl35/CjDQoF5JwH+x1VvB8lq32Y1n4nYRZyxsOKIOzgErnKgyACWcAQgpnSEXiOCWSpELtjMez9kHdWsensrrOjq5Onnzq1knMZByByyGWEWF160M8YtR/D8tBeAvc5TFjGK2Kr/fRTk11cU0DOfXaGK2MTH0LdDKVXgujzEoT0Z/MdJHO7/Csn8d8OVjOIVOVTlaCpXrblcpwbdWvLuXS9poYkefD+wobg6QD9jpMBTLKQTwQlCRSxF6yy8x8EI/yCdgPWRjlljhXjTpJ9L9k5RhPIu9CnRCjxMo5R0p6w4h/i9Eo58Xo8SXLqoRR7pj5nmP0KLn/4dnSw/P+iyae11Mq4ck2n5Lo9iMERSEttFiZVNt6qG5taaKatrGMm7SwQmXS7OihiW400fwMjIWLJ/t8EQHwooAWWatMprt6mO7sVMO00Ju0yFplMt3T9Cy9N0wrY/pKD9NtJg9/mq/fyrC+1oP1BhmmlTHd18P0x/ZEHhqslWE90IPVbZinanVTB01jpHrdQK0OqqZZUsM1UKuDqmmUVKsZqNVB7Riomwd110DdPKh7BurmQdU0VHLND6UioK7elycvyTH6538jeL8AAAD//wMA9EziKuEgAAA=";
        private const string csExp2XmlC = "H4sIAAAAAAACCuzb607bMBQA4P+T9g5RfiCQ2rg3oBQniIt20UCd1go2AbXS1m29GgclzkonPxTPwJPNaQODFCXbJKueZPWPe3KStOfTOSJygQd3N9T6gcOIBMy1q07FtjAbBEPCxq4d81G5aR94b9/Ac5/GuBvfUtwedXgoD3fIT3xKIt4eHYahP2+P2v3veMAteUEWte4i4toTzm9bAMxmM2dWd4JwDGqVShV8PTvtDCb4xi8TFnGfDbD9dNaw+Cxbfh7Lgh85vql6nWAa9H2GjvwIO+cEz86CIaaR0yUUn+BRyXqeULLO028qv2jyKlnHMeVxiF2GYx76tGR9jvuUDD7heTeYYuaymFIIlvd6um1tsZRvLsiQT7wGBMtFGv2AyXjCvRoE6WpxIvh95uIi9cf0tHyHbN6d3+I0KuP+MmDJUra4XLi2LE+LMG57FQj84vSOVKI4qYT9lCYTKWE4ehaQoWgh6i3WIH3zFwkQZK8JBwENwsxd3slaHifxF2F5YDQOvfeyCBAkq8zBvgwdUX8whaCfOQrBa5eEIHPzbLEgeLXk/wJRVQNxicR1LsSlQNfrgDjxw+kXPNSRoqaGwrGcgp6QGcooMGa5FmmGfhr1NU2oDcr3XXdjzPcVmVxMCMf/p0lDjYnY3BK5JpsIbRmNrMa2og5JOuDhfiu/S2R/IIEMShZlRw0K6PWuckGuEAKKNL5hSoNZLsdjin4eu2o8SuVyKddDICTW1h36cjTN31gaaeyZhw9tngMVPZGLIgtkLFYsqsZCG4uamVHaWNSNhTYWDTOjtLHYNhbaWOwom1GoaEYJY/HSYtdYaGPRVDajUNGMMhYZiz1joc2mX8XMKG0sqsZCG4uamVHaWNSNhTYWjTXvupaFMLuuKypmL1xHFUXP4psP90UmotczbbIKsmvaREOVprI2SWqevzEuRDnBMyorKnumV1SqLH/OnvyGHYI//t8A7xcAAAD//wMAGxib7G8wAAA=";
        private const string csExp3XmlC = "H4sIAAAAAAACCuydy27TQBSG90i8g+sdkGRy6TWy0xsqIFoFkagFURQ5ySQxndqV7ZCGVRcsEOqi0CZlyw6kSs0iK8Qqj+InYXLpLSpOSzPB0F/NYjIe+4tnTr8c2+NYmd3eZNJbatm6aahyJBSWJWrkzLxuFFW57BSC0/Js4u4dZVVjZZoubzGaLKQciy9O6e/osm47ycK8ZWnVZCGZfUNzjsQ3aNjxbVtX5ZLjbMUJqVQqoUosZFpFEg2HI+TFynIqV6KbWlA3bEczclQ+XSs/eC2Zfx5JUp44dDOSWKOWQa3MEtPsXCmr8+KCZtPQipmnLJTWGbUD0uVtAtJqb7f5Xrf/AtJimTlli6oGLTuWxgLSs3KW6bmntJo2N6ihGmXGFNIFn36GaKfI36zpeaeUGFdIt9CrfUz1YslJRBXSK3VWJGdrdjYSO2ne68t5o5qubtFeLa/XuhUS79e4wwuqzPsqrhuOnAgrRBvcPMWHjNF2j8inzXhDphvUPlfBq+zO8CY6ZdJ7c24N0r+KkjOZafVtZIl31WK7/kI1X1AoWokFpuU2FNIu9i3Nni3N9i1VyOXbBOpGKIX0DV9/NCnk0pj8k0iNiIlUNagGL4nUswZ8uSoolF9SxszKb4fioWZtnDQZztADCOAtBI5SU1ExmnIPdtyDpntwxAueuuo0POq03RGkrbWS7lDP4XlO88MKBsAA+09ho9RSTFCe79b23Vpd8lZSfd+tf3Lrn3nh72RSQ82fwbo565FFqeH5f9RrMRzcuQ36BznUDhWwh6PU07igrGl3r9WYu+dpJ8nd/dp+7fz8a+nScNNn8MC7TbxRempCjKdI67h1vO6pqfVMhuA0FIAA4jTUFUQ1KUZUr1uNVuOVp6gyY2MZQaIanOAu8AuNo7lOMtSDhRGiRtuHGDG/9eEoJTQl6Kju8D0/qhtooXVYCBbCiN16C00LSoVME4kQAhojBgUNVNCMIAXNtRpu/YenhQgsBAthxGAhPoNS0GRf4ta+8RfOXwMIIM5fD8dVgqZ7Sw/uux++e09Xqn1xa3vu4cc5TAgADzxMCPDyVPSfu4EK33z4qr0QwjHhtyw0ccsCYIDhloVreUnUpOCea65wK1UTXgIMMHjpgpcmhHupCS8BBhi8dC0vTfonXzqClwADDF5qe2nKP/kSvAQYYPBSx0vTyJcQdoAB5jMvzSBfQtgBBpjPftkwjJ82RNgBBpjPvBTB/CWEHWCA+cxLUcxfQtgBBpjPvBTD/CWEHWCA+cxL47geh7ADDDCfeWkC1+MQdoAB5jMvYb43wg4wwPzmJcz3RtgBBpgwL3WfgN5+7LlCrvxs+cQvAAAA//8DAJFA8RKvfgAA";

        private const string csExp0TextC = "H4sIAAAAAAACCnzXT28TRxjH8TsS78H4FNpA5nlmZ2d2pUih+XMpSKiEciiVZWALVh0bOXbb9MShh6riQEsSeu2tlZDIgVPVU17KvpLOzuzu81tYDJF4bCzlo/HsfDW789VsmbO7euXe5OciTzb56pUvi5PDk+dF/qBYzIrF6GA6Pn787NHEj1+Mj4ubd+ZPiunNw8m0ON4cfF39u/9ksjwsjpfV62JxPJnPtummqv5uDnZX0+VqUWzPitVyMZ5uDu6uHk0nj6vfMf++mG3PVtNp+JUqV4ON/aPny5PrV6/cnsyKY5V7zG01UvnOcOD/DMMrkle78+l84T/m9bvVx9RPSoWRZGQZtYyJjEbGVEZbjx5GOQ029iaLZeOi4KLg2r6xfWMYXlUu/2K7dVFwUXCl+2EkGVlGLWMio5ExldHWo3dxzoONB+PptHFxcHFwlacvytP35elbPwzDu9S8+zb8x4vWycHJwZkchJFkZBm1jImMRsZURluP3qlz7dfPb4/JbLz0e6Ph6sDV8estz16XZ+fhK9aRev66PP+9PP/DDy1VB6qOX/V+GElGllHLWFH5VhgrKnMYw1cdR1u/66lJngw27k7HJ8WiUSZBmcRFffnq8mLn+jC8EfZh+fKv6ufFfy0xCcQkfusHYSQZWUYtYyKjkTGV0dajJ5rcDDbuLeezohGaIDRBuHX57vLdw2F4XQEfjkZbLc0EmpENaWRDGtmQRjakkQ1pZEMa2ZAGN2Sap/4JnhVHJ6P7zxtdGnRp0H17eXF58c0wvK50o2vXRq0uDbo06OhWGCvdLRVGllHLBxIZjXwgldHWH/A6m9tG99Xk6bP2abYBaOMX/OYX/wUHoW2ED1uhDUIrQitCK0IrQitCK0IrQotCl7tGuPdj+5i44HNxAefzgHMfL58LOCc4JzgnOCc4JzgnOCc4h7gszxrc7eK7dvWyoMuibufyojz/dxjeqIBbCMwCMBNgJsBMgJkAMwFmAswEmCGQVE6qfjTuzH+YzJ6253UMCcWSbJVnf/uf8JCQ+vgpodgTUnBwKzi5FRzdCs5uBYe3gtNbwfGt8HEhHxZqjpu9Yvyk9daBiYUZfP5Z+es/w/hGOBnP/izPXpVvftsRc90akmOHSM4dIjl4iOTkIZKjh0jOHiI5fIjw9CHOyVdnb3V01GaaYneIO6Em/rjUFFNDDEvLsLQMS8uwtAxLy7C0DEvLnaXVOekYx9H91hmDQ/rDQL4fxrc/WUiK3SEtjSQtkSQtlSQtmSQtnSQtoSQtpSSNqaQkp6R2P2jdMUFUN6jWNWGnpHa/73HHGFEC7gTcCbgTcCfgTsCdgDvpuE1OpllvgccykfkQHhfcrIHHVJEBuAG4AbgBuAG4AbgBuOnA05zSGr7XumOzKO1f8PRD91txx4hRCu4U3Cm4U3Cn4E7BnYI77bhtTrZZcIHHlpHtX3C7Bh7bRhbgFuAW4BbgFuAW4BbgtgN3Oblmhws8Ro5c/4q7NfDYPXIAdwB3AHcAdwB3AHcAdx14llPWbnGRxwBS1r/k2Rp5DCJlIM9AnoE8A3kG8gzkGcgzlLPPn6rl++1dITaRVf9tQa25LsQ+soILg4Ibg4Irg4I7g4JLg4Jbg4Jrg+q4fQapWXGBxzgy9Z7iTGvgMZJMACeAE8AJ4ARwAjgBnDpwH0Ru9rjA6+sZ9+5x5k+fhlzf0PCKhnc0vKThLQ2vaXhPw4ta56bGvpBtNkEew8m6d4+zXiOP4WQIJ0M4GcLJEE6GcDKEkyGc3Akn+0Y24dwTeCwn95eTk08/nBzLyVBOhnIylJOhnAzlZCgnQzm5U072jWzLCfKYTu5PJ5s18phOhnQypJMhnQzpZEgnQzoZ0smddLKvZNqe5CKP8eT+ePKaeHKMJ0M8GeLJEE+GeDLEkyGeDPHkTjzZZ9LKUS70mE/uzyevySfHfDLkkyGfDPlkyCdDPhnyyZBPlnz+DwAA//8DAFcVviKsEwAA";
        private const string csExp1TextC = "H4sIAAAAAAACCmTTX4ubQBQF8PdAvoPsPmxL3Oyc8b/U0pKmL91CoWGfFkLanQep1RIVuu32u/fmXhlHJS83o/wcx3N2TV93OZL16mv5x+Tax3r1yTwfnn+Z/OFQVmb/VHbbz82TqdrtwbRdu30o28uFD6fuxAubfd3/1Jnv2ft5mf6bc1s2dYGtuvx8b9dXXX82RW367nyqfO9L/60qv1+e1/wwdVH3VcWPV/lRKe+VwK/Xq/uyNq3KaW/36qjyd1eed7Ve7ZqqOdOypvGyrH6rhEcMI0kgCXMJLIGll6OVwBJYCj/yiGEkSZOk55JmSbN0fW0lzZKWPe14xDCSFJAUzKWApYClu0crBSwFLL3f84hhJCkkKZxLIUshSzc3VgpZCmVPex4xjCRFJEVzKWIpknN6sVLEUjRKkSvFJMVzKWYpZunNPyvFLMXj28Xu2yUkJXMpYSlh6fHOSglLySglrpSSlM6llKWUpb9vrZSylI5S6koZSdlcyljKWCpurZSxlI3nlLnnBAo5FiGHpBwSc98fwyk5hxo5qIlHUccy6kPWJey3xegNaYfjYeJR4LEIPCTxkMhvNqMnmYd2PD3xKPZYxB6SewQLT5KPwPGCiUfhxyL8kPQjXHiSfzgFwKQBoApgUQFIBxAtPGkBnBpg0gNQEbAoAqQJkCoUzveQLiB2vNh6/wEAAP//AwCpBQShpwUAAA==";
        private const string csExp2TextC = "H4sIAAAAAAACCmzW207bQBCA4Xsk3iHKVZCS4BmfLRK1BSpVhRYpCC6ArELZShGujXJom8oPxTPwZLVnbOPJLtywUvR5vOzPcppvs03iOocHs+U/nXhDPDz4qnfXuxedzPLn/HGRqU+LtR7fLPWfy/xJp+vx9TLVZ/rnsNf9wLB3o1frZZ5NYOxU38Pe6TbdbFd6kuntZrVIh72r7WO6/FHx+bPOJtk2TelpTnL+62Wz6w1q+ejw4GKZ6bWTlNNcOMpJPvR75VefVvC+Os3TfFV+DMofq485f52QREhuF2lqgEAgEHinioc+rSrwrlAPLQgEAoEeg5h8TvN8ZYhIIpI47o1pROQRy2UrIolIIn4k0U3O9HqzzBabctMM1yXXFa/uknsymUxb1yXXZfczuV5ylS522hzVI9IjshgcFX1aVeRAqaOW9Ij0uqRfk99/61U1tEH7RPs87cnba6WVy8qeqkK1tk+237WDZLbJM22QAZEBkcfz+X2fVpV4r9RxKwYkBiQG5ySGLH7JrKOG5IbkDkejYZ9WlVsoVbRuSG7ILk8a8RFQl4vVs34y4IjgSJyEyDwJEcFR9yTEdFjVN4OMiYzFcY3N4xoTGXePKzhs3poJcFTAVRVNBJyVEhVwV+AIGOphLXJdF0gZLHIdmCgMkOWZCXNkgLJbtITLnQEK2K1HtsicGbhSdi0ylwaukL16ly0y1wae3AzPshkcHHhC9pttttBcG/iS9i009wa+oAOmz02Ym4Og2Q3V52W9G+91AGcHgYDDemaLzNVBKOXQInN4EAo5qvfZInN2EDWbwXLUbEZH5vIgEnLTno3m/CCWdGyhuUAQCWKd4MyUkRtER2wHOuZ2IDeIokFsGrTRHCGCpMFCc4QorzlsTrSFru86FPuBaO4H1tedyBDd9khbbA4RXWm7FptDRBEiNpee5Q8econoydsJOcXpqOjYnCKKyw/3bz/bMzhJ3LsB0XIFIjeJ4hLEoJnfzB25SuQqB2+vDc5dFvN55wW4SwwEHu6/gOUhHCiGey8QWl6AC8VQPCNqXsDyu+VEMWpeYMr/JXGjRTE6ebe5UYyEHe/Pb3kGt4rx3vyxZX6OFeP2Gf8BAAD//wMA42meKAQLAAA=";

        private const string csExp0BinC = "H4sIAAAAAAACCpJhYGBgAWImIM4MTy3KSy2Kd8tJLE7OSMoEMp0Si1P1fPNTUnP0QjJzUot1FMJAtGtKZklIanEJiJ9aVJyZn2drqGcAgjoKzqU5JaVFqbZ5qaUlRYk5OgoBpUk5mcneqZUh+dmpebZ5pTk5DFAbWRSAAExwMGACRpgqW11bXRYgtgWp4mNDhUxQVTyPpjQ8mrLl0ZQ1QAaEswbMbwDp4mdBhcxQXRwKj6ZOfjR1mgLPo2mTH02b+GjaJCADbA8UcjEBAQMTEyyU2B/1TDi0wUGTS+FRz0IQatgDtoANFbJClbPpH1p/aH0MS0x8vD4257PB1MUe2nBoQzRLvKJiPEgdFyMDFwMXFyOMZodbP70FaD1YYQw2hRywUIvNz8dtHCfMuFiHQxseTdvFoo9DIRcsoPQfTV0BRLh9wg3ziYK21qOOlVyPps54NHXCo+ldDtjChwdrEkA3kxdL5G4hGLl8CF1rkJPEFry6+LHo2kJQlwAhu9Zg0yVIyC6suoTIskuYLLtEyMpWomTFlxhZ8SVOVnxJkBWGkmSFoRRZdkmTZRcAAAD//wMAJ9q0ZskFAAA=";
        private const string csExp1BinC = "H4sIAAAAAAACCmTOT2vCMBgG8Np1/ulc675BwIOHxrrtJhgvmycZeCiehFFnDmFZCk1yGOp3Nyk+vZSHF5JfkjfvSxAEoaueK7svhOSbkzD5V3XiUucF10bne6H9wWdpygayjbJ/70tK2vsNuz2vtagUe8tffSj5sNLYmjPFralLScnOHqX42fL/ovrliikrZXD/PSTEDzJw6YEu357SKI0wYzid+uXY5QG0OHhK4iSOQLNZQy6Pba8LqA9aXfFwADosQEPQeQ0agdgcvWIQpaAn0JyBxqAsAz13KelS2qVJO0Tb/gYAAP//AwAfetHlxQEAAA==";
        private const string csExp2BinC = "H4sIAAAAAAACCnSRW0vDMBiGu5p1OnWb57PksoO20/t1N+qVCIJjN9v60bkIZaGFHhAhP2q/Yb/MZG3GFL6EhOR5875J+O4NwyBymHKEH8kimYWxN4rY91syZzzzhhFnz+zLoSO1eplH+ZBleSb3LM2iJPYfvQfVHfpU8LxImR+zIk9D7tD3Ysajz1f2M0wWLPbjgnOjuolQ2dZTTe4aRk3zMYgpGQuYrjkxNfeoJ8/LWfGmufMnp+/7A8Xbpv4JEXZXEBugW/J6xeu0v1p2yQAElIKlDb0gmJAJQE/xltXQ3HFdhwgAsT5v7SIP2kM+0Nw8SHHY8H2EHyA5hwhvITlthHe2ckDliJIfIfx4KwdUTsVPEH6K5Jwh/BzJuUD45b9CuqIsjHmFVfi6Eix7tVSKCILKcoNZbrUgLQNKhHD7pXCHOX4BAAD//wMAXe6gukYDAAA=";
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.
        private VisTileData testClass;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.
        private Dictionary<int, (Action<VisTileData>, int)> _testDefs = new();

        [TestInitialize]
        public void TestInitialize()
        {
            // Arrange
            testClass = new VisTileData();

            _testDefs.Add(0, ((t) => t.SetTileDefs<Werner_Flaschbier_Base.Model.Tiles>(new Werner_Flaschbier_Base.View.VTileDef()), 29));
            _testDefs.Add(1, ((t) => t.SetTileDefs<Enum29>(new Snake_Console.View.TileDef()), 16));
            _testDefs.Add(2, ((t) => t.SetTileDefs<Sokoban.ViewModels.TileDef>(new Sokoban.View.VisualsDef()), 23));
            _testDefs.Add(3, ((t) => t.SetTileDefs<Werner_Flaschbier_Base.Model.Tiles>(new Console.Views.TileDef()), 29));
        }

        [TestMethod()]
        public void GetTileDefTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        [DataRow(0, 0, new[] { "    ", "    " })]
        [DataRow(0, 1, new[] { "=-=-", "-=-=" })]
        [DataRow(0, 2, new[] { "─┴┬─", "─┬┴─" })]
        [DataRow(0, 3, new[] { " ╓╖ ", "▓░▒▓" })]
        [DataRow(0, 4, new[] { "⌐°@)", " ⌡⌡‼" })]
        [DataRow(1, 0, new[] { "  " })]
        [DataRow(1, 1, new[] { "|_" })]
        [DataRow(1, 2, new[] { "##" })]
        [DataRow(1, 3, new[] { "/\\" })]
        [DataRow(1, 4, new[] { "''" })]
        [DataRow(2, 0, new[] { "    ", "    " })]
        [DataRow(2, 1, new[] { "[_|]", "[|_]" })]
        [DataRow(2, 2, new[] { ". . ", " . ." })]
        [DataRow(2, 3, new[] { "    ", "<==>" })]
        [DataRow(2, 4, new[] { "|()|", "(__)" })]
        [DataRow(3, 0, new[] { "    " })]
        [DataRow(3, 1, new[] { "=-=-", "-=-=" })]
        [DataRow(3, 2, new[] { "─┴┬─", "─┬┴─" })]
        [DataRow(3, 3, new[] { " ╓╖ ", "▓░▒▓" })]
        [DataRow(3, 4, new[] { "⌐°@)", " ⌡⌡‼" })]
        public void SetTileDefsTest(int iAct1, int iAct2, string[] asExp)
        {
            _testDefs[iAct1].Item1(testClass);
            var act = testClass.GetTileDef(iAct2).lines;
            Assert.IsNotNull(act);
            for (int i = 0; i < asExp.Length; i++)
                Assert.AreEqual(asExp[i], act[i]);
        }

        [TestMethod()]
        //[DataRow(0, EStreamType.Code, new[] { csExp0CodeC })]
        //[DataRow(1, EStreamType.Code, new[] { csExp1CodeC })]
        //[DataRow(2, EStreamType.Code, new[] { csExp2CodeC })]
        //[DataRow(3, EStreamType.Code, new[] { csExp0CodeC })]
        [DataRow(0, EStreamType.Json, new[] { csExp0JsonC })]
        [DataRow(1, EStreamType.Json, new[] { csExp1JsonC })]
        [DataRow(2, EStreamType.Json, new[] { csExp2JsonC })]
        [DataRow(3, EStreamType.Json, new[] { csExp3JsonC })]
        [DataRow(0, EStreamType.Xml, new[] { csExp0XmlC })]
        [DataRow(1, EStreamType.Xml, new[] { csExp1XmlC })]
        [DataRow(2, EStreamType.Xml, new[] { csExp2XmlC })]
        [DataRow(3, EStreamType.Xml, new[] { csExp3XmlC })]
        [DataRow(0, EStreamType.Text, new[] { csExp0TextC })]
        [DataRow(0, EStreamType.Binary, new[] { csExp0BinC })]
        [DataRow(1, EStreamType.Text, new[] { csExp1TextC })]
        [DataRow(1, EStreamType.Binary, new[] { csExp1BinC })]
        [DataRow(2, EStreamType.Text, new[] { csExp2TextC })]
        [DataRow(2, EStreamType.Binary, new[] { csExp2BinC })]
        [DataRow(1, EStreamType.Xml, new[] { csExp1XmlC })]
        [DataRow(2, EStreamType.Xml, new[] { csExp2XmlC })]
        [DataRow(3, EStreamType.Xml, new[] { csExp3XmlC })]
        public void LoadFromStreamTest(int iAct1, EStreamType iAct2, string[] asExp)
        {
            var sAct = Decompress(asExp[0]);

            var ms = new MemoryStream();
            ms.Write(Encoding.UTF8.GetBytes(sAct));
            ms.Position = 0L;
            var act = testClass.LoadFromStream(ms, iAct2);
            Assert.IsTrue(act);

            var cExp = new VisTileData();
            _testDefs[iAct1].Item1(cExp);

            if (!cExp.Equals(testClass))
            {
                AssertAreEqual(cExp, testClass, ["KeyType"]);
            }
            else
                Assert.AreEqual(cExp, testClass);
        }

        [TestMethod()]
        [DataRow(0, EStreamType.Binary, new[] { csExp0BinC })]
        [DataRow(0, EStreamType.Code, new[] { csExp0CodeC })]
        [DataRow(0, EStreamType.Json, new[] { csExp0JsonC })]
        [DataRow(0, EStreamType.Text, new[] { csExp0TextC })]
        [DataRow(0, EStreamType.Xml, new[] { csExp0XmlC })]
        
        [DataRow(1, EStreamType.Binary, new[] { csExp1BinC })]
        [DataRow(1, EStreamType.Code, new[] { csExp1CodeC })]
        [DataRow(1, EStreamType.Json, new[] { csExp1JsonC })]
        [DataRow(1, EStreamType.Text, new[] { csExp1TextC })]
        [DataRow(1, EStreamType.Xml, new[] { csExp1XmlC })]
        
        [DataRow(2, EStreamType.Binary, new[] { csExp2BinC })]
        [DataRow(2, EStreamType.Code, new[] { csExp2CodeC })]
        [DataRow(2, EStreamType.Json, new[] { csExp2JsonC })]
        [DataRow(2, EStreamType.Text, new[] { csExp2TextC })]
        [DataRow(2, EStreamType.Xml, new[] { csExp2XmlC })]
      
        [DataRow(3, EStreamType.Json, new[] { csExp3JsonC })]
        [DataRow(3, EStreamType.Code, new[] { csExp0CodeC })]
        [DataRow(3, EStreamType.Xml, new[] { csExp3XmlC })]
        public void WriteToStreamTest(int iAct1, EStreamType iAct2, string[] asExp)
        {
            _testDefs[iAct1].Item1(testClass);
            var ms = new MemoryStream();
            var act = testClass.WriteToStream(ms, iAct2);
            Assert.IsTrue(act);

            ms.Position = 0L;
            var sr = new StreamReader(ms).ReadToEnd();
            if (iAct2 == EStreamType.Json)
                sr = sr.Replace("],", "],\r\n");
            var sExp = Decompress(asExp[0]);
            File.WriteAllText($"C:\\Temp\\VisTileData{iAct1}_{iAct2}_Expected.txt", sExp);
            File.WriteAllText($"C:\\Temp\\VisTileData{iAct1}_{iAct2}_Actual.txt", sr);
            if (sExp != sr)
            {
                Debug.WriteLine("Result:");
                Debug.WriteLine(sr);
                Debug.WriteLine("Packed:");
                Debug.WriteLine(Compress(sr));
                AssertAreEqual(sExp, sr);
            }
            else
                Assert.AreEqual(sExp, sr);

        }

        [TestMethod]
        [DynamicData(nameof(TestData),DynamicDataSourceType.Method)]
        public void WriteToStreamTest2(string name, int iAct1, EStreamType iAct2, string asExpFile)
        {
            _testDefs[iAct1].Item1(testClass);
            var ms = new MemoryStream();
            var act = testClass.WriteToStream(ms, iAct2);
            Assert.IsTrue(act);

            ms.Position = 0L;
            var sr = new StreamReader(ms).ReadToEnd();
            if (iAct2 == EStreamType.Json)
                sr = sr.Replace("],", "],\r\n");

            var sExp = File.ReadAllText(asExpFile);

            if (sExp != sr)
            {
                Debug.WriteLine("Result:");
                Debug.WriteLine(sr);
                AssertAreEqual(sExp, sr);
            }
            else
                Assert.AreEqual(sExp, sr);

        }

        private static string Compress(string input)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var gzipStream = new GZipStream(memoryStream, CompressionLevel.SmallestSize))
                using (var writer = new StreamWriter(gzipStream))
                {
                    writer.Write(input);
                }
                return Convert.ToBase64String(memoryStream.ToArray());
            }
        }

        private static string Decompress(string input)
        {
            try
            {
                using (var memoryStream = new MemoryStream(Convert.FromBase64String(input)))
                using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                using (var reader = new StreamReader(gzipStream))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                return input;
            }
        }
        private enum Enum29
        {
            _00 = 0,
            _01, _02, _03, _04, _05, _06, _07, _08, _09,
            _10, _11, _12, _13, _14, _15, _16, _17, _18, _19,
            _20, _21, _22, _23, _24, _25, _26, _27, _28, _29,

        }
    }
}