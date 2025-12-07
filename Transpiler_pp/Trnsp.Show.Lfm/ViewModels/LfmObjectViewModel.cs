using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using TranspilerLib.Pascal.Models;

namespace Trnsp.Show.Lfm.ViewModels;

/// <summary>
/// ViewModel for displaying an LfmObject in a TreeView.
/// </summary>
public partial class LfmObjectViewModel : ObservableObject
{
    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _typeName = string.Empty;

    [ObservableProperty]
    private bool _isInherited;

    [ObservableProperty]
    private bool _isExpanded = true;

    [ObservableProperty]
    private bool _isSelected;

    public ObservableCollection<LfmPropertyViewModel> Properties { get; } = [];
    public ObservableCollection<LfmObjectViewModel> Children { get; } = [];

    public string DisplayName => IsInherited 
        ? $"inherited {Name}: {TypeName}" 
        : $"{Name}: {TypeName}";

    public string Icon => IsInherited ? "??" : "??";

    public static LfmObjectViewModel? FromModel(LfmObject? lfmObject)
    {
        if (lfmObject == null)
            return null;

        var vm = new LfmObjectViewModel
        {
            Name = lfmObject.Name,
            TypeName = lfmObject.TypeName,
            IsInherited = lfmObject.IsInherited
        };

        foreach (var prop in lfmObject.Properties)
        {
            vm.Properties.Add(LfmPropertyViewModel.FromModel(prop));
        }

        foreach (var child in lfmObject.Children)
        {
            var childVm = FromModel(child);
            if (childVm != null)
            {
                vm.Children.Add(childVm);
            }
        }

        return vm;
    }
}

/// <summary>
/// ViewModel for displaying an LfmProperty.
/// </summary>
public partial class LfmPropertyViewModel : ObservableObject
{
    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _value = string.Empty;

    [ObservableProperty]
    private LfmPropertyType _propertyType = LfmPropertyType.Simple;

    public string DisplayValue => PropertyType switch
    {
        LfmPropertyType.Set => $"[{Value}]",
        LfmPropertyType.Binary => "{Binary Data}",
        LfmPropertyType.StringContinuation => $"\"{Value}\"",
        _ => Value
    };

    public string Icon => PropertyType switch
    {
        LfmPropertyType.Set => "??",
        LfmPropertyType.Binary => "??",
        LfmPropertyType.StringContinuation => "??",
        _ => "??"
    };

    public static LfmPropertyViewModel FromModel(LfmProperty prop)
    {
        var valueStr = prop.Value switch
        {
            System.Collections.IEnumerable enumerable when prop.Value is not string 
                => string.Join(", ", enumerable.Cast<object>()),
            null => "null",
            _ => prop.Value.ToString() ?? string.Empty
        };

        return new LfmPropertyViewModel
        {
            Name = prop.Name,
            Value = valueStr,
            PropertyType = prop.PropertyType
        };
    }
}
