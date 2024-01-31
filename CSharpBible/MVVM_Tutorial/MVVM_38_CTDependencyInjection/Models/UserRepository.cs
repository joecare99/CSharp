using System;
using MVVM_38_CTDependencyInjection.Models.Interfaces;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MVVM_38_CTDependencyInjection.Models
{
    public class UserRepository : IUserRepository
    {
        private ILog _log;

        public Guid GetUser(string name, string password) =>
            // Dummy-Implement
            name.ToUpper() switch
            {
                "DAVE" => new Guid("012345678ABCDEF012345678ABCDEF"),
                "JOE" => new Guid("112345678ABCDEF012345678ABCDEF"),
                "PETER" => new Guid("212345678ABCDEF012345678ABCDEF"),
                _ => Guid.Empty
            };

        public EPermission GetPermission(Guid id,Guid action) {
            return EPermission.All;
        }

        public UserRepository(ILog log) {
            _log = log;
        }
    }
}
