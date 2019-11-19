using System;

namespace StudentShare.API.Models
{
    public class Photo
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public DateTime DateAdded { get; set; }
        public bool MainPhoto { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }
    }
}