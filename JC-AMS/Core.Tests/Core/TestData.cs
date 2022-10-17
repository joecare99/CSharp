using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace JCAMS.Core.Tests
{
    internal static class TestData
    {
        internal const string cLoremIpsum =
@"Lorem ipsum dolor sit amet, consectetur adipisici elit, sed eiusmod tempor 
incidunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis 
nostrud exercitation ullamco laboris nisi ut aliquid ex ea commodi consequat. 
Quis aute iure reprehenderit in voluptate velit esse cillum dolore eu fugiat 
nulla pariatur. Excepteur sint obcaecat cupiditat non proident, sunt in culpa 
qui officia deserunt mollit anim id est laborum.";

        internal const string cBase64LoremIpsum = "TG9yZW0gaXBzdW0gZG9sb3Igc2l0IGFtZXQsIGNvbnNlY3R" +
            "ldHVyIGFkaXBpc2ljaSBlbGl0LCBzZWQgZWl1c21vZCB0ZW1wb3IgDQppbmNpZHVudCB1dCBsYWJvcmUg" +
            "ZXQgZG9sb3JlIG1hZ25hIGFsaXF1YS4gVXQgZW5pbSBhZCBtaW5pbSB2ZW5pYW0sIHF1aXMgDQpub3N0c" +
            "nVkIGV4ZXJjaXRhdGlvbiB1bGxhbWNvIGxhYm9yaXMgbmlzaSB1dCBhbGlxdWlkIGV4IGVhIGNvbW1vZG" +
            "kgY29uc2VxdWF0LiANClF1aXMgYXV0ZSBpdXJlIHJlcHJlaGVuZGVyaXQgaW4gdm9sdXB0YXRlIHZlbGl" +
            "0IGVzc2UgY2lsbHVtIGRvbG9yZSBldSBmdWdpYXQgDQpudWxsYSBwYXJpYXR1ci4gRXhjZXB0ZXVyIHNp" +
            "bnQgb2JjYWVjYXQgY3VwaWRpdGF0IG5vbiBwcm9pZGVudCwgc3VudCBpbiBjdWxwYSANCnF1aSBvZmZpY" +
            "2lhIGRlc2VydW50IG1vbGxpdCBhbmltIGlkIGVzdCBsYWJvcnVtLg==";

        internal const string cBase64Data2 =
            "tkSKmV0ot0HdTxKIAyBPxa0F3FNR7bRVvwY4iqDOt9nOXOKLM5GyIs5A1TlLh52MZ+OV4PczZt69" +
            "mjoACanw1GCfEPkvWvOvlXTB1UmXcxV42YyHxujuk45391ufLqgJrGeJCzw+M5JyYq4WXK4qcWp9PyGhw" +
            "zr9waHUi3ZNQVj2otsnnDoJkGKxw1yUE1z1m6p/KiWUGg73Un4iarI3cB/KWi4CpIsK+/TaUejzSHIV4l" +
            "7xDxrL/Mt/5/FYkzy+DMDjQEstjqooCkULknQzE/5vuan2VLTRRzXEsObecXdsXhkFZV1qaoQPtEgcVj6" +
            "YrL0Fc7jvyF7eY81BvC8WEuyAP/nUqKuHeE1ly0da0pQSM4BDjMdZIj8r8RxtlPZHWfcwMyzYCifMMFNT" +
            "/BlLi0srvPAMMStVK37MqKDmvNmQurKmQVzaJinu7OOOk2ayePy+0uVzfoGdxgGA8ISlPJHYm3dxoMxRv" +
            "3Ldhlthip7sDJWaYplBH5LsOsCOXCPR6+DV4ncc/uYgEyRpuJ8tX1CW/+e28xycp0UVPF7M0XqGxDcytp" +
            "ki9ZUOLGb2J/Ypi9Ly+ZX+LYs=";
internal static IEnumerable<object[]> AsATML1Data => new[]
       {
            new object[]{"Black",Color.Black,"#000000" },
            new object[]{ "White", Color.White,"#FFFFFF" },
            new object[]{ "DarkBlue", Color.DarkBlue,"#00008B" },
            new object[]{ "DarkCyan", Color.DarkCyan,"#008B8B" },
            new object[]{ "DarkGreen", Color.DarkGreen,"#006400" },
            new object[]{ "DarkOrange", Color.DarkOrange,"#FF8C00" },
            new object[]{ "DarkRed", Color.DarkRed,"#8B0000" },
            new object[]{ "DarkKhaki", Color.DarkKhaki,"#BDB76B" },
            new object[]{ "DarkGray", Color.DarkGray,"#A9A9A9" },
            new object[]{ "Blue", Color.Blue,"#0000FF" },
            new object[]{ "Cyan", Color.Cyan,"#00FFFF" },
            new object[]{ "Green", Color.Green,"#008000" },
            new object[]{ "GreenYellow", Color.GreenYellow,"#ADFF2F" },
            new object[]{ "Yellow", Color.Yellow,"#FFFF00" },
            new object[]{ "Orange", Color.Orange,"#FFA500" },
            new object[]{ "OrangeRed", Color.OrangeRed,"#FF4500" },
            new object[]{ "Red", Color.Red,"#FF0000" },
            new object[]{ "Purple", Color.Purple,"#800080" },
        };

        internal static IEnumerable<double> SortedArrayDouble1000 => new[] 
        { 
            000d, 001d, 002d, 003d, 004d, 005d, 006d, 007d, 008d, 009d,     
            010d, 011d, 012d, 013d, 014d, 015d, 016d, 017d, 018d, 019d, 
            020d, 021d, 022d, 023d, 024d, 025d, 026d, 027d, 028d, 029d, 
            030d, 031d, 032d, 033d, 034d, 035d, 036d, 037d, 038d, 039d, 
            040d, 041d, 042d, 043d, 044d, 045d, 046d, 047d, 048d, 049d, 
            050d, 051d, 052d, 053d, 054d, 055d, 056d, 057d, 058d, 059d,
            060d, 061d, 062d, 063d, 064d, 065d, 066d, 067d, 068d, 069d,
            070d, 071d, 072d, 073d, 074d, 075d, 076d, 077d, 078d, 079d,
            080d, 081d, 082d, 083d, 084d, 085d, 086d, 087d, 088d, 089d,
            090d, 091d, 092d, 093d, 094d, 095d, 096d, 097d, 098d, 099d,
            100d, 101d, 102d, 103d, 104d, 105d, 106d, 107d, 108d, 109d,
            110d, 111d, 112d, 113d, 114d, 115d, 116d, 117d, 118d, 119d,
            120d, 121d, 122d, 123d, 124d, 125d, 126d, 127d, 128d, 129d,
            130d, 131d, 132d, 133d, 134d, 135d, 136d, 137d, 138d, 139d,
            140d, 141d, 142d, 143d, 144d, 145d, 146d, 147d, 148d, 149d,
            150d, 151d, 152d, 153d, 154d, 155d, 156d, 157d, 158d, 159d,
            160d, 161d, 162d, 163d, 164d, 165d, 166d, 167d, 168d, 169d,
            170d, 171d, 172d, 173d, 174d, 175d, 176d, 177d, 178d, 179d,
            180d, 181d, 182d, 183d, 184d, 185d, 186d, 187d, 188d, 189d,
            190d, 191d, 192d, 193d, 194d, 195d, 196d, 197d, 198d, 199d,
            200d, 201d, 202d, 203d, 204d, 205d, 206d, 207d, 208d, 209d,
            210d, 211d, 212d, 213d, 214d, 215d, 216d, 217d, 218d, 219d,
            220d, 221d, 222d, 223d, 224d, 225d, 226d, 227d, 228d, 229d,
            230d, 231d, 232d, 233d, 234d, 235d, 236d, 237d, 238d, 239d,
            240d, 241d, 242d, 243d, 244d, 245d, 246d, 247d, 248d, 249d,
            250d, 251d, 252d, 253d, 254d, 255d, 256d, 257d, 258d, 259d,
            260d, 261d, 262d, 263d, 264d, 265d, 266d, 267d, 268d, 269d,
            270d, 271d, 272d, 273d, 274d, 275d, 276d, 277d, 278d, 279d,
            280d, 281d, 282d, 283d, 284d, 285d, 286d, 287d, 288d, 289d,
            290d, 291d, 292d, 293d, 294d, 295d, 296d, 297d, 298d, 299d,
            300d, 301d, 302d, 303d, 304d, 305d, 306d, 307d, 308d, 309d,
            310d, 311d, 312d, 313d, 314d, 315d, 316d, 317d, 318d, 319d,
            320d, 321d, 322d, 323d, 324d, 325d, 326d, 327d, 328d, 329d,
            330d, 331d, 332d, 333d, 334d, 335d, 336d, 337d, 338d, 339d,
            340d, 341d, 342d, 343d, 344d, 345d, 346d, 347d, 348d, 349d,
            350d, 351d, 352d, 353d, 354d, 355d, 356d, 357d, 358d, 359d,
            360d, 361d, 362d, 363d, 364d, 365d, 366d, 367d, 368d, 369d,
            370d, 371d, 372d, 373d, 374d, 375d, 376d, 377d, 378d, 379d,
            380d, 381d, 382d, 383d, 384d, 385d, 386d, 387d, 388d, 389d,
            390d, 391d, 392d, 393d, 394d, 395d, 396d, 397d, 398d, 399d,
            400d, 401d, 402d, 403d, 404d, 405d, 406d, 407d, 408d, 409d,
            410d, 411d, 412d, 413d, 414d, 415d, 416d, 417d, 418d, 419d,
            420d, 421d, 422d, 423d, 424d, 425d, 426d, 427d, 428d, 429d,
            430d, 431d, 432d, 433d, 434d, 435d, 436d, 437d, 438d, 439d,
            440d, 441d, 442d, 443d, 444d, 445d, 446d, 447d, 448d, 449d,
            450d, 451d, 452d, 453d, 454d, 455d, 456d, 457d, 458d, 459d,
            460d, 461d, 462d, 463d, 464d, 465d, 466d, 467d, 468d, 469d,
            470d, 471d, 472d, 473d, 474d, 475d, 476d, 477d, 478d, 479d,
            480d, 481d, 482d, 483d, 484d, 485d, 486d, 487d, 488d, 489d,
            490d, 491d, 492d, 493d, 494d, 495d, 496d, 497d, 498d, 499d,
            500d, 501d, 502d, 503d, 504d, 505d, 506d, 507d, 508d, 509d,
            510d, 511d, 512d, 513d, 514d, 515d, 516d, 517d, 518d, 519d,
            520d, 521d, 522d, 523d, 524d, 525d, 526d, 527d, 528d, 529d,
            530d, 531d, 532d, 533d, 534d, 535d, 536d, 537d, 538d, 539d,
            540d, 541d, 542d, 543d, 544d, 545d, 546d, 547d, 548d, 549d,
            550d, 551d, 552d, 553d, 554d, 555d, 556d, 557d, 558d, 559d,
            560d, 561d, 562d, 563d, 564d, 565d, 566d, 567d, 568d, 569d,
            570d, 571d, 572d, 573d, 574d, 575d, 576d, 577d, 578d, 579d,
            580d, 581d, 582d, 583d, 584d, 585d, 586d, 587d, 588d, 589d,
            590d, 591d, 592d, 593d, 594d, 595d, 596d, 597d, 598d, 599d,
            600d, 601d, 602d, 603d, 604d, 605d, 606d, 607d, 608d, 609d,
            610d, 611d, 612d, 613d, 614d, 615d, 616d, 617d, 618d, 619d,
            620d, 621d, 622d, 623d, 624d, 625d, 626d, 627d, 628d, 629d,
            630d, 631d, 632d, 633d, 634d, 635d, 636d, 637d, 638d, 639d,
            640d, 641d, 642d, 643d, 644d, 645d, 646d, 647d, 648d, 649d,
            650d, 651d, 652d, 653d, 654d, 655d, 656d, 657d, 658d, 659d,
            660d, 661d, 662d, 663d, 664d, 665d, 666d, 667d, 668d, 669d,
            670d, 671d, 672d, 673d, 674d, 675d, 676d, 677d, 678d, 679d,
            680d, 681d, 682d, 683d, 684d, 685d, 686d, 687d, 688d, 689d,
            690d, 691d, 692d, 693d, 694d, 695d, 696d, 697d, 698d, 699d,
            700d, 701d, 702d, 703d, 704d, 705d, 706d, 707d, 708d, 709d,
            710d, 711d, 712d, 713d, 714d, 715d, 716d, 717d, 718d, 719d,
            720d, 721d, 722d, 723d, 724d, 725d, 726d, 727d, 728d, 729d,
            730d, 731d, 732d, 733d, 734d, 735d, 736d, 737d, 738d, 739d,
            740d, 741d, 742d, 743d, 744d, 745d, 746d, 747d, 748d, 749d,
            750d, 751d, 752d, 753d, 754d, 755d, 756d, 757d, 758d, 759d,
            760d, 761d, 762d, 763d, 764d, 765d, 766d, 767d, 768d, 769d,
            770d, 771d, 772d, 773d, 774d, 775d, 776d, 777d, 778d, 779d,
            780d, 781d, 782d, 783d, 784d, 785d, 786d, 787d, 788d, 789d,
            790d, 791d, 792d, 793d, 794d, 795d, 796d, 797d, 798d, 799d,
            800d, 801d, 802d, 803d, 804d, 805d, 806d, 807d, 808d, 809d,
            810d, 811d, 812d, 813d, 814d, 815d, 816d, 817d, 818d, 819d,
            820d, 821d, 822d, 823d, 824d, 825d, 826d, 827d, 828d, 829d,
            830d, 831d, 832d, 833d, 834d, 835d, 836d, 837d, 838d, 839d,
            840d, 841d, 842d, 843d, 844d, 845d, 846d, 847d, 848d, 849d,
            850d, 851d, 852d, 853d, 854d, 855d, 856d, 857d, 858d, 859d,
            860d, 861d, 862d, 863d, 864d, 865d, 866d, 867d, 868d, 869d,
            870d, 871d, 872d, 873d, 874d, 875d, 876d, 877d, 878d, 879d,
            880d, 881d, 882d, 883d, 884d, 885d, 886d, 887d, 888d, 889d,
            890d, 891d, 892d, 893d, 894d, 895d, 896d, 897d, 898d, 899d,
            900d, 901d, 902d, 903d, 904d, 905d, 906d, 907d, 908d, 909d,
            910d, 911d, 912d, 913d, 914d, 915d, 916d, 917d, 918d, 919d,
            920d, 921d, 922d, 923d, 924d, 925d, 926d, 927d, 928d, 929d,
            930d, 931d, 932d, 933d, 934d, 935d, 936d, 937d, 938d, 939d,
            940d, 941d, 942d, 943d, 944d, 945d, 946d, 947d, 948d, 949d,
            950d, 951d, 952d, 953d, 954d, 955d, 956d, 957d, 958d, 959d,
            960d, 961d, 962d, 963d, 964d, 965d, 966d, 967d, 968d, 969d,
            970d, 971d, 972d, 973d, 974d, 975d, 976d, 977d, 978d, 979d,
            980d, 981d, 982d, 983d, 984d, 985d, 986d, 987d, 988d, 989d,
            990d, 991d, 992d, 993d, 994d, 995d, 996d, 997d, 998d, 999d, 
        };  
    }

}