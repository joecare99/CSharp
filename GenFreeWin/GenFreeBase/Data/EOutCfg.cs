using System;

namespace Gen_FreeWin;
public enum EOutCfg : int
{
    oNone = 0,
    o01_Person = 1,
    o02 = 2,
    o03 = 3,
    o04_Family = 4,
    o07_KeepFormat = 7,
    o8 = 8,
    o9 = 9,
    o10_EmitIDs = 10,
    o11 = 11,
    o12 = 12,
    o13 = 13,
    o14 = 14,
    o15 = 15,
    o17 = 17,
    o20 = 20,
    o21 = 21,
    o23 = 23,
    o24 = 24,
    o26 = 26,
    o27 = 27,
    o30 = 30,
    GodpWithoutData = 31,
    o32 = 32,
    o34 = 34,
    o35 = 35,
    NoCauseOfDeath = 36,
    o38 = 38,
    o39 = 39,
    o44 = 44,
    o45 = 45,
    o46 = 46,
    o47 = 47,
    o48 = 48,
}

[Obsolete("use EOutCfg") ]
public enum ENameSrchOpt
{
    EmitIDs = 10,
    EmitAncestNo = 11,
    EmitDescNo = 12,
    PicturePath = 13,
    Structured = 14,
    o24 = 24,
    Godparents = 30,
    Witnesses = 32,
    WitnWithoutData = 33,
    EmitDocumentNo = 34,
    ShortenPlaces = 35,
    NoCauseOfDeath = 36,
    WitnessOf = 37,
    GodparentOf = 38,
    EmitSources = 39,
    OrderDeathSecond = 40,
    EmitPictures = 41,
    PersonPictOnly = 42,
    PersonBaseDatesOnly = 43,
    o44_PictOrginalSize = 44,
}

[Obsolete("use EOutCfg")]
public enum EFraSelPrintNotes
{
    None = 0,
    o01_Person = 1,
    UpperPersonDate = 2,
    LowerPersonDate = 3,
    o04_Family = 4,
    UpperFamilyDate = 5,
    LowerFamilyDate = 6,
    o07_KeepFormat = 7,
}