namespace CustomerRepository.Model
{
    public interface ICustomerRepository
    {
        CCustomer? Get(Guid id);
        Guid Put(CCustomer customer);
        long Count { get;}
     }
}