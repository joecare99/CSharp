using System;

namespace GenFree.Data;

public enum nbTables
{
    Ahnen1,
    Ahnen2,
    Frauen1,
    Frauen2
}
public enum PersonFields
{
    AnlDatum,
    Sex,
    EditDat,
    Bem1,
    Pruefen,
    PersNr,
    Konv,
    Such1,
    Bem2,
    Bem3,
    OFB,
    religi,
    PUid,
    Such2,
    Such3,
    Such4,
    Such5,
    Such6,
}

public enum PersonIndex
{
    BeaDat,
    PerNr,
    Puid,
    reli,
    Such1,
    Such2,
    Such3,
    Such4,
    Such5,
    Such6
}
public enum EventFields
{
    Art,
    PerFamNr,
    DatumV,
    DatumB_S,
    DatumV_S,
    DatumB,
    KBem,
    Ort,
    Ort_S,
    Reg,
    Bem1,
    Bem2,
    Platz,
    VChr,
    LfNr,
    Bem3,
    Bem4,
    ArtText,
    Zusatz,
    priv,
    tot,
    DatumText,
    Causal,
    an,
    Hausnr,
    Hausnr1, // Obsolete
    GrabNr,
}

public enum EventIndex
{
    ArtNr,
    BeSu,
    CText,
    Datbs,
    DatInd,
    Datvs,
    EOrt,
    HaNu,
    JaTa,
    KText,
    NText,
    PText,
    Reg,
    Reg1
}
public enum FamilyFields
{
    AnlDatum,
    EditDat,
    Prüfen,
    Bem1,
    FamNr,
    Aeb,
    Name,
    Bem2,
    Bem3,
    Eltern,
    Fuid,
    Prae,
    Suf,
    ggv
}
public enum FamilyIndex
{
    BeaDat,
    Fam,
    Fuid
}

public enum OFBIndex
{
    InDNr,
    IndNum,
    Indn
}

public enum OFBFields
{
    PerNr,
    Kennz,
    TextNr
}

public enum PlaceFields
{
    Ort,
    Ortsteil,
    Kreis,
    Land,
    Staat,
    Staatk,
    PLZ,
    Terr,
    Loc,
    L,
    B,
    Bem,
    OrtNr,
    Zusatz,
    GOV,
    PolName,
    g
}
public enum PlaceIndex
{
    K,
    L,
    O,
    Orte,
    OrtNr,
    OT,
    Pol,
    S
}

public enum PictureFields
{
    ZuNr,
    Kennz,
    Bem,
    Datei,
    Pfad,
    LfNr,
    Form,
    Beschreibung,
    Format
}

public enum PictureIndex
{
    Bild,
    Nr,
    PerKen2,
    PerKenn
}
public enum NameFields
{
    PersNr,
    Kennz,
    Text,
    LfNr,
    Ruf,
    Spitz,
    LfNr1 // Only trmporary for conversion
}
public enum NameIndex
{
    NamKenn,
    PNamen,
    TxNr,
    Vollname
}

[Obsolete()]
public enum LinkFields
{
    Kennz,
    FamNr,
    PerNr
}

public enum LinkIndex
{
    ElSu,
    FamNr,
    FamPruef,
    FamSu,
    FamSu1,
    PAFI,
    Per
}

public enum TexteFields
{
    Txt,
    Kennz,
    TxNr,
    Bem,
    Leitname
}

public enum TexteIndex
{
    ITexte,
    KText,
    RTexte,
    LTexte,
    SSTexte,
    STexte,
    TxNr,
    TxNr1,
}

public enum WDBLFields
{
    Nr,
    Name,
    Ort,
    PLZ,
    Strasse,
    Fon,
    Mail,
    Http,
    Bem,
    Suchname
}

public enum WDBLIndex
{
    Name,
    Nr,
    Such
}

public enum AhnenFields
{
    Gen,
    PerNr,
    Weiter,
    EHE,
    Ahn,
    Name,
    Spitz
}

public enum QuFields
{
    _1,
    _2,
    _3,
    _4, 
    _5,
    _6,
    _7,
    _8,
    _9,
    _10,
    _11,
    _12,
    _13
}

public enum AhnenIndex
{
    Spitz,
    Namen,
    PerNr,
    Gen,
    Ahnen,
    Implex
}

public enum WitnessFields
{
    FamNr,
    PerNr,
    Kennz,
    Art,
    LfNr
}

public enum WitnessIndex
{
    ElSu,
    Fampruef,
    FamSu,
    Zeug,
    ZeugSu
}

public enum FrauenFields
{
    Nr,
    Name,
    PNr,
    Kek,
    Suchname
}

public enum FrauenIndex
{
    Name,
    Nr,
    Such
}

public enum PropertyFields
{
    Nr,
    Akte,
    Pers
}

public enum PropertyIndex
{
    Akt,
    NuAkPer,
    Per
}

public enum SourceLinkIndex
{
    Tab, // Person/Family-Index
    Tab22, // Event-Index
}
public enum SourceLinkFields
{
    /// <summary>
    /// Type of Link
    /// </summary>
    _1, // Kind    
    /// <summary>
    /// The Source Nr (Person/Family/Event)
    /// </summary>
    _2, // Nr
    _3, // NrNeu    
    /// <summary>
    /// The Page
    /// </summary>
    _4, // Page
    Aus,
    Orig,
    Kom,
    Art,
    LfNr,
}

public enum SourceFields
{
    _1,
    _2,
    _3,
    _4,
    _5,
    _6,
    _7,
    _8,
    _9,
    _10,
    _11,
    _12,
    _13
}

public enum SourceIndex
{
    Autor,
    Dopp,
    Nam,
    Nr,
    Zitat
}
public enum RepoFields
{
    Nr,
    Name,
    Ort,
    PLZ,
    Strasse,
    Fon,
    Mail,
    Http,
    Bem,
    Suchname
}
public enum ENameKennz : int
{
    nkGivnName = 0,
    nkName = 1,
    nkPrefix = 2,
    nkSuffix = 3,
    nkClanName = 4,
    nkPraeName = 5,
    nkAlias = 6,
    nkStatus = 7,
}

public enum ELinkKennz : int
{
    lkNone = 0,
    lkFather = 1,
    lkMother = 2,
    lkChild = 3,
    lkGodparent = 4,
    lkMarrWitness = 5,
    lkWitnOfEngage = 6,
    lkWitnOfMarr = 7,
    lkAdoptedChild = 8,
    lk9 = 9,
    //lkWitness = 4,
    //lkGodfather = 5,
    //lkGodmother = 6,
    //lkGodparent = 7,

}

public enum EGedSource
{
    _0 = 0,
    _1 = 1,
    _2 = 2,
    _3 = 3,
    _4 = 4,
    _5 = 5,
    _6 = 6,
    _7 = 7,
    _8 = 8,
    _9 = 9,
    _10 = 10,
    _11 = 11,
    _12 = 12,
    _13 = 13,
    _14 = 14,
    _15 = 15,
    _16 = 16,
    _17 = 17,
    _18 = 18,
    _19 = 19,
    _21 = 21,
}

public enum ETextKennz : int
{
    tkNone = '\0',
    /// <summary>
    /// The Prefix
    /// </summary>
    A_ = 'A',
    /// <summary>
    /// The Suffix
    /// </summary>
    B_ = 'B',
    /// <summary>
    /// The Alias
    /// </summary>
    C_ = 'C',
    /// <summary>The ???</summary>
    D_ = 'D',
    /// <summary>The occupation</summary>
    E_ = 'E',
    /// <summary>
    /// The Givenname (female)
    /// </summary>
    F_ = 'F',
    G_ = 'G',
    /// <summary>The place/village/town</summary>
    H_ = 'H',
    /// <summary>The suburb</summary>
    I_ = 'I',
    /// <summary>The district/county</summary>
    J_ = 'J',
    /// <summary>The country</summary>
    K_ = 'K',
    /// <summary>The state</summary>
    L_ = 'L',
    M_ = 'M',
    /// <summary>
    /// The Familyname
    /// </summary>
    tkName = 'N',
    /// <summary>
    /// The Chruch/Cemetry/Firm etc.
    /// </summary>
    O_ = 'O',
    P_ = 'P',
    Q_ = 'Q',
    R_ = 'R',
    S_ = 'S',
    /// <summary>The event-name</summary>
    T_ = 'T',
    /// <summary>The status/condition</summary>
    U_ = 'U',
    /// <summary>
    /// The Givenname (male)
    /// </summary>
    V_ = 'V',
    W_ = 'W',
    Y_ = 'Y',
    /// <summary>
    /// The religion
    /// </summary>
    Z_ = 'Z',
    tk1_ = '1',
    /// <summary>The predicate</summary>
    tk2_ = '2',
    tk3_ = '3',
    tk4_ = '4',
    /// <summary>
    /// The number of the house 
    /// </summary>
    tk5_ = '5',
    tk6_ = '6',
    /// <summary>The confession</summary>
    tk7_ = '7',
}
