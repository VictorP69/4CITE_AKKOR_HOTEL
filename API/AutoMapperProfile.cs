using AutoMapper;
using Microsoft.AspNetCore.Identity;
using API.Models;

namespace API
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile() 
        {
            CreateMap<IdentityUser, User>();
            CreateMap<User, IdentityUser>();
        }
    }
}
