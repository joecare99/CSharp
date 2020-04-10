using System;

namespace DataGridExWPF.Worker
{
    //Defines the customer object
    public class Customer
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Uri Email { get; set; }
        public bool IsMember { get; set; }
        public OrderStatus Status { get; set; }
    }
}
