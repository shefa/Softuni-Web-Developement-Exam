using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Videos.Models
{
    public class Tag
    {
        [Key]
        public int id { get; set; }

        public string Name { get; set; }
        public bool isAdultContent { get; set; }

        public ApplicationUser Owner { get; set; }
    }
}