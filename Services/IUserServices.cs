using EbayChat.Entities;

namespace EbayChat.Services
{
    public interface IUserServices
    {
        Task<User> GetUserByUsernameAndPassword(String username, String password);
    }
}
