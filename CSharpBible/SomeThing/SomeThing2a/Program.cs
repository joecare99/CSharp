#define ShowGen
using System.Threading;
using ConsoleDisplay.View;

// Initialisieren der Variablen
const int gridSize = 1 << 11;
const int cellCount = (0x13 << 1) + 1;
const int cellSpacing = cellCount & -cellCount;
const int cellSize = cellSpacing << ((cellSpacing << cellSpacing) + 1);
const int housePattern = 0x597b;
var houseChars = " _|";
#if ShowGen
var Scc = "   ,---, ' |-'-|";
#endif
int[] rowOffsets = new int[] { 0, cellSpacing - 4, cellCount - cellSpacing, -cellSpacing };
int[] colOffsets = new int[] { cellSpacing, cellCount, -cellSpacing, -cellCount };
int[] grid = new int[gridSize];

MyConsole console = new();
Rnd randomGenerator = new();
// Funktion zum Drucken von ASCII-Art
void PrintHouse(int x) => console.Write((x < cellSize) ? $"{houseChars[((housePattern >> (x++ << cellSpacing)) & 3) - 1]}{houseChars[((housePattern >> (x << cellSpacing)) & 3) - 1]}" : $"{houseChars}{houseChars}{houseChars}\r\n"[x..]);

// Initialisieren der Variablen
int queueHead = 0;
int queueTail = cellSize - cellSize,
    queueTail2=1;

int currentCell;
int nextCell;
int lastCell,
    LabOutIdx = 0; 
int[] queue = new int[gridSize],
    queue2 = new int[4];
// Erstellen des ASCII-Art-Hauses
grid[0] = cellSize;
lastCell = nextCell = (cellCount * cellCount) - cellSpacing;
grid[nextCell] = gridSize + 2;

while (queueTail2 != 0 || queueTail <= queueHead)
{
    currentCell = lastCell;
    lastCell= nextCell;

    queueTail2 = 0;
    // Überprüfen der Nachbarzellen
    for (int i = 0; i < rowOffsets.Length; i++)
    {
        nextCell = colOffsets[i] + currentCell;

        if ((nextCell >= 0) 
            && (nextCell < cellCount * cellCount) 
            && (rowOffsets[i] != (nextCell % cellCount)) 
            && ((grid[currentCell] & gridSize) != (grid[nextCell] & gridSize)))
        {
            queue2[queueTail2++] = i;
        }
    }

    // Wählen einer zufälligen Nachbarzelle
    if (queueTail2 != 0)
    {
        var lastDirection = queue2[randomGenerator.Next(queueTail2)];
        nextCell = colOffsets[lastDirection] + currentCell;

        grid[currentCell] |= (cellSpacing << lastDirection);
        grid[nextCell] |= gridSize | (cellSpacing << ((lastDirection + 2) % 4));

        queue[queueHead++] = nextCell;
    }
    else if (queueTail <= queueHead)
    {
        nextCell = queue[queueTail++];
    }
#if ShowGen
    if (!console.IsOutputRedirected)
    {
        console.SetCursorPosition((currentCell % cellCount) * 2, currentCell / cellCount + 1);
        console.Write(Scc.Substring(grid[currentCell] & 0xe, 2));
        Thread.Sleep(10);
    }
#endif
}
#if ShowGen
if (!console.IsOutputRedirected)
    console.SetCursorPosition(0, 0);
#endif
// Drucken des ASCII-Art-Hauses
PrintHouse(cellSize - 2);
for (currentCell = cellSpacing; currentCell < cellCount; currentCell++)
{
    PrintHouse(cellSize >> cellSpacing);
}
PrintHouse(cellSize + cellSpacing);

for (currentCell = cellSpacing; currentCell < cellCount + cellSpacing; currentCell++)
{
    for (nextCell = cellSpacing; nextCell < cellCount + cellSpacing; nextCell++)
    {
        PrintHouse(grid[LabOutIdx++] & 6);
    }

    PrintHouse(cellSize);
}
