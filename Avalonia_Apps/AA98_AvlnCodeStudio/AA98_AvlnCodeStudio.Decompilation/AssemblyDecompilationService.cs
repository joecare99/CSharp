using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Code.Navigation;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;

namespace AA98_AvlnCodeStudio.Decompilation;

/// <summary>
/// Reads managed PE metadata and writes explicitly requested decompilations to an isolated cache.
/// </summary>
public sealed class AssemblyDecompilationService : IAssemblyDecompilationService
{
    private const long MaximumAssemblyLength = 512L * 1024 * 1024;
    private const int MaximumDecompiledSourceLength = 16 * 1024 * 1024;
    private const int MaximumTypeCount = 50_000;
    private const int MaximumMembersPerType = 25_000;
    private static readonly UTF8Encoding s_utf8WithoutBom = new(false);

    private readonly string _artifactRoot;

    /// <summary>Creates a service whose generated files are confined to <paramref name="artifactRoot"/>.</summary>
    public AssemblyDecompilationService(string artifactRoot)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(artifactRoot);
        _artifactRoot = Path.GetFullPath(artifactRoot);
    }

    /// <summary>Gets the default per-user cache root, outside the project workspace.</summary>
    public static string GetDefaultArtifactRoot()
    {
        string localData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        if (string.IsNullOrWhiteSpace(localData))
        {
            string home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            if (string.IsNullOrWhiteSpace(home))
            {
                throw new InvalidOperationException("The operating system did not provide a per-user data directory.");
            }

            localData = Path.Combine(home, ".local", "share");
        }

        return Path.Combine(localData, "AA98_AvlnCodeStudio", "Decompilation");
    }

    /// <inheritdoc/>
    public Task<AssemblyInspection> InspectAsync(string assemblyPath, CancellationToken cancellationToken = default)
    {
        string normalizedPath = NormalizeAssemblyPath(assemblyPath);
        return Task.Run(() => Inspect(normalizedPath, cancellationToken), cancellationToken);
    }

    /// <inheritdoc/>
    public Task<DecompiledTypeArtifact> DecompileTypeAsync(
        string assemblyPath,
        string typeFullName,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(typeFullName);
        string normalizedPath = NormalizeAssemblyPath(assemblyPath);
        return Task.Run(
            () => DecompileType(normalizedPath, typeFullName, cancellationToken),
            cancellationToken);
    }

    private static string NormalizeAssemblyPath(string assemblyPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(assemblyPath);
        string normalizedPath = Path.GetFullPath(assemblyPath);
        FileInfo file = new(normalizedPath);
        if (!file.Exists)
        {
            throw new FileNotFoundException("The assembly file does not exist.", normalizedPath);
        }

        if (file.Length == 0 || file.Length > MaximumAssemblyLength)
        {
            throw new InvalidDataException($"The assembly must be between 1 byte and {MaximumAssemblyLength} bytes.");
        }

        return normalizedPath;
    }

    private static AssemblyInspection Inspect(string assemblyPath, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        using FileStream stream = new(assemblyPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        using PEReader peReader = new(stream, PEStreamOptions.LeaveOpen);
        if (!peReader.HasMetadata)
        {
            throw new BadImageFormatException("The file is not a managed assembly.", assemblyPath);
        }

        MetadataReader reader = peReader.GetMetadataReader();
        if (!reader.IsAssembly)
        {
            throw new BadImageFormatException("The managed PE does not contain an assembly manifest.", assemblyPath);
        }

        AssemblyDefinition assembly = reader.GetAssemblyDefinition();
        List<AssemblyTypeInfo> types = [];
        foreach (TypeDefinitionHandle handle in reader.TypeDefinitions)
        {
            cancellationToken.ThrowIfCancellationRequested();
            TypeDefinition type = reader.GetTypeDefinition(handle);
            string name = reader.GetString(type.Name);
            if (name == "<Module>")
            {
                continue;
            }

            if (types.Count >= MaximumTypeCount)
            {
                throw new InvalidDataException($"The assembly contains more than {MaximumTypeCount} types.");
            }

            if (type.GetMethods().Count + type.GetFields().Count + type.GetProperties().Count > MaximumMembersPerType)
            {
                throw new InvalidDataException($"The type '{GetFullTypeName(reader, handle)}' has more than {MaximumMembersPerType} members.");
            }

            types.Add(new AssemblyTypeInfo(
                GetFullTypeName(reader, handle),
                name,
                MetadataTokens.GetToken(handle),
                IsPublic(type.Attributes),
                type.GetMethods().Select(method => reader.GetString(reader.GetMethodDefinition(method).Name)).ToArray(),
                type.GetFields().Select(field => reader.GetString(reader.GetFieldDefinition(field).Name)).ToArray(),
                type.GetProperties().Select(property => reader.GetString(reader.GetPropertyDefinition(property).Name)).ToArray()));
        }

        types.Sort(static (left, right) => StringComparer.Ordinal.Compare(left.FullName, right.FullName));
        return new AssemblyInspection(
            assemblyPath,
            reader.GetString(assembly.Name),
            assembly.Version.ToString(),
            types);
    }

    private DecompiledTypeArtifact DecompileType(
        string assemblyPath,
        string typeFullName,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string assemblyHash = ComputeFileHash(assemblyPath, cancellationToken);
        TypeDefinitionHandle typeHandle = FindTypeDefinition(assemblyPath, typeFullName, cancellationToken);
        CSharpDecompiler decompiler = new(assemblyPath, new DecompilerSettings());
        decompiler.CancellationToken = cancellationToken;
        string source = decompiler.DecompileTypesAsString([typeHandle]);
        if (string.IsNullOrWhiteSpace(source))
        {
            throw new InvalidDataException("The selected type did not produce source text.");
        }

        if (source.Length > MaximumDecompiledSourceLength)
        {
            throw new InvalidDataException($"The decompiled type exceeds the {MaximumDecompiledSourceLength}-character output limit.");
        }

        cancellationToken.ThrowIfCancellationRequested();
        if (!string.Equals(assemblyHash, ComputeFileHash(assemblyPath, cancellationToken), StringComparison.Ordinal))
        {
            throw new IOException("The assembly changed while it was being decompiled; no artifact was written.");
        }

        EnsureArtifactRootIsSeparateFromAssembly(assemblyPath);
        string sourceHash = Convert.ToHexString(SHA256.HashData(s_utf8WithoutBom.GetBytes(source))).ToLowerInvariant();
        string typeToken = MetadataTokens.GetToken(typeHandle).ToString("x8");
        string assemblyDirectory = Path.Combine(_artifactRoot, "decompilation", assemblyHash);
        string fileName = $"{SanitizeFileName(typeFullName)}-{typeToken}-{sourceHash[..16]}.cs";
        string artifactPath = Path.GetFullPath(Path.Combine(assemblyDirectory, fileName));
        EnsureContained(Path.Combine(_artifactRoot, "decompilation"), artifactPath);
        WriteArtifact(assemblyDirectory, artifactPath, source, cancellationToken);

        int line = FindTypeDeclarationLine(source, FindSimpleTypeName(typeFullName));
        return new DecompiledTypeArtifact(
            assemblyPath,
            typeFullName,
            artifactPath,
            source,
            new CodeLocation(artifactPath, line, 1));
    }

    private static TypeDefinitionHandle FindTypeDefinition(
        string assemblyPath,
        string typeFullName,
        CancellationToken cancellationToken)
    {
        using FileStream stream = new(assemblyPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        using PEReader peReader = new(stream, PEStreamOptions.LeaveOpen);
        MetadataReader reader = peReader.GetMetadataReader();
        TypeDefinitionHandle match = default;
        foreach (TypeDefinitionHandle handle in reader.TypeDefinitions)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!string.Equals(GetFullTypeName(reader, handle), typeFullName, StringComparison.Ordinal))
            {
                continue;
            }

            if (!match.IsNil)
            {
                throw new InvalidDataException($"The type name '{typeFullName}' is ambiguous in the assembly.");
            }

            match = handle;
        }

        return !match.IsNil
            ? match
            : throw new KeyNotFoundException($"The assembly does not contain the type '{typeFullName}'.");
    }

    private static string GetFullTypeName(MetadataReader reader, TypeDefinitionHandle handle)
    {
        TypeDefinition type = reader.GetTypeDefinition(handle);
        string name = reader.GetString(type.Name);
        TypeDefinitionHandle declaringType = type.GetDeclaringType();
        if (!declaringType.IsNil)
        {
            return $"{GetFullTypeName(reader, declaringType)}+{name}";
        }

        string typeNamespace = reader.GetString(type.Namespace);
        return typeNamespace.Length == 0 ? name : $"{typeNamespace}.{name}";
    }

    private static bool IsPublic(TypeAttributes attributes)
    {
        TypeAttributes visibility = attributes & TypeAttributes.VisibilityMask;
        return visibility is TypeAttributes.Public or TypeAttributes.NestedPublic;
    }

    private static string ComputeFileHash(string path, CancellationToken cancellationToken)
    {
        using FileStream stream = new(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        if (stream.Length == 0 || stream.Length > MaximumAssemblyLength)
        {
            throw new InvalidDataException($"The assembly must be between 1 byte and {MaximumAssemblyLength} bytes.");
        }

        using IncrementalHash hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
        byte[] buffer = new byte[64 * 1024];
        int count;
        while ((count = stream.Read(buffer, 0, buffer.Length)) > 0)
        {
            cancellationToken.ThrowIfCancellationRequested();
            hash.AppendData(buffer, 0, count);
        }

        return Convert.ToHexString(hash.GetHashAndReset()).ToLowerInvariant();
    }

    private void EnsureArtifactRootIsSeparateFromAssembly(string assemblyPath)
    {
        string assemblyDirectory = Path.GetDirectoryName(assemblyPath)!;
        if (IsWithin(assemblyDirectory, _artifactRoot) || IsWithin(_artifactRoot, assemblyDirectory))
        {
            throw new InvalidOperationException("The artifact root must be separate from the assembly directory.");
        }

        EnsureNoReparsePoints(_artifactRoot);
    }

    private void WriteArtifact(
        string artifactDirectory,
        string artifactPath,
        string source,
        CancellationToken cancellationToken)
    {
        string decompilationRoot = Path.Combine(_artifactRoot, "decompilation");
        EnsureContained(decompilationRoot, artifactDirectory);
        EnsureContained(decompilationRoot, artifactPath);
        EnsureNoReparsePoints(artifactDirectory);
        Directory.CreateDirectory(artifactDirectory);
        EnsureNoReparsePoints(artifactDirectory);

        if ((File.Exists(artifactPath) || Directory.Exists(artifactPath))
            && (File.GetAttributes(artifactPath) & FileAttributes.ReparsePoint) != 0)
        {
            throw new IOException("The artifact file cannot be a symbolic link or reparse point.");
        }

        if (File.Exists(artifactPath))
        {
            VerifyExistingArtifact(artifactPath, source);
            return;
        }

        string temporaryPath = Path.Combine(artifactDirectory, $".{Guid.NewGuid():N}.tmp");
        try
        {
            using (FileStream stream = new(
                temporaryPath,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None,
                bufferSize: 64 * 1024,
                FileOptions.Asynchronous | FileOptions.WriteThrough))
            using (StreamWriter writer = new(stream, s_utf8WithoutBom))
            {
                writer.WriteAsync(source.AsMemory(), cancellationToken).GetAwaiter().GetResult();
                writer.FlushAsync(cancellationToken).GetAwaiter().GetResult();
                stream.Flush(flushToDisk: true);
            }

            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                File.Move(temporaryPath, artifactPath, overwrite: false);
            }
            catch (IOException) when (File.Exists(artifactPath))
            {
                VerifyExistingArtifact(artifactPath, source);
            }
        }
        finally
        {
            if (File.Exists(temporaryPath))
            {
                File.Delete(temporaryPath);
            }
        }
    }

    private static void VerifyExistingArtifact(string artifactPath, string source)
    {
        if (!string.Equals(File.ReadAllText(artifactPath, s_utf8WithoutBom), source, StringComparison.Ordinal))
        {
            throw new IOException("An existing file conflicts with the generated decompilation artifact.");
        }
    }

    private static void EnsureNoReparsePoints(string path)
    {
        string fullPath = Path.GetFullPath(path);
        string? current = Path.GetPathRoot(fullPath);
        if (string.IsNullOrEmpty(current))
        {
            throw new IOException("The artifact path does not have a filesystem root.");
        }

        foreach (string segment in fullPath[current.Length..]
                     .Split([Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar], StringSplitOptions.RemoveEmptyEntries))
        {
            current = Path.Combine(current, segment);
            FileAttributes attributes;
            try
            {
                attributes = File.GetAttributes(current);
            }
            catch (FileNotFoundException)
            {
                continue;
            }
            catch (DirectoryNotFoundException)
            {
                continue;
            }

            if ((attributes & FileAttributes.ReparsePoint) != 0)
            {
                throw new IOException("The artifact root cannot contain symbolic links or reparse points.");
            }
        }
    }

    private static void EnsureContained(string root, string path)
    {
        string relative = Path.GetRelativePath(Path.GetFullPath(root), Path.GetFullPath(path));
        if (Path.IsPathRooted(relative)
            || relative == ".."
            || relative.StartsWith($"..{Path.DirectorySeparatorChar}", PathComparison)
            || relative.StartsWith($"..{Path.AltDirectorySeparatorChar}", PathComparison))
        {
            throw new IOException("The artifact path resolves outside the configured artifact root.");
        }
    }

    private static bool IsWithin(string root, string path)
    {
        string relative = Path.GetRelativePath(Path.GetFullPath(root), Path.GetFullPath(path));
        return !Path.IsPathRooted(relative)
            && relative != ".."
            && !relative.StartsWith($"..{Path.DirectorySeparatorChar}", PathComparison)
            && !relative.StartsWith($"..{Path.AltDirectorySeparatorChar}", PathComparison);
    }

    private static string SanitizeFileName(string typeFullName)
    {
        StringBuilder builder = new(Math.Min(typeFullName.Length, 96));
        foreach (char character in typeFullName)
        {
            builder.Append(char.IsAsciiLetterOrDigit(character) || character is '.' or '-' or '_'
                ? character
                : '_');
            if (builder.Length >= 96)
            {
                break;
            }
        }

        return builder.Length == 0 ? "type" : builder.ToString();
    }

    private static int FindTypeDeclarationLine(string source, string simpleTypeName)
    {
        Regex declaration = new(
            $@"\b(?:class|struct|interface|enum|record|delegate)\s+@?{Regex.Escape(simpleTypeName)}\b",
            RegexOptions.CultureInvariant);
        using StringReader reader = new(source);
        int lineNumber = 0;
        string? line;
        while ((line = reader.ReadLine()) is not null)
        {
            lineNumber++;
            if (declaration.IsMatch(line))
            {
                return lineNumber;
            }
        }

        return 1;
    }

    private static string FindSimpleTypeName(string typeFullName)
    {
        int separator = Math.Max(typeFullName.LastIndexOf('.'), typeFullName.LastIndexOf('+'));
        string name = typeFullName[(separator + 1)..];
        int arity = name.IndexOf('`');
        return arity >= 0 ? name[..arity] : name;
    }

    private static StringComparison PathComparison =>
        OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
}
