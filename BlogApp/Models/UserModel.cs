using Microsoft.AspNetCore.Identity;

namespace BlogApp.Models
{
    public class UserModel : IdentityUser
    {
        public string Name { get; set; }
    }
}
