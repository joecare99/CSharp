namespace TranspilerLib.Interfaces.Code;

/// <summary>
/// Represents an interface for IEC (International Electrotechnical Commission) codes,  extending the functionality of
/// the <see cref="ICodeBase"/> interface.
/// </summary>
/// <remarks>This interface serves as a marker or base for defining IEC-specific code implementations. It inherits
/// from <see cref="ICodeBase"/>, allowing for shared functionality across code-related types.</remarks>
public interface IIECCode : ICodeBase
{
}