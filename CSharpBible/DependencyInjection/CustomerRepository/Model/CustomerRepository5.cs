﻿using CustomerRepository.Model;

namespace CustomerRepositoryTests.Model
{
    /// <summary>Class CusomerRepository5.
    /// Implements the <see cref="ICustomerRepository" /></summary>
    /// <remarks>This repository uses <b>Dependency Injection</b></remarks>
    /// <autogeneratedoc />
    public class CustomerRepository5 : ICustomerRepository
    {
        #region Properties
        #region private properties
        private readonly Dictionary<Guid,CCustomer> _customers = new Dictionary<Guid, CCustomer>();
        private IClock _Clock { get; }
        private ILog _Log { get; }
        #endregion

        public long Count => throw new NotImplementedException();
        #endregion

        #region Methods
        public CustomerRepository5(IClock clock,ILog log)
        {
            _Clock = clock;
            _Log = log;
        }

        public CCustomer? Get(Guid id)
        {
            if (!_customers.TryGetValue(id, out CCustomer? result))
            {
                _Log.Error(_Clock.Now, "no such customer");
                throw new ArgumentException("no such customer");
            }
            return result;
        }

        public Guid Put(CCustomer customer)
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}