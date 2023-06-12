using System.Drawing;

namespace CreateCards.Model
{
    public class CardDrawDef
    {
        const float hOffs= 0.075f;
        const float vOffs = 0.07f;
        public string PrintValue;
        public double ValSize=0.1d;
        public double SuitSize;
        public PointF[] PntVals = new[] { new PointF(hOffs, vOffs), new PointF(hOffs, 1f-vOffs), new PointF(1f-hOffs, vOffs), new PointF(1f-hOffs, 1f- vOffs) };
        public PointF[] PntSVals = new[] { new PointF(hOffs, vOffs*2), new PointF(hOffs, 1f - vOffs*2), new PointF(1f-hOffs, vOffs*2), new PointF(1f-hOffs, 1f - vOffs*2) };
        public PointF[] PntSuits = { };
        public CardDrawDef(string _pv,double _ss=0.2d) { 
            PrintValue = _pv; SuitSize = _ss;
        }
    }
}