using ConsoleLib.CommonControls;

namespace ConsoleLib.Interfaces;

/// <summary>Optional renderer capability for standard form controls.</summary>
public interface IFormWidgetRenderer
{
    void DrawCheckBox(CheckBox checkBox);
    void DrawComboBox(ComboBox comboBox);
    void DrawProgressBar(ProgressBar progressBar);
    void DrawStatusBar(StatusBar statusBar);
    void DrawTabControl(TabControl tabControl);
}
