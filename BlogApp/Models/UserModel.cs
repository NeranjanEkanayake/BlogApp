using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BlogApp.Models
{
    public class UserModel : IdentityUser
    {
        public required string Name { get; set; }
    }
}
