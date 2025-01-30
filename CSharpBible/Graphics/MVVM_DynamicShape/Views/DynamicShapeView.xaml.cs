using MVVM_DynamicShape.ViewModels;
using System.Windows.Controls;

namespace MVVM_DynamicShape.Views
{
    /// <summary>
    /// Interaktionslogik für DynamicShapeView.xaml
    /// </summary>
    public partial class DynamicShapeView : Page
    {
        #region Methode
        public DynamicShapeView()
        {
            DataContextChanged += (s, e) =>
            {
                if (DataContext is DynamicShapeViewModel viewModel)
                {
         //           viewModel.onCreateShape = DoCreateShape;
                }
            };
            InitializeComponent();            
        }


        #endregion
    }
}
