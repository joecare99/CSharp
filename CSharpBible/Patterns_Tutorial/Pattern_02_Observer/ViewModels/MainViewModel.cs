using Pattern_02_Observer.Models;
using Pattern_02_Observer.Properties;

namespace Pattern_02_Observer.ViewModels
{
    public class MainViewModel
    {
        Model model = new Model();

        public string Greeting => Resources.ResourceManager.GetString(model.Greeting); 
    }
}
