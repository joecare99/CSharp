using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace WinAhnenNew.ViewModels
{
    /// <summary>
    /// Provides the command surface for the main WinAhnen window.
    /// </summary>
    public sealed partial class FrmAhnenWinMainViewModel : ViewModelBase
    {
        [RelayCommand] private void GebDatumTaufdatum1() { }
        [RelayCommand] private void Datenlschen1() { }
        [RelayCommand] private void IDSatznr1() { }
        [RelayCommand] private void IDslschen1() { }
        [RelayCommand] private void Datenschutzein1() { }
        [RelayCommand] private void Datenschutzaus1() { }
        [RelayCommand] private void Druckereinstellung1() { }
        [RelayCommand] private void Bildschirmdrucken1() { }
        [RelayCommand] private void ExcelExport1() { }
        [RelayCommand] private void EhenKiordnen1() { }
        [RelayCommand] private void Beenden1() 
        { 
            //Applikation beenden
            Application.Current.Shutdown();
        }
        [RelayCommand] private void AHNENDOSeinlesen1() { }
        [RelayCommand] private void OSBeinlesen1() { }
        [RelayCommand] private void GedcomDateischreiben1() { }
        [RelayCommand] private void GedcomDateieinlesen1() { }
        [RelayCommand] private void GedcomXMLschreiben1() { }
        [RelayCommand] private void GedcomXMLeinlesen1() { }
        [RelayCommand] private void FOKODateierstellen1() { }
        [RelayCommand] private void FOKODateieditieren1() { }
        [RelayCommand] private void InyTafelerstellen1() { }
        [RelayCommand] private void Adressendateischreiben1() { }
        [RelayCommand] private void Neu1() { }
        [RelayCommand] private void Schen1() { }
        [RelayCommand] private void DatensatzNr1() { }
        [RelayCommand] private void GlobaleSuche1() { }
        [RelayCommand] private void Partnersuche1() { }
        [RelayCommand] private void Anzeigen1() { }
        [RelayCommand] private void Drucken1() { }
        [RelayCommand] private void Verwandtschaft1() { }
        [RelayCommand] private void Vettern1() { }
        [RelayCommand] private void Basen1() { }
        [RelayCommand] private void Familienblatt2() { }
        [RelayCommand] private void FamilienblattHTML1() { }
        [RelayCommand] private void FamGrafik1() { }
        [RelayCommand] private void OFBDruck1() { }
        [RelayCommand] private void AllePersonen1() { }
        [RelayCommand] private void FrbestimmtenOrt1() { }
        [RelayCommand] private void AllePersonen2() { }
        [RelayCommand] private void Probandenname1() { }
        [RelayCommand] private void Quellenliste1() { }
        [RelayCommand] private void NamensListeeditieren1() { }
        [RelayCommand] private void OrtsDatei1() { }
        [RelayCommand] private void BerufeVerwaltung1() { }
        [RelayCommand] private void QuellenListeeditieren1() { }
        [RelayCommand] private void HofnamenVerwaltung1() { }
        [RelayCommand] private void Listenaktualisieren1() { }
        [RelayCommand] private void Listen2() { }
        [RelayCommand] private void Proband1() { }
        [RelayCommand] private void AllePersonen3() { }
        [RelayCommand] private void Basisahnen1() { }
        [RelayCommand] private void Grafikalt1() { }
        [RelayCommand] private void Grafik1() { }
        [RelayCommand] private void Liste1() { }
        [RelayCommand] private void Geschachtelt1() { }
        [RelayCommand] private void GrafikI1() { }
        [RelayCommand] private void Grafikneu1() { }
        [RelayCommand] private void GregorianischerKalender1() { }
        [RelayCommand] private void FrzRevolutionskalender1() { }
        [RelayCommand] private void Osterberechnung1() { }
        [RelayCommand] private void ChristlicheNamenstage1() { }
        [RelayCommand] private void Einfgen1() { }
        [RelayCommand] private void Entfernen1() { }
        [RelayCommand] private void Bmpjpg1() { }
        [RelayCommand] private void AlleBilderentfernen1() { }
        [RelayCommand] private void Bernehmen1() { }
        [RelayCommand] private void Prfen1() { }
        [RelayCommand] private void Einzelpersonen1() { }
        [RelayCommand] private void DoppelteEintrge1() { }
        [RelayCommand] private void NichtverknpfteBeziehungen1() { }
        [RelayCommand] private void ID1() { }
        [RelayCommand] private void Name1() { }
        [RelayCommand] private void Hausname1() { }
        [RelayCommand] private void Aus1() { }
        [RelayCommand] private void Hilfe1() { }
        [RelayCommand] private void ZuAHNENWIN1() { }
        [RelayCommand] private void Fenstertitelndern1() { }
        [RelayCommand] private void Datensichern1() { }
        [RelayCommand] private void SicherungsDateieinlesen2() { }
    }
}
