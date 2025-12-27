using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using static BaseLib.Helper.TestHelper;
using System.IO.Compression;

namespace VTileEdit.Models.Tests
{
    [TestClass()]
    public class VisTileDataTests
    {
        private const string csExp0CodeC = "H4sIAAAAAAACCqxXwW7bRhC9G/A/bAgUkGxZpIW0h1hyrUhM0MJGi8qODrIg0NRKJkByhV1KNmsY8KGHosghbWyn195awEB88KnoSZ/CL+lwl5S4lEg7SgiBXO68NzM7MztaqurG513ra6qK6oxh58T2UXy9QC3i4NeGg3svDYYFaOydEopQAnRgUS5qUGx4uJ8Uadtb8Ktola9DBEftG8xDB6RvDSzAvvQTCmTRD+6CArTxZS6uq2qSkU+t4amHBpaNa8qbQ3g08aBsMgWZxBkZrl9Tvm9stcjAU3Y5KbwaM17BLKJIXkZ120Z8miGKGaYT3C8LQ+rMklBSZWPHMai/W1Xj0Zdd3ZhZ7hC1fOZhZyd+bRCXEVigxUa24ZffWPhsR4aWm9Q4g1eYDv1JOCpeD08xCmnIhZJgI8PE5QinysCqMfbIELuYhgVBTKSCYEaKPeEusCxjDdtgDMUpEXPfOSMbO9iFEHvgS5VhjEyKBzUlwl2ET3apgMEFnwBs2IxkEtbXRuMT2zKRmbQMNRiNxBa4WF8LayDlcDwVRsgDOOrjgeVankVcxDwKMU3QVJk3otYEwgRFY/SJC9tPEDrdThf1JpHxlkdRDXUEI7w6e0r4ULolUKo7I8+XZLWt2pZSQnsKPGsC1LSoJ2GC66vg+iG4voMBx/KJOz53JThtw7Zlm8HN++DmFgn87fvg9vfg9g8YRDYw8yzXCNctm3r7bnq/V+QsFLz9K/xd/Sc4P0I1YirB1enH6cdjjj7u9VSBa3nExRKsO72f3nc4rPfsWS+KhYsdv3c0ku1/+AXsz6DHSehP4caU9RKyXGvzTF5Wd296H9z+y7FqCruPB7JWNbj5G35Ll3VAJrMaiSO9uRH8+o+I882fwc274MNve8mINaFeMuqhCQXmZ+X6ISfXvaMU6y5dIQ/LWO0s1kMe66j9ZGN3CVrzycaSrKPmSsbaK1pb4D1pq/X01ZKmr5Y1fcW06avlTV8xcfqKmdNXTV1E7PK/pqf1e5PYhLJPaPInvocTLb7B+akWr51r2tL+rp1/oy/r6dr581fL+jYo0kto2b1S5/cKn6mIcU4nB8OvljftmUvpNq2db3MbdW1+365ntepH0elu/ShB7tmPwlNtW17WQptOxSPVkudpSvVjKU9Sz5Ul7WxOtqiZTcoWtXNYOTI9m5UtauewcmTNHFqOrJ3HS+31nJ3+GkeHztTxrpy95/ncyKCGww/MNSWkKrtxzyhXVS5Moin2xtRlu4X4DIhsy8WshArRCT06OvN2gQZDWkLLBCdDWgSuaEpFMBTrTZhadkTnnUocgskEU2r1MVp0ZdGHHONh4KIzbEF3x863fO1FYetinpnPNQOfXGPbgwYKqTFgtDNXLURlrhcA4FCdUsPXxadEIXnGLgnvZsWQoJtxh3bhA+hJziXtlvexO/RO0YbkTUfrRoJuwl/x1xCuLPz+yHJY/GPMHY7ZAzBfmBgUWUDVduBRRZ/iCTA2N4tzfQsh6Fhd0PwSnDwklXDV3JOCcLdjoa8iz+OVFaVUhHUY6YvmL+NgR5UXV0vxxQl8cRWKqLbL51rWzzgKfzgsPC9VQtWX/wMAAP//AwByNfB7/RAAAA==";
        private const string csExp1CodeC = "H4sIAAAAAAACCqxVXW/TMBR9n7T/cBWESLcuSbuNAWsKWxkTCMRDJ15KNbmp21lK4spOu4Wx/86NHXfx6iIQWJGSnPt1rnNyHYZ7/7Z2d8IQzqSk2SQtwaw3MOQZvSQZvT4nkmqnZXHDBUDD6QsTyjQQlBR02jRFnQO8ulH3uPJQXp+JLOALn7IZQ9/zspHANn3NNxLA3v9ZKlcv4YtSsPlNATOW0tj7doW393QWJNKDhGcLkpex92lwMOSzwuuroGoN1nF+0oLaHsBZmoKCJQgqqVjRaaALhetKOklPLrOMiLLfC83T/+1uKVk+h2EpC5qdmtcBzyXHBplcpKQMvjF6e2q7Bu8FucVXhCs+DaL69eqGQhUGOUpCLkhCg9ovtB17ZFnwOc2pqATBEwjRsA4yTBQFua3YICVSgvkkGvuYLVKa0Ry3uEAuPUkpJILOYq/2u6/u8sHDghuc0Jmkkm8N2N1ZLCcpSyBpVkYN1k/6F7jf3ak08ISwgaodKtAdpnTGclYwnoMsBO5pIyy04xaCrXCbUDRkynP8/XTAaDwaw/WqLj4sBMQw0hHVGr3zALxxG1NeR5GF/7w2eMfCnz0zeNfCw+8GP7TwFy8MfmTn/2nwYwvvPRj8pYV/Dw1+YuH3fYO/svD4wOCvLbzdrvGO3e9BbHC73/19g3e34Idb8KMtuN1vvK5b9ztWWv4zgSQ85UL+hSomZUEbmhio+CeaiO6iE4ckorujDw5FoPfAIYjo7uzCoQf0vnDIoQEfu5O8dMMnbviVO/drJ9yJ3HDHDXfdsLvLjrvLzrEbtiTwGwFc0np4PRkTwXYpKGxBBMnU4I29KtTrGykFvVAZm96CFkuRy75vZgmkLKeyDX496esRrFQEs7log8swmYsWxmqttrCQydso5Rr1SsB6mPIVFYJNKWxS2eTwm+LVxtWz0L/Il9lb1XtL17p//Cb/WgaP7mVa4H+Fn4bg0+ljam0KVF50QEJnQpDyQh9JfnNWtzW7tRga4Yn5cXM8SP+IXLNu8Jnm8+IG9iw2o2hcG8YNvnpiVJ1V59g2wnqQPBI20TMs76+IAIah0SneevA3TDBif7/1mG9jC0ZsjJnPkeQV71ZdKya+pjti8LxmbjprWZ+i0mGdr8YfzGbXyjNqab2Z4MnttyDuK2zIftB6+6tHv9vuVKkffgEAAP//AwAjWdJIRQsAAA==";
        private const string csExp2CodeC = "H4sIAAAAAAACCqxX/26bSBD+P1LeYYV0EqQ2+KzeVWqw7xLHqXpq0kpE9R+ugzZ47aDjh7WL3XAh79Rn6JN1dhccFgxH1KJI4J3vm/l2mJ0MlnXyc9fxkWWhM8ZIeBekqLjeIicOyTscEvccMyJB2+Q+pgiVQFc+FaYJJTghy7Jp8Hsf/oaD4R8cIVAfMEvQVbz0Vz5gz9OSA9X0Mao5QCe/5hK+bC/epNRf3ydo5QdkpH2+gdsFWZke05AXhxscpSPtn0nfiVeJNhYkfk32PN0zUG430VkQILHMECWM0B1ZmjKQtY8kndhsG4aYpmPbKp5+7e62zI/WyElZQsLT4uckjlgMG/TZJsCp+dknX09VqHlB8Vf4CctcT0mo/HlzTxCnoQhKgm2wR8wcZ6lAG2+TeE0iQnlBxB6ywLAnFUqEBNYUbBJgxlDxSuTa+3ATkJBEkOIEtNiMEORRshppOe6R39mTBgFrmgCMAxY3Eo6PNtu7wPeQV44MNZg/ySPweHzEa6AiuFjiGUoAjpZk5Ud+4scRYgmFnJZolsrbUH8HaYKiwcs4guMnCfPFfIHcXR7cSSgaoblk8Gv+t8Zv2qIHTqfhJkkV29zNFloPwUPmLiRohoNAwZjIRAIDd1NiLoM4pvUgHGSPRmMJuiAs8SPMN6dAM93IBFR3XUNCP0GlkYpD+/s3Q8DGbuaWYR93hHLfCty6vf0i0F9c15JoJ4kjooB6/X5PgDLXzUqg91HNX9Om3StM/yXLLkl0r9VtFyhXRc06oa5nnUI63YQ53ZQ5HaXVorpF1KyEm3ZCXU+rQd0iaBk26wa7nnWL6nQU53RU53SVVwGWqr6fZeWqr9TJ/x+PCkH//q1gZLe3qmvnpa6dquuxPCxZ1rcVz9OXes4JC9Huu/VQLw5iyl7QOO/ShJTa5kTwK21z8DB4c7BnDh5evznUJwcPw7ODvREMly39cG+u9cCK5UDbGzz8OT3Y6sBw2dLeVK31lqbsUWljqmXWzGk2Oc2kZtOshdVimzazmk2zFlaLzWmhtdhmbbyqUSmISvoPFksbpvImDvNbMO60A185zy2n+R3Jh7XKWGQ2n2uxtsEUh2LQHGmcqo2LvmDaljCW0ZQkWxqxsV7MTijwI8J6SM8n23zkFC0Brda0hw4Z7tbUAK5sPAYEKvyWQh0abUU3ksNjDAmi/pKgupS6hpbgPHH57KdPo234l9i7IWM9Pr+gnw0DnyrbIIEmCa8Gw9Pps2tpMoVfAICgM0pxOpUjuF6eTXtS3b4YSnSv6MIRfDh0EleOa34g0Tq5RyeKmvlgkRsWJb2y/fOd8bm9SbD8r/AsuGCvILy+wxT5QB2cws1GL1ECjFevjGd/tRTM/QV4PgeRN/GQ71oo0aXcuY9+y5UXOzOUV8HrMPeXrz8Vyc4rr6gW4+0dfKnoBhqNxZrj/0fy9PNH/XVvyF0//QAAAP//AwBpvTAoNRAAAA==";
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
        private const string csExp3Json = @"[[0,{""lines"":[""    ""],""colors"":[{""fgr"":0,""bgr"":0},{""fgr"":0,""bgr"":0},{""fgr"":0,""bgr"":0},{""fgr"":0,""bgr"":0}]}],[1,{""lines"":[""=-=-"",""-=-=""],""colors"":[{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6}]}],[2,{""lines"":[""\u2500\u2534\u252C\u2500"",""\u2500\u252C\u2534\u2500""],""colors"":[{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4}]}],[3,{""lines"":["" \u2553\u2556 "",""\u2593\u2591\u2592\u2593""],""colors"":[{""fgr"":14,""bgr"":0},{""fgr"":14,""bgr"":0},{""fgr"":14,""bgr"":0},{""fgr"":14,""bgr"":0},{""fgr"":10,""bgr"":2},{""fgr"":2,""bgr"":2},{""fgr"":2,""bgr"":0},{""fgr"":2,""bgr"":2}]}],[4,{""lines"":[""\u2310\u00B0@)"","" \u2321\u2321\u203C""],""colors"":[{""fgr"":15,""bgr"":6},{""fgr"":15,""bgr"":6},{""fgr"":15,""bgr"":6},{""fgr"":15,""bgr"":6},{""fgr"":15,""bgr"":6},{""fgr"":15,""bgr"":6},{""fgr"":15,""bgr"":6},{""fgr"":15,""bgr"":6}]}],[5,{""lines"":[""/\u00AF\u00AF\\"",""\\__/""],""colors"":[{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6}]}],[6,{""lines"":[""]\u00B0\u00B0["",""_!!_""],""colors"":[{""fgr"":10,""bgr"":1},{""fgr"":0,""bgr"":10},{""fgr"":0,""bgr"":10},{""fgr"":10,""bgr"":1},{""fgr"":10,""bgr"":1},{""fgr"":0,""bgr"":10},{""fgr"":0,""bgr"":10},{""fgr"":10,""bgr"":1}]}],[7,{""lines"":[""\u25C4\u00B0@["",""_!!\\""],""colors"":[{""fgr"":10,""bgr"":1},{""fgr"":0,""bgr"":10},{""fgr"":0,""bgr"":10},{""fgr"":10,""bgr"":1},{""fgr"":10,""bgr"":1},{""fgr"":0,""bgr"":10},{""fgr"":0,""bgr"":10},{""fgr"":10,""bgr"":1}]}],[8,{""lines"":[""]oo["",""_!!_""],""colors"":[{""fgr"":10,""bgr"":1},{""fgr"":0,""bgr"":10},{""fgr"":0,""bgr"":10},{""fgr"":10,""bgr"":1},{""fgr"":10,""bgr"":1},{""fgr"":0,""bgr"":10},{""fgr"":0,""bgr"":10},{""fgr"":10,""bgr"":1}]}],[9,{""lines"":[""]@\u00B0\u25BA"",""/!!_""],""colors"":[{""fgr"":10,""bgr"":1},{""fgr"":0,""bgr"":10},{""fgr"":0,""bgr"":10},{""fgr"":10,""bgr"":1},{""fgr"":10,""bgr"":1},{""fgr"":0,""bgr"":10},{""fgr"":0,""bgr"":10},{""fgr"":10,""bgr"":1}]}],[10,{""lines"":[""/\u2568\u2568\\"",""\\__/""],""colors"":[{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6}]}],[11,{""lines"":["" \u002B*\u2229"",""\u2558\u2550\u25CA@""],""colors"":[{""fgr"":15,""bgr"":6},{""fgr"":15,""bgr"":6},{""fgr"":15,""bgr"":6},{""fgr"":15,""bgr"":6},{""fgr"":15,""bgr"":6},{""fgr"":15,""bgr"":6},{""fgr"":15,""bgr"":6},{""fgr"":15,""bgr"":6}]}],[12,{""lines"":[""    "",""    ""],""colors"":[{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6}]}],[13,{""lines"":[""\u2500\u2534\u252C\u2534"",""\u2500\u252C\u2534\u2500""],""colors"":[{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4}]}],[14,{""lines"":[""\u252C\u2534\u252C\u2500"",""\u2534\u252C\u2534\u2500""],""colors"":[{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4}]}],[15,{""lines"":[""\u252C\u2534\u252C\u2534"",""\u2534\u252C\u2534\u2500""],""colors"":[{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4}]}],[16,{""lines"":[""\u252C\u2534\u252C\u2500"",""\u2534\u252C\u2534\u252C""],""colors"":[{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4}]}],[17,{""lines"":[""\u252C\u2534\u252C\u2534"",""\u2534\u252C\u2534\u252C""],""colors"":[{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4}]}],[18,{""lines"":[""\u252C\u2534\u252C\u2500"",""\u2534\u252C\u2534\u252C""],""colors"":[{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4}]}],[19,{""lines"":[""\u252C\u2534\u252C\u2534"",""\u2534\u252C\u2534\u252C""],""colors"":[{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4}]}],[20,{""lines"":[""\u2500\u2534\u252C\u2500"",""\u2500\u252C\u2534\u2500""],""colors"":[{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4}]}],[21,{""lines"":[""\u2500\u2534\u252C\u2534"",""\u2500\u252C\u2534\u2500""],""colors"":[{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4}]}],[22,{""lines"":[""\u252C\u2534\u252C\u2500"",""\u2534\u252C\u2534\u2500""],""colors"":[{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4}]}],[23,{""lines"":[""\u252C\u2534\u252C\u2534"",""\u2534\u252C\u2534\u2500""],""colors"":[{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4}]}],[24,{""lines"":[""\u252C\u2534\u252C\u2500"",""\u2534\u252C\u2534\u252C""],""colors"":[{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4}]}],[25,{""lines"":[""\u252C\u2534\u252C\u2534"",""\u2534\u252C\u2534\u252C""],""colors"":[{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4}]}],[26,{""lines"":[""\u252C\u2534\u252C\u2500"",""\u2534\u252C\u2534\u252C""],""colors"":[{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4}]}],[27,{""lines"":[""\u252C\u2534\u252C\u2534"",""\u2534\u252C\u2534\u252C""],""colors"":[{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4}]}]]";
        private const string csExp0XmlC = "H4sIAAAAAAACCuzbx2vbMBR+8O/IdjjSD8T/AsUK3zzd87LBmkIUt67QUeWCiZESKhSK3ICAP9AOAq9rnNxK/n4gBsYiDG44pmGRgj2BneQPenLSKYoVXNpOMXmY6UmIZfhVW0VSfoCigumSltJAXUVMDXUVKzONpbGFaqm2pAFNXafKE+pY1lpDi8qfEUlcGtfMu2eWz6hzOB9/OfVUFh3AfmaI0NYZ5mOjWeHs6ktBTde4zpOrBOmh23DRdukZFlqPPX9MSgMBhbXz8/jUSewhgp1D3pu9APAAAA//8DAJFA8RKvfgAA";
        private const string csExp1XmlC = "H4sIAAAAAAACCuzbx2b9TMR+8O/IdjjSD8T/AsUK3zzd87LBmkIUt67QUeWCiZESKhSK3ICAP9AOAq9rnNxK/n4gBsYiDG44pmGRgj2BneQPenLSKYoVXNpOMXmY6UmIZfhVW0VSfoCigumSltJAXUVMDXUVKzONpbGFaqm2pAFNXafKE+pY1lpDi8qfEUlcGtfMu2eWz6hzOB9/OfVUFh3AfmaI0NYZ5mOjWeHs6ktBTde4zpOrBOmh23DRdukZFlqPPX9MSgMBhbXz8/jUSewhgp1D3pu9APAAAA//8DAJFA8RKuvfgAA";
        private const string csExp2XmlC = "H4sIAAAAAACCuzbx2b8TMR+8O/IdjjSD8T/AsUK3zzd87LBmkIUt67QUeWCiZESKhSK3ICAP9AOAq9rnNxK/n4gBsYiDG44pmGRgj2BneQPenLSKYoVXNpOMXmY6UmIZfhVW0VSfoCigumSltJAXUVMDXUVKzONpbGFaqm2pAFNXafKE+pY1lpDi8qfEUlcGtfMu2eWz6hzOB9/OfVUFh3AfmaI0NYZ5mOjWeHs6ktBTde4zpOrBOmh23DRdukZFlqPPX9MSgMBhbXz8/jUSewhgp1D3pu9APAAAA//8DAJFA8RKuvfgAA";
        private const string csExp3XmlC = "H4sIAAAAAACCuzbx2f9TMR+8O/IdjjSD8T/AsUK3zzd87LBmkIUt67QUeWCiZESKhSK3ICAP9AOAq9rnNxK/n4gBsYiDG44pmGRgj2BneQPenLSKYoVXNpOMXmY6UmIZfhVW0VSfoCigumSltJAXUVMDXUVKzONpbGFaqm2pAFNXafKE+pY1lpDi8qfEUlcGtfMu2eWz6hzOB9/OfVUFh3AfmaI0NYZ5mOjWeHs6ktBTde4zpOrBOmh23DRdukZFlqPPX9MSgMBhbXz8/jUSewhgp1D3pu9APAAAA//8DAJFA8RKuvfgAA";

        private VisTileData testClass;
        private Dictionary<int, (Action<VisTileData>, int)> _testDefs = new();

        [TestInitialize]
        public void TestInitialize()
        {
            // Arrange
            testClass = new VisTileData();

            _testDefs.Add(0, ((t) => t.SetTileDefs<Werner_Flaschbier_Base.Model.Tiles>(new Werner_Flaschbier_Base.View.VTileDef()), 29));
            _testDefs.Add(1, ((t) => t.SetTileDefs<Enum29>(new Snake_Console.View.TileDef()), 16));
            _testDefs.Add(2, ((t) => t.SetTileDefs<Sokoban.ViewModels.TileDef>(new Sokoban_Base.View.VisualsDef()), 23));
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
        [DataRow(0, 5, new[] { "/¯¯\"", "\__/""})]
        [DataRow(0, 6, new[] { "]°°[", "_!!_" })]
        [DataRow(0, 7, new[] { "◄°@[", "_!!\\" })]
        [DataRow(0, 8, new[] { "]oo[", "_!!_" })]
        [DataRow(0, 9, new[] { "]@°►", "/!!_" })]
        [DataRow(1, 0, new[] { "  " })]
        [DataRow(1, 1, new[] { "|_" })]
        [DataRow(1, 2, new[] { "##" })]
        [DataRow(1, 3, new[] { "/\\" })]
        [DataRow(1, 4, new[] { "''" })]
        [DataRow(1, 5, new[] { "||" })]
        [DataRow(1, 6, new[] { "<}" })]
        [DataRow(1, 7, new[] { "\/" })]
        [DataRow(1, 8, new[] { "{>" })]
        [DataRow(1, 9, new[] { "=-" })]
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
        [DataRow(3, 5, new[] { "/¯¯\"", "\__/""})]
        [DataRow(3, 6, new[] { "]°°[", "_!!_" })]
        [DataRow(3, 7, new[] { "◄°@[", "_!!\\" })]
        [DataRow(3, 8, new[] { "]oo[", "_!!_" })]
        [DataRow(3, 9, new[] { "]@°►", "/!!_" })]
        public void SetTileDefsTest(int iAct1, int iAct2, string[] asExp)
        {
            _testDefs[iAct1].Item1(testClass);
            var act = testClass.GetTileDef((Enum)(Enum29)iAct2).lines;
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
                AssertAreEqual(cExp, testClass, []);
            }
            else
                Assert.AreEqual(cExp, testClass);
        }

        [TestMethod()]
        [DataRow(0, EStreamType.Code, new[] { csExp0CodeC })]
        [DataRow(1, EStreamType.Code, new[] { csExp1CodeC })]
        [DataRow(2, EStreamType.Code, new[] { csExp2CodeC })]
        [DataRow(3, EStreamType.Code, new[] { csExp0CodeC })]
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
        public void TileKeysExposeInsertedEntries()
        {
            _testDefs[0].Item1(testClass);

            var keys = testClass.Keys.ToList();

            Assert.AreEqual(_testDefs[0].Item2, keys.Count);
            Assert.IsTrue(keys.All(key => key is Enum));
        }

        [TestMethod]
        public void SetTileInfoReturnsClonedCopy()
        {
            _testDefs[0].Item1(testClass);
            var targetKey = testClass.Keys.First();
            var expected = new TileInfo
            {
                Category = TileCategory.Creature,
                SubCategory = "Boss",
                Tags = new[] { "lava", "flying" }
            };

            testClass.SetTileInfo(targetKey, expected);

            var retrieved = testClass.GetTileInfo(targetKey);
            CollectionAssert.AreEqual(expected.Tags.ToArray(), retrieved.Tags.ToArray());
            Assert.AreEqual(expected.Category, retrieved.Category);
            Assert.AreEqual(expected.SubCategory, retrieved.SubCategory);

            retrieved.SubCategory = "Changed";
            retrieved.Tags = new[] { "other" };

            var secondFetch = testClass.GetTileInfo(targetKey);
            Assert.AreEqual("Boss", secondFetch.SubCategory);
            CollectionAssert.AreEqual(new[] { "lava", "flying" }, secondFetch.Tags.ToArray());
        }

        [TestMethod]
        public void Json2RoundtripPreservesMetadata()
        {
            _testDefs[0].Item1(testClass);
            var targetKey = testClass.Keys.First();
            testClass.SetTileInfo(targetKey, new TileInfo
            {
                Category = TileCategory.Item,
                SubCategory = "Artifact",
                Tags = new[] { "legendary", "quest" }
            });

            using var stream = new MemoryStream();
            Assert.IsTrue(testClass.WriteToStream(stream, EStreamType.Json2));
            stream.Position = 0;

            var clone = new VisTileData();
            Assert.IsTrue(clone.LoadFromStream(stream, EStreamType.Json2));

            var clonedInfo = clone.GetTileInfo(targetKey);
            Assert.AreEqual(TileCategory.Item, clonedInfo.Category);
            Assert.AreEqual("Artifact", clonedInfo.SubCategory);
            CollectionAssert.AreEqual(new[] { "legendary", "quest" }, clonedInfo.Tags.ToArray());
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