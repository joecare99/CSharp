using Basic_Del01_Action.Models;
using Basic_Del01_Action.Properties;

namespace Basic_Del01_Action.ViewModels
{
    public class MainViewModel
    {
        Model model = new Model();

        public string Greeting => Resources.ResourceManager.GetString(model.Greeting); 
    }
}
