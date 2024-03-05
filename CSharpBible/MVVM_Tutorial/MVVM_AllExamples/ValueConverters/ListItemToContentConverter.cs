using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;
using MVVM_AllExamples.Models;

namespace MVVM_AllExamples.ValueConverters;

public class ListItemToContentConverter : IValueConverter
{

    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
            return null;
        if (value is ExItem lbi)
        {
            var v = Activator.CreateInstance(lbi.ExType);
            if (v is Page fe)
            {
                if (lbi.Additionals is Dictionary<string, string> d)
                {
                    var tv = new TabControl();
                    string Title = lbi.Description;
                    string Description = lbi.Description;
                    if (d.TryGetValue("Title", out string? t))
                        Title = t;
                    if (d.TryGetValue("Description", out string? dsc))
                        Description = dsc;
                    tv.ToolTip = Description;
                    tv.Items.Add(new TabItem() { Header = Title, Content = new Frame() { Content = fe } });
                    if (d.Keys.FirstOrDefault((o) => o.EndsWith("Views")) is string xaml)
                    {
                        tv.Items.Add(new TabItem()
                        {
                            Header = "Xaml",
                            Content = new TextBox() { Text = d[xaml], IsReadOnly = true, VerticalScrollBarVisibility = ScrollBarVisibility.Visible }
                        });
                    }
                    if (d.Keys.FirstOrDefault((o) => o.EndsWith("_xaml")) is string xamlcs)
                    {
                        tv.Items.Add(new TabItem()
                        {
                            Header = "Xaml.cs",
                            Content = new TextBox() { Text = d[xamlcs], IsReadOnly = true, VerticalScrollBarVisibility = ScrollBarVisibility.Visible }
                        });
                    }
                    if (d.Keys.FirstOrDefault((o) => o.EndsWith("ViewModel")) is string vm)
                    {
                        tv.Items.Add(new TabItem()
                        {
                            Header = "ViewModel",
                            Content = new TextBox() { Text = d[vm], IsReadOnly = true, VerticalScrollBarVisibility = ScrollBarVisibility.Visible }
                        });
                    }
                    if (d.Keys.FirstOrDefault((o) => o.EndsWith("Model") && !o.EndsWith("ViewModel")) is string mdl)
                    {
                        tv.Items.Add(new TabItem()
                        {
                            Header = "Model",
                            Content = new TextBox() { Text = d[mdl], IsReadOnly = true, VerticalScrollBarVisibility = ScrollBarVisibility.Visible }
                        });
                    }
                    var vcc = 0;
                    foreach (string vc in d.Keys.Where((o) => o.EndsWith("Converter")) )
                    {
                        tv.Items.Add(new TabItem()
                        {
                            Header = $"ValueConverter{++vcc}",
                            Content = new TextBox() { Text = d[vc], IsReadOnly = true, VerticalScrollBarVisibility = ScrollBarVisibility.Visible }
                        });
                    }

                    return tv;
                }
                else
                    return new Frame() { Content = fe };
            }
            else
                return v;
        }
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
