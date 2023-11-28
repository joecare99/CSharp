using Basic_Del03_General.Models;
using Basic_Del03_General.Properties;

namespace Basic_Del03_General.ViewModels
{
    public class MainViewModel
    {
        Model model = new Model();

        public string Greeting => Resources.ResourceManager.GetString(model.Greeting);

        public double CalcResult model.myCalc(3,2);
    }
}
