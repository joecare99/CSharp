using System.Drawing;

namespace CreateCards.Model
{
    public struct CardDrawDef
    {
        public string PrintValue;
        public double SuitSize;
        public double ValSize;
        public PointF[] PntVals;
        public PointF[] PntSuits;
    }
}