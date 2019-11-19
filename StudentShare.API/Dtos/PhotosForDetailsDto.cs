using System;

namespace StudentShare.API.Dtos
{
    public class PhotosForDetailsDto
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public DateTime DateAdded { get; set; }
        public bool MainPhoto { get; set; }
        
    }
}