using System;
using System.Collections.Generic;

namespace StudentShare.API.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string Course { get; set; }
        public string KnownAs { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        public string AboutMe { get; set; }
        public ICollection<Photo> Photo { get; set; }
        public ICollection<Like> Likees { get; set; }
        public ICollection<Like> Likers { get; set; }
    }
}