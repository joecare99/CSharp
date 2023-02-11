using CustomerRepository.Model;
using Microsoft.Extensions.Internal;
using System.Reflection.Metadata.Ecma335;

namespace CustomerRepositoryTests.Model
{
    public class CusomerRepository1 : ICustomerRepository
    {
        private Dictionary<Guid,CCustomer> _customers;

        public long Count => throw new NotImplementedException();

        public SystemClock Clock { get; }

        public CLog Log { get; }

        public CusomerRepository1()
        {
            Clock = new SystemClock();
            Log = new CLog();
        }
        public CCustomer Get(Guid id)
        {
            CCustomer result;
            if(!_customers.TryGetValue(id, out result))
            {
                Log.Error(Clock.UtcNow,"no such customer")
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