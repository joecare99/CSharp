using System.Collections.Generic;
using System.Linq;

namespace AGVFkt.Model
{
    public class FB_MAVG
    {
        List<double> _adVal;
        int _iMax = 0;
        int _iBuffer = 0;

        public FB_MAVG(int iMax)
        {
            _iMax = iMax;
            _adVal = new List<double>(_iMax);
            foreach (var i in Enumerable.Range(0, _iMax))
                _adVal.Add(0d);
        }

        public double Calc(double X,uint n,bool xReset)
        {
            if (xReset)
            {
                _iBuffer = 0;
                for (int i = 0; i < _iMax; i++)
                    _adVal[i] = X;
            }
            else
            {
                _adVal[_iBuffer] = X;
                _iBuffer++;
                if (_iBuffer >= _iMax)
                    _iBuffer = 0;
            }
            double dSum = 0;
            for (int i = 0; i < _iMax; i++)
                dSum += _adVal[i];
            return dSum / _iMax;
        }

    }
}