﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using NetTopologySuite.Geometries;

namespace MoviesAPI.Entities
{
    public class MovieTheater
    {
        public int Id { get; set; }

        [Required] 
        public string Name { get; set; }
        public Point Location { get; set; }
    }
}
