using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenFree.Interfaces.Model
{
    public interface INB_Person : IUsesRecordset<int>, IUsesID<int>
    {
        void Append(int persInArb, bool xAppenWitt = true);
    }
}
