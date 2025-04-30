namespace Gen_FreeWin;

public enum EUserText : int
{
    tNone = 0,
    tName = 1,
    tGivenname = 2,
    tBirthdaySh = 3,
    tBaptismSh = 4,
    tDeathSh = 5,
    tBurialSh = 6,
    tOccupation = 7,
    tResidence = 8,
    tDel_PersonNo = 9,
    tBorn_Name = 10,
    tDeath_PNo_FNo_ANo = 11,
    tChild_AS = 12,
    tMarrCount = 13,
    tFamilyNo = 14,
    tCreationDt = 15,
    tEngagement = 16,
    tChildCount = 17,
    tProclamation = 18,
    tMarriage = 19,
    tMarriageRelig = 20,
    tDivorce = 21,
    tHLT_Sealing = 22,
    tMarrWitness = 23,
    tIllegalRel = 24,
    tPartnership = 25,
    tGrandfather = 26,
    tGrandmother = 27,
    tUnknown = 28,
    tCommonFamName = 29,
    tWitnOfEngage = 30,
    tWitnOfMarr = 31,
    tAdoptedChild = 32,
    tReligion = 33,
    tBirth = 34,
    tBaptism = 35,
    tDeath = 36,
    tBurial = 37,
    tConfirmation = 38,
    tAlias = 39,
    tDate = 40,
    tText = 41,
    tPlace = 42,
    tChurchCemetrEtc = 43,
    t44 = 44,
    tEventNotes1 = 45,
    tEventNotes2 = 46,
    tNMSave = 47,
    tNMCancel = 48,
    tNMNewPlace = 49,
    tNo = 50,
    tYes = 51,
    tPersonInFams = 52,
    tPersonInFam = 53,
    tGodparents = 54,
    tGodparentOf = 55,
    tNMPrint = 56,
    tNMBack = 57,
    tSpouseUnknown = 58,
    tSiblingNHalfsiblings = 59,
    tSiblings = 60,
    tPersonSheetFor = 61,
    tFamilySheetFor = 62,
    tDelete = 63,
    tNMSaveToFamily = 64,
    t65 = 65,
    tMarrTo = 66,
    t67 = 67,
    t68 = 68,
    t69 = 69,
    t70 = 70,
    t71 = 71,
    t72 = 72,
    t73 = 73,
    t74 = 74,
    t75 = 75,
    t76 = 76,
    t77 = 77,
    t78 = 78,
    t79 = 79,
    t80 = 80,
    /// <summary>
    /// Eltern
    /// </summary>
    t82 = 82,
    /// <summary>
    /// Familien
    /// </summary>
    t83_Families = 83,
    /// <summary>
    /// Personen
    /// </summary>
    t84_Persons = 84,
    /// <summary>
    /// Places
    /// </summary>
    t85_Places = 85,
    /// <summary>
    /// Mandants
    /// </summary>
    t86_Mandants = 86,
    /// <summary>
    /// manage texts
    /// </summary>
    t87 = 87,
    /// <summary>
    /// Printing
    /// </summary>
    t88_Print = 88,
    /// <summary>
    /// Import/Export
    /// </summary>
    t89 = 89,
    /// <summary>
    /// Addresses
    /// </summary>
    t90_Address = 90,
    /// <summary>
    /// Calculations
    /// </summary>
    t91 = 91,
    /// <summary>
    /// Function-Keys
    /// </summary>
    t92 = 92,
    /// <summary>
    /// Configuration
    /// </summary>
    t93_Config = 93,
    /// <summary>
    /// Check persons
    /// </summary>
    t94 = 94,
    /// <summary>
    /// Check families
    /// </summary>
    t95 = 95,
    t96 = 96,
    t97_EndProg = 97,
    t98 = 98,
    /// <summary>Orte</summary>
    t99_Places = 99, // Orte
    t100_Dates = 100,
    t101_Texts = 101,
    t102 = 102,
    t103 = 103,
    t104 = 104,
    t105 = 105,
    t106 = 106,
    t107 = 107,
    t108 = 108,
    t109 = 109,
    t110 = 110,
    t111 = 111,
    t112 = 112,
    t113 = 113,
    t114 = 114,
    t115 = 115,
    t116 = 116,
    t117 = 117,
    t118 = 118,
    t119 = 119,
    t120 = 120,
    t121 = 121,
    t123 = 123,
    t124 = 124,
    t125 = 125,
    t126 = 126,
    t127 = 127,
    t128 = 128,
    /// <summary>
    /// Media
    /// </summary>
    t129 = 129,
    /// <summary>
    /// Kinder aus allen Ehen
    /// </summary>
    t130 = 130,
    t131 = 131,
    t132 = 132,
    t133 = 133, // Großeltern mütterlicherseits Familie=
    t134 = 134, // Großeltern mütterlicherseits unbekannt.
    t135 = 135, // Eltern unbekannt.
    t136 = 136, // Vater einfügen
    t137 = 137, // Mutter einfügen
    t138 = 138, // Kind einfügen
    t139 = 139, // Ort:
    t140 = 140, // Ortsteil:
    t141 = 141, // Kreis:
    t142 = 142, // Land:
    t143 = 143, // Staat:
    t144 = 144, // Kirche/Friedhof/Firma etc.:
    t145 = 145, // Geburtsname:
    t146 = 146, // Vorname weiblich:
    t147 = 147, // Vorname männlich:
    t148 = 148, // Namenszusatz 1:
    t149 = 149, // Namenszusatz 2:
    t150 = 150, // Berufe:
    t151 = 151, // Kurzbemerkung:
    t152 = 152, // Platz:
    t153 = 153, // Orte:
    t154 = 154, // Kreise:
    t155 = 155, // Länder:
    t156 = 156, // Staaten:
    t157 = 157, // Kirche/Friedhof/Firma etc.:
    t158 = 158, // Suffix:
    t159 = 159, // Prefix:
    t162 = 162, // Vater
    t163 = 163, // Mutter        
    /// <summary>
    /// Name               Geburt Taufe  Gestorben  Begraben  Beruf  Titel  Wohnort
    /// </summary>
    t166 = 166, // Ehepartner        
    /// <summary>
    ///                      DO    DO       DO        DO     B VBO   T VBO   SVBO
    /// </summary>
    t167 = 167, // Ehepartnerin        
    /// <summary>
    /// wurde noch nicht eingegeben!
    /// </summary>
    t172 = 172,
    /// <summary>
    /// Person
    /// </summary>
    t173 = 173,
    /// <summary>
    /// Familie
    /// </summary>
    t174 = 174,
    t175 = 175,
    t176 = 176,
    t177 = 177,
    t178 = 178,
    t179 = 179,
    /// <summary>
    /// Geburtsname
    /// </summary>
    t180 = 180, // Geburtsname        
    /// <summary>
    /// Vorname weiblich
    /// </summary>
    t181 = 181, // Vorname weiblich
    /// <summary>Vorname männlich</summary>
    t182 = 182, // Vorname männlich
    /// <summary>Namenszusatz 1</summary>
    t183 = 183, // Namenszusatz 1
    /// <summary>Namenszusatz 2</summary>
    t184 = 184, // Namenszusatz 2
    /// <summary>Berufe</summary>
    tTitle = 185, // Berufe
    /// <summary>Kurzbemerkung</summary>
    t186 = 186,
    /// <summary>Platz</summary>
    t187 = 187,
    /// <summary>Ortsteile</summary>
    t188 = 188,
    /// <summary>Kreise</summary>
    t189 = 189,
    /// <summary>Länder</summary>
    t190 = 190,
    /// <summary>Staaten</summary>
    t191 = 191,
    t192 = 192,
    t193 = 193,    
    /// <summary>Kirche/Friedhof/Firma etc.</summary>
    t194 = 194,
    /// <summary>Suffix</summary>
    t195 = 195,
    /// <summary>Prefix</summary>
    t196 = 196,
    t197 = 197,
    t198 = 198,
    /// <summary>
    /// Check Family
    /// </summary>
    t200 = 200,
    t201 = 201,
    t202 = 202,
    t204 = 204,    
    /// <summary>
    /// Family-errors
    /// </summary>
    t205 = 205,
    t207 = 207,
    /// <summary>
    /// switched off
    /// </summary>
    t208 = 208,
    /// <summary>
    /// difference of age for couple 
    /// </summary>
    t209 = 209,
    /// <summary>
    /// Managing sources
    /// </summary>
    t210 = 210,
    /// <summary>
    ///bei Heirat zu jung !
    /// </summary>
    t211 = 211,
    /// <summary>
    ///  bei Heirat schon verstorben !
    /// </summary>
    t212 = 212,
    /// <summary>
    /// Vater vor Zeugung Kind verstorben !
    /// </summary>
    t213 = 213,
    /// <summary>
    /// Mutter vor Geburt Kind verstorben !
    /// </summary>
    t214 = 214,
    /// <summary>
    /// Jahre vor der Heirat geboren!
    /// </summary>
    t215 = 215,
    /// <summary>
    /// Mutter bei Geburt Kind
    /// </summary>
    t216 = 216,
    t217 = 217,
    /// <summary>
    /// Ausgabe der Bemerkungen
    /// </summary>
    t218 = 218,
    /// <summary>
    /// Personenbemerkungen
    /// </summary>
    t219 = 219,
    /// <summary>
    /// obere Bemerkungen zum Personendatum
    /// </summary>
    t220 = 220,
    /// <summary>
    /// untere Bemerkungen zum Personendatum
    /// </summary>
    t221 = 221,
    /// <summary>
    /// Familienbemerkungen
    /// </summary>
    t222 = 222,
    /// <summary>
    /// obere Bemerkungen zum Familiendatum
    /// </summary>
    t223 = 223,
    /// <summary>
    /// untere Bemerkungen zum Familiendatum
    /// </summary>
    t224 = 224,
    /// <summary>
    /// Personen und Familiennummern ausgeben?
    /// </summary>
    t234 = 234,
    /// <summary>
    /// . Proband
    /// </summary>
    t235 = 235,
    t236_Families = 236,
    t237_Persons = 237,
    /// <summary>
    /// &Nachfahrenberechnung
    /// </summary>
    t238 = 238,
    /// <summary>
    /// Nachf.-Nr.:
    /// </summary>
    t239 = 239,
    /// <summary>
    /// &Ahnenberechnung
    /// </summary>
    t241 = 241,
    t243 = 243,
    t244 = 244,
    tCivilState = 245, // Zivilstand
    t247 = 247,
    /// <summary>
    /// Dubletten
    /// </summary>
    t248 = 248,
    /// <summary>
    /// Bemerkungen duchsuch.
    /// </summary>
    t249_Property = 249, // !!!
    t250 = 250,
    t251 = 251,
    t252 = 252,
    t253 = 253,
    t254 = 254,
    t255 = 255,
    t256 = 256,
    t257 = 257,
    tAKA = 258, // AKA        
    /// <summary>
    /// Enter License Key
    /// </summary>
    t278 = 278,
    t287 = 287,
    /// <summary>
    /// Zeugen 
    /// </summary>
    t301 = 301,
    /// <summary>
    /// Zeuge bei 
    /// </summary>
    t302 = 302,
    t303 = 303,
    t304 = 304,
    t336 = 336,
    t341 = 341,
    t347 = 347,
    t348 = 348,
    /// <summary>
    /// Birth day today
    /// </summary>
    t435 = 435,
    /// <summary>
    /// Death day today
    /// </summary>
    t436 = 436,
    /// <summary>
    /// Marriage day today
    /// </summary>
    t437 = 437,
    /// <summary>
    /// Rel. marriage day today
    /// </summary>
    t438 = 438,
    /// <summary>
    /// Having birthday in today
    /// </summary>
    t439 = 439,
    /// <summary>
    /// Having death day in today
    /// </summary>
    t440 = 440,
    /// <summary>
    /// Having marriage day in today
    /// </summary>
    t441 = 441,
    /// <summary
    /// Having rel. marriage day in today
    /// </summary>
    t442 = 442,
    /// <summary>
    /// The T443
    /// </summary>
    t443 = 443,
    t444 = 444,
    t445 = 445,
    t446 = 446,
    t447 = 447,
    t448 = 448,
    t449 = 449,
    t450 = 450,
    t451 = 451,
    t452 = 452,
    t453 = 453,
    t454 = 454,
    t455 = 455,
    t456 = 456,
    t457 = 457,
    t458 = 458,
    t459 = 459,
    t460 = 460,
    /// <summary>
    /// Quellenverwaltung
    /// </summary>
    t246 = 246,
    /// <summary>
    /// Gest./Begr.:
    /// </summary>
    t258 = 258,
    /// <summary>
    /// Präfix
    /// </summary>
    t259 = 259,
    /// <summary>
    /// fiktiv.Heiratsdatum
    /// </summary>
    t260 = 260,
    /// <summary>
    /// Dimissoriale
    /// </summary>
    t261 = 261,
    /// <summary>
    /// Das Programm muss neu gestartet werde, um die Änderungen zu übernehmen
    /// </summary>
    t262 = 262,
    /// <summary>
    /// &Zeugen
    /// </summary>
    t263 = 263,
    /// <summary>
    /// Geburt
    /// </summary>
    t264 = 264,
    /// <summary>
    /// Sex
    /// </summary>
    t267 = 267,
    /// <summary>
    /// Fenstergröße
    /// </summary>
    t270 = 270,
    /// <summary>
    /// Bildschirmauflösung
    /// </summary>
    t271 = 271,
    /// <summary>
    /// suchen Nu&mmer
    /// </summary>
    t272 = 272,
    /// <summary>
    /// su&chen Name
    /// </summary>
    t273 = 273,
    /// <summary>
    /// Partnersuche
    /// </summary>
    t274 = 274,
    /// <summary>
    /// Registersuche
    /// </summary>
    t275 = 275,
    /// <summary>
    /// Recherche
    /// </summary>
    t276 = 276,
    /// <summary>
    /// &Familienblatt
    /// </summary>
    t277 = 277,
    /// <summary>
    /// gesperrt
    /// </summary>
    t279 = 279,
    t280 = 280,
    t281 = 281,
    t282 = 282,
    t283 = 283,
    t284 = 284,
    t285 = 285,
    t286 = 286,
    t288 = 288,
    t289 = 289,
    t290 = 290,
    t291 = 291,
    t292 = 292,
    t293 = 293,
    t294 = 294,
    t295 = 295,
    t296 = 296,
    t297 = 297,
    t298 = 298,
    t299 = 299,
    t300 = 300,
    t305 = 305,
    t306 = 306,
    t307 = 307,
    t308 = 308,
    t309 = 309,
    t310 = 310,
    t311 = 311,
    t312 = 312,
    t313 = 313,
    t314 = 314,
    t315 = 315,
    t316 = 316,
    t317 = 317,
    t318 = 318,
    t319 = 319,
    t320 = 320,
    t321 = 321,
    t322 = 322,
    t323 = 323,
    t324 = 324,
    t325 = 325,
    t326 = 326,
    t327 = 327,
    t328 = 328,
    t329 = 329,
    t330 = 330,
    t331 = 331,
    t332 = 332,
    t333 = 333,
    t334 = 334,
    t335 = 335,
    t337 = 337,
    t338 = 338,
    t339 = 339,
    t340 = 340,
    t342 = 342,
    t343 = 343,
    t344 = 344,
    t345 = 345,
    t346 = 346,
    t349 = 349,
    t350 = 350,
    t351 = 351,
    t352 = 352,
    t353 = 353,
    t354 = 354,
    t355 = 355,
    t356 = 356,
    t357 = 357,
    t358 = 358,
    t359 = 359,
    t360 = 360,
    t361 = 361,
    t362 = 362,
    t363 = 363,
    t364 = 364,
    t365 = 365,
    t366 = 366,
    t367 = 367,
    t368 = 368,
    t369 = 369,
    t370 = 370,
    t371 = 371,
    t372 = 372,
    t373 = 373,
    t374 = 374,
    t375 = 375,
    t376 = 376,
    t377 = 377,
    t378 = 378,
    t379 = 379,
    t380 = 380,
    t381 = 381,
    t382 = 382,
    t383 = 383,
    t384 = 384,
    t385 = 385,
    t386 = 386,
    t387 = 387,
    t388 = 388,
    t389 = 389,
    t390 = 390,
    t391 = 391,
    t392 = 392,
    t393 = 393,
    t394 = 394,
    t395 = 395,
    t396 = 396,
    t397 = 397,
    t398 = 398,
    t399 = 399,
    t400 = 400,
    t401 = 401,
    t402 = 402,
    t403 = 403,
    t404 = 404,
    t405 = 405,
    t406 = 406,
    t407 = 407,
    t408 = 408,
    t409 = 409,
    t410 = 410,
    t411 = 411,
    t412 = 412,
    t413 = 413,
    t414 = 414,
    t415 = 415,
    t416 = 416,
    t417 = 417,
    t418 = 418,
    t419 = 419,
    t420 = 420,
    t421 = 421,
    t422 = 422,
    t423 = 423,
    t424 = 424,
    t425 = 425,
    t426 = 426,
    t427 = 427,
    t428 = 428,
    t429 = 429,
    t430 = 430,
    t431 = 431,
    t432 = 432,
    t433 = 433,
    t434 = 434,
    
}
