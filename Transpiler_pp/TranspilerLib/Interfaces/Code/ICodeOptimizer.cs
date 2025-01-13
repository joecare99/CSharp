namespace TranspilerLib.Interfaces.Code
{
    public interface ICodeOptimizer
    {
        bool _noWhile { get; set; }

        void TestItem(ICodeBlock item);
    }
}