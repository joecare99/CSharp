using Basic_Del00_Template.Models;
using Basic_Del00_Template.Properties;

namespace Basic_Del00_Template.ViewModels
{
    public class MainViewModel
    {
        Model model = new Model();

        public string Greeting => Resources.ResourceManager.GetString(model.Greeting); 
    }
}
