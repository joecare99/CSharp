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
    private readonly IActionResolver _actionResolver;

    // Properties that indicate a TForm-derived component
    private static readonly HashSet<string> _formIndicatorProperties = new(StringComparer.OrdinalIgnoreCase)
    {
        "ClientHeight", "ClientWidth", "Menu", "ActiveControl", 
        "FormStyle", "BorderStyle", "BorderIcons", "WindowState",
        "Position", "LCLVersion", "ShowInTaskBar", "OnCreate", "OnDestroy"
    };

    private static readonly Dictionary<string, Func<LfmComponentBase>> _componentCreators = new(StringComparer.OrdinalIgnoreCase)
    {
        // Forms
        ["TForm"] = () => new TForm(),
        ["TForm1"] = () => new TForm(),
        ["TFrame"] = () => new TForm(),
        ["TFrame1"] = () => new TForm(),
        
        // Labels
        ["TLabel"] = () => new TLabel(),
        ["TDBText"] = () => new TStaticText(),
        ["TStaticText"] = () => new TStaticText(),
        
        // Edit controls
        ["TEdit"] = () => new TEdit(),
        ["TMaskEdit"] = () => new TEdit(),
        ["TDBEdit"] = () => new TEdit(),
        ["TLabeledEdit"] = () => new TLabeledEdit(),
        ["TMemo"] = () => new TMemo(),
        ["TDBMemo"] = () => new TMemo(),
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
        
        // Page controls (tabs)
        ["TPageControl"] = () => new TPageControl(),
        ["TTabSheet"] = () => new TTabSheet(),
        ["TTabbedNotebook"] = () => new TTabbedNotebook(),
        ["TNotebook"] = () => new TNotebook(),
        ["TPage"] = () => new TPage(),
        
        // Checkboxes and radio buttons
        ["TCheckBox"] = () => new TCheckBox(),
        ["TDBCheckBox"] = () => new TCheckBox(),
        ["TRadioButton"] = () => new TRadioButton(),
        
        // List controls
        ["TComboBox"] = () => new TComboBox(),
        ["TDBComboBox"] = () => new TComboBox(),
        ["TListBox"] = () => new TListBox(),
        ["TCheckListBox"] = () => new TListBox(),
        
        // Sliders and progress
        ["TTrackBar"] = () => new TTrackBar(),
        ["TProgressBar"] = () => new TProgressBar(),
        
        // Spin edits
        ["TSpinEdit"] = () => new TSpinEdit(),
        ["TFloatSpinEdit"] = () => new TFloatSpinEdit(),
        ["TUpDown"] = () => new TUpDown(),
        
        // Grids
        ["TDrawGrid"] = () => new TDrawGrid(),
        ["TStringGrid"] = () => new TStringGrid(),
        ["TValueListEditor"] = () => new TValueListEditor(),
        
        // Images and shapes
        ["TImage"] = () => new TImage(),
        ["TPaintBox"] = () => new TPaintBox(),
        ["TShape"] = () => new TShape(),
        ["TBevel"] = () => new TBevel(),
        ["TSplitter"] = () => new TSplitter(),
        
        // Menus
        ["TMainMenu"] = () => new TMainMenu(),
        ["TPopupMenu"] = () => new TPopupMenu(),
        ["TMenuItem"] = () => new TMenuItem(),
        
        // Toolbars and status bars
        ["TToolBar"] = () => new TToolBar(),
        ["TToolButton"] = () => new TToolButton(),
        ["TCoolBar"] = () => new TCoolBar(),
        ["TStatusBar"] = () => new TStatusBar(),
        
        // Actions
        ["TActionList"] = () => new TActionList(),
        ["TAction"] = () => new TAction(),
        ["TFileOpen"] = () => new TFileOpen(),
        ["TFileSaveAs"] = () => new TFileSaveAs(),
        ["TFileExit"] = () => new TFileExit(),
        ["TEditCut"] = () => new TEditCut(),
        ["TEditPaste"] = () => new TEditPaste(),
        ["TEditCopy"] = () => new TEditCopy(),
        ["TEditDelete"] = () => new TEditDelete(),
        ["TEditUndo"] = () => new TEditUndo(),

        // Image lists
        ["TImageList"] = () => new TImageList(),
        
        // Non-visual components
        ["TTimer"] = () => new TTimer(),
        ["TOpenDialog"] = () => new TUnknownComponent(),
        ["TSaveDialog"] = () => new TUnknownComponent(),
        ["TOpenPictureDialog"] = () => new TUnknownComponent(),
        ["TSavePictureDialog"] = () => new TUnknownComponent(),
        ["TPrintDialog"] = () => new TUnknownComponent(),
        ["TFontDialog"] = () => new TUnknownComponent(),
        ["TColorDialog"] = () => new TUnknownComponent(),
        ["TConfig"] = () => new TUnknownComponent(),
    };

    /// <summary>
    /// Creates a new ComponentFactory with default ActionResolver.
    /// </summary>
    public ComponentFactory() : this(new ActionResolver())
    {
    }

    /// <summary>
    /// Creates a new ComponentFactory with the specified ActionResolver.
    /// </summary>
    public ComponentFactory(IActionResolver actionResolver)
    {
        _actionResolver = actionResolver;
    }

    /// <inheritdoc/>
    public LfmComponentBase CreateComponent(LfmObject lfmObject)
    {
        return CreateComponent(lfmObject, isRoot: false);
    }

    /// <summary>
    /// Creates a component from an LfmObject, with optional root detection.
    /// </summary>
    private LfmComponentBase CreateComponent(LfmObject lfmObject, bool isRoot)
    {
        var typeName = lfmObject.TypeName;
        
        // Try exact match first
        if (_componentCreators.TryGetValue(typeName, out var creator))
        {
            var component = creator();
            component.ApplyProperties(lfmObject);
            return component;
        }

        // Try prefix match (e.g., TForm1 -> TForm)
        creator = _componentCreators
            .Where(kvp => typeName.StartsWith(kvp.Key, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(kvp => kvp.Key.Length)
            .Select(kvp => kvp.Value)
            .FirstOrDefault();

        if (creator != null)
        {
            var component = creator();
            component.ApplyProperties(lfmObject);
            return component;
        }

        // For root objects or objects with Form-like properties, treat as TForm
        if (isRoot || IsFormLikeObject(lfmObject))
        {
            var formComponent = new TForm();
            formComponent.ApplyProperties(lfmObject);
            return formComponent;
        }

        // Fallback to unknown component
        var unknownComponent = new TUnknownComponent();
        unknownComponent.ApplyProperties(lfmObject);
        return unknownComponent;
    }

    /// <summary>
    /// Checks if an LfmObject has properties typical of a TForm.
    /// </summary>
    private static bool IsFormLikeObject(LfmObject lfmObject)
    {
        // Check if the object has any Form-indicator properties
        return lfmObject.Properties.Any(p => 
            _formIndicatorProperties.Contains(p.Name));
    }

    /// <inheritdoc/>
    public LfmComponentBase? CreateComponentTree(LfmObject? rootObject)
    {
        if (rootObject == null)
            return null;

        // Root object is always treated with isRoot=true
        var rootComponent = CreateComponent(rootObject, isRoot: true);
        BuildChildComponents(rootComponent, rootObject);
        
        // After building the tree, resolve all action links
        _actionResolver.ResolveActions(rootComponent);
        
        return rootComponent;
    }

    private void BuildChildComponents(LfmComponentBase parent, LfmObject lfmObject)
    {
        foreach (var childLfm in lfmObject.Children)
        {
            var childComponent = CreateComponent(childLfm, isRoot: false);
            childComponent.Parent = parent;
            parent.Children.Add(childComponent);
            
            // Recursively build children
            BuildChildComponents(childComponent, childLfm);
        }
    }
}
