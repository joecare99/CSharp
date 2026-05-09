using System;

namespace System.Diagnostics.CodeAnalysis;

#if !NET5_0_OR_GREATER
/// <summary>
/// Specifies that when a method returns the specified <see cref="ReturnValue"/>, the associated parameter will not be <see langword="null"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
public sealed class NotNullWhenAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotNullWhenAttribute"/> class.
    /// </summary>
    /// <param name="returnValue">The return value condition.</param>
    public NotNullWhenAttribute(bool returnValue)
    {
        ReturnValue = returnValue;
    }

    /// <summary>
    /// Gets the return value condition.
    /// </summary>
    public bool ReturnValue { get; }
}
#endif