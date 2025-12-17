using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranspilerLib.DriveBASIC.Data;

/// <author>C. Rosewich</author>
/// <since>17.05.2009</since>
/// <info>Token steht fuer den Befehl der ausgefuehrt werden soll.</info>
public enum EDriveToken
{
    /// <author>C. Rosewich</author>
    /// <info>Befehl mit diesem Token tut Nichts</info>
    tt_Nop = 0,

    /// <author>C.Rosewich</author>
    /// <since>17.05.2009</since>
    /// <info>Zuweisung: Variable := Ausdruck </info>
    tte_let = 1,
    // Zuweisung, variable,Koordinate := variable,Koordinate,Konstante (+-*/ mod and or Xor) variable,Koordinate,Konstante

    /// <mod__constraintReferencedElement>design:node:::6lmgksd6s0f_n</mod__constraintReferencedElement>
    /// <author>C. Rosewich</author>
    /// <since>17.05.2009</since>
    /// <info>Befehl mit diesem Token setzt Abarbeitung an anderer Stelle fort</info>
    tt_goto = 2, // Goto, Gosub

    /// <author>C. Rosewich</author>
    /// <since>17.05.2009</since>
    /// <info>Befehl mit diesem Token setzt Abarbeitung an anderer Stelle fort</info>
    tte_goto2 = 3, // Goto, Gosub

    /// <author>C.Rosewich</author>
    /// <since>17.05.2009</since>
    /// <info>Negativer Zweig einer IF-Abfrage</info>
    tt_else = 4, // .. Else

    /// <author>C.Rosewich</author>
    /// <since>17.05.2009</since>
    /// <info>Bedingte Ausfuehrung der naechsten Befehle bis Else oder EndIf</info>
    tte_if = 5, // if ..then

    /// <author>C. Rosewich</author>
    /// <since>23.05.2009</since>
    /// <info>Schleife mit Schleifenzaehler, (festgelegte Durchgaenge) </info>
    tt_for = 6,

    /// <author>C.Rosewich</author>
    /// <since>17.05.2009</since>
    /// <info>Schleife solange Ausdruck erfuellt</info>
    tte_while = 7, // while ...

    /// <author>C.Rosewich</author>
    /// <since>17.05.2009</since>
    /// <info>Beendet Schleifen/If-Konstrukte oder Ablauf und kehrt evtl. zur naechsten Ebene zurueck</info>
    tt_end = 8, // end (if, While, for ...)

    /// <author>C.Rosewich</author>
    /// <since>17.05.2009</since>
    /// <info>Warte auf Zeit/Ereigniss</info>
    tte_wait = 9, // pause,

    /// <author>C.Rosewich</author>
    /// <since>17.05.2009</since>
    /// <info>Setzt eine Nachricht: Kommentar,Status,Meldung oder Fehler</info>
    tt_Msg = 10, // comment,state,meldung,error nummer

    /// <author>C.Rosewich</author>
    /// <since>17.05.2009</since>
    /// <info>Setzt eine Nachricht: Kommentar,Status,Meldung oder Fehler</info>
    tte_Msg2 = 11, // comment,state,meldung,error nummer

    /// <author>C.Rosewich</author>
    /// <since>17.05.2009</since>
    /// <info>Aufruf einer externer Funktionen evtl. mit einem Parameter</info>
    tt_Funct = 12, // fuehre funktion aus und warte auf deren beendigung

    /// <author>C.Rosewich</author>
    /// <since>17.05.2009</since>
    /// <info>Aufruf einer externer Funktionen evtl. mit einem Parameter</info>
    tte_funct2 = 13, // fuehre funktion aus und warte auf deren beendigung

    /// <author>C.Rosewich</author>
    /// <since>17.05.2009</since>
    /// <info>Fuehrt sobald/solange Ausruck erfuellt ist den naechsten Befehl aus.</info>
    tt_Sync = 14,

    /// <author>C.Rosewich</author>
    /// <since>17.05.2009</since>
    /// <info>Fuehrt sobald/solange Ausruck erfuellt ist den naechsten Befehl aus.</info>
    tte_sync2 = 15,

    /// <author>C.Rosewich</author>
    /// <since>17.05.2009</since>
    /// <Info>Fahrbefehl auf Punkt/Koordinate, Weiterschaltung bei Genauhaltfenster</Info>
    tte_drive = 17,

    /// <author>C. Rosewich</author>
    /// <since>20.05.2009</since>
    /// <Info>Fahr-Befehl auf Punkt/Koordinate, WeiterSchaltung bei Grossem Fenster</Info>
    tte_drive_via = 19,
    // fahre aus aktueller koordinate auf Punkt , Achse auf koordinate ... ptp, interpolierend , koordiniert

    /// <author>C.Rosewich</author>
    /// <since>17.05.2009</since>
    /// <info>Fahre auf Punkt/Koordinate, Weiterschaltung, sofort</info>
    tte_drive_async = 21
}
