using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Videos.Models
{
    public class Location
    {
        [Key]
        public int id { get; set; }
        public string Country { get; set; }
        public string City { get; set; }

    }
}