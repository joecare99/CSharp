using System;
using System.Drawing;
using System.IO;
using CreateCards.Model;

public static class Program
{
    public static Action<string[]> Run { get; set; } = DoRun;

    public static void Main(string[] args)
        => Run(args);

    private static void DoRun(string[] args)
    {
        foreach (string suit in new[] { "♣", "♠", "♥", "♦" })
            foreach (CardValues value in Enum.GetValues(typeof(CardValues)))
            {
                Card card = new Card(suit, value, suit is "♣" or "♠" ? Color.Black : Color.Red);
                Bitmap cardImage = card.GetCard(400, 600);
                cardImage.Save(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), $"card_{suit}{value.ToString().Trim('_')}.png"));
            }
    }
}