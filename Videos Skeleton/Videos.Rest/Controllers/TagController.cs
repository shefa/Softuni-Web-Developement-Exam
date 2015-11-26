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
    public class TagController : ApiController
    {
        private VideosDbContext db = new VideosDbContext();
        //post

        [HttpPost]
        [Route("Tags")]
        public IHttpActionResult Post(Tag tag)
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

            if (tag == null || tag.Name == null)
            {
                return this.BadRequest();
            }

            if (db.Tags.Any(t => t.Name == tag.Name && t.isAdultContent == tag.isAdultContent)) return BadRequest();

            // not specified in the task again....
            // I added this check myself, since the tags require a Owner ( for them to be able to be deleted )
            if (user == null) return Unauthorized();

            tag.Owner = user;

            db.Tags.Add(tag);
            db.SaveChanges();

            return CreatedAtRoute(
                "DefaultApi",
                new { controller = "Tags", id = tag.id },
                new
                {
                    id = tag.id,
                    name = tag.Name,
                    isAdultContent = tag.isAdultContent
                }
                );
            
        }

        [HttpDelete]
        [Route("Tags/{id:int}")]
        public IHttpActionResult Delete(int? id)
        {
            var userId = User.Identity.GetUserId();
            ApplicationUser user = null;
            if (userId!=null)
            {
                user = db.Users.Find(userId);
            }

            var tag = db.Tags.Find(id);
            if (tag == null) return NotFound();
            if (db.Videos.Any(v => v.Tags.Any(t => t.id == id)))
            { 
                return Conflict();
            }

            // authro
            if (user == null || tag.Owner != user) return Unauthorized();

            db.Tags.Remove(tag);
            db.SaveChanges();
            return Ok(new { message = "Tag #" + id+" deleted." });
        }

        // i should write test here

        [HttpPut]
        [Route("Tags/{id:int}")]
        public IHttpActionResult Edit(int? id, Tag tag_data)
        {
            var userId = User.Identity.GetUserId();
            ApplicationUser user = null;
            if (userId != null)
            {
                user = db.Users.Find(userId);
            }

            if (tag_data == null || tag_data.Name == null) return BadRequest();


            var tag = db.Tags.Find(id);
            if (tag == null) return NotFound();
            // authro !! edit, all logged users can change!
            if (user == null ) return Unauthorized();

            tag.Name = tag_data.Name;
            tag.isAdultContent = tag_data.isAdultContent;
            
            db.SaveChanges();
            return Ok(new {
                id = tag.id,
                name = tag.Name,
                isAdultContent = tag.isAdultContent
            });
        }

    }
}