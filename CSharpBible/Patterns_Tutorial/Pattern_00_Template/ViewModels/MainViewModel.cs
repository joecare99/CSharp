using Pattern_00_Template.Models;
using Pattern_00_Template.Properties;

namespace Pattern_00_Template.ViewModels
{
    public class MainViewModel
    {
        Model model = new Model();

        public string Greeting => Resources.ResourceManager.GetString(model.Greeting); 
    }
}