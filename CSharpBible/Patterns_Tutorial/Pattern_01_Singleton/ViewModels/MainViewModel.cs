using Pattern_01_Singleton.Models;
using Pattern_01_Singleton.Properties;

namespace Pattern_01_Singleton.ViewModels
{
    public class MainViewModel
    {
        Model model = new Model();

        public string Greeting => Resources.ResourceManager.GetString(model.Greeting); 

        public MainViewModel() {
            MyUserContext = UserContext.Instance;
        }

        public UserContext MyUserContext { get; set; }
        public string? EqualityMsg => Resources.ResourceManager.GetString(nameof(EqualityMsg));
    }
}
