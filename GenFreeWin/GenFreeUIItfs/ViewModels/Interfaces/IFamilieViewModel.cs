using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace GenFree.ViewModels.Interfaces;

public interface IFamilieViewModel: INotifyPropertyChanged
{
    IContainerControl View { get; set; }
    int iFamNr { get; set; }
    IPersonRedViewModel Father { get; }
    IPersonRedViewModel Mother { get; }

    void Fameinlesen(int famInArb, out short rich);
    void Familie_FormClosed(object sender, FormClosedEventArgs e);
    void Familie_Load(object sender, EventArgs e);
}