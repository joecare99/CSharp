using System.Collections.Generic;
using RnzTrauer.Core.Domain;

namespace RnzTrauer.Core.Services;

/// <summary>Extracts reviewable facts from OCR/PDF text without performing persistence.</summary>
public interface INoticeTextParser
{
    ParsedNoticeFacts Parse(DeathNotice notice, string text, IReadOnlyCollection<string> placeNames);
}
