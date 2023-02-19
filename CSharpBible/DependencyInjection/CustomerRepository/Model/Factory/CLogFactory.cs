using CustomerRepository.Model;

namespace CustomerRepository.Model.Factory
{
    public class CLogFactory : ILogFactory
    {
        public CLogFactory()
        {
        }

        public ILog Get()
        {
            return new CLog();
        }
    }
}