using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogApp.Models
{
    public class BlogModel
    {
        [Key]
        public int BlogId { get; set; }
        
        [StringLength(50)]
        public required string Title { get; set; }
        
        [StringLength(200)]
        public required string Description { get; set; }
        
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [ValidateNever]
        public string UserId {  get; set; } 
        
        [ForeignKey("UserId")]
        [ValidateNever]
        public UserModel Author { get; set; }
        
        [Required]
        public ICollection<CommentsModel> Comments { get; set; } = new List<CommentsModel>();
    }
}
