using System;
using System.Globalization;
using System.Threading;

class MicroVM
{
    // Registers / storage
    int secret, input, counter, operand2;
    int pc;
    bool result;
    object lastObj;

    // Runtime
    Random rnd = new Random();
    bool Debug = true;       // Toggle debug
    int MaxSteps = 10000;    // Safety limit

    // Sprachagnostische Textliste
    string[] templates = {
        "Bitte Zahl eingeben: ",
        "Wert {0} liegt über dem Erwartungsbereich\n",
        "Wert {0} liegt unter dem Erwartungsbereich\n",
        "Treffer!\n",
        "Analyse...\n",
        "Durchschnittswert: {0}\n",
        "Varianz: {0:F3}\n",
        "Korrelation: {0:F2}\n",
        "[{0}]",
        " {0}%\n",
        "{0}\n"
    };

    void TraceHeader(int instr, bool fParam, bool fSet, bool fJump, bool fOut, int arg, byte op, int step, int pcStart)
    {
        if (!Debug) return;
        System.Diagnostics.Debug.WriteLine($"[STEP {step}] pc={pcStart} op=0x{op:X2} instr={instr} flags(P={b(fParam)} S={b(fSet)} J={b(fJump)} O={b(fOut)}) arg={arg}");
        System.Diagnostics.Debug.WriteLine($"         state: input={input} secret={secret} counter={counter} operand2={operand2} result={result} lastObj=({FmtObj(lastObj)})");
    }

    void TraceFooter(int pcAfter)
    {
        if (!Debug) return;
        System.Diagnostics.Debug.WriteLine($"         -> pc={pcAfter} result={result} lastObj=({FmtObj(lastObj)})\n");
    }

    string FmtObj(object o)
    {
        if (o == null) return "null";
        return $"{o} : {o.GetType().Name}";
    }

    string b(bool v) => v ? "1" : "0";

    int SafeToInt(object o, string context)
    {
        try
        {
            if (o is int i) return i;
            if (o is double d) return (int)d;
            if (o is string s && int.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out int si)) return si;
            throw new InvalidCastException($"Cannot convert {FmtObj(o)} to int in {context}");
        }
        catch (Exception ex)
        {
            if (Debug) Console.WriteLine($"[ERROR] {ex.Message}");
            throw;
        }
    }

    enum OpCode
    {
        Nop = 0x00,
        Sleep = 0x01,

        Read = 0x10,
        ReadP = 0x11,
        Write = 0x18,
        WriteP = 0x19,

        Move0 = 0x20,
        Load = 0x21,
        Store = 0x22,
        StoreP = 0x23,
        Func = 0x28,
        FuncP = 0x29,
        Imm = 0x2B,

        CmpLess = 0x30,
        CmpEq = 0x32,

        End = 0x40, // Instr=4 ohne Param → Programmende
        JumpP = 0x41,
        JumpCond = 0x45,
        JumpCount = 0x47,

        Rand = 0x51
    }

    void Run(int[] prog)
    {
        int step = 0;
        while (pc < prog.Length)
        {
            if (++step > MaxSteps)
            {
                Console.WriteLine("[ABORT] Step limit exceeded.");
                return;
            }

            int pcStart = pc;
            byte op = (byte)prog[pc++];
            int instr = (op >> 4) & 0xF;
            bool fParam = (op & 1) != 0;
            bool fSet = (op & 2) != 0;
            bool fJump = (op & 4) != 0;
            bool fOut = (op & 8) != 0;

            int arg = fParam ? prog[pc++] : 0;
            TraceHeader(instr, fParam, fSet, fJump, fOut, arg, op, step, pcStart);

            try
            {
                if (instr == 0) // NOP / Sleep
                {
                    if (fParam && arg > 0) Thread.Sleep(arg);
                }
                else if (instr == 1) // IO
                {
                    if (fOut)
                    {
                        if (arg < 0 || arg >= templates.Length)
                            throw new IndexOutOfRangeException($"Template index {arg} out of range.");
                        string tmpl = templates[arg];
                        Console.Write(string.Format(CultureInfo.InvariantCulture, tmpl, lastObj));
                        System.Diagnostics.Debug.Write(string.Format(CultureInfo.InvariantCulture, tmpl, lastObj));
                    }
                    else
                    {
                        string line = Console.ReadLine();
                        if (int.TryParse(line, NumberStyles.Any, CultureInfo.InvariantCulture, out int val))
                            lastObj = input = val;
                        else
                            throw new FormatException($"Input '{line}' is not a valid integer.");
                    }
                }
                else if (instr == 2) // Move
                {
                    if (!fParam)
                    {
                        // 1. Ohne Argument
                        lastObj = 0d;
                    }
                    else if (fOut && fSet)
                    {
                        // 5. Immediate Load: lastObj = arg
                        lastObj = arg;
                    }
                    else if (fOut)
                    {
                        // 4. Function Call
                        switch (arg)
                        {
                            case 0: lastObj = rnd.Next(1, 101); break;
                            case 1: lastObj = rnd.NextDouble(); break;
                            case 2: lastObj = new string('#', rnd.Next(1, 20)); break;
                            case 3: lastObj = new string('|', rnd.Next(5, 20)); break;
                            default: throw new InvalidOperationException($"Unknown function id {arg}.");
                        }
                    }
                    else if (fSet)
                    {
                        // 2. Store lastObj -> storage[arg]
                        int v = SafeToInt(lastObj, "Store lastObj");
                        switch (arg)
                        {
                            case 0: input = v; break;
                            case 1: secret = v; break;
                            case 2: counter = v; break;
                            case 3: operand2 = v; break;
                            default: throw new InvalidOperationException($"Unknown storage id {arg} for Store.");
                        }
                    }
                    else
                    {
                        // 3. Load storage[arg] -> lastObj
                        switch (arg)
                        {
                            case 0: lastObj = input; break;
                            case 1: lastObj = secret; break;
                            case 2: lastObj = counter; break;
                            case 3: lastObj = operand2; break;
                            default: throw new InvalidOperationException($"Unknown storage id {arg} for Load.");
                        }
                    }
                }
                else if (instr == 3) // Compare
                {
                    // fSet=1 -> Equal; fSet=0 -> Less
                    int lv = SafeToInt(lastObj, "Compare lastObj");
                    if (fSet) result = (lv == secret);
                    else result = (lv < secret);
                }
                else if (instr == 4) // Jump
                {
                    // Jump ohne Ziel = Halt
                    if (!fParam) { TraceFooter(pc); return; }

                    if (fSet)
                    {
                        // Zählersprung: dekrementiere lastObj, schreibe zurück in counter, springe solange >0
                        int n = SafeToInt(lastObj, "Jump counter");
                        n--;
                        lastObj = n;
                        counter = n; // konsistent zurückschreiben
                        if (n > 0) pc = arg;
                    }
                    else if (fJump)
                    {
                        if (result) pc = arg;
                    }
                    else
                    {
                        pc = arg;
                    }
                }
                else if (instr == 5) // Rand/Secret über lastObj
                {
                    // Setze secret anhand arg (max inkl.), schreibe auch lastObj
                    if (arg <= 0) throw new ArgumentException("Secret upper bound must be > 0.");
                    secret = rnd.Next(1, arg + 1);
                    lastObj = secret;
                }
                else if (instr == 15) // Halt
                {
                    TraceFooter(pc);
                    return;
                }
                else
                {
                    throw new InvalidOperationException($"Unknown instruction {instr}.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EXCEPTION] {ex.GetType().Name}: {ex.Message}");
                Console.WriteLine($"           at pc={pcStart}, instr={instr}, arg={arg}");
                return;
            }

            TraceFooter(pc);
        }
    }

    static void Main()
    {
        var vm = new MicroVM();

        // Demo: Immediate-Load + Schleife (5x Analyse...)
        int[] prog = {
    // Zufallszahl 1..20 in secret
    (int)OpCode.Rand, 20,

    // Prompt + Eingabe
    (int)OpCode.WriteP, 0,   // "Bitte Zahl eingeben: "
    (int)OpCode.Read,        // Eingabe → lastObj=input

    // Analyse-Sequenz
    (int)OpCode.Sleep, 500,
    (int)OpCode.WriteP, 4,   // "Analyse..."

    (int)OpCode.Sleep, 300,
    (int)OpCode.FuncP, 0,    // Zufallswert
    (int)OpCode.WriteP, 5,   // "Durchschnittswert: {0}"

    (int)OpCode.Sleep, 300,
    (int)OpCode.FuncP, 1,    // Double
    (int)OpCode.WriteP, 6,   // "Varianz: {0:F3}"

    (int)OpCode.Sleep, 300,
    (int)OpCode.FuncP, 1,    // Double
    (int)OpCode.WriteP, 7,   // "Korrelation: {0:F2}"

    (int)OpCode.Sleep, 300,
    (int)OpCode.FuncP, 2,    // Balken
    (int)OpCode.WriteP, 8,   // "[{0}]"
    (int)OpCode.FuncP, 0,    // Prozent
    (int)OpCode.WriteP, 9,   // " {0}%"

    (int)OpCode.Sleep, 300,
    (int)OpCode.FuncP, 3,    // Chartzeile
    (int)OpCode.WriteP, 10,  // "{0}\n"

    // --- Fix: Eingabe laden vor Compare ---
    (int)OpCode.Load, 0,     // input -> lastObj
    (int)OpCode.CmpEq,
    (int)OpCode.JumpCond, 59, // wenn gleich → Treffer

    (int)OpCode.CmpLess,
    (int)OpCode.JumpCond, 55, // wenn kleiner → Unter

    // Über-Block
    (int)OpCode.WriteP, 1,   // "Wert {0} liegt über..."
    (int)OpCode.JumpP, 2,    // zurück zur Eingabe

    // Unter-Block (Adresse 54)
    (int)OpCode.WriteP, 2,   // "Wert {0} liegt unter..."
    (int)OpCode.JumpP, 2,    // zurück zur Eingabe

    // Treffer-Block (Adresse 62)
    (int)OpCode.WriteP, 3,   // "Treffer!"
    (int)OpCode.End          // Programmende
};

        vm.Run(prog);
    }
}
