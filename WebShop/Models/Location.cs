using System;
using System.Collections.Generic;

#nullable disable

namespace WebShop.Models
{
    public partial class Location
    {
        public int LocationId { get; set; }
        public string Name { get; set; }
        public int? Parent { get; set; }
        public int? Levels { get; set; }
        public string Slug { get; set; }
        public string NameWithType { get; set; }
        public string Type { get; set; }
    }
}
