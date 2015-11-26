using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Videos.Rest.Models;
using Videos.Models;
using Videos.Rest.Models.BindingModels;
using Videos.Data;
using System.Linq;

namespace Videos.Rest.Controllers
{
    [RoutePrefix("api")]
    public class PlaylistController : ApiController
    {
        private VideosDbContext db = new VideosDbContext();

        [HttpPost]
        [Route("Playlists")]
        public IHttpActionResult Post(PlaylistAddBindingModel mod)
        {
            var userId = User.Identity.GetUserId();
            ApplicationUser user = null;
            if (userId != null)
            {
                user = db.Users.Find(userId);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (String.IsNullOrEmpty(mod.name))
            {
                return BadRequest();
            }

            if (user == null) return Unauthorized();

            var playlist = new Playlist();
            playlist.Name = mod.name;
            playlist.Owner = user;

            db.Playlists.Add(playlist);
            db.SaveChanges();

            return CreatedAtRoute(
                "DefaultApi",
                new { controller = "Playlists", id = playlist.id },
                new
                {
                    id = playlist.id,
                    Name = playlist.Name,
                    Owner = playlist.Owner.Email
                }
                );
        }

        // add video to playlist
        [HttpPost]
        [Route("playlists/{id:int}/addVideo")]
        public IHttpActionResult AddVideo(int id, AddVideoToPlaylistBindingModel mod)
        {
            var userId = User.Identity.GetUserId();
            ApplicationUser user = null;
            if (userId != null)
            {
                user = db.Users.Find(userId);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (mod.videoId == null || db.Videos.Any(v => v.id == mod.videoId) == false)
            {
                return this.BadRequest();
            }

            Playlist play = db.Playlists.Find(id);

            if (play == null) return NotFound();

            if (user == null) return Unauthorized();

            Video vid = db.Videos.Find(mod.videoId);

            play.Videos.Add(vid);
            db.SaveChanges();

            return Ok();
        }

        //add tag to playlist
        [HttpPost]
        [Route("playlists/{id:int}/addTag")]
        public IHttpActionResult AddTag(int id, AddTagToPlaylistBindingModel mod)
        {
            var userId = User.Identity.GetUserId();
            ApplicationUser user = null;
            if (userId != null)
            {
                user = db.Users.Find(userId);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (mod.tagId == null || db.Tags.Any(v => v.id == mod.tagId) == false)
            {
                return BadRequest();
            }

            Playlist play = db.Playlists.Find(id);

            if (play == null) return NotFound();

            if (user == null) return Unauthorized();

            Tag tag = db.Tags.Find(mod.tagId);

            if (play.Tags.Any(t => t.id == tag.id)) return BadRequest(); // Conflict()?

            play.Tags.Add(tag);
            db.SaveChanges();
            return Ok();
        }

        //task 11 get playlists of the CURRENT USER by LOCATION (if provided ) and by ADULT CONTENT (if provided)
        [HttpGet]
        [Route("Playlists")]
        public IHttpActionResult GetPlaylists([FromUri] int startPage, [FromUri] int limit, [FromUri] int? locationId, [FromUri] bool? adultContentAllowed)
        {

            var userId = User.Identity.GetUserId();
            ApplicationUser user = null;
            if (userId != null)
            {
                user = db.Users.Find(userId);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (user == null) return Unauthorized();


            bool adult = false;
            if (adultContentAllowed != null && adultContentAllowed == true) adult = true;

            var ans = db.Playlists
                .Where(p => p.Owner.Email == user.Email)
                .Where(p => adult == false || !p.Tags.Any(t => t.isAdultContent))
                .Select(p => new
                {
                    id = p.id,
                    Name = p.Name,
                    Owner = p.Owner.Email,
                    tags = p.Tags.Select(t => new
                    {
                        id = t.id,
                        name = t.Name,
                        isAdultContent = t.isAdultContent
                    }),
                    videos = p.Videos
                                 .Where(v => locationId == null || v.Location.id == locationId)
                                 .Select(v => new
                                 {
                                     id = v.id,
                                     title = v.Title,
                                     country = v.Location.Country,
                                     city = v.Location.City,
                                     owner = v.Owner.Email
                                 })
                }).Skip(startPage*limit).Take(limit);

            return Ok(ans);


            
            
        }
    }
}
