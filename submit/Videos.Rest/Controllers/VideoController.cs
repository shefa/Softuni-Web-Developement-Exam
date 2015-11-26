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
    public class VideoController : ApiController
    {
        private VideosDbContext db = new VideosDbContext();



        // task 2 get all videos by locaiton
        [HttpGet]
        [Route("videos")]
        public IHttpActionResult Get([FromUri] int? locationId)
        {
            if (locationId == null) return BadRequest();

            // The task doesnt ask to check whether location with such id exists!
            // If it did, it wasn't clear. Please write clearly what we should implement!
            // EDIT: i wrote it, i think it should be this way
            if (db.Locations.Any(l => l.id == locationId) == false) return BadRequest();

            var vids = db.Videos
                .Where(v => v.Location.id == locationId)
                .OrderBy(v=>v.Owner.UserName) // order by owner username then by title
                .ThenBy(v=>v.Title)
                .Select(v => new
                {
                    id = v.id,
                    title = v.Title,
                    country = v.Location.Country,
                    city = v.Location.City,
                    owner = v.Owner.Email
                })
                ;
            return Ok(vids);
            
            
        }

        // task 3 post video

        [HttpPost]
        [Route("videos")]
        public IHttpActionResult Post(VideoAddBindingModel mod)
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

            if (mod == null || mod.locationId == null)
            {
                return BadRequest();
            }

            if (user == null) return Unauthorized();
            // AGAIN, you didn't specify whether we should return not found when location with such id doesnt exist
            // !!!! if you did, uncomment this
            if (!db.Locations.Any(l => l.id == mod.locationId)) return BadRequest();

            var vid = new Video();
            vid.Location = db.Locations.Find(mod.locationId);
            vid.Title = mod.Title;
            vid.Owner = user;
            db.Videos.Add(vid);
            db.SaveChanges();
            return CreatedAtRoute(
                "DefaultApi",
                new { controller = "videos", id = vid.id },
                new
                {
                    id = vid.id,
                    Title = vid.Title,
                    status = vid.Status,
                    country = vid.Location.Country,
                    city = vid.Location.City,
                    Owner = vid.Owner.Email
                }
            );
        }

        // task 4 add tags to video
        [HttpPost]
        [Route("videos/{id:int}/addTag")]
        public IHttpActionResult AddTag(int id, TagAddToVideoBindingModel tagId)
        {
            var userId = User.Identity.GetUserId();
            ApplicationUser user = null;
            if (userId != null)
            {
                user = db.Users.Find(userId);
            }
            if (user == null) return Unauthorized();

            if (tagId == null ||tagId.tagId==null) return BadRequest();
            var tag = db.Tags.Find(tagId.tagId);
            // not specified wheter not found or bad request
            if (tag == null) return BadRequest();

            var movie = db.Videos.Find(id);
            if (movie == null) return NotFound();
            if (movie.Tags.Any(t => t.id == tag.id) == true) return BadRequest(); // or conflict

            movie.Tags.Add(tag);
            db.SaveChanges();
            return Ok();
        }
    }
}