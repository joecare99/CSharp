using System.Collections.Generic;
using IntegrationTestApp.Pages;

namespace IntegrationTestApp.Models;

internal class DefaultPageProvider : IPageProvider
{
 public IEnumerable<DemoPage> GetPages() =>
 [
 new("Automation",typeof(AutomationPage), () => new AutomationPage()),
 new("Button", typeof(ButtonPage), () => new ButtonPage()),
 new("CheckBox", typeof(CheckBoxPage), () => new CheckBoxPage()),
 new("ComboBox", typeof(ComboBoxPage), () => new ComboBoxPage()),
 new("ContextMenu", typeof(ContextMenuPage), () => new ContextMenuPage()),
 new("DesktopPage", typeof(DesktopPage), () => new DesktopPage()),
 new("Embedding", typeof(EmbeddingPage), () => new EmbeddingPage()),
 new("Gestures", typeof(GesturesPage), () => new GesturesPage()),
 new("ListBox", typeof(ListBoxPage), () => new ListBoxPage()),
 new("Menu", typeof(MenuPage), () => new MenuPage()),
 new("Pointer", typeof(PointerPage), () => new PointerPage()),
 new("RadioButton", typeof(RadioButtonPage), () => new RadioButtonPage()),
 new("Screens", typeof(ScreensPage), () => new ScreensPage()),
 new("ScrollBar", typeof(ScrollBarPage), () => new ScrollBarPage()),
 new("Slider", typeof(SliderPage), () => new SliderPage()),
 new("Window Decorations", typeof(WindowDecorationsPage), () => new WindowDecorationsPage()),
 new("Window", typeof(WindowPage), () => new WindowPage()),
 ];
}
