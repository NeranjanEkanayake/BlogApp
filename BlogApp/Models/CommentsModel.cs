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

        //connect to UserModel
        public required string UserId { get; set; }
        public required int BlogId { get; set; }

        [ForeignKey("UserId")]
        public UserModel Author { get; set; }

        //Connect to BlogModel
                       
        [ForeignKey("BlogId")]
        public BlogModel Blog { get; set; }
    }
}
