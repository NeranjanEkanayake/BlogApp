using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonData.DTO
{
    public class CommentDTO
    {
        public int ID { get; set; }
        public string Content { get; set; }
        public string UserId {  get; set; }
        public int BlogId { get; set; }
    }
}
