using Basic_Del01_Action.Models;
using Basic_Del01_Action.Properties;
using System;

namespace Basic_Del01_Action.ViewModels
{
    public class MainViewModel
    {
        Model model = new Model();

        public string Greeting => Resources.ResourceManager.GetString(model.Greeting); 

        public Span<int> GetSortedData(Action<string>? Wrt=null)
        {
            Span<int> data = model.data;
            data.QuickSort(Wrt);
            return data;
        }
    }
}
