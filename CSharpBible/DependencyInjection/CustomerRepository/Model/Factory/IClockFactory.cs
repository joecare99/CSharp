using CustomerRepository.Model;

namespace CustomerRepository.Model.Factory
{
    public interface IClockFactory
    {
        IClock Get();
    }
}