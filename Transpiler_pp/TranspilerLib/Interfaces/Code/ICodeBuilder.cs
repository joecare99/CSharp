using TranspilerLib.Data;

namespace TranspilerLib.Interfaces.Code
{
    public interface ICodeBuilder
    {

        ICodeBuilderData NewData(ICodeBlock block);
        void OnToken(TokenData tokenData, ICodeBuilderData data);
    }
}