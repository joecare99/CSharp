using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using TranspilerLib.Data;
using TranspilerLib.Interfaces.Code;
using TranspilerLib.Models.Scanner;
using TranspilerLib.Pascal.Models.Scanner;

#pragma warning disable IDE0130
namespace TranspilerLib.Pascal.Helper;

// Spezialisierte Registrierung für PasCodeBlock (Beibehaltung des alten Namens zur Kompatibilität).
public sealed class ICodeBlockPasCodeBlockConverter : GenericInterfaceConverter<ICodeBlock, PasCodeBlock>
{
    // Optional: nur relevante Properties explizit angeben (ansonsten alle).
    public ICodeBlockPasCodeBlockConverter() : base("Name", "Code", "Type", "SubBlocks") { }
}
