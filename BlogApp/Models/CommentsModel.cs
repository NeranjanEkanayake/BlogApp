using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogApp.Models
{
    public class CommentsModel
    {
        [Key]
        public int Id { get; set; }
        
        public string Content { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public UserModel Author { get; set; }

        public int BlogId { get; set; }

        [ForeignKey("BlogId")]
        public BlogModel Blog { get; set; }
    }
}
