using API.DTO.UserDto;
using API.Models;

namespace API.Services.UserService
{
    public interface IUserService
    {
        public Task<List<User>> GetAll();
        public Task<User> Get(Guid id);
        public Task<User> GetByEmail(string email);
        public Task<User> Update(Guid id, PutUserDto putUserDto);
        public Task<User> Delete(Guid id);
        public Task<User?> GetCurrentUser();
    }
}
