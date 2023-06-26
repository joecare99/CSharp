using Basic_Del02_Filter.Models;
using Basic_Del02_Filter.Properties;
using System;

namespace Basic_Del02_Filter.ViewModels
{
    public class MainViewModel
    {
        Model model = new Model();

        public string Greeting => Resources.ResourceManager.GetString(model.Greeting);

        public int[] GetList=>model.data;

        public int[] GetFilteredData(Predicate<int> filter)
        {
            var data = model.data;
            return data.Filter(filter);           
        }
    }
}
