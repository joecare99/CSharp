using CommunityToolkit.Mvvm.Input;
using GenFree.Helper;
using MVVM.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Forms;

namespace Gen_FreeWin.Views;

public interface IOFBViewModel: INotifyPropertyChanged
{
    IContainerControl View { get; set; }
    IRelayCommand List1_DblClickCommand { get; }
    IRelayCommand List2_DblClickCommand { get; }
    IRelayCommand List3_DblClickCommand { get; }
    IRelayCommand List4_DblClickCommand { get; }
    IRelayCommand List5_0_DblClickCommand { get; }
    IRelayCommand List5_1_DblClickCommand { get; }
    IRelayCommand List5_2_DblClickCommand { get; }
    IRelayCommand Text1_KeyEnterCommand { get; }
    IRelayCommand ApplyCommand { get; }
    IRelayCommand List50_AddCommand { get; }
    IRelayCommand List51_AddCommand { get; }
    IRelayCommand List52_AddCommand { get; }

    ObservableCollection<string> List1_Items { get; set; }
    ObservableCollection<string> List2_Items { get; set; }
    ObservableCollection<string> List3_Items { get; set; }
    ObservableCollection<IListItem<int>> List4_Items { get; set; }
    ObservableCollection<string> List50_Items { get; set; }
    ObservableCollection<string> List51_Items { get; set; }
    ObservableCollection<IListItem<int>> List52_Items { get; set; }
    
    string Text1_Text { get; set; }
    string Text2_0_Text { get; set; }
    string Text2_1_Text { get; set; }
    string Text2_2_Text { get; set; }
    
    bool Check1_Checked { get; set; }
    bool List1_Visible { get; set; }
    bool List2_Visible { get; set; }
    bool List3_Visible { get; set; }
    bool List4_Visible { get; set; }
    string List1_SelectedItem { get; set; }
    string List2_SelectedItem { get; set; }
    string List3_SelectedItem { get; set; }
    IListItem<int> List4_SelectedItem { get; set; }
    IListItem<int> List52_SelectedItem { get; set; }
    Action DoClose { get; set; }
    Action<float> InitView { get; set; }
    Action<string> SetFocus { get; set; }
    string List50_SelectedItem { get; set; }
    string List51_SelectedItem { get; set; }

    void OFB_Load(object sender, EventArgs e);
}