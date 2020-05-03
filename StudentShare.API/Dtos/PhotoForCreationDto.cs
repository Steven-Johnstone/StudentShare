using System;
using Microsoft.AspNetCore.Http;

namespace StudentShare.API.Dtos
{
    public class PhotoForCreationDto
    {
        public string Url { get; set; }

        public IFormFile File { get; set; } // needed for sending a file with an HTTP request

        public string Description { get; set; }

        public DateTime DateAdded { get; set; }

        public string PublicId { get; set; }

        public PhotoForCreationDto()
        {
            DateAdded = DateTime.Now; 
        }
    }
}