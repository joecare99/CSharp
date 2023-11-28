// See https://aka.ms/new-console-template for more information
using Gen_FreeWin;
using System;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        DataModul.DataOpen("C:\\Gen_FreeWin\\Binau\\Gen_Plusdaten.mdb");
        DataModul.OpenNBDatabase("C:\\Gen_FreeWin\\Init\\GedAus.mdb");
        DataModul.Db_Def(DataModul.NB, Console.WriteLine);
        //DataModul.DB_Dump(DataModul.NB_Ahn1Table, Console.WriteLine)
        Console.ReadLine();
        DataModul.DataClose();
    }
}