using System;
using MVVM_38_CTDependencyInjection.Models.Interfaces;
using System.Collections.Generic;

namespace MVVM_38_CTDependencyInjection.Models
{
    public class UserRepository : IUserRepository
    {
        private ILog _log;

        public Guid GetUser(string name, string password) =>
            // Dummy-Implement
            name.ToUpper() switch
            {
                "DAVE" =>  new Guid("01234567-89AB-CDEF-0123-456789ABCDEF"),
                "JOE" =>   new Guid("11234567-89AB-CDEF-0123-456789ABCDEF"),
                "PETER" => new Guid("21234567-89AB-CDEF-0123-456789ABCDEF"),
                _ => Guid.Empty
            };

        public IEnumerable<string> GetUsers() =>
            new[] { "Dave", "Joe", "Peter" };

        public EPermission GetPermission(Guid id,Guid action) {
            return EPermission.All;
        }

        public UserRepository(ILog log) {
            _log = log;
        }
    }
}
