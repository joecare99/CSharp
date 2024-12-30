using Basic_Del04_TestImposibleStuff.Models;
using Basic_Del04_TestImposibleStuff.Properties;

namespace Basic_Del04_TestImposibleStuff.ViewModels
{
    public class MainViewModel
    {
        Model model = new Model();

        public string Greeting => Resources.ResourceManager.GetString(model.Greeting); 
    }
}
