﻿// ***********************************************************************
// Assembly         : Werner_Flaschbier_Base
// Author           : Mir
// Created          : 08-01-2022
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="LevelDefs.cs" company="Werner_Flaschbier_Base">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
namespace Werner_Flaschbier_Base.Model
{
    /// <summary>
    /// Class LevelDefs.
    /// </summary>
    public static class LevelDefs
    {
        /// <summary>
        /// The obild
        /// </summary>
        private static readonly long[][] obild = new long[][] {
            new long[] {
                0x0492492492492492, //####################
                0x0449249AAD24926A, //#=======O#OO======O#
                0x0449A49AAD29244A, //#===O===O#OO=####==#
                0x046DB6AA95A4954A, //#=OOOOO#O##OO===#O=#
                0x0492289A49A9144A, //####=#==O===O##=#==#
                0x0449A49A91B5146A, //#===O===O##=OO#=#=O#
                0x046DB694D124944A, //#=OOOOO=#i#=====#==#
                0x049228835249254A, //####=#= =O#######O=#
                0x044925255524924A, //#=====###O#O=======#
                0x050925134936DB4A, //#w====#==O===OOOOO=#
                0x044925124924924A, //#=====#============#
                0x0492492492492492  //####################
            },
            new long[] {
                0x0492492492492492, //####################
                0x04C009124924924A, //#i   ##============#
                0x058009124924924A, //#g   ##============#
                0x048925124924924A, //##====#============#
                0x040009124924924A, //#    ##============#
                0x058009124924924A, //#g   ##============#
                0x048925124924924A, //##====#============#
                0x040001124924924A, //#     #============#
                0x058001124924924A, //#g    #============#
                0x044925124924924A, //#=====#============#
                0x050004924924924A, //#w   ==============#
                0x0492492492492492  //####################
            },
            new long[] {
                0x0492492492492492, //####################
                0x05ADB5124926A24A, //#gOOOO#=======O#===#
                0x056DA8926934924A, //#OOOO#====O==O=====#
                0x056D400049A4A24A, //#OOO#    ===O==#===#
                0x056A34804D26AB6A, //#OO#=O=  ==O==O#OOO#
                0x0553340D4934A552, //#O#i=O  gO===O=##O##
                0x0462492495A4A24A, //#=w########OO==#===#
                0x041000034949244A, //# #     =O==#####==#
                0x0400000351448002, //#       =O#=#==    #
                0x0400000350010002, //#       =O#   #    #
                0x0400000351250002, //#       =O#===#    #
                0x0492492492492492  //####################
            },
            new long[] {
                0x0492492492492492, //####################
                0x050000044924926A, //#w      #=========O#
                0x040000044924944A, //#       #=======#==#
                0x049209249209254A, //#### ####### ####O=#
                0x040600058000244A, //#  g    #g     ##==#
                0x049209249209246A, //#### ####### ####=O#
                0x040018040003244A, //#    g  #     g##==#
                0x049209249209254A, //#### ####### ####O=#
                0x058000040600224A, //#g      #  g   #===#
                0x049209249209226A, //#### ####### ###==O#
                0x040000000000264A, //#              #i==#
                0x0492492492492492  //####################
            },
            new long[] {
                0x0492492492492492, //####################
                0x0400189249249242, //#    g============ #
                0x0400011249249282, //#     #==========# #
                0x0400011292492482, //#     #==######### #
                0x04C0012489249A82, //#i    ####======O# #
                0x0492495292291282, //#######O=###=##==# #
                0x0449A01200035282, //#===O  ==     gO=# #
                0x044DB51A92291282, //#==OOO#=O###=##==# #
                0x0489291280001A82, //##===##==#     =O# #
                0x05A80102B0001282, //#gO   # =#g    ==# #
                0x042C010080001202, //# Ow  #  #     ==  #
                0x0492492492492492  //####################
            },
            new long[] {
                0x04924924AA492492, //##########O#########
                0x044924924924924A, //#==================#
                0x045249249249248A, //#=################=#
                0x04700000B000000A, //#=g      #g       =#
                0x045249249249248A, //#=################=#
                0x044600008600000A, //#= g     # g      =#
                0x045249249249248A, //#=################=#
                0x0440C00080C0000A, //#=  g    #  g     =#
                0x045249249249248A, //#=################=#
                0x044018009249248A, //#=   g   #########=#
                0x051249249924924A, //#w########i========#
                0x0492492492492492  //####################
            },
            new long[] {
                0x0492492492492492, //####################
                0x0449248030000002, //#======   g        #
                0x0492292492492402, //####=############  #
                0x0400C02640000002, //#   g  #i=         #
                0x0492092492492492, //#### ###############
                0x0580000000000DB2, //#g              ggg#
                0x05B6000000000032, //#ggg              g#
                0x0492492492492482, //################## #
                0x044A24A24A000002, //#==#===#===#       #
                0x044A28A28A032492, //#==#=#=#=#=#  g#####
                0x0509289289000032, //#w===#===#==      g#
                0x0492492492492492  //####################
            },
            new long[] {
                0x0492492492492492, //####################
                0x0400000000000002, //#                  #
                0x0412492492492482, //# ################ #
                0x0410000000000082, //# #              # #
                0x0410492492492082, //# # ############ # #
                0x041044930924A082, //# # #====w=====# # #
                0x0408580000002082, //# = #g         # # #
                0x0410492292492082, //# # ####=####### # #
                0x0596400000000082, //#g#g#            # #
                0x0492492492492482, //################## #
                0x04C0000000000002, //#i                 #
                0x0492492492492492  //####################
            },
            new long[] {
                0x0492492492492492, //####################
                0x05800000B0000002, //#g       #g        #
                0x0412492002492482, //# ######   ####### #
                0x0400000080000002, //#        #         #
                0x0492092492482492, //#### ######### #####
                0x05800000B0000002, //#g       #g        #
                0x0400000080000002, //#        #         #
                0x0492092492482492, //#### ######### #####
                0x05800000B0000002, //#g       #g        #
                0x041249208249200A, //# ###### # #####  =#
                0x0400000080000062, //#        #       =w#
                0x0492492692492492  //########i###########
            },
            new long[] {
                0x0492492492492492, //####################
                0x044924925124924A, //#=========#========#
                0x0436089251289292, //# gg #====#==#===###
                0x040008001008936A, //#    #    #  #===OO#
                0x04000B001008936A, //#    #g   #  #===OO#
                0x044928925609224A, //#====#====#g ###===#
                0x04000892560AD24A, //#    #====#g #OO===#
                0x0400C800160AD24A, //#   g#    #g #OO===#
                0x0400CB0016089292, //#   g#g   #g #===###
                0x044928925608936A, //#====#====#g #===OO#
                0x050928924808925A, //#w===#=====  #====i#
                0x0492492492492492  //####################
            },
            new long[] {
                0x0492492492492492, //####################
                0x0448000349200032, //#==     =O===     g#
                0x044828834A212482, //#== =#= =O=#= #### #
                0x044828834A212482, //#== =#= =O=#= #### #
                0x044828C34A210182, //#== =#=w=O=#= #  g #
                0x044828834A210492, //#== =#= =O=#= # ####
                0x044828834A210492, //#== =#= =O=#= # ####
                0x044828834A210492, //#== =#= =O=#= # ####
                0x040008834A210C02, //#    #= =O=#= # g  #
                0x040408824A212482, //#  w #= ===#= #### #
                0x0400088000013032, //#    #=       #i  g#
                0x0492492492492492  //####################
            },
            new long[] {
                0x0492492492492492, //####################
                0x044924924924924A, //#==================#
                0x044D24924924924A, //#==O===============#
                0x044D24924924924A, //#==O===============#
                0x044D24924924924A, //#==O===============#
                0x044924924924924A, //#==================#
                0x044924924924924A, //#==================#
                0x0449249249249262, //#=================w#
                0x05B6DB6DB6DB624A, //#ggggggggggggggg===#
                0x05B6DB6DB6DB6D4A, //#ggggggggggggggggO=#
                0x04F6DB6DB6DB634A, //#igggggggggggggg=O=#
                0x0492492492492492  //####################
            },
            new long[] {
                0x0492492492492492, //####################
                0x0400000000000002, //#                  #
                0x0452492492492482, //#=################ #
                0x0450000000000082, //#=#              # #
                0x0450000000002082, //#=#            # # #
                0x0450000100002082, //#=#      w     # # #
                0x0450000000002082, //#=#            # # #
                0x0452492492492082, //#=############## # #
                0x0449249249249282, //#================# #
                0x0492492492492482, //################## #
                0x04C0DB6DB6000002, //#i  gggggggg       #
                0x0492492492492492  //####################
            },
            new long[] {
                0x0492492492492492, //####################
                0x045145145145144A, //#=#=#=#=#=#=#=#=#==#
                0x045105145105144A, //#=#= =#=#=#= =#=#==#
                0x0441041441041442, //#= = = =#= = = =#= #
                0x0441041041041042, //#= = = = = = = = = #
                0x0441041041041042, //#= = = = = = = = = #
                0x0441C41041C41042, //#= =g= = = =g= = = #
                0x0471071071071072, //#=g= =g= =g= =g= =g#
                0x0441441C41441C42, //#= =#= =g= =#= =g= #
                0x045145105145109A, //#=#=#=#= =#=#=#= #i#
                0x051145145145148A, //#w#=#=#=#=#=#=#=##=#
                0x0492492492492492  //####################
            },
            new long[] {
                0x0492492493492492, //###########i########
                0x0180180006006000, // g   g     g   g    
                0x0006006000006030, //   g   g       g  g 
                0x0000000000C00000, //            g       
                0x0180180010000000, // g   g    #         
                0x0000000190000C00, //         g#     g   
                0x0180000010C00000, // g        # g       
                0x0000006010000000, //       g  #         
                0x0000C00000180180, //    g        g   g  
                0x0180006000000000, // g     g            
                0x0000000000006000, //               g    
                0x0492492892492492  //########w###########
            },
            new long[] {
                0x0492492492492492, //####################
                0x044924926A24964A, //#=========O#====i==#
                0x056926D46A030182, //#OO===OO#=O#  g  g #
                0x056926D46A0924B2, //#OO===OO#=O# #####g#
                0x056DB6D46A000002, //#OOOOOOO#=O#       #
                0x056DB6D46A36DA4A, //#OOOOOOO#=O#=OOOO==#
                0x056D36D46DB49B6A, //#OOO=OOO#=OOOO==OOO#
                0x056DB6D46DB6DB6A, //#OOOOOOO#=OOOOOOOOO#
                0x0449249469B6DB4A, //#=======#=O=OOOOOO=#
                0x044924946924924A, //#=======#=O========#
                0x044C24944924924A, //#==w====#==========#
                0x0492492492492492  //####################
            },
            new long[] {
                0x0492492492492492, //####################
                0x0448001255B6D44A, //#==    ===#OOOOO#==#
                0x045818225126D46A, //#=i  g #==#===OO#=O#
                0x040000224924956A, //#      #========#OO#
                0x049249249244956A, //#############===#OO#
                0x043000800044954A, //# g   =     #===#O=#
                0x040001018044934A, //#     #  g  #====O=#
                0x049149200044954A, //###=####    #===#O=#
                0x044924A48A49254A, //#======###=######O=#
                0x044C24A24924924A, //#==w===#===========#
                0x044924A24924924A, //#======#===========#
                0x0492492492492492  //####################
            },
            new long[] {
                0x0492492492492492, //####################
                0x0449449B6D54A24A, //#===#===OOOO#O=#===#
                0x045945124954A24A, //#=i=#=#=====#O=#===#
                0x045245249124928A, //#=###=#####======#=#
                0x046DB5124949228A, //#=OOOO#=====####=#=#
                0x049129149244928A, //###==##=#####====#=#
                0x044818000925248A, //#==  g    ====####=#
                0x049248001124924A, //######    #========#
                0x040009249249248A, //#    #############=#
                0x0449276C0024928A, //#=====ggg   =====#=#
                0x046124924924924A, //#=w================#
                0x0492492492492492  //####################
            },
            new long[] {
                0x0492492512492492, //#########w##########
                0x0400000040000002, //#        =         #
                0x0006086080C30C00, //   g # g #  g g g   
                0x0400080080000002, //#    #   #         #
                0x0006086000C30C00, //   g # g    g g g   
                0x0400000000049002, //#            ===   #
                0x0006006000C49C00, //   g   g    g===g   
                0x0400080000049002, //#    #       ===   #
                0x0006086000C30C00, //   g # g    g g g   
                0x0400080000000002, //#    #             #
                0x0006006000C30C00, //   g   g    g g g   
                0x049249A492492492  //######i#############
            },
            new long[] {
                0x0492492492492492, //####################
                0x0449249369249A4A, //#========OO=====O==#
                0x0430001369249A4A, //# g    ==OO=====O==#
                0x044929236A48944A, //#====###=OO###==#==#
                0x040600244AC8950A, //#  g   ##==#g#==#w=#
                0x044B291248089492, //#==i=##====  #==####
                0x049248944A48924A, //######==#==###=====#
                0x0449249492249B4A, //#=======####====OO=#
                0x040000006DA9254A, //#        =OOO####O=#
                0x040600008924928A, //#  g     #=======#=#
                0x040000008924A24A, //#        #=====#===#
                0x0492492492492492  //####################
            },
            new long[] {
                0x0492492492492492, //####################
                0x0449249369A8924A, //#========OO=O#=====#
                0x044936936928940A, //#====OO==OO==#==# =#
                0x044924925528940A, //#=========#O=#==# =#
                0x0490C00491289582, //### g   ###==#==#g #
                0x0449249251A8940A, //#=========#=O#==# =#
                0x044924925124940A, //#=========#=====# =#
                0x044924925249244A, //#=========#######==#
                0x0449276D90000000, //#=====gggg#         
                0x0036DB625049248A, //  gggggg==# ######=#
                0x044924931009924A, //#========w#  #i====#
                0x0492492492492492  //####################
            },
            new long[] {
                0x049249224D249AD2, //########===O====Oi##
                0x056DA9638A492A92, //#OOOO##g=g=#####O###
                0x056DA0514036DB6A, //#OOOO  O O  =OOOOOO#
                0x056D21555036DB6A, //#OOO= #O#O# =OOOOOO#
                0x056DA1555036DB6A, //#OOOO #O#O# =OOOOOO#
                0x044921535024924A, //#==== #O=O# =======#
                0x044921555049248A, //#==== #O#O# ######=#
                0x044924D54924924A, //#======O#O=========#
                0x044924D489249292, //#======O##=======###
                0x044924D24924924A, //#======O===========#
                0x044930924924924A, //#====w=============#
                0x0492492492492492  //####################
            },
            new long[] {
                0x04D2492492492492, //#i##################
                0x0406080080010082, //#  g #   #    #  # #
                0x0400012480010002, //#     ####    #    #
                0x0400400000C00182, //#   #       g    g #
                0x0400400000010002, //#   #         #    #
                0x0400000400010002, //#       #     #    #
                0x0480080492000002, //##   #  ####       #
                0x0430080000002402, //# g  #         ##  #
                0x0400092000092002, //#    ###     ###   #
                0x040000000000000C, //#                 =w
                0x0400C10030000C12, //#   g #   g     g ##
                0x0492492492492492  //####################
            },
            new long[] {
                0x0492492492492492, //####################
                0x044924924924928A, //#================#=#
                0x045248A49249228A, //#=####=#########=#=#
                0x045000000000224A, //#=#            #===#
                0x045001249249228A, //#=#   ##########=#=#
                0x0451BB6DB6D5A28A, //#=#=OggggggggOi#=#=#
                0x04514B6DB6D4228A, //#=#=##gggggggO #=#=#
                0x045129249249228A, //#=#==###########=#=#
                0x045424924924928A, //#=#w=============#=#
                0x045245248A49248A, //#=###=####=#######=#
                0x044924924924924A, //#==================#
                0x0492492492492492  //####################
            },
            new long[] {
                0x042D4924924ADB52, //# OO##########OOOO##
                0x04492512494ADB5A, //#=====#=====##OOOOi#
                0x054D451251289292, //#O=O#=#===#==#===###
                0x056D450011280002, //#OOO#=#   #==#     #
                0x056D456011280002, //#OOO#=#g  #==#     #
                0x056D451251280032, //#OOO#=#===#==#    g#
                0x054D456011280DB2, //#O=O#=#g  #==#  ggg#
                0x056D45001128924A, //#OOO#=#   #==#=====#
                0x044945125128A28A, //#===#=#===#==#=#=#=#
                0x044945125128A28A, //#===#=#===#==#=#=#=#
                0x050944925124924A, //#w==#=====#========#
                0x0449492492492492  //#===################
            },
            new long[] {
                0x0492492492492492, //####################
                0x046D25A26D24924A, //#=OO==i#==OO=======#
                0x044949226D24DB6A, //#===####==OO===OOOO#
                0x0400449289495B6A, //#   #====#==###OOOO#
                0x0430406089255B6A, //# g #  g #====#OOOO#
                0x044D24929245148A, //#==O=====####=#=##=#
                0x049549248924928A, //###O######=======#=#
                0x044924903124928A, //#=======  g======#=#
                0x049249200129224A, //########   ==###===#
                0x044924900124924A, //#=======   ========#
                0x0509249C0124924A, //#w======g  ========#
                0x0492492492492492  //####################
            },
            new long[] {
                0x0492492492492492, //####################
                0x04EDB692B524924A, //#iOOOOO==#gO=======#
                0x056DB692B224924A, //#OOOOOO==#g#=======#
                0x056DB692B134924A, //#OOOOOO==#g==O=====#
                0x056DA4927124924A, //#OOOO=====g========#
                0x056D2495B624924A, //#OOO====#ggg=======#
                0x0569249DB6C4924A, //#OO=====ggggg======#
                0x0549249DB6C4924A, //#O======ggggg======#
                0x0449249DB6C4984A, //#=======ggggg===w==#
                0x0449249DB6C4924A, //#=======ggggg======#
                0x0449249DB6C4924A, //#=======ggggg======#
                0x0492492492492492  //####################
            },
            new long[] {
                0x0492492492492492, //####################
                0x04C924000624924A, //#i====     g=======#
                0x049249249249248D, //##################=O
                0x04481A9206A4924A, //#==  gO==  gO======#
                0x044800920024924A, //#==   ===   =======#
                0x0A52492492492492, //O=##################
                0x0448009B8026DB4A, //#==   ==Og  ==OOOO=#
                0x044E00920024924A, //#==g  ===   =======#
                0x049249249249248D, //##################=O
                0x04481A9200A4800A, //#==  gO==   O==   =#
                0x050800938024E00A, //#w=   ===g  ===g  =#
                0x0492492492492492  //####################
            },
            new long[] {
                0x0492492492492492, //####################
                0x046548924924924A, //#=wO##=============#
                0x044DA8924924924A, //#==OO#=============#
                0x044DA9149249248A, //#==OO##=##########=#
                0x044DA5124B00008A, //#==OO=#====i     #=#
                0x044DA524924B6C8A, //#==OO=########ggg#=#
                0x044DA56DB60B6C8A, //#==OO=#ggggg #ggg#=#
                0x0455A560000B648A, //#=#OO=#g     #gg##=#
                0x044DA560000B6D4A, //#==OO=#g     #gggO=#
                0x044DA5000009248A, //#==OO=#      #####=#
                0x044DA4800004924A, //#==OO==      ======#
                0x0492492492492492  //####################
            },
            new long[] {
                0x0492492492492492, //####################
                0x05B6DB6C001B6DB2, //#gggggggg    gggggg#
                0x0592492492492482, //#g################ #
                0x0596DB61B6D80C82, //#g#ggggg ggggg  g# #
                0x0596492492492082, //#g#g############ # #
                0x0596400030192CB2, //#g#g#     g  g##g#g#
                0x0596400C30C1A0B2, //#g#g#   g g g i# #g#
                0x0416482492492C82, //# #g## #########g# #
                0x0590C300060060B2, //#g# g g    g   g #g#
                0x0412492092492482, //# ###### ######### #
                0x0586180030C30C32, //#g g g    g g g g g#
                0x0492492492492892  //################w###
            },
            new long[] {
                0x0492492492492492, //####################
                0x0410410410410402, //# # # # # # # # #  #
                0x0410410410410402, //# # # # # # # # #  #
                0x0410410410410402, //# # # # # # # # #  #
                0x0790590588588582, //ig# #g# #g= #g= #g #
                0x0410410210410404, //# # # # = # # # #  w
                0x0410410410410402, //# # # # # # # # #  #
                0x0410408410210202, //# # # = # # = # =  #
                0x0408410410410402, //# = # # # # # # #  #
                0x0410410410410402, //# # # # # # # # #  #
                0x0416416416416432, //# #g# #g# #g# #g# g#
                0x0492492492492492  //####################
            },
            new long[] {
                0x0492492492492492, //####################
                0x04492892494C924A, //#====#======#i=====#
                0x056DA800490ADB6A, //#OOOO#   === #OOOOO#
                0x056DA8004A015B6A, //#OOOO#   ==#  #OOOO#
                0x056DA70049402B6A, //#OOOO=g  ===#  #OOO#
                0x054DA4A4924B056A, //#O=OO==#######g #OO#
                0x056DB6928925004A, //#OOOOOO==#====#  ==#
                0x056DB49289249B8A, //#OOOOO===#======Og=#
                0x044924928924DDB2, //#========#=====Oggg#
                0x0449249289249DB2, //#========#======ggg#
                0x044984928924ADB2, //#===w====#=====#ggg#
                0x0492492492492492  //####################
            },
            new long[] {
                0x04D2492492492492, //#i##################
                0x0400349249A4056A, //#   =O======O=  #OO#
                0x0406552492A7056A, //#  g#O######O=g #OO#
                0x049244800AA40B6A, //#####==   =#O=  OOO#
                0x0449490C0A24026A, //#===### g =#==  ==O#
                0x046145016D49246A, //#=w=#=#  OOO#####=O#
                0x044946924924944A, //#===#=O=========#==#
                0x049001249245248A, //###   #######=####=#
                0x0400C1000000008A, //#   g #          #=#
                0x044921049249248A, //#==== # ##########=#
                0x0449250000C0924A, //#=====#     g =====#
                0x0492492492492492  //####################
            },
            new long[] {
                0x0492492492492492, //####################
                0x04C924DB6D249262, //#i=====OOOOO======w#
                0x044924DB6D24924A, //#======OOOOO=======#
                0x0492491289492002, //#######==#==####   #
                0x0449B49289382C02, //#===OO===#===g #g  #
                0x044925249240224A, //#=====#######  #===#
                0x040006DB4920124A, //#    =OOOO===  ====#
                0x043006DB4920124A, //# g  =OOOO===  ====#
                0x049249149224924A, //#######=####=======#
                0x044900004A24924A, //#===     ==#=======#
                0x044903004924924A, //#===  g  ==========#
                0x0492492492492492  //####################
            },
            new long[] {
                0x0492492485492492, //########## O########
                0x0465B6DB6DB6DB52, //#=wOOOOOOOOOOOOOOO##
                0x0452B6DB6DB6DA4A, //#=##OOOOOOOOOOOOO==#
                0x044956DAEDB6D44A, //#===#OOOOiOOOOOO#==#
                0x04492ADB6DB6924A, //#====#OOOOOOOOO====#
                0x040008DB6DB5124A, //#    #=OOOOOOO#====#
                0x040608AB6DA8024A, //#  g #=#OOOOO#  ===#
                0x044924A29244024A, //#======#=####=  ===#
                0x044924920127024A, //#========  ===g ===#
                0x044924938124024A, //#========g ===  ===#
                0x044924920124024A, //#========  ===  ===#
                0x0492492492492492  //####################
            },
            new long[] {
                0x0492492492492492, //####################
                0x0441449249249242, //#= =#============= #
                0x040146A492490382, //#  =#=O######## =g #
                0x044045136D28900A, //#=  #=#==OOO=#==  =#
                0x045005136D28800A, //#=#  =#==OOO=#=   =#
                0x051201136D28000A, //#w##  #==OOO=#    =#
                0x0490C1336D280492, //### g #i=OOO=#  ####
                0x044001249128804A, //#=    #####==#=  ==#
                0x040000924928900A, //#     =======#==  =#
                0x0581452492492232, //#g =#=##########= g#
                0x0409449249249242, //# ==#============= #
                0x0492492492492492  //####################
            },
            new long[] {
                0x0492495492492492, //#######O############
                0x044924D24924924A, //#======O===========#
                0x044924D24929254A, //#======O=====####O=#
                0x044A00D492B6954A, //#==#  =O####OOO=#O=#
                0x044A01524925154A, //#==#  #O======#=#O=#
                0x044A01549225154A, //#==#  #O####==#=#O=#
                0x044AC1524A49148A, //#==#g #O===####=##=#
                0x044A49124E01344A, //#==####====g  #i#==#
                0x044944949001244A, //#===#===###   ###==#
                0x046145244800924A, //#=w=#=###==   =====#
                0x044944925124924A, //#===#=====#========#
                0x0492492492492492  //####################
            },
            new long[] {
                0x0492492492492492, //####################
                0x0449249249269202, //#=============O==  #
                0x04124924924AA202, //# ############O#=  #
                0x041124DB4924A382, //# #====OOO=====#=g #
                0x0411495B52492B6A, //# #=###OOO######OOO#
                0x059124944924024A, //#g#=====#=====  ===#
                0x059548944924024A, //#g#O##==#=====  ===#
                0x059129249226824A, //#g#==#######==O ===#
                0x0591270000029492, //#g#===g       O=####
                0x059249249248924A, //#g############=====#
                0x0409000DB6689262, //# ==    ggggi#====w#
                0x0492492492492492  //####################
            },
            new long[] {
                0x0492492492492492, //####################
                0x040920018124924A, //# ===    g ========#
                0x041249249249248A, //# ################=#
                0x0411200030249288, //# #==     g =====#= 
                0x04124924524922B2, //# #######=######=#g#
                0x058924944924A282, //#g======#======#=# #
                0x04924914912CA282, //#######=###==i=#=# #
                0x0449251252492282, //#=====#===######=# #
                0x0449252440180082, //#=====###=   g   # #
                0x044924949249248A, //#=======##########=#
                0x050924924018004A, //#w========   g   ==#
                0x0492492492492492  //####################
            },
            new long[] {
                0x04924924924AD492, //##############OO####
                0x050924924936D24A, //#w===========OOO===#
                0x044A49124A44948A, //#==####====##===##=#
                0x0449240D8004944A, //#=====  gg   ===#==#
                0x049249249249544A, //###############O#==#
                0x044925126924924A, //#=====#===O========#
                0x0452449469252492, //#=###===#=O===######
                0x04524924AA49618A, //#=########O####g g=#
                0x0449449469250202, //#===#===#=O===# =  #
                0x045001124A250C0A, //#=#   #====#==# g =#
                0x0450C1144A24E05A, //#=# g #=#==#===g =i#
                0x0492492492492492  //####################
            },
            new long[] {
                0x0492492492492492, //####################
                0x050944944944924A, //#w==#===#===#======#
                0x0449240240248D82, //#=====  ==  === gg #
                0x0491440440448002, //###=#=  #=  #==    #
                0x04E9470470448002, //#iO=#=g #=g #==    #
                0x04AA492492492292, //##O#############=###
                0x04492B000140024A, //#====#g    =#   ===#
                0x049228000120024A, //####=#     ==   ===#
                0x043009229258024A, //# g  ###=####g  ===#
                0x040008003158024A, //#    #    g=#g  ===#
                0x044924000140024A, //#=====     =#   ===#
                0x0492492492492492  //####################
            },
            new long[] {
                0x0492492492492492, //####################
                0x0489240C0600125A, //##====  g  g   ===i#
                0x044A492492492492, //#==#################
                0x045224924925124A, //#=##==========#====#
                0x044A29249245148A, //#==#=########=#=##=#
                0x048A25000045144A, //##=#==#     #=#=#==#
                0x044A4480A0451452, //#==##==  #w #=#=#=##
                0x045249000644944A, //#=#####    g#===#==#
                0x045125249249248A, //#=#===############=#
                0x045144928928928A, //#=#=#====#===#===#=#
                0x044944A24A24A24A, //#===#==#===#===#===#
                0x0492492492492492  //####################
            },
            new long[] {
                0x0490410410410492, //### # # # # # # ####
                0x0450410410410442, //#=# # # # # # # #= #
                0x0448208208208242, //#== = = = = = = == #
                0x0450410410410442, //#=# # # # # # # #= #
                0x04D6596596596472, //#i#g#g#g#g#g#g#g#=g#
                0x0492492492492482, //################## #
                0x0450410410410442, //#=# # # # # # # #= #
                0x0450410410410442, //#=# # # # # # # #= #
                0x0448208208208242, //#== = = = = = = == #
                0x0450410410410442, //#=# # # # # # # #= #
                0x0516596596596442, //#w#g#g#g#g#g#g#g#= #
                0x0492492492492492  //####################
            },
            new long[] {
                0x0490410410410492, //### # # # # # # ####
                0x0450410410410442, //#=# # # # # # # #= #
                0x0448208208208242, //#== = = = = = = == #
                0x0516596596596472, //#w#g#g#g#g#g#g#g#=g#
                0x0750410410410442, //iO# # # # # # # #= #
                0x0552492492492482, //#O################ #
                0x0550410410410442, //#O# # # # # # # #= #
                0x0550410410410442, //#O# # # # # # # #= #
                0x0448208208208242, //#== = = = = = = == #
                0x0496596596596442, //###g#g#g#g#g#g#g#= #
                0x0490410410410442, //### # # # # # # #= #
                0x0492492492492492  //####################
            }
        };

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public static int Count { get => obild.Length;}

        /// <summary>
        /// Gets the level.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <returns>System.Nullable&lt;FieldDef&gt;[].</returns>
        static public FieldDef[]? GetLevel(int level)
        {
            if (level < 0 || level >= obild.Length) return null;
            FieldDef[] result=new FieldDef[20*12];
            for (int i = 0; i < 12; i++)
                for (int j = 0; j < 20; j++)
                {
                    result[j+i*20] = (FieldDef)((obild[level][i] >> ((19-j)*3)) & 7);
                }
            return result;
        }
    }
}
