using System;
using System.Dynamic;
using System.Reflection;

namespace GenFree.Helper;

/// <summary>
/// Provides a dynamic wrapper around a concrete CLR type by forwarding member access
/// to the wrapped instance's public properties and methods.
/// </summary>
/// <remarks>
/// The proxy creates a new instance of the wrapped type and exposes its public
/// members through the <see cref="DynamicObject"/> API. Property reads and writes are
/// handled by <see cref="TryGetMember"/> and <see cref="TrySetMember"/>, while method
/// invocations are handled by <see cref="TryInvokeMember"/>.
/// </remarks>
public class DynProxy : DynamicObject
{
    private readonly Type _t;
    private readonly object? _o;

    /// <summary>
    /// Initializes a new instance of the <see cref="DynProxy"/> class for the specified type.
    /// </summary>
    /// <param name="t">The type whose public properties and methods should be exposed.</param>
    /// <exception cref="ArgumentNullException"><paramref name="t"/> is <see langword="null"/>.</exception>
    /// <exception cref="MissingMethodException">The target type cannot be instantiated with a parameterless constructor.</exception>
    public DynProxy(Type t)
    {
        _t = t ?? throw new ArgumentNullException(nameof(t));
        _o = Activator.CreateInstance(t);
    }

    /// <summary>
    /// Attempts to retrieve the value of a public readable property on the wrapped instance.
    /// </summary>
    /// <param name="binder">The binder that describes the requested member.</param>
    /// <param name="result">When this method returns <see langword="true"/>, contains the property value; otherwise <see langword="null"/>.</param>
    /// <returns><see langword="true"/> when a readable public property with the requested name exists; otherwise <see langword="false"/>.</returns>
    public override bool TryGetMember(GetMemberBinder binder, out object? result)
    {
        if (_t.GetProperty(binder.Name) is PropertyInfo p && p.CanRead)
        {
            result = p.GetValue(_o);
            return true;
        }

        result = null;
        return false;
    }

    /// <summary>
    /// Attempts to assign a value to a public writable property on the wrapped instance.
    /// </summary>
    /// <param name="binder">The binder that describes the requested member.</param>
    /// <param name="value">The value to assign.</param>
    /// <returns><see langword="true"/> when a writable public property with the requested name exists; otherwise <see langword="false"/>.</returns>
    public override bool TrySetMember(SetMemberBinder binder, object? value)
    {
        if (_t.GetProperty(binder.Name) is PropertyInfo p && p.CanWrite)
        {
            p.SetValue(_o, value);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Attempts to invoke a public method on the wrapped instance.
    /// </summary>
    /// <param name="binder">The binder that describes the invoked member.</param>
    /// <param name="args">The arguments passed to the method.</param>
    /// <param name="result">When this method returns <see langword="true"/>, contains the method result; otherwise <see langword="null"/>.</param>
    /// <returns><see langword="true"/> when a public method with the requested name exists; otherwise <see langword="false"/>.</returns>
    public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object? result)
    {
        if (_t.GetMethod(binder.Name) is MethodInfo m)
        {
            result = m.Invoke(_o, args);
            return true;
        }

        result = null;
        return false;
    }
}
