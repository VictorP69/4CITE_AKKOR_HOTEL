using Microsoft.AspNetCore.Identity;

namespace API.Models
{
    public class User : IdentityUser
    {
        public string Pseudo { get; set; }
        public UserRole Role { get; set; }
    }
}
