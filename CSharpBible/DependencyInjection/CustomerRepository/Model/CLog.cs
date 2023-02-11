using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace CustomerRepository.Model
{
    public class CLog : ILog
    {
        public void Error(DateTimeOffset time, string message)
        {
            // Todo:
        }

        public IEnumerable<CLogEntry> Get() { return null }
    }
}
