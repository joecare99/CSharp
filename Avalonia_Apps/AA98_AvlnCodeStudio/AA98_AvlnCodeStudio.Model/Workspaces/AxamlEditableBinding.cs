using System;
using System.Linq;

namespace AA98_AvlnCodeStudio.Model.Workspaces;

public static class AxamlEditableBinding
{
    public static string? ReadPath(
        string? expression,
        out AxamlBindingMode bindingMode,
        out AxamlBindingUpdateSourceTrigger updateSourceTrigger)
    {
        bindingMode = AxamlBindingMode.Default;
        updateSourceTrigger = AxamlBindingUpdateSourceTrigger.Default;
        if (string.IsNullOrWhiteSpace(expression))
        {
            return null;
        }

        string value = expression.Trim();
        string? kind = new[] { "Binding", "CompiledBinding", "ReflectionBinding" }
            .FirstOrDefault(name => value.StartsWith("{" + name, StringComparison.Ordinal)
                && value.Length > name.Length + 1
                && (char.IsWhiteSpace(value[name.Length + 1]) || value[name.Length + 1] == ','));
        string extensionName = "{" + kind;
        if (kind is null
            || !value.EndsWith('}')
            || value.Length > extensionName.Length
                && !char.IsWhiteSpace(value[extensionName.Length])
                && value[extensionName.Length] != ',')
        {
            return null;
        }

        string arguments = value[extensionName.Length..^1].Trim();
        if (arguments.StartsWith(','))
        {
            arguments = arguments[1..];
        }

        string[] tokens = arguments
            .Split(',')
            .Select(static token => token.Trim())
            .ToArray();
        string? path = null;
        bool modeSpecified = false;
        bool updateSourceTriggerSpecified = false;
        bool namedArgumentSeen = false;
        foreach (string token in tokens)
        {
            if (token.Length == 0)
            {
                return null;
            }

            int separator = token.IndexOf('=');
            if (separator < 0)
            {
                if (path is not null || namedArgumentSeen || !AxamlMvvmMemberName.IsValid(token))
                {
                    return null;
                }

                path = token;
                continue;
            }

            namedArgumentSeen = true;
            string optionName = token[..separator].Trim();
            string optionValue = token[(separator + 1)..].Trim();
            if (string.Equals(optionName, "Path", StringComparison.Ordinal))
            {
                if (path is not null || !AxamlMvvmMemberName.IsValid(optionValue))
                {
                    return null;
                }

                path = optionValue;
            }
            else if (string.Equals(optionName, "Mode", StringComparison.Ordinal))
            {
                if (modeSpecified
                    || !Enum.GetNames<AxamlBindingMode>().Contains(optionValue, StringComparer.OrdinalIgnoreCase)
                    || !Enum.TryParse(optionValue, ignoreCase: true, out AxamlBindingMode parsedMode)
                    || !Enum.IsDefined(parsedMode))
                {
                    return null;
                }

                bindingMode = parsedMode;
                modeSpecified = true;
            }
            else if (string.Equals(optionName, "UpdateSourceTrigger", StringComparison.Ordinal))
            {
                if (updateSourceTriggerSpecified
                    || !Enum.GetNames<AxamlBindingUpdateSourceTrigger>().Contains(optionValue, StringComparer.OrdinalIgnoreCase)
                    || !Enum.TryParse(
                        optionValue,
                        ignoreCase: true,
                        out AxamlBindingUpdateSourceTrigger parsedTrigger)
                    || !Enum.IsDefined(parsedTrigger))
                {
                    return null;
                }

                updateSourceTrigger = parsedTrigger;
                updateSourceTriggerSpecified = true;
            }
            else
            {
                return null;
            }
        }

        return path;
    }

}
