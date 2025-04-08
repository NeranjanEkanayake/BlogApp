using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonData.DTO
{
    public class CommentDTO
    {
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public UserViewDTO Author { get; set; }
    }
}
