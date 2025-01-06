using TranspilerLib.Interfaces.Code;

namespace TranspilerLib.Models.Interpreter
{
    internal class InterpData
    {
        private ICodeBlock? next;

        public InterpData(ICodeBlock? next)
        {
            this.next = next;
        }

        public ICodeBlock pc { get; set; }
    }
}