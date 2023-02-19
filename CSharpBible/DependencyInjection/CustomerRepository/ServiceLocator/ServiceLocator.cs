using CustomerRepository.Model;

namespace CustomerRepository.ServiceLocator
{
    public static class ServiceLocator
    {
        public static IClock Clock { get; set; }
        public static ILog Log { get; set; }
    }
}