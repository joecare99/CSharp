// See https://aka.ms/new-console-template for more information
using GenFree.Data;
using System;

internal class Program
{
    private static void Main(string[] args)
    {
        DataModul.DAODBEngine_definst = new GenFree.Data.DB.DBImplementOleDB.DBEngine();
        
        DataModul.DataOpen("C:\\Gen_FreeWin\\Muster_UTF8\\Gen_Plusdaten.mdb");
        DataModul.OpenNBDatabase("C:\\Gen_FreeWin\\Init\\GedAus.mdb");
        DataModul.Db_Def(DataModul.MandDB, Console.WriteLine);
        //DataModul.DB_Dump(DataModul.NB_Ahn1Table, Console.WriteLine)
        Console.ReadLine();
        var iCnt = 0;
        foreach (var cPerson in DataModul.Person.ReadAll())
        {
            if (DataModul.Names.ReadPersonNames(cPerson.ID, out var asNames, out var atVorns))
                cPerson.SetPersonNames(asNames, atVorns,false);
            cPerson.SetPersonEvents(DataModul.Event.ReadEntityEvents(cPerson.ID, false));

            Console.WriteLine($"ID: {cPerson.ID,4}  {cPerson.SurName}, {cPerson.Givennames}  *{cPerson.dBirth:dd.MM.yyyy} +{cPerson.dDeath:dd.MM.yyyy}");
            foreach (var evt in cPerson.Events)
                Console.WriteLine($"\t{evt.eArt}:"+DataModul.Event_GetLabelText2(evt, Event_PreDisplay));

            if (iCnt++ > 40) break;
        }
        Console.ReadLine();
        DataModul.DataClose();
    }

    public static string Event_PreDisplay(bool xCitation = false, bool xWitness = false, bool xAnnotation = false, bool xBC = false, bool xReg = false)
    {
        string text = xCitation ? "§ " : ". ";
               text += xWitness ? "Z " : ". ";
            text += xAnnotation ? "B " : ". ";
                    text += xBC ? "< " : ". ";
                   text += xReg ? "U " : ". ";
        return text;
    }

}