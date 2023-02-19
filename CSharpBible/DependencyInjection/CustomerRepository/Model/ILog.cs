using CustomerRepository.Model;

namespace CustomerRepository.Model
{
    public interface ILog
    {
        public void Error(DateTimeOffset time, string message);
        public IEnumerable<CLogEntry> Get();
    }
}