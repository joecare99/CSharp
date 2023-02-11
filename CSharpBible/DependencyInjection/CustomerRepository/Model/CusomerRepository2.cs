using CustomerRepository.Model;
using Microsoft.Extensions.Internal;
using System.Reflection.Metadata.Ecma335;

namespace CustomerRepositoryTests.Model
{
    public class CusomerRepository2 : ICustomerRepository
    {
        private Dictionary<Guid,CCustomer> _customers;

        public long Count => throw new NotImplementedException();

        public IClock Clock { get; }

        public ILog Log { get; }

        public CusomerRepository2()
        {
            var clockFactory = new CClockFactory();
            Clock = clockFactory.Get();

            var logFactory = new CLogFactory();
            Log = logFactory.Get();
        }

        public CCustomer Get(Guid id)
        {
            CCustomer result;
            if(!_customers.TryGetValue(id, out result))
            {
                Log.Error(Clock.Now, "no such customer");
                throw new ArgumentException("no such customer");
            }
            return result;
        }

        public Guid Put(CCustomer customer)
        {
            throw new NotImplementedException();
        }


    }
}