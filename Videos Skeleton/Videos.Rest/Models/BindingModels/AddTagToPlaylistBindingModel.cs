﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Videos.Rest.Models.BindingModels
{
    public class AddTagToPlaylistBindingModel
    {
        public int? tagId { get; set; }
    }
}