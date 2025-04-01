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
        public string Title { get; set; }
        [Required]
        [StringLength(200)]
        public string Description { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }

        [Required]
        public string UserId {  get; set; }
        [Required]
        [ForeignKey("UserId")]
        public UserModel? Author { get; set; }
        
    }
}
