using IDR.Core.Models;
using IDR.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Threading;
using System.Threading.Tasks;

namespace IDR.Infrastructure.Pe;

public sealed class PeImageLoader : IPeImageLoader
{
    public async Task<PeImage> LoadAsync(string sourcePath, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(sourcePath))
        {
            throw new ArgumentException("A source path is required.", nameof(sourcePath));
        }

        byte[] image = await File.ReadAllBytesAsync(sourcePath, cancellationToken).ConfigureAwait(false);
        try
        {
            using MemoryStream stream = new(image, writable: false);
            using PEReader reader = new(stream, PEStreamOptions.LeaveOpen);
            if (!reader.HasMetadata && reader.PEHeaders.PEHeader is null)
            {
                throw new InvalidDataException("The file is not a valid PE image.");
            }

            PEHeaders headers = reader.PEHeaders;
            PEHeader peHeader = headers.PEHeader
                ?? throw new InvalidDataException("The PE optional header is missing.");

            PeSection[] sections = headers.SectionHeaders
                .Select(section => new PeSection(
                    section.Name,
                    (uint)section.VirtualAddress,
                    checked((uint)Math.Max(section.VirtualSize, section.SizeOfRawData)),
                    (uint)section.PointerToRawData,
                    (uint)section.SizeOfRawData,
                    (section.SectionCharacteristics & SectionCharacteristics.ContainsCode) != 0))
                .ToArray();

            if (sections.Length == 0)
            {
                throw new InvalidDataException("The PE image contains no sections.");
            }

            PeDataDirectoryReader directoryReader = new(image, headers.SectionHeaders.ToArray(), peHeader);
            IReadOnlyList<DelphiForm> forms = directoryReader.ReadForms(out IReadOnlyList<string> formDiagnostics);
            return new PeImage(sourcePath, image, sections, (uint)peHeader.AddressOfEntryPoint)
            {
                ImageBase = peHeader.ImageBase,
                Imports = directoryReader.ReadImports(),
                Exports = directoryReader.ReadExports(),
                ResourceStrings = directoryReader.ReadStringResources(),
                Forms = forms,
                FormDiagnostics = formDiagnostics
            };
        }
        catch (BadImageFormatException exception)
        {
            throw new InvalidDataException("The file is not a valid PE image.", exception);
        }
        catch (ArgumentException exception)
        {
            throw new InvalidDataException("The file is not a valid PE image.", exception);
        }
    }
}
