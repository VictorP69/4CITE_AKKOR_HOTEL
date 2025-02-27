﻿using System.Reflection.Metadata;

namespace API.DTO
{
    public class PostHotelDto
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public List<Blob> PictureList { get; set; }
    }
}
