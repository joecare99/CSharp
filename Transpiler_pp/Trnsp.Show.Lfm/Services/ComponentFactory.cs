using System;
using System.Collections.Generic;
using System.Linq;
using Trnsp.Show.Lfm.Models.Components;
using TranspilerLib.Pascal.Models;

namespace Trnsp.Show.Lfm.Services;

/// <summary>
/// Factory for creating LFM components based on type name.
/// </summary>
public class ComponentFactory : IComponentFactory
{
    private static readonly Dictionary<string, Func<LfmComponentBase>> _componentCreators = new(StringComparer.OrdinalIgnoreCase)
    {
        // Forms
        ["TForm"] = () => new TForm(),
        ["TForm1"] = () => new TForm(),
        
        // Labels
        ["TLabel"] = () => new TLabel(),
        ["TStaticText"] = () => new TLabel(),
        
        // Edit controls
        ["TEdit"] = () => new TEdit(),
        ["TMaskEdit"] = () => new TEdit(),
        ["TLabeledEdit"] = () => new TEdit(),
        ["TMemo"] = () => new TMemo(),
        ["TRichEdit"] = () => new TMemo(),
        
        // Buttons
        ["TButton"] = () => new TButton(),
        ["TBitBtn"] = () => new TBitBtn(),
        ["TSpeedButton"] = () => new TSpeedButton(),
        
        // Panels and containers
        ["TPanel"] = () => new TPanel(),
        ["TScrollBox"] = () => new TScrollBox(),
        ["TGroupBox"] = () => new TGroupBox(),
        ["TRadioGroup"] = () => new TRadioGroup(),
        ["TCheckGroup"] = () => new TCheckGroup(),
        
        // Checkboxes and radio buttons
        ["TCheckBox"] = () => new TCheckBox(),
        ["TRadioButton"] = () => new TRadioButton(),
        
        // List controls
        ["TComboBox"] = () => new TComboBox(),
        ["TListBox"] = () => new TListBox(),
        ["TCheckListBox"] = () => new TListBox(),
        
        // Sliders and progress
        ["TTrackBar"] = () => new TTrackBar(),
        ["TProgressBar"] = () => new TProgressBar(),
        
        // Images
        ["TImage"] = () => new TImage(),
        ["TPaintBox"] = () => new TPaintBox(),
        ["TShape"] = () => new TImage(),
        
        // Non-visual components
        ["TTimer"] = () => new TTimer(),
        ["TOpenDialog"] = () => new TUnknownComponent(),
        ["TSaveDialog"] = () => new TUnknownComponent(),
        ["TOpenPictureDialog"] = () => new TUnknownComponent(),
        ["TSavePictureDialog"] = () => new TUnknownComponent(),
        ["TPrintDialog"] = () => new TUnknownComponent(),
        ["TFontDialog"] = () => new TUnknownComponent(),
        ["TColorDialog"] = () => new TUnknownComponent(),
        ["TMainMenu"] = () => new TUnknownComponent(),
        ["TPopupMenu"] = () => new TUnknownComponent(),
        ["TImageList"] = () => new TUnknownComponent(),
        ["TActionList"] = () => new TUnknownComponent(),
    };

    /// <inheritdoc/>
    public LfmComponentBase CreateComponent(LfmObject lfmObject)
    {
        var typeName = lfmObject.TypeName;
        
        // Handle inherited forms (e.g., TForm1 inherits TForm)
        if (!_componentCreators.TryGetValue(typeName, out var creator))
        {
            // Try to find base type
            creator = _componentCreators
                .Where(kvp => typeName.StartsWith(kvp.Key, StringComparison.OrdinalIgnoreCase))
                .Select(kvp => kvp.Value)
                .FirstOrDefault();
        }

        var component = creator?.Invoke() ?? new TUnknownComponent();
        component.ApplyProperties(lfmObject);
        
        return component;
    }

    /// <inheritdoc/>
    public LfmComponentBase? CreateComponentTree(LfmObject? rootObject)
    {
        if (rootObject == null)
            return null;

        var rootComponent = CreateComponent(rootObject);
        BuildChildComponents(rootComponent, rootObject);
        
        return rootComponent;
    }

    private void BuildChildComponents(LfmComponentBase parent, LfmObject lfmObject)
    {
        foreach (var childLfm in lfmObject.Children)
        {
            var childComponent = CreateComponent(childLfm);
            childComponent.Parent = parent;
            parent.Children.Add(childComponent);
            
            // Recursively build children
            BuildChildComponents(childComponent, childLfm);
        }
    }
}
