using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;
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
            object? v = null;
            try
            {
                v = Activator.CreateInstance(lbi.ExType);
            }
            catch (Exception ex)
            {
                var sMsg = ex.Message;
                if (ex.InnerException != null)
                {
                    sMsg += "\r\n" + ex.InnerException.Message;
                    if (ex.InnerException.InnerException != null)
                    {
                        sMsg += "\r\n" + ex.InnerException.InnerException.Message;
                        sMsg += "\r\n" + ex.InnerException.InnerException.StackTrace;
                    }
                    else
                        sMsg += "\r\n" + ex.InnerException.StackTrace;
                }
                    else
                        sMsg += "\r\n" + ex.StackTrace;
                return new TextBox() { Text = sMsg, IsReadOnly = true, VerticalScrollBarVisibility = ScrollBarVisibility.Visible };
            }
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
                    try
                    {
                        tv.Items.Add(new TabItem() { Header = Title, Content = new Frame() { Content = fe } });
                    }
                    catch (Exception ex)
                    {
                        tv.Items.Add(new TabItem() { Header = Title, Content = new TextBox() { Text = ex.Message, IsReadOnly = true, VerticalScrollBarVisibility = ScrollBarVisibility.Visible } });
                    }
                    if (d.Keys.FirstOrDefault((o) => o.EndsWith("View")) is string xaml)
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
