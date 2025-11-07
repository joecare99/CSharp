using System.Collections.Generic;
using IntegrationTestApp.Pages;

namespace IntegrationTestApp.Models;

internal class DefaultPageProvider : IPageProvider
{
 public IEnumerable<Page> GetPages() =>
 [
 new("Automation", () => new AutomationPage()),
 new("Button", () => new ButtonPage()),
 new("CheckBox", () => new CheckBoxPage()),
 new("ComboBox", () => new ComboBoxPage()),
 new("ContextMenu", () => new ContextMenuPage()),
 new("DesktopPage", () => new DesktopPage()),
 new("Embedding", () => new EmbeddingPage()),
 new("Gestures", () => new GesturesPage()),
 new("ListBox", () => new ListBoxPage()),
 new("Menu", () => new MenuPage()),
 new("Pointer", () => new PointerPage()),
 new("RadioButton", () => new RadioButtonPage()),
 new("Screens", () => new ScreensPage()),
 new("ScrollBar", () => new ScrollBarPage()),
 new("Slider", () => new SliderPage()),
 new("Window Decorations", () => new WindowDecorationsPage()),
 new("Window", () => new WindowPage()),
 ];
}
