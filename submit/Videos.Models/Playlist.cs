using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Videos.Models
{
    public class Playlist
    {
        private ICollection<Tag> tags;
        private ICollection<Video> videos;

        public Playlist()
        {
            tags = new HashSet<Tag>();
            videos = new HashSet<Video>();
        }
        [Key]
        public int id
        {
            get; set;
        }

        public string Name { get; set; }

        public ApplicationUser Owner { get; set; }

        public virtual ICollection<Tag> Tags
        {
            get
            {
                return this.tags;
            }
            set
            {
                this.tags = value;
            }
        }
        public virtual ICollection<Video> Videos
        {
            get
            {
                return this.videos;
            }
            set
            {
                this.videos = value;
            }
        }
    }
}