using CustomerRepository.Model;

namespace CustomerRepositoryTests.Model
{
    public class CLogFactory
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