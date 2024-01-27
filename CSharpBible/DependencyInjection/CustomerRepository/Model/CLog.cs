using System;
using System.Collections.Generic;

namespace CustomerRepository.Model
{
    public class CLog : ILog
    {
        private List<CLogEntry> _entries = new List<CLogEntry>();

        public void Error(DateTimeOffset time, string message)
        {
            _entries.Add(new CLogEntry() { Time = time, Message = message });
        }

        public IEnumerable<CLogEntry> Get() => _entries;
    }
}
