using System;
using System.Collections.Generic;
using System.Threading;

class Program
{
    enum Op
    {
        RndSecret = 1, ReadNumber = 2, CmpEq = 3, CmpLt = 4, BrTrue = 5,
        PrintSentence = 6, PrintLabelAndNumber = 7, PrintRandomStatus = 8,
        Progress = 9, Chart = 10, Jump = 11, Halt = 99
    }

    class VM
    {
        public int Secret, Input, PC;
        public bool Result;
        public Random Rnd = new Random();
    }

    static void Main()
    {
        // Fetzen (obfuskiert): erstes Element = Offset, danach Codepoint+Offset
        var shards = new List<int[]>
        {
            new[]{300,365,410,397,408,415,401},                   // 0: "Analyse"
            new[]{250,336,347,364,355,347,360,372},               // 1: "Varianz"
            new[]{240,310,351,357,354,345,341,354},               // 2: "Fourier"
            new[]{200,277,311,310,316,301},                       // 3: "Monte"
            new[]{260,335,371,378,357,374,365,357,370,382},       // 4: "Kovarianz"
            new[]{150,196,196,196},                               // 5: "..."
            new[]{100,132},                                       // 6: " "
            new[]{180,238,212},                                   // 7: ": "
            new[]{200,297,298,303,301,315,299,304,308,311,315,315,301,310}, // 8: "abgeschlossen"
            new[]{220,307,321,334,336},                           // 9: "Wert"
            new[]{210,318,315,311,313,326},                       //10: "liegt"
            new[]{190,275,268,274,259,272},                       //11: "UNTER"
            new[]{300,520,366,369,382},                           //12: "ÜBER"
            new[]{160,260,261,269},                               //13: "dem"
            new[]{230,299,344,349,327,344,346,347,340,333,345,328,331,344,331,335,329,334}, //14: "Erwartungsbereich"
            new[]{210,289,322,326,315,319,307,318,311},           //15: "Optimale"
            new[]{250,326,496,365,367,360,353},                   //16: "Lösung"
            new[]{200,303,301,302,317,310,300,301,310},           //17: "gefunden"
            new[]{120,153},                                       //18: "!"
            new[]{240,308,357,354,339,344,355,339,344,350,345,356,356,355,359,341,354,356}, //19: "Durchschnittswert"
            new[]{190,265,301,304,304,291,298,287,306,295,301,300}, //20: "Korrelation"
            new[]{50,60},                                         //21: "\n"
            new[]{200,266,305,316,316,301},                       //22: "Bitte"
            new[]{150,240,247,254,258},                           //23: "Zahl"
            new[]{170,271,275,280,273,271,268,271,280},           //24: "eingeben"
            new[]{140,186},                                       //25: "."
        };

        // Sätze als Reihen von Shard-Indizes (alle enden mit "\n" via Shard 21)
        var sentences = new List<int[]>
        {
            new[]{0,5,21},                                        // 0: "Analyse...\n"
            new[]{1,5,21},                                        // 1: "Varianz...\n"
            new[]{2,5,21},                                        // 2: "Fourier...\n"
            new[]{3,5,21},                                        // 3: "Monte...\n"
            new[]{4,5,21},                                        // 4: "Kovarianz...\n"

            new[]{22,6,23,6,24,7,21},                             // 5: "Bitte Zahl eingeben: \n"

            new[]{0,6,8,7,9,6,10,6,11,6,13,6,14,25,21},           // 6: Unter-Meldung
            new[]{0,6,8,7,9,6,10,6,12,6,13,6,14,25,21},           // 7: Über-Meldung
            new[]{0,6,8,7,15,6,16,6,17,18,21},                    // 8: Erfolg-Meldung
        };

        // Labels für Zahlen/Stats (ohne Newline; dieser folgt nach Zahl)
        var labels = new List<int[]>
        {
            new[]{19,7},                                          // 0: "Durchschnittswert: "
            new[]{1,7},                                           // 1: "Varianz: "
            new[]{20,7},                                          // 2: "Korrelation: "
        };

        var statusIdx = new int[] { 0, 1, 2, 3, 4 };

        // Bytecode mit korrekten Offsets (Unter @28, Über @32, Erfolg @36)
        int[] program =
        {
            (int)Op.RndSecret, 20,            // 0

            // LOOP @2
            (int)Op.PrintSentence, 5,         // 2: Prompt
            (int)Op.ReadNumber, 0,            // 4: Eingabe

            (int)Op.PrintRandomStatus, 0,     // 6
            (int)Op.Progress, 20,             // 8
            (int)Op.PrintLabelAndNumber, 0,   // 10
            (int)Op.PrintLabelAndNumber, 1,   // 12
            (int)Op.PrintLabelAndNumber, 2,   // 14
            (int)Op.Chart, (6<<8)|10,         // 16

            (int)Op.CmpEq, 0,                 // 18
            (int)Op.BrTrue, 36,               // 20 -> Erfolg @36
            (int)Op.CmpLt, 0,                 // 22
            (int)Op.BrTrue, 28,               // 24 -> Unter @28
            (int)Op.Jump, 32,                 // 26 -> Über @32

            // Unter-Block @28
            (int)Op.PrintSentence, 6,         // 28
            (int)Op.Jump, 2,                  // 30 zurück zur LOOP

            // Über-Block @32
            (int)Op.PrintSentence, 7,         // 32
            (int)Op.Jump, 2,                  // 34 zurück zur LOOP

            // Erfolg @36
            (int)Op.PrintSentence, 8,         // 36
            (int)Op.Halt, 0                   // 38
        };

        Run(new VM(), program, shards, sentences, labels, statusIdx);
    }

    // Dekodiert und schreibt einen Satz (Liste von Shards)
    static void WriteSentence(int[] sentence, List<int[]> shards)
    {
        for (int i = 0; i < sentence.Length; i++)
        {
            var s = shards[sentence[i]];
            int off = s[0];
            for (int j = 1; j < s.Length; j++)
                Console.Write((char)(s[j] - off));
        }
    }

    static void Run(VM vm, int[] prog, List<int[]> shards, List<int[]> sentences, List<int[]> labels, int[] statusIdx)
    {
        while (vm.PC < prog.Length)
        {
            var op = (Op)prog[vm.PC++]; int arg = prog[vm.PC++];
            switch (op)
            {
                case Op.RndSecret:
                    vm.Secret = vm.Rnd.Next(1, arg + 1);
                    break;

                case Op.ReadNumber:
                    {
                        // Einlesen ohne String-Literale
                        string s = Console.ReadLine();
                        int x; vm.Input = int.TryParse(s, out x) ? x : int.MinValue;
                        break;
                    }

                case Op.CmpEq: vm.Result = (vm.Input == vm.Secret); break;
                case Op.CmpLt: vm.Result = (vm.Input < vm.Secret); break;
                case Op.BrTrue: if (vm.Result) vm.PC = arg; break;
                case Op.Jump: vm.PC = arg; break;

                case Op.PrintSentence:
                    WriteSentence(sentences[arg], shards);
                    break;

                case Op.PrintLabelAndNumber:
                    {
                        // Label ausgeben
                        var lbl = labels[arg];
                        for (int i = 0; i < lbl.Length; i++)
                        {
                            var s = shards[lbl[i]];
                            int off = s[0];
                            for (int j = 1; j < s.Length; j++)
                                Console.Write((char)(s[j] - off));
                        }

                        // Zahl/Wert ohne Format-String
                        if (arg == 0)
                        {
                            int v = vm.Rnd.Next(10, 100);
                            Console.Write(v);
                        }
                        else if (arg == 1)
                        {
                            double v = Math.Round(vm.Rnd.NextDouble(), 3);
                            Console.Write(v);
                        }
                        else
                        {
                            double v = Math.Round(vm.Rnd.NextDouble(), 2);
                            Console.Write(v);
                        }
                        Console.Write((char)10); // Newline
                        break;
                    }

                case Op.PrintRandomStatus:
                    {
                        int w = statusIdx[vm.Rnd.Next(statusIdx.Length)];
                        WriteSentence(sentences[w], shards);
                        break;
                    }

                case Op.Progress:
                    RenderProgress(arg, 24);
                    break;

                case Op.Chart:
                    {
                        int min = (arg >> 8) & 0xFF;
                        int max = arg & 0xFF;
                        RenderChart(vm.Rnd, min, max);
                        break;
                    }

                case Op.Halt:
                    return;
            }
        }
    }

    // Fortschrittsbalken ohne String-Literale
    static void RenderProgress(int width, int delayMs)
    {
        for (int i = 0; i <= width; i++)
        {
            Console.Write((char)91); // '['
            for (int k = 0; k < i; k++) Console.Write((char)35);      // '#'
            for (int k = 0; k < (width - i); k++) Console.Write((char)32); // ' '
            Console.Write((char)93); // ']'
            Console.Write((char)32); // ' '
            int pct = (i * 100 / width);
            Console.Write(pct);
            Console.Write((char)37); // '%'
            Console.Write((char)13); // '\r'
            Thread.Sleep(delayMs);
        }
        Console.Write((char)10); // '\n'
    }

    // ASCII-Chart ohne String-Literale
    static void RenderChart(Random rnd, int minBars, int maxBars)
    {
        int bars = rnd.Next(minBars, maxBars + 1);
        for (int i = 0; i < bars; i++)
        {
            int h = rnd.Next(5, 20);
            for (int k = 0; k < h; k++) Console.Write((char)124); // '|'
            Console.Write((char)10);
        }
    }
}
