using System;
using GenInterfaces.Data;
using GenInterfaces.Interfaces.Genealogic;

namespace OFBCreator.Core.Models.Gedcom;

internal sealed class GedcomDate : IGenDate
{
    public Guid UId { get; init; } = Guid.NewGuid();
    public EGenType eGenType => EGenType.GenDate;
    public int ID { get; init; }
    public DateTime? LastChange => null;
    public EDateModifier eDateModifier { get; set; } = EDateModifier.Text;
    public EDateType eDateType1 { get; set; } = EDateType.Full;
    public DateTime Date1 { get; set; }
    public EDateType? eDateType2 { get; set; }
    public DateTime? Date2 { get; set; }
    public string? DateText { get; set; }

    public override string ToString()
    {
        return DateText ?? Date1.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
    }
}
