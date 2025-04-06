using System.ComponentModel.DataAnnotations;

namespace CommonData.Models.DTO
{
    public class LoginViewDTO
    {
        [Required]
        public string UserName {  get; set; }

        [Required]
        public string Password { get; set; }
    }
}
