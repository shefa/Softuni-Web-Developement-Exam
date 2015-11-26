using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Videos.Models
{
    public class Video
    {
        private ICollection<Tag> tags;

        public Video()
        {
            tags = new HashSet<Tag>();
        }

        [Key]
        public int id { get; set; }
        
        //[Required]
        public string Title { get; set; }
        public VideoStatus Status { get; set; }
        public Location Location { get; set; }
        // TODO
        public ApplicationUser Owner { get; set; }
        

        public virtual ICollection<Tag> Tags
        {
            get { return this.tags; }
            set { this.tags = value; }
        }

    }
}
