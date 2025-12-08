using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Document.Base.Models.Interfaces;
using Document.Base.Registration;

namespace Document.Base.Factories;

public static class UserDocumentFactory
{
    private static readonly ConcurrentDictionary<string, Func<IUserDocument>> _byKey =
        new(StringComparer.OrdinalIgnoreCase);

    private static readonly ConcurrentDictionary<string, string> _extToKey =
        new(StringComparer.OrdinalIgnoreCase);

    private static readonly ConcurrentDictionary<string, string> _ctToKey =
        new(StringComparer.OrdinalIgnoreCase);

    private static volatile bool _scanned;
    public static bool AutoScanOnFirstUse { get; set; } = true;

    // Registrierung per Code
    public static void Register(string key, Func<IUserDocument> creator,
        IEnumerable<string>? extensions = null, string? contentType = null, bool overwrite = false)
    {
        if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("Key darf nicht leer sein.", nameof(key));
        if (creator is null) throw new ArgumentNullException(nameof(creator));

        if (!overwrite && _byKey.ContainsKey(key))
            throw new InvalidOperationException($"Key '{key}' ist bereits registriert.");

        _byKey[key] = creator;

        if (extensions != null)
        {
            foreach (var ext in extensions.Select(NormalizeExtension))
            {
                if (string.IsNullOrEmpty(ext)) continue;
                _extToKey[ext] = key;
            }
        }

        if (!string.IsNullOrWhiteSpace(contentType))
        {
            _ctToKey[contentType!] = key;
        }
    }

    public static void Register<T>(string key,
        IEnumerable<string>? extensions = null, string? contentType = null, bool overwrite = false)
        where T : IUserDocument, new()
        => Register(key, static () => new T(), extensions, contentType, overwrite);

    public static bool Unregister(string key)
    {
        var removed = _byKey.TryRemove(key, out _);
        // Entfernungen aus ext/ct Maps (best-effort)
        foreach (var kv in _extToKey.Where(kv => string.Equals(kv.Value, key, StringComparison.OrdinalIgnoreCase)).ToList())
            _extToKey.TryRemove(kv.Key, out _);
        foreach (var kv in _ctToKey.Where(kv => string.Equals(kv.Value, key, StringComparison.OrdinalIgnoreCase)).ToList())
            _ctToKey.TryRemove(kv.Key, out _);
        return removed;
    }

    public static IReadOnlyCollection<string> RegisteredKeys
        => _byKey.Keys.ToArray();

    // Erzeugung
    public static IUserDocument Create(string keyOrExtOrContentTypeOrPath)
    {
        if (string.IsNullOrWhiteSpace(keyOrExtOrContentTypeOrPath))
            throw new ArgumentException("Eingabe darf nicht leer sein.", nameof(keyOrExtOrContentTypeOrPath));

        EnsureScanned();

        // Pfad?
        if (LooksLikePath(keyOrExtOrContentTypeOrPath))
        {
            var doc = CreateForPath(keyOrExtOrContentTypeOrPath);
            if (doc is not null) return doc;
        }

        // Content-Type?
        if (keyOrExtOrContentTypeOrPath.Contains('/'))
        {
            var doc = CreateForContentType(keyOrExtOrContentTypeOrPath);
            if (doc is not null) return doc;
        }

        // Extension?
        if (keyOrExtOrContentTypeOrPath.StartsWith('.'))
        {
            var doc = CreateForExtension(keyOrExtOrContentTypeOrPath);
            if (doc is not null) return doc;
        }

        // Key
        if (_byKey.TryGetValue(keyOrExtOrContentTypeOrPath, out var factory))
            return factory();

        throw new KeyNotFoundException($"Kein Dokument für '{keyOrExtOrContentTypeOrPath}' registriert.");
    }

    public static bool TryCreate(string keyOrExtOrContentTypeOrPath, out IUserDocument? doc)
    {
        try
        {
            doc = Create(keyOrExtOrContentTypeOrPath);
            return true;
        }
        catch
        {
            doc = null;
            return false;
        }
    }

    public static IUserDocument? CreateForExtension(string extension)
    {
        EnsureScanned();
        var ext = NormalizeExtension(extension);
        if (string.IsNullOrEmpty(ext)) return null;
        return _extToKey.TryGetValue(ext, out var key) && _byKey.TryGetValue(key, out var factory)
            ? factory()
            : null;
    }

    public static IUserDocument? CreateForContentType(string contentType)
    {
        EnsureScanned();
        if (string.IsNullOrWhiteSpace(contentType)) return null;
        return _ctToKey.TryGetValue(contentType, out var key) && _byKey.TryGetValue(key, out var factory)
            ? factory()
            : null;
    }

    public static IUserDocument? CreateForPath(string path)
    {
        EnsureScanned();
        var ext = Path.GetExtension(path);
        return string.IsNullOrEmpty(ext) ? null : CreateForExtension(ext);
    }

    // Assembly-Scan (Attribute)
    public static void ScanAssemblies(IEnumerable<Assembly>? assemblies = null)
    {
        assemblies ??= AppDomain.CurrentDomain.GetAssemblies();

        foreach (var asm in assemblies)
        {
            Type[] types;
            try { types = asm.GetTypes(); }
            catch { continue; }

            foreach (var t in types)
            {
                if (!typeof(IUserDocument).IsAssignableFrom(t) || t.IsAbstract || t.IsInterface)
                    continue;

                var attrs = t.GetCustomAttributes<UserDocumentProviderAttribute>(inherit: false).ToArray();
                if (attrs.Length == 0) continue;

                // Erzeuge Creator
                Func<IUserDocument> creator = () => (IUserDocument)Activator.CreateInstance(t)!;

                foreach (var a in attrs)
                {
                    var keys = (a.Keys?.Length ?? 0) > 0 ? a.Keys! : Array.Empty<string>();
                    foreach (var key in keys)
                    {
                        if (string.IsNullOrWhiteSpace(key)) continue;
                        // Mehrere Attribute können denselben Key definieren – letztes gewinnt.
                        _byKey[key] = creator;
                    }

                    if (a.Extensions is { Length: > 0 })
                    {
                        foreach (var ext in a.Extensions)
                        {
                            var n = NormalizeExtension(ext);
                            if (!string.IsNullOrEmpty(n) && keys.FirstOrDefault() is { } firstKey)
                                _extToKey[n] = firstKey;
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(a.ContentType) && keys.FirstOrDefault() is { } k)
                    {
                        _ctToKey[a.ContentType!] = k;
                    }
                }
            }
        }
    }

    private static void EnsureScanned()
    {
        if (_scanned) return;
        if (AutoScanOnFirstUse)
        {
            ScanAssemblies();
        }
        _scanned = true;
    }

    private static string NormalizeExtension(string extension)
    {
        if (string.IsNullOrWhiteSpace(extension)) return string.Empty;
        var ext = extension.Trim();
        if (!ext.StartsWith('.')) ext = "." + ext;
        return ext.ToLowerInvariant();
    }

    private static bool LooksLikePath(string input)
    {
        // Einfache Heuristik: enthält Verzeichnungs-Trenner oder Punkt mit möglicher Endung
        return input.Contains('\\') || input.Contains('/') || input.EndsWith(".html", StringComparison.OrdinalIgnoreCase)
               || input.EndsWith(".htm", StringComparison.OrdinalIgnoreCase) || input.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase);
    }
}