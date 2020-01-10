using System;
using Microsoft.AspNetCore.Http;

namespace DatingApp.Api.DTOs
{
    public class PhotoCreateDto
    {
        public string Url { get; set; }
        public IFormFile File { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
        public string PublicId { get; set; }

        public PhotoCreateDto()
        {
            DateAdded  = DateTime.Now;
        }
    }
}