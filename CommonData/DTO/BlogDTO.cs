using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonData.DTO
{
    public class BlogDTO
    {
        public int BlogId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string UserId {  get; set; }
    }
}
