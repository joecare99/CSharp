using System.Collections.Generic;
using AlternatingAppearanceOfItems.Models.Interfaces;

namespace AlternatingAppearanceOfItems.Models;

// Pseudocode:
// - Verwende C# 12 Collection Expressions, um die List<ILeague>-Basisliste direkt im Konstruktor zu befüllen.
// - Nutze base([...]) für Übergabe an List<T>-Konstruktor.
// - Für verschachtelte Listen (Divisions, Teams) Collection-/Objektinitialisierer mit Divisions = { ... } und Teams = { ... } (kein Setter nötig).
// - Ersetze alle Add-Aufrufe durch deklarative Initialisierung.

public class ListLeagueList : List<ILeague>
{
    public ListLeagueList() : base([
        new League("League A")
        {
            Divisions =
            {
                new Division("Division A")
                {
                    Teams =
                    {
                        new Team("Team I"),
                        new Team("Team II"),
                        new Team("Team III"),
                        new Team("Team IV"),
                        new Team("Team V")
                    }
                },
                new Division("Division B")
                {
                    Teams =
                    {
                        new Team("Team Blue"),
                        new Team("Team Red"),
                        new Team("Team Yellow"),
                        new Team("Team Green"),
                        new Team("Team Orange")
                    }
                },
                new Division("Division C")
                {
                    Teams =
                    {
                        new Team("Team East"),
                        new Team("Team West"),
                        new Team("Team North"),
                        new Team("Team South")
                    }
                }
            }
        },
        new League("League B")
        {
            Divisions =
            {
                new Division("Division A")
                {
                    Teams =
                    {
                        new Team("Team1"),
                        new Team("Team2"),
                        new Team("Team3"),
                        new Team("Team4"),
                        new Team("Team5")
                    }
                },
                new Division("Division B")
                {
                    Teams =
                    {
                        new Team("Team Diamond"),
                        new Team("Team Heart"),
                        new Team("Team Club"),
                        new Team("Team Spade")
                    }
                },
                new Division("Division C")
                {
                    Teams =
                    {
                        new Team("Team Alpha"),
                        new Team("Team Beta"),
                        new Team("Team Gamma"),
                        new Team("Team Delta"),
                        new Team("Team Epsilon")
                    }
                }
            }
        }
    ])
    { }
}
