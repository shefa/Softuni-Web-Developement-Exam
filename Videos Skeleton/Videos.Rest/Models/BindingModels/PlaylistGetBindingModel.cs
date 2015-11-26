using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Videos.Rest.Models.BindingModels
{
    public class PlaylistGetBindingModel
    {
        public int startPage { get; set; }
        public int limit { get; set; }

        public int? locationId { get; set; }
        public bool? adultContentAllowed { get; set; }
    }
}