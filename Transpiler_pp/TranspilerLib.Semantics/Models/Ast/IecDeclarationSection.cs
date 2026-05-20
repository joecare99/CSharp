namespace TranspilerLib.IEC.Models.Ast;

/// <summary>
/// Defines the supported IEC declaration sections for the current typed declaration model.
/// </summary>
public enum IecDeclarationSection
{
    /// <summary>
    /// The declaration section is not known.
    /// </summary>
    Unknown,

    /// <summary>
    /// The declaration belongs to a VAR_INPUT block.
    /// </summary>
    Input,

    /// <summary>
    /// The declaration belongs to a VAR_OUTPUT block.
    /// </summary>
    Output,

    /// <summary>
    /// The declaration belongs to a VAR_IN_OUT block.
    /// </summary>
    InOut,

    /// <summary>
    /// The declaration belongs to a VAR_INST block.
    /// </summary>
    Instance,

    /// <summary>
    /// The declaration belongs to a generic VAR block.
    /// </summary>
    Local,
}
