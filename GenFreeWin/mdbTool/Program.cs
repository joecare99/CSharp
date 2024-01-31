// See https://aka.ms/new-console-template for more information
using Gen_FreeWin;
using GenFree.Data;
using System;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        DataModul.DAODBEngine_definst = new GenFree.DB.DBImplementOleDB.DBEngine();
        DataModul.DataOpen("C:\\Gen_FreeWin\\Binau\\Gen_Plusdaten.mdb");
        DataModul.OpenNBDatabase("C:\\Gen_FreeWin\\Init\\GedAus.mdb");
        DataModul.Db_Def(DataModul.MandDB, Console.WriteLine);
        //DataModul.DB_Dump(DataModul.NB_Ahn1Table, Console.WriteLine)
        Console.ReadLine();
        DataModul.DataClose();
    }
}