#define ShowGen
#define _SingleTask
using System.Threading;
using ConsoleDisplay.View;

MyConsoleBase console = new MyConsole();
const int T2k = 1<<11, //2048 (2^11)
          CellPerDimension = (0x13 << 1) + 1, // 39 
          One = CellPerDimension & -CellPerDimension, // 1
          Eight = One << ((One << One) + 1), // 8
          TextCode = 0x597b; // 2-Bit Coded Permutatin Index
var TextOut = " _|";
#if ShowGen
var Scc = "   ,---, ' |-'-|";
#endif
int[] DirStopArray = new int[] { 0, One - 4, CellPerDimension - One, -One },
	Dir = new int[] { One, CellPerDimension, -One, -CellPerDimension },

    Labyrinth = new int[T2k];

void WOut(int idx)=> console.Write((idx < Eight)?$"{TextOut[((TextCode >> (idx++ << One)) & 3) - 1]}" +
    $"{TextOut[((TextCode >> (idx << One)) & 3) - 1]}":$"{TextOut}{TextOut}{TextOut}\r\n"[idx..]);

    int FiFoPopIdx = 0,
        DirCount = Eight - Eight,
        NextCell,
		ActCell,
		StoredCell,
		FifoPushIdx = T2k - T2k,
		ActCellData,
		LabOutIdx = 0;
    int[] Fifo = new int[T2k],
		PossibDir = new int[4];
    Rnd rnd = new();
    Labyrinth[0] = Eight;
    StoredCell =
    NextCell = CellPerDimension * CellPerDimension - One;
    Labyrinth[StoredCell] = T2k + 2;
    while (DirCount != 0 || FifoPushIdx >= FiFoPopIdx)
    {
    ActCell
#if !SingleTask
    = StoredCell;StoredCell  
#endif
    = NextCell;
    ActCellData = Labyrinth[ActCell];
        DirCount = 0;
        foreach (int ActDir in DirStopArray)
        {
            NextCell = Dir[ActDir & 3] + ActCell;
            if ((NextCell >= 0)
                && (NextCell < CellPerDimension * CellPerDimension)
                && (ActDir != (NextCell % CellPerDimension))
                && (ActCellData & T2k) != (Labyrinth[NextCell] & T2k))
            {
                PossibDir[DirCount++] = ActDir;
            }
        }
        if (DirCount != 0)
        {
            var ActDir = PossibDir[rnd.Next(DirCount)] & 3;
            NextCell = Dir[ActDir] + ActCell;

            Labyrinth[ActCell] |= (One << ActDir);
            Labyrinth[NextCell] |= T2k | (One << ((ActDir + 2) % 4));

            Fifo[FifoPushIdx++] = NextCell;
        }
        else
        
            if (FifoPushIdx >= FiFoPopIdx) NextCell = Fifo[FiFoPopIdx++];
        
#if ShowGen
        console.SetCursorPosition((ActCell % CellPerDimension) * 2, ActCell / CellPerDimension+1);
        console.Write(Scc.Substring(Labyrinth[ActCell] & 0xe, 2));
        Thread.Sleep(10);
#endif
    }

#if ShowGen
    console.SetCursorPosition(0, 0);
#endif
    WOut(Eight - 2);
    for (ActCell = One; ActCell < CellPerDimension; ActCell++)
        WOut(Eight >> One);
    // write a Linebreak
    WOut(Eight + One);
    for (ActCell = One; ActCell < CellPerDimension + One; ActCell++)
    {
        for (NextCell = One; NextCell < CellPerDimension + One; NextCell++)
            WOut(Labyrinth[LabOutIdx++] & 6);
        WOut(Eight); // Write the last column and the linebreak separately
    }




