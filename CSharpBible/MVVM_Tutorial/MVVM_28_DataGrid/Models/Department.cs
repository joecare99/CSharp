using MVVM.ViewModel;

namespace MVVM_28_DataGrid.Models
{
    public class Department : NotificationObject
    {
        public int Id { get; set; } = -1;
        public string? Name => Properties.Resources.ResourceManager.GetString($"dep_{Id}");
        public string? Description => Properties.Resources.ResourceManager.GetString($"dep_{Id}_Desc");

    }
}
