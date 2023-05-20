using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM_36_ComToolKtSavesWork.Models
{
    public class UserRepository : IUserRepository
    {
        public User? Login(string username, string password) => new() { Username = "DevDave", Id = 1 };
    }
}
