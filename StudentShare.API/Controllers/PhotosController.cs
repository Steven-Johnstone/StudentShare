using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StudentShare.API.Data;
using StudentShare.API.Dtos;
using StudentShare.API.Helpers;
using StudentShare.API.Models;
using System.Linq;

namespace StudentShare.API.Controllers
{
    [Authorize]
    [Route("api/users/{userId}/photos")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly IStudentRepository _repo;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private Cloudinary _cloudinary;

        public PhotosController(IStudentRepository repo, IMapper mapper,
        
        IOptions<CloudinarySettings> cloudinaryConfig)
        {
            _cloudinaryConfig = cloudinaryConfig;
            _mapper = mapper;
            _repo = repo;

            Account acc = new Account( // set the new account to hold the name and API details
                _cloudinaryConfig.Value.CloudName,
                _cloudinaryConfig.Value.ApiKey,
                _cloudinaryConfig.Value.ApiSecret
            );

            //create new instance of cloudinary and pass account details
            _cloudinary = new Cloudinary(acc);
        }

        [HttpGet("{id}", Name = "GetPhoto")] // setting a name for getting a photo
        public async Task<IActionResult> GetPhoto(int id)
        {
            var photoFromRepo = await _repo.GetPhoto(id); // returns the photo

            var photo = _mapper.Map<PhotoForReturnDto>(photoFromRepo); // maps the photo from the repo

            return Ok(photo);
        }

        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userId,[FromForm] PhotoForCreationDto photoForCreationDto)
        {
            // compare userId against the route paramater of userId to see if they match 
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized(); // deny access

            var userFromRepo = await _repo.GetUser(userId); // gets the userId from the database

            var file = photoForCreationDto.File; // stores the file from the DTO

            var uploadResult = new ImageUploadResult(); // stores result that comes back from cloudinary

            if (file.Length > 0) // checks if there is something in the file
            {
                using (var stream = file.OpenReadStream()) // read the file into memory
                {
                    var uploadParams = new ImageUploadParams() // give cloudinary our upload paramaters
                    {
                        File = new FileDescription(file.Name, stream), // specifies the file
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face") // crops the image to show only the face section
                    };

                    uploadResult = _cloudinary.Upload(uploadParams); // make use of upload method from cloudinary using the upload paramaters above
                }
            }

            //sets the url and id into our DTO for use
            photoForCreationDto.Url = uploadResult.Uri.ToString(); 
            photoForCreationDto.PublicId = uploadResult.PublicId;

            var photo = _mapper.Map<Photo>(photoForCreationDto); // map our dto into our photo

            if (!userFromRepo.Photo.Any(u => u.MainPhoto)) // checks if this is the first photo upload
                photo.MainPhoto = true; // returns the photo as main photo

            userFromRepo.Photo.Add(photo); // adds the photo

            

            if (await _repo.SaveAll()) // if the save is successful
            {
                var photoToReturn = _mapper.Map<PhotoForReturnDto>(photo); // maps photo to hold the details in the dto
                return CreatedAtRoute("GetPhoto", new { id = photo.Id}, photoToReturn ); // returns ok if successful
            }

            return BadRequest("Could not add the photo"); // returns message if unsuccessful
      
        }

        [HttpPost("{id}/setMain")] //used to set photo as main photo
        public async Task<IActionResult> SetMainPhoto(int userId, int id)
        {
            // compare userId against the route paramater of userId to see if they match 
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized(); // deny access

            var user = await _repo.GetUser(userId); // gets the user from the repo

            if (!user.Photo.Any(p => p.Id == id)) // if the id passing in doesnt match any photos in collection return unauthorized
                return Unauthorized();

            var photoFromRepo = await _repo.GetPhoto(id); // gets photo from the repo

            if (photoFromRepo.MainPhoto) // if currently set as main photo return message
                return BadRequest("This is already the main photo.");

            var currentMainPhoto = await _repo.GetMainPhoto(userId); // fetching the current main photo

            currentMainPhoto.MainPhoto = false; // setting old main photo to false

            photoFromRepo.MainPhoto = true; // sets new photo to true

            if (await _repo.SaveAll()) // if save works return nothing
                return NoContent();

            return BadRequest("Could not update main photo"); //if save fails return bad request message

        }

        [HttpDelete("{id}")]

        public async Task<IActionResult> DeletePhoto(int userId, int id)
        {
            // compare userId against the route paramater of userId to see if they match 
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized(); // deny access

            var user = await _repo.GetUser(userId); // gets the user from the repo

            if (!user.Photo.Any(p => p.Id == id)) // if the id passing in doesnt match any photos in collection return unauthorized
                return Unauthorized();

            var photoFromRepo = await _repo.GetPhoto(id); // gets photo from the repo

            if (photoFromRepo.MainPhoto) // if currently set as main photo return message
                return BadRequest("This is your main photo, this cannot be deleted.");

            if(photoFromRepo.PublicId != null)
            {
                var deleteParams = new DeletionParams(photoFromRepo.PublicId); // setting up params to allow deletions

                var result = _cloudinary.Destroy(deleteParams); // cloudinary delete method

                if (result.Result == "ok") { // if the result of deletion is ok
                _repo.Delete(photoFromRepo); // delete the photo
                }
            }

            if(photoFromRepo.PublicId == null)
            {
                _repo.Delete(photoFromRepo); // delete the photo
            }

            if (await _repo.SaveAll()) // if save works return ok
                return Ok();
            
            return BadRequest("Failed to delete the photo"); // if save fails return bad request message
        }

    }
}
