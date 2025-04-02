using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogApp.Models
{
    public class CommentsModel
    {
        [Key]
        public int Id { get; set; }

        public required string Content { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; }
               
        [StringLength(10)]
        public required string UserId { get; set; }                
        [ForeignKey("UserId")]
        public required UserModel Author { get; set; }
                
        public required int BlogId { get; set; }               
        [ForeignKey("BlogId")]
        public required BlogModel Blog { get; set; }
    }
}
