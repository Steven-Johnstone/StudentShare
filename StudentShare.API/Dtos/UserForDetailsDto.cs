using System;
using System.Collections.Generic;
using StudentShare.API.Models;

namespace StudentShare.API.Dtos
{
    public class UserForDetailsDto
    {
        
        public int Id { get; set; }
        public string Username { get; set; }
        public string Course { get; set; }
        public string KnownAs { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        public string AboutMe { get; set; }
        public string PhotoUrl { get; set; }
        public ICollection<PhotosForDetailsDto> Photo { get; set; }
    }
}