using API.DTO.UserDto;
using API.Models;
using API.Repository.UserRepository;
using AutoMapper;
using System.Security.Claims;

namespace API.Services.UserService
{
    public class UserService(IUserRepository userRepository, IMapper mapper,IHttpContextAccessor httpContextAccessor) : IUserService
    {
        private readonly ClaimsPrincipal _currentUser = httpContextAccessor.HttpContext?.User;

        public async Task<List<User>> GetAll()
        {
            var users = await userRepository.GetAll();
            return users;
        }
        public async Task<User> Get(Guid id)
        {
            var user = await userRepository.Get(id);
            return user;
        }
        public async Task<User> GetByEmail(string email)
        {
            var user = await userRepository.GetByEmail(email);
            return user;
        }
        public async Task<User> Update(Guid id, PutUserDto putUserDto)
        {
            var updatedUser = await userRepository.Update(id, putUserDto);
            return mapper.Map<User>(updatedUser);
        }
        public async Task<User> Delete(Guid id)
        {
            var deletedUser = await userRepository.Delete(id);
            return mapper.Map<User>(deletedUser);
        }
        public async Task<User?> GetCurrentUser()
        {
            var userIdClaim = _currentUser?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return null;

            return await userRepository.Get(Guid.Parse(userIdClaim));
        }
    }
}
