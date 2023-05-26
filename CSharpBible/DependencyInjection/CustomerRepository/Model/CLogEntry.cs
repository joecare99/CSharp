using System;

namespace CustomerRepository.Model
{
    public class CLogEntry
    {
        public DateTimeOffset Time { get; set; }= new DateTimeOffset(new DateTime(2000,1,1));
        public string Message { get; set; } = "";
    }
}