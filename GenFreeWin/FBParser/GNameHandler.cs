using System.Text;

namespace FBParser;

/// <summary>
/// Provides given-name based sex guessing and learning logic used by the parser.
/// </summary>
public sealed class GNameHandler
{
    /// <summary>
    /// Represents a diagnostic callback raised by the given-name handler.
    /// </summary>
    /// <param name="message">The diagnostic message.</param>
    /// <param name="type">The original message type code.</param>
    public delegate void SendMessage(string message, int type);

    private const string Unknown = "�";
    private const string Unknown2 = "...";

    private readonly SortedDictionary<string, string> _gNameList = new(StringComparer.Ordinal);
    private bool _cfgLearnUnknown = true;
    private string _gNameFile = string.Empty;
    private bool _gNameListChanged;
    private SendMessage? _onError;

    /// <summary>
    /// Gets or sets the error callback.
    /// </summary>
    public SendMessage? OnError
    {
        get => _onError;
        set => _onError = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether unknown names should be learned with the placeholder sex marker.
    /// </summary>
    public bool CfgLearnUnknown
    {
        get => _cfgLearnUnknown;
        set => _cfgLearnUnknown = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the internal name list changed.
    /// </summary>
    public bool Changed
    {
        get => _gNameListChanged;
        set => _gNameListChanged = value;
    }

    /// <summary>
    /// Initializes the handler state.
    /// </summary>
    public void Init()
    {
        _gNameList.Clear();
        _cfgLearnUnknown = true;
        _gNameListChanged = false;
        _gNameFile = string.Empty;
    }

    /// <summary>
    /// Finalizes the handler and persists changes when a backing file was configured.
    /// </summary>
    public void Done()
    {
        try
        {
            if (_gNameListChanged && !string.IsNullOrEmpty(_gNameFile))
            {
                if (File.Exists(_gNameFile))
                {
                    File.Delete(_gNameFile);
                }

                SafeSave(_gNameFile);
            }
        }
        finally
        {
            _gNameList.Clear();
        }
    }

    /// <summary>
    /// Loads a given-name mapping file.
    /// </summary>
    /// <param name="fileName">The source file path.</param>
    public void LoadGNameList(string fileName)
    {
        if (File.Exists(fileName))
        {
            _gNameList.Clear();
            foreach (var line in File.ReadLines(fileName, Encoding.UTF8))
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                var index = line.IndexOf('=');
                if (index < 0)
                {
                    if (!_gNameList.ContainsKey(line))
                    {
                        _gNameList[line] = string.Empty;
                    }
                }
                else
                {
                    _gNameList[line[..index]] = line[(index + 1)..];
                }
            }
        }
        else
        {
            _gNameList.Clear();
        }

        _gNameFile = fileName;
        _gNameListChanged = false;
    }

    /// <summary>
    /// Saves the given-name mapping file.
    /// </summary>
    /// <param name="fileName">The target file path, or an empty string to reuse the configured file name.</param>
    public void SaveGNameList(string fileName = "")
    {
        if (string.IsNullOrEmpty(fileName) && string.IsNullOrEmpty(_gNameFile))
        {
            return;
        }

        if (!string.IsNullOrEmpty(fileName))
        {
            _gNameFile = fileName;
        }

        SafeSave(_gNameFile);
        _gNameListChanged = false;
    }

    /// <summary>
    /// Sets the mapping file name without writing the file.
    /// </summary>
    /// <param name="fileName">The file path.</param>
    public void SetGnlFileName(string fileName = "")
    {
        if (string.IsNullOrEmpty(fileName) && string.IsNullOrEmpty(_gNameFile))
        {
            return;
        }

        if (!string.IsNullOrEmpty(fileName))
        {
            _gNameFile = fileName;
        }
    }

    /// <summary>
    /// Learns the supplied sex marker for the specified given name tokens.
    /// </summary>
    /// <param name="name">The name text.</param>
    /// <param name="sex">The sex marker.</param>
    public void LearnSexOfGivnName(string name, char sex)
    {
        foreach (var token in SplitBySpace(name))
        {
            var currentName = token;
            if (IsAbbrev(currentName))
            {
                continue;
            }

            if (TestFor(currentName, 1, Unknown) || TestFor(currentName, 1, Unknown2))
            {
                continue;
            }

            if ((currentName.Length < 3 && !currentName.Equals("NN", StringComparison.OrdinalIgnoreCase))
                || currentName.EndsWith('.', StringComparison.Ordinal)
                || currentName.EndsWith('=', StringComparison.Ordinal))
            {
                if (currentName.Length > 0)
                {
                    _onError?.Invoke($"\"{currentName}\" is not a valid Name", 0);
                }
            }
            else if (currentName.Length > 1 && char.IsLower(currentName[0]))
            {
                _onError?.Invoke($"\"{currentName}\" is not a valid Name (case)", 0);
            }
            else if (currentName.Length > 0
                && !StartsWith(currentName, "(")
                && !StartsWith(currentName, "\"")
                && (GetValueOrEmpty(currentName).Length == 0 || GetValueOrEmpty(currentName) == "_"))
            {
                _gNameList[currentName] = sex.ToString();
                _gNameListChanged = true;
            }
            else if (!StartsWith(currentName, "("))
            {
                break;
            }
        }
    }

    /// <summary>
    /// Guesses the sex marker from one or more given-name tokens.
    /// </summary>
    /// <param name="name">The given-name text.</param>
    /// <param name="learn">A value indicating whether invalid or unknown names may be learned or reported.</param>
    /// <returns>The guessed sex marker, or <c>'U'</c> when unknown.</returns>
    public char GuessSexOfGivnName(string name, bool learn = true)
    {
        const char defaultResult = 'U';

        foreach (var token in SplitBySpace(name))
        {
            var currentName = token;
            if (IsAbbrev(currentName))
            {
                continue;
            }

            if ((currentName.Length < 3 && !currentName.Equals("NN", StringComparison.OrdinalIgnoreCase))
                || currentName.EndsWith('.', StringComparison.Ordinal)
                || currentName.EndsWith('=', StringComparison.Ordinal))
            {
                if (currentName.Length > 0 && learn)
                {
                    _onError?.Invoke($"\"{currentName}\" is not a valid Name", 0);
                }
            }
            else if (!StartsWith(currentName, "(") && !StartsWith(currentName, "\""))
            {
                var value = GetValueOrEmpty(currentName);
                if (value.Length > 0 && value != "_")
                {
                    return value[0];
                }

                if (_cfgLearnUnknown && learn)
                {
                    LearnSexOfGivnName(currentName, '_');
                }
            }
        }

        return defaultResult;
    }

    private static IEnumerable<string> SplitBySpace(string text)
        => (text ?? string.Empty).Split([' '], StringSplitOptions.RemoveEmptyEntries);

    private static bool IsAbbrev(string value)
        => value.Length == 2 && value.EndsWith('.', StringComparison.Ordinal) && char.IsUpper(value[0]);

    private static bool TestFor(string text, int positionOneBased, string test)
    {
        var index = Math.Max(0, positionOneBased - 1);
        if (index + test.Length > text.Length)
        {
            return false;
        }

        return string.CompareOrdinal(text, index, test, 0, test.Length) == 0;
    }

    private static bool StartsWith(string text, string prefix)
        => text.Length > 0 && text[0].ToString() == prefix;

    private string GetValueOrEmpty(string key)
        => _gNameList.TryGetValue(key, out var value) ? value ?? string.Empty : string.Empty;

    private void SafeSave(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            return;
        }

        var directory = Path.GetDirectoryName(Path.GetFullPath(fileName));
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var tempFile = Path.GetTempFileName();
        try
        {
            using (var writer = new StreamWriter(tempFile, false, new UTF8Encoding(false)))
            {
                foreach (var pair in _gNameList)
                {
                    writer.Write(pair.Key);
                    writer.Write('=');
                    writer.WriteLine(pair.Value ?? string.Empty);
                }
            }

            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            File.Move(tempFile, fileName);
        }
        catch
        {
            try
            {
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }
            }
            catch
            {
            }

            throw;
        }
    }
}
