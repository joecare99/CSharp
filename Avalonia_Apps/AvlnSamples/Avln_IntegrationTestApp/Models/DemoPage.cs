using System;
using Avalonia.Controls;

namespace IntegrationTestApp.Models;

internal record DemoPage(string Name, Type PageType, Func<Control> CreateContent);
