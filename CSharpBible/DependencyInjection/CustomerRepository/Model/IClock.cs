using System;

namespace CustomerRepository.Model
{
    public interface IClock
    {
        DateTime Now { get; }
    }
}
