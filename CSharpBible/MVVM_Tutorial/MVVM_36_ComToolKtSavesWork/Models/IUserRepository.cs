namespace MVVM_36_ComToolKtSavesWork.Models
{
    public interface IUserRepository
    {
        User? Login(string username, string password);
    }
}