using System;
using System.Collections.Generic;

namespace CustomerRepository.Model
{
    public interface ILog
    {
        public void Error(DateTimeOffset time, string message);
        public IEnumerable<CLogEntry> Get();
    }
}