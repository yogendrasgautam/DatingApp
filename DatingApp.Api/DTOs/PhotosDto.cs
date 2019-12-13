using System;

namespace DatingApp.Api.DTOs
{
    public class PhotosDto
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
        public bool IsActive { get; set; }
    }
}