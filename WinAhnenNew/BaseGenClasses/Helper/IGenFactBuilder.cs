using GenInterfaces.Data;
using GenInterfaces.Interfaces.Genealogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseGenClasses.Helper;

public interface IGenFactBuilder
{
    IGenFact Emit(EFactType type, IGenEntity mainEnt, string data, bool xReplace = true);
    IGenFact Emit(EFactType type, IGenEntity mainEnt, IGenDate date, string data, bool xReplace = true);
    IGenFact Emit(EFactType type, IGenEntity mainEnt, IGenDate date,IGenPlace place, string data, bool xReplace = true);
}
