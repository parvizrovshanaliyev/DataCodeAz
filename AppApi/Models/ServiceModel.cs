﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AppApi.Models
{
    public class ServiceModel
    {
        public int ID { get; set; }
        [Required]
        [MaxLength(150)]
        [MinLength(5)]
        public string Title { get; set; }

        [Required]
        [MinLength(5)]
        public string Description { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public string Image { get; set; }

        public IFormFile Photo { get; set; }
    }
}
