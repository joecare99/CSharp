using MVVM_37_TreeView.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM_37_TreeView.Services
{
    public class BooksService : IBooksService
    {
        const string cDev_Cat = "Software Development";
        const string cFantasy_Cat = "Fantasy";
        const string cDoku_Cat = "Documentary";
        private static List<Book> _books = new() {
            new("Schrödinger programmiert c#","Bernhard Wurm",cDev_Cat,new[]{ 5,5,4,5,4,4,5,4,5,4,4,3,4 }),
            new("Patterns of Enterprise Application Architecture","Marin Fowler",cDev_Cat,new[]{3,4,5,2,5,4,5,5,1,5,3 }),
            new("Drachenfeuer","Wolfgang & Heike Hohlbein",cFantasy_Cat,new[]{ 2,3,5,2,1,4,3,3,2,4,3,4 }),
            new("Der pragmatische Programmierer","Andrew Hunt",cDev_Cat,new[]{4,5,5,1,5,3,3,4,5,2,5 }),
            new("Programmieren Lernen","Bernhard Wurm",cDev_Cat,new[]{ 5,4,5,4,4,3,4,5,5,4,5,4,4 }),
            new("Herr der Ringe","John R. Tolkin",cFantasy_Cat,new[]{ 3,2,4,3,4,2,3,5,2,1,4,3 }),
            new("Limit","Frank Schätzing",cFantasy_Cat,new[]{ 4,3,4,5,4,5,4,5,4,1,4,5,5,2 }),
            new("Per Anhalter durch die Galaxis","Douglas Adams",cFantasy_Cat,new[]{ 4,3,4,5,4,5,4,5,4,1,4,5,5,2 }),
            new("Ender's Game","Orson Scott Card",cFantasy_Cat,new[]{ 4,3,4,5,4,5,4,5,4,1,4,5,5,2 }),
            new("Kuckucksei","Clifford Stoll",cDoku_Cat,new[]{ 5,5,5,5,4,5,4,5,4,3,4,5,5,2 }),
        };

        public IEnumerable<Book> GetBooks() => _books;
       
    }
}
