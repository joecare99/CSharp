namespace CustomerRepository.Model
{
    public interface ICustomerRepository2:ICustomerRepository
    {
        ILog Log { get; }
        IClock Clock { get; }
     }
}