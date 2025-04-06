using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CommonData.Models
{
    public class UserModel : IdentityUser
    {
        public required string Name { get; set; }
    }
}
