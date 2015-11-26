using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using Videos.Models;

namespace Videos.Data
{
    public class VideosDbContext : IdentityDbContext<ApplicationUser>
    {
        public VideosDbContext()
            : base("DefaultConnection")
        {
        }

        public static VideosDbContext Create()
        {
            return new VideosDbContext();
        }

        public virtual IDbSet<Playlist> Playlists { get; set; }

        public virtual IDbSet<Video> Videos { get; set; }

        public virtual IDbSet<Location> Locations { get; set; }

        public virtual IDbSet<Tag> Tags { get; set; }
    }
}
