using EbayChat.Entities;
using Microsoft.EntityFrameworkCore;

namespace EbayChat.Services.ServicesImpl
{
    public class UserServices : IUserServices
    {
        private readonly CloneEbayDbContext _context;

        public UserServices(CloneEbayDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserByUsernameAndPassword(string username, string password)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.username == username && u.password == password);
        }
    }
}
