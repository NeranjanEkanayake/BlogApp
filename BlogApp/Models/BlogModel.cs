using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogApp.Models
{
    public class BlogModel
    {
        [Key]
        public int BlogId { get; set; }
        [Required]
        [StringLength(50)]
        public required string Title { get; set; }
        [Required]
        [StringLength(200)]
        public required string Description { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Required]
        public required string UserId {  get; set; } 
        [Required]
        [ForeignKey("UserId")]
        public UserModel? Author { get; set; }
        
        [Required]
        public ICollection<CommentsModel> Comments { get; set; } = new List<CommentsModel>();
    }
}
