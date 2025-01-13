using Microsoft.VisualStudio.TestTools.UnitTesting;
using VTileEdit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleDisplay.View;
using System.IO;
using System.Diagnostics;
using static BaseLib.Helper.TestHelper;
using System.IO.Compression;
using System.Buffers.Text;

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
        private const string csExp3Json = @"[[0,{""lines"":[""    ""],""colors"":[{""fgr"":0,""bgr"":0},{""fgr"":0,""bgr"":0},{""fgr"":0,""bgr"":0},{""fgr"":0,""bgr"":0}]}],[1,{""lines"":[""=-=-"",""-=-=""],""colors"":[{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6}]}],[2,{""lines"":[""\u2500\u2534\u252C\u2500"",""\u2500\u252C\u2534\u2500""],""colors"":[{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4}]}],[3,{""lines"":["" \u2553\u2556 "",""\u2593\u2591\u2592\u2593""],""colors"":[{""fgr"":14,""bgr"":0},{""fgr"":14,""bgr"":0},{""fgr"":14,""bgr"":0},{""fgr"":14,""bgr"":0},{""fgr"":10,""bgr"":2},{""fgr"":2,""bgr"":2},{""fgr"":2,""bgr"":0},{""fgr"":2,""bgr"":2}]}],[4,{""lines"":[""\u2310\u00B0@)"","" \u2321\u2321\u203C""],""colors"":[{""fgr"":15,""bgr"":6},{""fgr"":15,""bgr"":6},{""fgr"":15,""bgr"":6},{""fgr"":15,""bgr"":6},{""fgr"":15,""bgr"":6},{""fgr"":15,""bgr"":6},{""fgr"":15,""bgr"":6},{""fgr"":15,""bgr"":6}]}],[5,{""lines"":[""/\u00AF\u00AF\\"",""\\__/""],""colors"":[{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6}]}],[6,{""lines"":[""]\u00B0\u00B0["",""_!!_""],""colors"":[{""fgr"":10,""bgr"":1},{""fgr"":0,""bgr"":10},{""fgr"":0,""bgr"":10},{""fgr"":10,""bgr"":1},{""fgr"":10,""bgr"":1},{""fgr"":0,""bgr"":10},{""fgr"":0,""bgr"":10},{""fgr"":10,""bgr"":1}]}],[7,{""lines"":[""\u25C4\u00B0@["",""_!!\\""],""colors"":[{""fgr"":10,""bgr"":1},{""fgr"":0,""bgr"":10},{""fgr"":0,""bgr"":10},{""fgr"":10,""bgr"":1},{""fgr"":10,""bgr"":1},{""fgr"":0,""bgr"":10},{""fgr"":0,""bgr"":10},{""fgr"":10,""bgr"":1}]}],[8,{""lines"":[""]oo["",""_!!_""],""colors"":[{""fgr"":10,""bgr"":1},{""fgr"":0,""bgr"":10},{""fgr"":0,""bgr"":10},{""fgr"":10,""bgr"":1},{""fgr"":10,""bgr"":1},{""fgr"":0,""bgr"":10},{""fgr"":0,""bgr"":10},{""fgr"":10,""bgr"":1}]}],[9,{""lines"":[""]@\u00B0\u25BA"",""/!!_""],""colors"":[{""fgr"":10,""bgr"":1},{""fgr"":0,""bgr"":10},{""fgr"":0,""bgr"":10},{""fgr"":10,""bgr"":1},{""fgr"":10,""bgr"":1},{""fgr"":0,""bgr"":10},{""fgr"":0,""bgr"":10},{""fgr"":10,""bgr"":1}]}],[10,{""lines"":[""/\u2568\u2568\\"",""\\__/""],""colors"":[{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6}]}],[11,{""lines"":["" \u002B*\u2229"",""\u2558\u2550\u25CA@""],""colors"":[{""fgr"":15,""bgr"":6},{""fgr"":15,""bgr"":6},{""fgr"":15,""bgr"":6},{""fgr"":15,""bgr"":6},{""fgr"":15,""bgr"":6},{""fgr"":15,""bgr"":6},{""fgr"":15,""bgr"":6},{""fgr"":15,""bgr"":6}]}],[12,{""lines"":[""    ""],""colors"":[{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6},{""fgr"":14,""bgr"":6}]}],[13,{""lines"":[""\u2500\u2534\u252C\u2534"",""\u2500\u252C\u2534\u2500""],""colors"":[{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4}]}],[14,{""lines"":[""\u252C\u2534\u252C\u2500"",""\u2534\u252C\u2534\u2500""],""colors"":[{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4}]}],[15,{""lines"":[""\u252C\u2534\u252C\u2534"",""\u2534\u252C\u2534\u2500""],""colors"":[{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4}]}],[16,{""lines"":[""\u252C\u2534\u252C\u2500"",""\u2534\u252C\u2534\u252C""],""colors"":[{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4}]}],[17,{""lines"":[""\u252C\u2534\u252C\u2534"",""\u2534\u252C\u2534\u252C""],""colors"":[{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4}]}],[18,{""lines"":[""\u252C\u2534\u252C\u2500"",""\u2534\u252C\u2534\u252C""],""colors"":[{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4}]}],[19,{""lines"":[""\u252C\u2534\u252C\u2534"",""\u2534\u252C\u2534\u252C""],""colors"":[{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4}]}],[20,{""lines"":[""\u2500\u2534\u252C\u2500"",""\u2500\u252C\u2534\u2500""],""colors"":[{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4}]}],[21,{""lines"":[""\u2500\u2534\u252C\u2534"",""\u2500\u252C\u2534\u2500""],""colors"":[{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4}]}],[22,{""lines"":[""\u252C\u2534\u252C\u2500"",""\u2534\u252C\u2534\u2500""],""colors"":[{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4}]}],[23,{""lines"":[""\u252C\u2534\u252C\u2534"",""\u2534\u252C\u2534\u2500""],""colors"":[{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4}]}],[24,{""lines"":[""\u252C\u2534\u252C\u2500"",""\u2534\u252C\u2534\u252C""],""colors"":[{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4}]}],[25,{""lines"":[""\u252C\u2534\u252C\u2534"",""\u2534\u252C\u2534\u252C""],""colors"":[{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4}]}],[26,{""lines"":[""\u252C\u2534\u252C\u2500"",""\u2534\u252C\u2534\u252C""],""colors"":[{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4}]}],[27,{""lines"":[""\u252C\u2534\u252C\u2534"",""\u2534\u252C\u2534\u252C""],""colors"":[{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4},{""fgr"":15,""bgr"":4}]}]]";
        private const string csExp0JsonC = "H4sIAAAAAAACCuSX32rCMBTG7wd7B83dRsWTk6RaQVALe4laCo5tDGQFx67Ed1/SHGtb64o46LJ4cUq/9E9++dJzjkkCwZ5t3z9ePtksYQP9Y4E9pMH9HXvOt/nODO3Z69uOzSBgm+J4CHpV0oOZXsKrk5+P5iM9eR3n7ZPnku4Pyyf2KhEEViHWX6gATBTSRIytosFOQ1a0F+ihdlhF75GnV/cpEayobTcDoEQRwwEhRsV5xIuIVunwE85X92rpuLewlPCiAi3XEJ9smCm4dgxgBYsH82EZBXkZQcQd7oXna9mHRHSqSjc2YMsnimtj3zrLxk59e2EVKLVO2Zhonmw4zC7gHLcLP0tOHH6SWm78jWcRzqSZSmJJu49wtE0u8Uxr9uS5k6ZENYjFcZOhWi01ztg1HA6NJIAqnFJ0Mwlw3ihKALh61EiIERUlVRCqwrZ4uXAqaXO8qsX7qyaJ7jZJyP/SJvFmH1GDaTSF9SVwkVZ105beuk8b3uQtxm7RTm7y1jXaqVfeRj55i+DTH3XkPtVbRJ/qLQqf6i1Kn3IyKq9ysle9FHrTS6XfAAAA//8DAFaB8smZGAAA";
        private const string csExp1JsonC = "H4sIAAAAAAACCrTT3QqCMBQH8Pugd5Dj7cJtzizBm6Kn2CQoKgJJMLpS3z3MIWepBB282eAP58fZx9Gaswry++PyhESD50HGlgs4F3lRtkkF11sJSczg1O68YYMka9oSLbBTH8cdEdky1UMospLEku9PSHLQEoqsFGIpMGaCUrZOIAtlFlMYMy/OZdytP9QRtDcj59JqgrT+7i7cN6Tjxg5oAhK2wVj1ae9AArcYTFeEixPOADBGoZwZWKUUSg5+265bKWg4B6rmQKM5UGdG0n+fJ3sDAAD//wMAEDG8jDQFAAA=";
        private const string csExp2JsonC = "H4sIAAAAAAACCqyUTWuDQBBA74X+h7AnhTVZ1+9ALi3+ilUHGtpSCAkk9JTJf+9uNrarxHbL6ME56Hs+cBmlBD+z3cf+9cTWii30xbgdLX98YNvD7nA0j87s7f3I1gVnL2aKS3sxL6jYxRVgq3GF0P6Opz0uXXy5WF6/rud9PBY3XvZ8cqe++RQied5srrOeMGVjU+qaMAhRmwKA0JfPBiW2wdyfRNg31YDgq8td3arrmsZYGoDVhCG9GfLeULgGHkVcCxAA/yj45kvir6loJyMeHEy0PPyDj4m8JPYnRD4l9mdEPh/1g+1HX74g8uWoH2y/N1/ReClo/TIm8pLYnxD51GOhRYi+C03OvCDl4HwG1vTjw67zTytmTisn0pLaLFHEyH7C21fN19d+AQAA//8DADTfrvXyBwAA";
        private const string csExp3JsonC = "H4sIAAAAAAACCuSXUWvCMBDH3wf7Dpq3jYp3l6S2gmBb2JeopbCxjYGs4NiT+N2XtLG2ta7bFCSLDyf+2yb88j/vrmkK3pat396fP9g8ZSP1YZl3e8OeinWx0dqWvbxu2Bw89lh+77w/KdlOL5tic7fFZDFhHlNx0b8pCvO8X694VclAUBNi9UkSQEcudKSkUhTY4VIlVjeoS/2w0uwjDltfUzKwvJUfGkDyMvojgxiWv0MsI1XKgJ9wfLq/lva5RbVEJxXoucfwiY6ZHJVjADEs7xSfBuaEdQSeDLjnH5/lNSRDJ5t0Uw0WPZi40vat8nxq1X/PbwJllVNVTBVPPh7nJ3D26YJHxQnhO6nnwUusZXBm3VKSCJN9BkfZZBNP0LKnKKw0JWxBLPdJRjKOFM7UNhyEThEg6Qcm2lkEEDtNCYDie4VEFJqmJEtCWdqWREurijbSz2ayS54oH55puPgvMw12m34LpjPBtY/ARlo5TFt7az+tf5a3lNhFOzvLW9toA6e8DV3ylsClt2pCl/otkUv9lrhL/ZaESzWZpFM12alZipyZpbIvAAAA//8DAM4Z33L3FwAA";
       
        private const string csExp0XmlC = "H4sIAAAAAAACCuycPU/bQBjH90r9DsFbW4WDdqmQHSi12wVExYtoVVBkkkticbEj2yFkY+hQVQy0kNC1WytFIkOmqlM+yn2SXlzeLFGHyheD3b8UCefu7B/xnX88fvwEdX6/znJ71PUsx9aU2ekZJUftklO27KqmNP1K/rkyX3j4QH3humZ7pXLxw26vtxs0J3a2vbl9z9KUmu835ghptVrTrWfTjlslT2dmZsnb5aW1Uo3Wzbxle75pl6hyuVd5/F6KYOdyahgbtIlW8+K38Kw5X2xoyrrFqKcUjHrDb6vEHDd8TXxKRkc7KeeDxDBm2dS7fCsaPN8V4wrBNjl/c8tulYSPppYc5riho79qMvZy1HqtUTRXqm5hkZmlXZWMNkN9O1d9O6E+ldx0NCCASBlCJaELJXwpq+QGHfyrInTL9WUbQstr+QhDiF5NsiHeUcac1l/Otm66uxcD4s4qQAClGjR5o2yajMk2Cj854CcDftITGxFmCYb1gpEHkg2zWbN8GjEDq7Qcf54BAeTeQxKISajnW7bpi1sx6TcvvHPMO92oGxjePebdz7z7RWwkGaZIijvBuD3jtUupHXFFnPfHxVw71F2jJJ04qZ9o8kJ5w8w2daUHJYdHw/7Co6hcCD/8Nnod/Eo4GpEVeIIDTjo5k5fKmu/YVLZTyPBseLYVoZStYpEgeQIQQJlMnhg2rbeLGw3ZXtke9of99xFeKU5NFSV7ZVyAuMiadLJ5dklBdQKIZM4VZiT5c5WUM1atak36kxx++kHc4ozxxha8AW9gRlLqDb0lPcm67TgINaAMzEhGlbFEK9Ijje2FYZ93f0Zog0Ab0AZmJI3aCFKly86euHKlJ0x557t4IWUKEED/Zcr0z8NdnZpl6cUiTx7zjz+iSkU6X3nniJ9+WsDzXXDAydTzXb1Zr2fgyzP46wQQQPenNL64McHi+AGK4wEBJMvF8YFDNuU7pHe7L9gM4BBAAMmAQzYmKZEBJAIIIJmXiH73gUgPDgEEkBQHIvrdByKQCCCApDkjgkgECxYQSCReSgShCFYsILBIHIsY+O9nWK+AwCFxIhEDVSJYsIBAInFyIgbKRLBgAYFEYuVEDNSJYMUCAovEqRMx8HgGCxYQSCRWoYiBxzNYsYDAIrEqRRCLYMUCAovELBVBMIIlCwg0EjReNYV7fgMAAP//AwCw+aV5WXgAAA==";
        private const string csExp1XmlC = "H4sIAAAAAAACCuzYW2vCMBQA4PfB/kOJsD3MGp27OE2VXZgvG4MpbANh1Bo1mKWSxlWZ+++LQ90KihueFIQ8NT056Wny0VZDauM37rxTGbFQeKiQyyOHiiDsMNHz0Eh13RKqVff3yKWU/uShuziISXMypI4eLKLyOGIe6is1LGMcx3EuLuZC2cPH+XwBP9/fNYI+ffNdJiLli4Ci5ajO5lFI13Yckiz7HdNRf3EXESsr3fCQvmaZCYWqeYL9TckNPUdOm4xTNE/SaZwJGi1PdSBSUudVdQvPm8tcnEwmQchDmRh8O+L8ehb9FdThbk9W63pGBM9aia62DlxxPxgQ3E70EbzqYqYrEJyYVHJVCV7h8n+rArTV9BXY6qnPFF2zlDe+HDzSzvZcQEXSEDuGFstkgMW+F8vkwwVQIA2pIrQUbgFLvVDOw3jNWtYlpWJ7LJgaaXidQHsdHqbpBfRwwdRIw+sU/Ns1tV4Gvc6gvQ64qnzaV6JBsnNosha2Xga9StBeHwc9VbFkBskuoMk8137FTP5jBt/eyGYtmEkw8D0O17NgJsHAtziOjiyYSbCiBdstsBMLtltgpxZst8DANzs8+6Pjr2A/oWTPFwAAAP//AwATlI94cR0AAA==";
        private const string csExp2XmlC = "H4sIAAAAAAACCuzZ3WrbMBQA4PvB3sHooiRQW7aTNj+VU/ZDd7NRWAvbaBvhOkpiqsrFVucG9FB9hj5Z5ZJ0MxvqxhQlw7qKfCRL9pfDwULo8O6aOt9JXqQZi0Dg+cAhLMkmKZtF4JZP3T44HL1+hd7kebw4nq5+2OJ0cUMceTMrhndFGoE55zdDCMuy9MqOl+UzGPp+AL9++niSzMl17Kas4DFLCHi+a/LyXUCu7TiovuxTTEbj1VMU6ZDLRgTknMOUcTDyEYxfGnwi35GS05QSsBwkh9GUkeL5UgYKnstxo6c2XF78YTeC9dlQktEsr81+dEvpuyr6U1CGp7N89EG+MoJVq9Z1KQNvaZxcIXhZ60Pw18kQrC1ZR0HwN6x/Tx3opj7D4kJBfSbwhTnq93F+9ZlMtgU71I3tOZ4yr2W/dmxCmEJ72b8d3h3DdWSH8oMo2pnxA83qX+YpJ/+Lele3umi1hUK9hXG7yd572rO8yuOH+7Yq02WOY4GbzL6vmx2Ox+cK8nOMoWbvb4TSrFSArwZsh3hPt/iu6+4qxAXGwnCGbxd4336vGPUe2I9xgzsf7btModbGjdYOrLZB7dBWEoPaHattULtrK4lB7T2rbVB7fw2VBKsriWiuds9qG9Tur6GSYHUlabD2wGobPNjxbSUxqB1YbYPaoa0kBrU7VtugdndDZ2euEE0+OwvtmeVm3LXvL1sP92p1MR43O9V7NtU34t5fQ6pXrqoDTCHc6s9ptPvA5vu/uv8I1XseAQAA//8DADuPYn7wKwAA";
        private const string csExp3XmlC = "H4sIAAAAAAACCuycz2/SYBjH7yb+D11vauClMPaDtGxOoxeNiVuiRgzp4B1r1rVLW2TcOHgwZofpgHn1pgnJOHAynvqnvH+JL4SNNc4yu3edZd+EhPK+b/uBvg8fHt4+oK7s75rSe+q4hm1pspLOyBK1KnbVsGqaXPe2UkvySvHuHfWh4+jNF1und1Zzo7lHJb6z5Rb2XUOTtz1vr0BIo9FIN3Jp26mRbCajkNfPn61XtumunjIs19OtCpXP9qpO30vmbElSg9hRG2/VT5+FaxQ8vqHJ/JgFw/LkYkYl+rTB6/w1mnTDMKk8HsSHmYZF3bOHvMH1HD6uONom4wdno0lwuFqxTdsJ7P6kbpqPhq3nGnnzVs0prpl6ZUclw81A3+akbzPQp5KLjgbEpRAqCUxOMD5UckGE/XvUKaKjTktpqT+ibtLNezXBQfmGmqbd+MvJfqw7O6cDrjqpAAGUaFAcQsmKFgprt1h7wNo9vhEiltGw3mhkS7BgXm0bHg2ZgJe0evVpBgSQ/x4Sh0BywvNg1jlina4UJo/uEet+Zt0vfCPO7ERQtgnG5RlPHUqtkHfCuP+qmHOHummUoBMn9BXFIZJ54ZnIwaHfX70X4hGJHXwb3lq/Yk5BRCWb4ICTTE4cRsmLNgrxT/yTUohQSuUywXIJQADN6HLJgmilvPP7fv9tiFLKc3NlwUqZlhaumXV6vWvqglLpGBDxnCvMSPznKg5dLAr/TnP8gX+nmeKLEnwBX2BGEuiLJeHphW0juYAsMCMzKItl4bJY9fus+zPEFwS+gC8wI8n0hSK8ZJGwznd+w4ooQADd0hVRRXhJqvTgPvv4I6z8o/OVdQ7Z8adVXLYFB5wZu2yrZBPw0wp87tyaD7jcNZZID1AiDQggs10irYgvbRx7YeqPLAYwCCCAJN4g+Ws0yAAGAQSQGTfIws3nID0YBBBAkmqQxZvPQWAQQABJrEGWkIMgWgGBQSIbZBk5CKIVEBgk8v9dZfCHV4hWQGCQyAZRUA+CaAUEBolskCzqQRCtgMAgkQ2SQz0IohUQGCSyQeZxLQbRCggMEtkgeVyLQbQCAoNENghqUhGtgMAg0Q2CmlREKyAwyKhx0hTs+Q0AAP//AwBCDfCDg3QAAA==";
        
        private const string csExp0TextC = "H4sIAAAAAAACCoTXv2/bRhQH8D1A/gdFU9M69r1HHu9IQIDdyF6aAEGT1ENdCErCJkJlKdCPNO6UoUNRZEgb2+narQUMxEOmopP/FP4lpe5Oel9SdGV7eKQP4AePx/fF3R3PR7OM7c0bDwc/5Vm8xTdvfJWfPDp5mWeH+WSUT3oHw/706Ysng7L8sj/Nt++Pn+XD7UeDYT7dajWv2Wp9k0+mg/GoQ9tq8bvVujsfzuaTvDPK57NJf7jVejB/Mhw8XTxr/EM+6ozmw6F7tMr2j1/OTlqfuSfcvnnj3mCUT1VWwu6pnsp2263yp+2uSK7ujofjyTSzYZV6rVRYsipZykjKWEotZSKlCWVpo6w7mMxqNHI0crTOnc6dtrta0MqLTpVGjpbshyWrkqWMpIyl1FImUppQljTODvvDYY3GjsaOVpy+KU4/FacXZdF2d2l598L9402Vyo4aH4Slq5KljKSMpdRSJlKaUJbUKOvm09lg1J+VO6Qmjpw48u+5OHtfnJ27dx157fn74vy34vz3sqhqI//O98PSVclSRlIutLznyoWW/TPdO/elCXdLbZw9GPZP8kkNGjto7Fv79t3V5e7ttrvh9mTx9s/F35t/q8rYv/6DsHBVspSRlLGUWspEShPKUqmzh7PxKK8htUNqh9y5+nj18ajtrhfGo15vp6rTsjm1bE4tm1PL5tSyObVsTi2bU+PmTLL9UX580nv8sgZMHDBxwO+uLq8uv2276wWwd+tWrwpMHJD2whL1ek+5kqWMZEEspZYFiZQmLCiBJgC/Hjx/Uf++jTMa/6Y//Fy+aYc0S+RRFWkEaQRpBGkEaQRpBGkEaRBpA7L7Y/2rsY5ofRvHY+ez1zTRis+Kz4rPis+Kz4rPis+iLw2+e/n39R6mDph64O7VZXH+T9vdWBh31oypGFMxpmJMxZiKMRVjKsYUjaT8h3J//Gowel6f5D5lyMfMTnH2V/nnPhlS13wzpGCiKxjpCma6gqGuYKorGOsK5rrCb4coDKBu3n9WB4fs8eHT+uLz4pe/2/6GG5dnfxRn74oPv+7W0CRziEgGEZFMIiIZRUQyi4hkGBHJNCLCcUScdefHx/UUJx9IxJUcJ74myImhuQzNZWguQ3MZmsvQXIbmcqW5kYvN3uM61OcQRfXo/NT2t/8/OymS8KRI0pMiiU+KJD8pkgClSBKUIolQijBDKfbwwzrc5xKFYAq8ZeZTHOCfroPHAI8BHgM8BngM8BjgMcDjClyHjq/JfViRrst9y/UmuQa5BrkGuQa5BrkGuQa5rsgTL+/W4T7EKGlueVKHX9TgCcATgCcATwCeADwBeALwpAI3oeVrch9tZJpbbjbJDcgNyA3IDcgNyA3IDchNRW7DLl+T+8Qj29xzu0luQW5BbkFuQW5BbkFuQW4r8nS5zdfoPgspbW56uomeAj0Fegr0FOgp0FOgp0BPkc7K0/frhwmfjqyajxNq03lCwYFCwYlCwZFCwZlCwaFCwalCwbFCVeAUer4m9zHJ1DjNmTbJCeQEcgI5gZxATiAnkFNFzmGfr8nDCY4b9znzhqHIeIjDUxwe4/Achwc5PMnhUa5yluNlgK7TfYRy1LjPOdpEhwhliFCGCGWIUIYIZYhQhgjlSoRyiNDumtxnKDdnKMcbvlCGDGXIUIYMZchQhgxlyFCGDOVKhvIyQ9fpPkS5OURZb6JDiDKEKEOIMoQoQ4gyhChDiHIlRDlZTvQ1uo9Rbo5R3hSjDDHKEKMMMcoQowwxyhCjDDHKlRhlsxrpa3YfpNwcpLwpSBmClCFIGYKUIUgZgpQhSBmClCVI/wMAAP//AwA9YL725BMAAA==";
        private const string csExp1TextC = "H4sIAAAAAAACCmzSS2vbQBSG4b3B/0EkizRYceYb3UVVWlx30wQCMVkFjJPMQlSViiVBc/vvOT4jopmR8eZ4JB6NRu+q6esuRzKf3ZYvKpc+5rPf6nnz/E/ld5uyUuunslteN0+qapcb1Xbt8q5sDxd+7rodLyzWdf9XZr73eT8v03+1b8umLrAUh5/vrfqq6/eqqFXf7XeV7930D1X5eHhe80fVRd1XFT9e5FshvC8aPp/PrspatSKnvV2Jrci/n3jeyXy2aqpm3+ZyWBX/RcIjhpEgEAQXAkNg6G1rQ2Ao/MUjhpEgSZB0IcmQZOj01Iak3tGKRwwjQQFBgQsFDAUMXd7bUMDQjzWPGEaCQoJCFwoZChk6O7OhUO9ozSOGkaCIoMiFIoYifUZvNhSNUGRCMUGxC8UMxQx9fbeheHy12Hy1hKDEhRKGEobuL20oGaHEhFKCUhdKGUoZev1mQ+kIpSaUEZS5UMZQxlBxYUPZeEaZeUagtDFJG7pt6Lh932lSjBiEpVHfmPY9BK4LvygcDYYGS6PIMYkcunLozBcLR5OGJi2NSsekdOjUERzXAkMLLI1yxyR36N4RHteM4mElD2oek+aho0d0XDOyh9U9KHxMwocuHzr9wv0KsaHFn9oHAAAA//8DAOesOI+PBQAA";
        private const string csExp2TextC = "H4sIAAAAAAACCnTW207bQBAG4Hsk3sHKVZCS4B2frSRqy0GqCi1SEFwAWYWylSxcG+XQNpUfimfgyRrP2DTj3YWrFejbn2F+lpNyU6xTzz08mGV/VeoP4PDgi9peb19UOiufy8dFIT8tVmp0k6nfl+WTykfXWa5O1Y+Bs//1gXOjlqusLCZi5NafA+dkk683SzUp1Ga9XOQD52rzmGffa718VsWk2OQ5XuamZz9f1lun38hHhwcXWaFWbroLc+FKN/3Qc3YfPTyJ/6eTMi+Xq1Q03+X+cSMERXq7yHPNE+gJ9O5k9dDDU+3dVfKBewI9nzxIz/OyXGogIAgIjpwRBgQKuDtyEBCEjwh66alarbNisd5NTGM9ZD32c3vIjieTKWc9Ys+R9dOrfLFVelAfRR/Fqn9U9fBUi30pj7jo74tBI377pZZ1ZE0OUA4o6/jttcZ2x5qeykpyOtinw3S2LguliSGKIYrH8/l9D081eC/lMQdDBMMzBCMCPxfGoBGyEbKD4XDQw1PNVlJWnI2IpZwx/fLl5WL5rJ40N0Y3ZjsQW3Yg3t+BBJdUftXEBMWErWliWdNkf02FS+StvvlUJUFdqtrdpzJJffldxoomqcFtKiW4K2wua5UAcmc6S8USwKsKtq4CY70mrsGlZgmPu57N9ZjrN9M1uNQv4fMx+LYx+MwN2vEaYKqXCDgc2OCAwSHBZzpLHRNhOwfZo2Mzh04dRMjYqMlrcKlkIuJuZHMj5sbNfA0ulUzE7RjIjdsxdN2YuW3RTDB1TSQcTmwwqxs0dZvpLlDfwGWDANcyCGB9g7ZvJpgKB4LDwgbzZwzaDTbAzVsGbBIAlkkAqxx47ytskKl04HHZs8msdNA+aYY/akCtA58/PkC1mw6rrsxeNug+baYbqH7Qed7A9r4Be+AgbLPrxQZqIFAD+2+vLU0drObzbviQ0VE3vOEKKiNEnfCRLXzEbojb8IbfKNUR4jb8lP7zoT5W1XDckWMmJ93shhuol5B0sie27Mn7Df8AAAD//wMAuoivtdEKAAA=";
        
        private const string csExp0BinC = "H4sIAAAAAAACCpJhYGBgAWImIC4MTy3KSy2Kd8tJLE7OSMoEMp0Si1P1fPNTUnP0QjJzUot1FLCr0VEISy0qzszPszXUMwBBHQXn0pyS0qJU27zU0pKixBwdhYDSpJzMZO/UypD87NQ827zSnBwGqM0sCkAAJjgYMAEjTJWtrq0uCxDbglTxsaFCJqgqnkdTGh5N2fJoyhogA8JZA+Y3gHTxs6BCZqguDoVHUyc/mjpNgefRtMmPpk18NG0SkAG2Bwq5mICAgYkJFlrsj3omHNrgoMml8KhnIQg17AFbwIYKWaHK2fQPrT+0PoYlJj5eH5vz2WDqYg9tOLQhmiVeUTEepI6LkYGLgYuLEUazw62f3gK0HqwwBptCDlioxebn4zaOE2ZcrMOhDY+m7WLRx6GQCxZQ+o+mrgAi3D7hhvlEQVvrUcdKrkdTZzyaOuHR9C4HbOHDgzUJoJvJiyVytxCMXD6ErjXISWILXl38WHRtIahLgJBda7DpEiRkF1ZdQmTZJUyWXSJkZStRsuJLjKz4EicrviTICkNJssJQiiy7pMmyCwAAAP//AwCEr84E0QUAAA==";
        private const string csExp1BinC = "H4sIAAAAAAACCmTOT2vCMBgG8Np1/ulc675BwIOHxrrtJhgvmycZeCiehFFnDmFZCk1yGOp3Nyk+vZSHF5JfkjfvSxAEoaueK7svhOSbkzD5V3XiUucF10bne6H9wWdpygayjbJ/70tK2vsNuz2vtagUe8tffSj5sNLYmjPFralLScnOHqX42fL/ovrliikrZXD/PSTEDzJw6YEu357SKI0wYzid+uXY5QG0OHhK4iSOQLNZQy6Pba8LqA9aXfFwADosQEPQeQ0agdgcvWIQpaAn0JyBxqAsAz13KelS2qVJO0Tb/gYAAP//AwAfetHlxQEAAA==";
        private const string csExp2BinC = "H4sIAAAAAAACCnSRXUvDMBSGu5p1OnWb39+SyxbaTu/bXah3IgiO3WzroZsRykIL3YoI+VH7DftlJv0YVTgJCeR5z/sm4dxrmkbk0uWafySLZBbG8BQumTuK2Pdb8sm4O4w4e2FfNq3rNh2xdBklsf/oPqhp0+eMr7KU+THLVmnIbfqezXg0f2U/w2TBYj/OONfKuwiVI98a8tTSGhUfg5iSsYBpzolecZe6sl7uirf1nT85nu8PFO/q1V+IMC1BTACr4M2SN6m3WVtkAAIKwagM/SCYkAlAX/GO0aq47Tg2EQAirzd2kQftIR9obx+kOGz5PsIPkJxDhHeQnC7Ce7UcUDmi4EcIP67lgMop+QnCT5GcM4SfIzkXCL/810hHFI3Rr7AOX5eCYW7WShFBUFpuMMttJUjLgBIhHK8Q7jDHLwAAAP//AwBX8XAjSAMAAA==";
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
            _testDefs.Add(2, ((t) => t.SetTileDefs<Sokoban_Base.ViewModel.TileDef>(new Sokoban_Base.View.VisualsDef()), 23));
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
        [DataRow(3, 0, new[] { "    ", "    " })]
        [DataRow(3, 1, new[] { "=-=-", "-=-=" })]
        [DataRow(3, 2, new[] { "─┴┬─", "─┬┴─" })]
        [DataRow(3, 3, new[] { " ╓╖ ", "▓░▒▓" })]
        [DataRow(3, 4, new[] { "⌐°@)", " ⌡⌡‼" })]
        public void SetTileDefsTest(int iAct1, int iAct2, string[] asExp)
        {
            _testDefs[iAct1].Item1(testClass);
            var act = testClass.GetTileDef((Enum)(Enum29)iAct2).lines;
            Assert.IsNotNull(act);
            for (int i = 0; i < asExp.Length; i++)
                Assert.AreEqual(asExp[i], act[i]);
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