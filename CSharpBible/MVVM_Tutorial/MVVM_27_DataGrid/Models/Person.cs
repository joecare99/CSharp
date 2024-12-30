using System;

namespace MVVM_27_DataGrid.Models
{
    public class Person
    {
        public int Id { get; set; } = -1;
        public string? FirstName { get; set; } = null;
        public string? LastName { get; set; } = null;
        public string? Email { get; set; } = null;
        public DateTime? Birthday { get; set; } =null;
        public Department? Department { get; set; } = null;
    }
}
