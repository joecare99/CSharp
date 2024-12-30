using MVVM.ViewModel;

namespace MVVM_27_DataGrid.Models
{
    public class Department : NotificationObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

    }
}
