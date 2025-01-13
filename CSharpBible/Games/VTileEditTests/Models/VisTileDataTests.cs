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
        private const string csExp0JsonC = "H4sIAAAAAAACCuSYXW+bMBSG7yf1P1DfbaKJfcA0ICElQatWTZN2UbUXTYSS1G1QXZj4uKii/PcaQ9LRmH5MkTLqRDokr+0DD6+xj1ih85w9EOShK5bGLA3P+CxbLOeR+DmeZaz3K7lhvHcRcZaZhrqPaVyyNIuS2Cc9XH5NIyh4XqTMj1mRpzNuGr+LOY8WP9njRXLPYj8uOEemPDcgb4XOs+8Pf/JH5N3OuEiIrqKbfIk820Q/WHS3zJEH66q7hbzr1eai8V8peBSzTDQiQ3xEbnmYmkdf0CLhSZrJcbd3qRw1l8e1eVBlui7FGoWoUPwT/0SgiOirUYhdZ3O2+Q8qNZBAhTQpgGJcRssuIwSVIjCfmyqx6iCa1Oi0Pqv9fCGHlBrolnJiljjUktExamBX/neJjFApb3iNd+/8h6XNLIStBK0KVvRp0NotRltEuInxGA+/lg9kqQDZRmwFbzjr7N7nQ0gNVqpi7ZeYo7M6TkprJ2HY7+Az66jwppWLVbwWdOHxcdgCt5lYZGfBI/g1STFwH7kacKdtC1Jg1/O0hhMWdo9uoLQuSTpsmKtEGm6mI9DxSMD1uwlHcMtSAtQZ1LHLSwkhLZsgxjD+JgAB3HoTpJKXSkuD0bCD2wKBfypF/28DrfcXcJb9uQo40lbTNNBeFK/NG9Jddvp+9q3vn4Xd2YvvEHSR/XQvvneTfaCx766+vgPW9yUFEH33dwB993ew9N3fwdZ3nQeq8TqvcV0H2tV10/UTAAAA//8DAP3NjozYGgAA";
        private const string csExp1JsonC = "H4sIAAAAAAACCrTTQWvCMBQH8Ptg36E8r7E2aZ0z0IuuMBmDHcQdVhlVo4bFdjTJQWq/+9bqpM6UwaoUUvrS/v6PNMlgpNgGA4XJmAsWLLiyn5MFE9IeM6mkPeGymHiIVFQWQu04ZBDEekP6yDp+VM59P7NU8iT2se0UF7KGWiidMj9mWqWRQNaLngk+f2LbcfLBYj/WQgAqmyBAMxjJYPOptkCXkZAMwStfqDVQguCR8dVaAcX5/nUX6Fv2071TIQSPmfyeBMuCKbq9gXkiklSWry9XKdAegllxd3J0VpnmRfGgYpO6ezeruHtAvCNbKZ24xOS2WjUuOWu3UjpxXZPbCcMa2DsouCJXaie0Z6LLvdDbj39kGCJ+JXSNi71r7N7Vde4O8wssTM/Ih50L0PcmOitbDy7A902832684Nh4GBFqDhvPY9tvDpPa3T3Yj80j3OtHeNeP6F4/wnhe/f/+5Gn+BQAA//8DAPgKlDHiBgAA";
        private const string csExp2JsonC = "H4sIAAAAAAACCrTV22rCMAAG4PvB3qHkSiHWNq2nQm90hckYDCbuwtbgIWoxtqMHxjB99/Wgm46oBGILzUXar/9P23QPhgnZ6cAC7+E2nM8C3J/FRB375Os1XBKqjnxKnsgKKqfzUBmTKPbDwNZVrdihMkhpkkbEDkiaRDMKlbd0Tv3FC/kehVsS2EFKKYDl3RCw9mAYO7vP5BtYqxnNQfDhL5MNsEwInom/3iTAQll1ugGsyf4YUzshqB+QOJ8ESr7ldjl48PEBLEIaRnF53WodAasDwbwYtczLMviL6TxsgpmXYxOGveuYeY4hHqYqapksH/mYrh00dK4ZV3q6qaYZA9suR+eC2+K7Js9ltTrL3RrGdTGtxU1Z5SuOfa1+zOtghsXwNg9vTqeuW5guxs0Lnnnw2udeh+fBRgPmHMOY3Uj3T+tKfdw9mW+izv1IWKVhYU2XqiGpTQ2pmim1aUuq1r7QFFdNmZjWkap1LzTFVVNBrSdTQ5rMpkiXqiGpTQ2pmimwtDcYE1va0V1/HIj7rdQq909n06lo7M5dY3dvxDac4ufCWKO6oaDek5/dy34AAAD//wMAgAPs4z8KAAA=";
        private const string csExp3JsonC = "H4sIAAAAAAACCuSYy2rjMBSG9wPzDq52M7iJdGS5scGQxEyZUgqzKO2iCSZJ1caMag++LErIu1e+JB03cm8EgqsETpJfsuRPv6xzyAqdZfyBIBdd8yTiSXAqZuliOQ/l1/Es5b2L+JaL3mUoeGoa6j6mccWTNIwjj/Rw8TYNPxdZnnAv4nmWzIRp/MnnIlyc88fL+C+PvCgXApnl3IDcFTpLfz38yx6RezcTckB0Hd5mS+RaJvrNw/tlhlxYV90pcm9Wm5vG/w0hwoinshEZ8oWm5vdvaBGLOEnLC+7uk7L7vPxcm59SputCrOcmqrm9Y+9YcsnoqW+BWPVo9nb8g0oNJFAhTXJgGBeRWkUEv1Ik5nNTJVYdZJMandWzWs83ckipgU6VO6nAYbSMtlEDO+Vvh5QRKuUNr/Huyn9Y2uxC2ErQqmBFnwat1WI0JdJNjMd4+EPSFvgUyDZi6r/hrL27zoeQGqxMxdovMEendZwU1k6CoN/BZ9ZW4U0rF6t4I+mCo6OgBW6zscjOgUfwa5Liwn2M1YA7aTuQfKvepzWctLB7dAOldXHcYcMcJdJwsx2BjUcSrt9NOIJbjhJg9qCOXT5KCGlJghjD+KcEBHDqJMhKXlZa6o+GHUwLBD5WO+5/ten7qy1qfa1qi7QVIA20F5Vmc0G6y87ez771/auw23vxHfwusp/sxfdusg809t3R13fA+v6jAETf/A6gb34Hqm9+B0vfcx6Yxue8xnUdaFfXTddPAAAA//8DAOQcp7s2GgAA";
       
        private const string csExp0XmlC = "H4sIAAAAAAACCuycTVPTQByH7874HWJuKnT7AoJMUl50UEecOrYDOuJ00nRLA0vCJBsLnDh4cBwOKLR69aYzzMiBk+OpHyWfxG2ovHQwBe2WDP0NPWz3JU+zu3n4J9lEm1xfZcpb6nqWY+tqKpFUFWqbTtmyl3TV55XhcXUye/OGNm8wnxb8NUZzlTx3RXHe2qRzlsdzlWnXNTZylVxpmZpcERu0vYl1z9LVKudrE4TUarVELZNw3CWSTiZT5OWzubxZpavGsGV73LBNqh63KndvpYrfoyjaE05XU9kF6trULc4ywzOrJUskZwyPJp45ZcoSBYtRb0g5v86QMt/ebbHXrb8h5YHPuO9S3aY+dw02pDz3S8wyn9KNgrNCbd32GdPIEfj4N6TDpPiyYJV5NTuikaNEO/cxtZaqPJvWSDsVNiQnLcONZP5Ub/fltL1R2Fij7VyRbxxlKKJfJ7hI6KroqwnL5mo2qRGje/W8GDJGWz2iHlcTFZllU+9UhsjywuHNhmnS/nKJChrp3KZmOsxxOyizoi8ftPLPZIuCypKbnWGGuaKRVrKjtHRSWuoo1cj52wQKqAFBaaTjUOtUg0bOFcy/aCclRzv6sD4cqR1RrkvSzivKmFP761A8NNyVP1V6M/QAAjiAwH5qKi1HU8HeVrB3GOzti0SkrsKK+2HdLUnaWqhanEYOzwta7tVkAAywawrrp5Yykk7agvpuUG9En7gFjd2g8TFofBKJq4mkeho/g/X/rEcupXbkcdSu0RvcqQ3GB9nTDpWwh/3U04ikqGl7p3kwdTv6slKw/bX12fp1ZeFSb8Nn8MAbJF4/PTUqx1Ok+aP5YzFSU4vFIsFlKAABxGWoC4jqnhxRvWkeNA9eR4qqeOtWUZKouge4M+KucX/uk/T0ZKGPqP72IUYsbn3YTwmNSTqr+/xOnNV1tdAiLAQLYcQG3kLjkkIhx0EghAmNEYOCuiroviQFTTUPgsbPSAsRWAgWwojBQmIFpaSV2ySofxMfXL8GEEBcv+6NqyQt91bu3gnef49erlT/EtR3gs8fprAgADzwsCAgylPp6/c0HP41AgjgNYunMtKfSznEcymAAYbnUi7lJVkrv9uuucDzcofwEmCAwUtnvDQq3UuH8BJggMFLl/LSvfjES/vwEmCAwUstL43FJ16ClwADDF4KvTSOeAnTDjDAYual+4iXMO0AAyxmr69M4v2VmHaAARYzL6WwfgnTDjDAYualNNYvYdoBBljMvJTB+iVMO8AAi5mXRnA/DtMOMMBi5qVR3I/DtAMMsJh5Ceu9Me0AAyxuXsJ6b0w7wACT5iWNPOF0NSPSGpk3xCs1C/4ao7lKPjzU89YmnbM8LtocNc2VlqnJs78BAAD//wMAG2/34GGCAAA=";
        private const string csExp1XmlC = "H4sIAAAAAAACCuyabWvbMBDH3w/2HYwL7YskVh7arU1ll65PG2vJWEK6QWEo9iXRqsjBkpdm6777ZMfLOtPSMSwbgsib8//udNL9cBzOwUd3M2Z9g0jQkLt2y2naFnA/DCifuHYsx419+8h7+QIPCYthEM8Z9MZ9GSl3n36HSypkb3wcRWTZG/dGX8GXllqQi+6doK49lXLeRWixWDiLjhNGE9RuNlvo09Vl35/CjDQoF5JwH+x1VvB8lq32Y1n4nYRZyxsOKIOzgErnKgyACWcAQgpnSEXiOCWSpELtjMez9kHdWsensrrOjq5Onnzq1knMZByByyGWEWF160M8YtR/D8tBeAvc5TFjGK2Kr/fRTk11cU0DOfXaGK2MTH0LdDKVXgujzEoT0Z/MdJHO7/Csn8d8OVjOIVOVTlaCpXrblcpwbdWvLuXS9poYkefD+wobg6QD9jpMBTLKQTwQlCRSxF6yy8x8EI/yCdgPWRjlljhXjTpJ9L9k5RhPIu9CnRCjxMo5R0p6w4h/i9Eo58Xo8SXLqoRR7pj5nmP0KLn/4dnSw/P+iyae11Mq4ck2n5Lo9iMERSEttFiZVNt6qG5taaKatrGMm7SwQmXS7OihiW400fwMjIWLJ/t8EQHwooAWWatMprt6mO7sVMO00Ju0yFplMt3T9Cy9N0wrY/pKD9NtJg9/mq/fyrC+1oP1BhmmlTHd18P0x/ZEHhqslWE90IPVbZinanVTB01jpHrdQK0OqqZZUsM1UKuDqmmUVKsZqNVB7Riomwd110DdPKh7BurmQdU0VHLND6UioK7elycvyTH6538jeL8AAAD//wMA9EziKuEgAAA=";
        private const string csExp2XmlC = "H4sIAAAAAAACCuzb627aMBQA4P+T9g5RflStBDG3tpQ6qXrRLlorpoHaTW2xAhjwcJ0qcUaZ/FB9hj7ZHEi7NlTJNsnCkyz+mGMngfPpHBE5wIO7G2r9wGFEAubaVadiW5gNgiFhY9eO+ajctA+8t2/guU9j3I1vKW6POjyU0x3yE5+SiLdHh2Hoz9ujdv87HnBLnpBFrbuIuPaE89sWALPZzJnVnSAcg1qlUgVfz047gwm+8cuERdxnA2w/HTUsPsqWn8ey4EeOb6peJ5gGfZ+hIz/CzjnBs7NgiKnTJRSf4FHJej5fss7TLyq/Z/IqWccx5XGIXYZjHvq0ZH2O+5QMPuF5N5hi5rKYUgiWl3q6am0xlG8uyJBPvAYEy0Ea/YDJeMK9GgTpaHEg+H3k4iT1x+Vp9g7ZvDu/xWlUxv1lwJKZbHE5cG2ZnRZh3PYqEPjFyzsSieIkE/bTMrmQEoajZwEZihag3mIM0jd/sQCC7DnhIKBBmLnKO5nL4yT+IiwnRuPQey+TAEEyykz2ZeiI+oMpBP3MLASvnRKCzMWzyYLg1ZT/C0RVDcQlEte5EJcCXa8D4sQPp1/wUEeKmhoKx3IKakKuUEaBMcu1SFfop1FfU4faoHzfdTfGfF+RycWEcPx/mjTUmIjNLZFrsonQltHIamwrqpCkAh7ut/KrRNYHEsigZFF21KCAXu8qF+QKIaBI4xumNJjlcjwu0c9jV41HqVwu5XoIhMTaqkNfjqb5jaWRxp65+dDmPlDRHbkoskDGYsWiaiy0saiZHqWNRd1YaGPRMD1KG4ttY6GNxY6yHoWKepQwFi8tdo2FNhZNZT0KFfUoY5Gx2DMW2mz6VUyP0saiaiy0saiZHqWNRd1YaGPRWPOua1kIs+u6omL2wnVUUXQvvvlwX2Qiej1TJqsgu6ZMNFRpKiuTJOf5G+NClBM8o7KismdqRaXK8nH25Bl2CP74rwHeLwAAAP//AwBj2cAlbjAAAA==";
        private const string csExp3XmlC = "H4sIAAAAAAACCuydy27TQBSG90i8g+sdkGRy6TWy0xsqIFoFkagFURQ5ySQxndqV7ZCGVRcsEOqi0CZlyw6kSs0iK8Qqj+InYXLpLSpOSzPB0F/NYjIe+4tnTr8c2+NYmd3eZNJbatm6aahyJBSWJWrkzLxuFFW57BSC0/Js4u4dZVVjZZoubzGaLKQciy9O6e/osm47ycK8ZWnVZCGZfUNzjsQ3aNjxbVtX5ZLjbMUJqVQqoUosZFpFEg2HI+TFynIqV6KbWlA3bEczclQ+XSs/eC2Zfx5JUp44dDOSWKOWQa3MEtPsXCmr8+KCZtPQipmnLJTWGbUD0uVtAtJqb7f5Xrf/AtJimTlli6oGLTuWxgLSs3KW6bmntJo2N6ihGmXGFNIFn36GaKfI36zpeaeUGFdIt9CrfUz1YslJRBXSK3VWJGdrdjYSO2ne68t5o5qubtFeLa/XuhUS79e4wwuqzPsqrhuOnAgrRBvcPMWHjNF2j8inzXhDphvUPlfBq+zO8CY6ZdJ7c24N0r+KkjOZafVtZIl31WK7/kI1X1AoWokFpuU2FNIu9i3Nni3N9i1VyOXbBOpGKIX0DV9/NCnk0pj8k0iNiIlUNagGL4nUswZ8uSoolF9SxszKb4fioWZtnDQZztADCOAtBI5SU1ExmnIPdtyDpntwxAueuuo0POq03RGkrbWS7lDP4XlO88MKBsAA+09ho9RSTFCe79b23Vpd8lZSfd+tf3Lrn3nh72RSQ82fwbo565FFqeH5f9RrMRzcuQ36BznUDhWwh6PU07igrGl3r9WYu+dpJ8nd/dp+7fz8a+nScNNn8MC7TbxRempCjKdI67h1vO6pqfVMhuA0FIAA4jTUFUQ1KUZUr1uNVuOVp6gyY2MZQaIanOAu8AuNo7lOMtSDhRGiRtuHGDG/9eEoJTQl6Kju8D0/qhtooXVYCBbCiN16C00LSoVME4kQAhojBgUNVNCMIAXNtRpu/YenhQgsBAthxGAhPoNS0GRf4ta+8RfOXwMIIM5fD8dVgqZ7Sw/uux++e09Xqn1xa3vu4cc5TAgADzxMCPDyVPSfu4EK33z4qr0QwjHhtyw0ccsCYIDhloVreUnUpOCea65wK1UTXgIMMHjpgpcmhHupCS8BBhi8dC0vTfonXzqClwADDF5qe2nKP/kSvAQYYPBSx0vTyJcQdoAB5jMvzSBfQtgBBpjPftkwjJ82RNgBBpjPvBTB/CWEHWCA+cxLUcxfQtgBBpjPvBTD/CWEHWCA+cxL47geh7ADDDCfeWkC1+MQdoAB5jMvYb43wg4wwPzmJcz3RtgBBpgwL3WfgN5+7LlCrvxs+cQvAAAA//8DAJFA8RKvfgAA";
        
        private const string csExp0TextC = "H4sIAAAAAAACCnzXT28TRxjH8TsS78H4VNpA5nl2Z2d2pUihcbgUJFSgHEplGdiCVcdGjk2bnjj0UFUcaCGh195aCYkcOFU95aXsK+n6mbWf3/7BwOHxYikfxuP5Mgez5XSRsb986e745zyLd/jypa/yk3snz/PsQT6f5vPhzcno+PGzR+Ny/HJ0nF+/PXuST67fG0/y451e93t2et/k8+PxbLpH183q907vYDlZLOf53jRfLuajyU7vzvLRZPx49bNmP+TTvelyMpEfbbLDo+eLk95n8hOuXr50azzNj01Wwm6Zocn2+73yV19ekb46mE1m8/Jt5b/kYPU285MxMpKOrGOkY6yj1THR0VVjiaNsMJ4vGjYSG4lt79retb68WtnKF3sbG4mNxJYcykg6so6RjrGOVsdER1eNpY2zB6PJpGFjsbHYircvi7cfi7fvy6EvT2n99L38xcuNlcXKYo1vykg6so6RjrGOVsdER1eNpTXKBvnxYjwdLcpN0iBHQo7CR12cvilOz+TjjgL37E1x9ntx9kc5bLiRcKPwsR/KSDqyjpGOKy7fkHHFZZZRPvYwuuppyY2zO5PRST5vSGORxmFxX72+ON+/2pcHsi+LV3+t/rz8b8OMhRmHHXBTRtKRdYx0jHW0OiY6umosmTa7u5hN84bSitKKcvfiw8WHh315vUI+HA53NzwrPKsb1OoGtbpBrW5QqxvU6ga1ukEtbtAkO5zmRyfD+88bwkSEiQi/uzi/OP+2L69XwuGVK8ONMBFhIkK6IeNKeMPIyDpG+oZYR6tvSHR01RtKoauEX4+fPmt+y50gXfiw3/1SftiidGvlw43SidKp0qnSqdKp0qnSqdKp0qHSV8rBj82vjhejDws5mwnQt5fRC9Ar0CvQK9Ar0CvQK9Ar0CMwrYC38u+bq5iKMA3C/Yvz4uzfvjxYIXcRmQoyVWSqyFSRqSJTRaaKTBWZIpJM+Lbcnr0YT582j/TQGwrB2S1O/y7/yPeGTPuLQyE7ZOBsN3C4GzjdDRzvBs53Awe8gRPe4DeIqDqIBvnoSZNcZSh0qPfF58Wv//TDAzk3T/8sTl8X737bV3ZVJNIDiUhPJCI9koj0TCLSQ4lITyUiPZaI8FwizgbLo6Nm0ynEibhWdeJ21in0iBgWmGGBGRaYYYEZFphhgRkWmGsLHElEh/eb1BAlipoh/dgPjz9ZUgptokhbSpHGlCKtKUWaU4q0pxRpUCnSolKESaU40B806aFSVGWqAq7/D0BxRf/YQQ+9ohjoMdBjoMdAj4EeAz0Gelyj22rVW/bQLrJNe1h2u8UeYkYW7BbsFuwW7BbsFuwW7LZmT4J90KSHqFHSvexJk/5e6aFylAA9AXoC9AToCdAToCdAT2p0Vy17yx5aR6572d0We2gfObA7sDuwO7A7sDuwO7C7mt1Xu71lDw0k373ufos9ZJE82D3YPdg92D3YPdg92H3Nnq63ewsf8khp98KnW/Ahl5QCPgV8CvgU8CngU8CngE8RzybgD5sXjVBMNt1XDbPlrhHqyQZuGwauGwbuGwYuHAZuHAauHAbuHKZGp2rdW/aQTqbO051piz0klAnsBHYCO4GdwE5gJ7BTzc7Vfm/Zqxsed+535k8fkVxd8vCWh9c8vOfhRQ9venjVw7te7bLH66i28SGrHHXud4624ENWGbLKkFWGrDJklSGrDFllyCrXsspVVgcte+gqd3eV409/Vzl0laGrDF1l6CpDVxm6ytBVhq5yrau87mobH8LK3WFluwUfwsoQVoawMoSVIawMYWUIK0NYuRZWTtYnfAsf0srdaeUtaeWQVoa0MqSVIa0MaWVIK0NaGdLKtbSy2xzxLX2IK3fHlbfElUNcGeLKEFeGuDLElSGuDHFliCtrXP8HAAD//wMAPgv00hIUAAA=";
        private const string csExp1TextC = "H4sIAAAAAAACCmTTX4ubQBQF8PdAvoPsPmxL3Oyc8b/U0pKmL91CoWGfFkLanQep1RIVuu32u/fmXhlHJS83o/wcx3N2TV93OZL16mv5x+Tax3r1yTwfnn+Z/OFQVmb/VHbbz82TqdrtwbRdu30o28uFD6fuxAubfd3/1Jnv2ft5mf6bc1s2dYGtuvx8b9dXXX82RW367nyqfO9L/60qv1+e1/wwdVH3VcWPV/lRKe+VwK/Xq/uyNq3KaW/36qjyd1eed7Ve7ZqqOdOypvGyrH6rhEcMI0kgCXMJLIGll6OVwBJYCj/yiGEkSZOk55JmSbN0fW0lzZKWPe14xDCSFJAUzKWApYClu0crBSwFLL3f84hhJCkkKZxLIUshSzc3VgpZCmVPex4xjCRFJEVzKWIpknN6sVLEUjRKkSvFJMVzKWYpZunNPyvFLMXj28Xu2yUkJXMpYSlh6fHOSglLySglrpSSlM6llKWUpb9vrZSylI5S6koZSdlcyljKWCpurZSxlI3nlLnnBAo5FiGHpBwSc98fwyk5hxo5qIlHUccy6kPWJey3xegNaYfjYeJR4LEIPCTxkMhvNqMnmYd2PD3xKPZYxB6SewQLT5KPwPGCiUfhxyL8kPQjXHiSfzgFwKQBoApgUQFIBxAtPGkBnBpg0gNQEbAoAqQJkCoUzveQLiB2vNh6/wEAAP//AwCpBQShpwUAAA==";
        private const string csExp2TextC = "H4sIAAAAAAACCmzW207bQBCA4Xsk3iHKVZCS4BmfLRK1BSpVhRYpCC6ArELZShGujXJom8oPxTPwZLVnbOPJLtywAn0eL/uznObbbJO4zuHBbPlPJ94QDw++6t317kUns/w5f1xk6tNircc3S/3nMn/S6fh6meoz/XPY635/2LvRq/UyzyYwdqrPYe90m262Kz3J9HazWqTD3tX2MV3+qPT8WWeTbJum9DAnOf/1stn1BrV8dHhwscz02knKYS4c5SQf+r3yo08reF+d5mm+Kn8Myi+rH3P+OiGJkNwu0tQAgUAg8E4VD31aVeBdoR5aEAgEAj0GMfmc5vnKEJFEJHHcG9OIyCOWy1ZEEpFE/Eiim5zp9WaZLTblphmuS64rXt0l92QymbauS67L7mdyveQqXey0OapHpEdkMTgq+rSqyIFSRy3pEel1Sb8mv//Wq2pog/aJ9nnak7fXSiuXlT1VhWptn2y/awfJbJNn2iADIgMij+fz+z6tKvFeqeNWDEgMSAzOSQxZ/JJZRw3JDckdjkbDPq0qt1CqaN2Q3JBdnjTiI6AuF6tn/WTAEcGROAmReRIigqPuSYjpsKpvBhkTGYvjGpvHNSYy7h5XcNi8NRPgqICrKpoIOCslKuCuwBEw1MNa5LoukDJY5DowURggyzMT5sgAZbdoCZc7AxSwW49skTkzcKXsWmQuDVwhe/UuW2SuDTy5GZ5lMzg48ITsN9tsobk28CXtW2juDXxBB0yfmzA3B0GzG6rPy3o33usAzg4CAYf1zBaZq4NQyqFF5vAgFHJU77NF5uwgajaD5ajZjI7M5UEk5KY9G835QSzp2EJzgSASxDrBmSkjN4iO2A50zO1AbhBFg9g0aKM5QgRJg4XmCFFec9icaAtd33Uo9gPR3A+srzuRIbrtkbbYHCK60nYtNoeIIkRsLj3LHzzkEtGTtxNyitNR0bE5RRSXH+7ffrZncJK4dwOi5QpEbhLFJYhBM7+ZO3KVyFUO3l4bnLss5vPOC3CXGAg83H8By0M4UAz3XiC0vAAXiqF4RtS8gOV3y4li1LzAlP9L4kaLYnTybnOjGAk73p/f8gxuFeO9+WPL/Bwrxu0z/gMAAP//AwCejgGnAwsAAA==";
        
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
        [DataRow(3, 0, new[] { "    " })]
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