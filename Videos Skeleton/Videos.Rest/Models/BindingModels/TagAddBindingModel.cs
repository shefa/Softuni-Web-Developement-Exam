using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Videos.Rest.Models.BindingModels
{
    public class TagAddBindingModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public bool? isAdultContent { get; set; }
    }
}