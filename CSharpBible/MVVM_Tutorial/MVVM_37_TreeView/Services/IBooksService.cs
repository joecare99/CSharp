using MVVM_37_TreeView.Models;
using System.Collections.Generic;

namespace MVVM_37_TreeView.Services
{
    public interface IBooksService
    {
        IEnumerable<Book> GetBooks();
    }
}