using GenInterfaces.Data;
using GenInterfaces.Interfaces.Genealogic;

namespace BaseGenClasses.Helper;

public interface IGenConnectBuilder
{
    IGenConnects Emit(EGenConnectionType type, IGenPerson person);
}