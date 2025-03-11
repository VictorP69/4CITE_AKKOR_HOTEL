using API.DTO.UserDto;
using API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API.Repository.UserRepository
{
    public class UserRepository(UserManager<User> userManager) : IUserRepository
    {
        public async Task<List<User>> GetAll()
        {
            var users = await userManager.Users.ToListAsync();
            return users;
        }
        public async Task<User> Get(Guid id)
        {
            var user = await userManager.FindByIdAsync(id.ToString());
            return user;
        }
        public Task<User> GetByEmail(string email)
        {
            var user = userManager.FindByEmailAsync(email);
            return user;
        }

        public async Task<User> Update(Guid id, PutUserDto putUserDto)
        {
            var user = await Get(id);
            user.Pseudo = putUserDto.Pseudo;
            await userManager.UpdateAsync(user);
            return user;
        }

        public async Task<User> Delete(Guid id)
        {
            var user = await Get(id);
            await userManager.DeleteAsync(user);
            return user;
        }
    }
}
