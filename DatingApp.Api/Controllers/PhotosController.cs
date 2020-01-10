using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.Api.Data;
using DatingApp.Api.DTOs;
using DatingApp.Api.Helpers;
using DatingApp.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DatingApp.Api.Controllers
{
    [Authorize]
    [Route("api/users/{userId}/photos")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly IDatingRepository _datingRepo;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudinarySettings;

        private Cloudinary _cloudinary;
        public PhotosController(IDatingRepository datingRepo, IMapper mapper, 
            IOptions<CloudinarySettings> cloudinarySettings)
        {
            _datingRepo = datingRepo;
            _mapper = mapper;
            _cloudinarySettings = cloudinarySettings;

            Account acc = new Account(_cloudinarySettings.Value.CloudName,_cloudinarySettings.Value.ApiKey, _cloudinarySettings.Value.ApiSecret);
            _cloudinary = new Cloudinary(acc);
        }

        [HttpGet("{id}", Name="GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var photo = await _datingRepo.GetPhoto(id);
            var photoForReturn = _mapper.Map<PhotoForReturnDto>(photo);

            return Ok(photoForReturn);   
        }

        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userId, [FromForm] PhotoCreateDto photoCreateDto)
        {
             if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            
            var userFromRepo = await _datingRepo.GetUser(userId);
            var uploadResult = new ImageUploadResult();
            var file = photoCreateDto.File;
            if(file.Length > 0)
            {
                using(var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams{
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                    };

                    uploadResult = _cloudinary.Upload(uploadParams);
                }
            }

            photoCreateDto.Url = uploadResult.Uri.ToString();
            photoCreateDto.PublicId = uploadResult.PublicId.ToString();

            var photo = _mapper.Map<Photo>(photoCreateDto);
            if(!userFromRepo.Photos.Any(a => a.IsActive))
                photo.IsActive = true;

            userFromRepo.Photos.Add(photo);

            if(await _datingRepo.SaveAll())
            {
                var photoToReturn = _mapper.Map<PhotoForReturnDto>(photo);
                return CreatedAtRoute("GetPhoto",new  {userId = userId, id = photo.Id},photoToReturn);
            }
            else{
                return BadRequest("Could not upload a photo");
            }

        }

        [HttpPost("{id}/setActive")]
        public async Task<IActionResult> SetActivePhoto(int userId, int id)
        {
             if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var user = await _datingRepo.GetUser(userId);

            if(!user.Photos.Any(p => p.Id == id))
                return Unauthorized();

            var photoForActive = await _datingRepo.GetPhoto(id);

            if(photoForActive.IsActive)
                return BadRequest("The photo is allready active");

            var currentActivePhoto = await _datingRepo.GetActivePhotoForUser(userId);
            currentActivePhoto.IsActive = false;

            photoForActive.IsActive = true;

            if(await _datingRepo.SaveAll())
                return NoContent();

            return BadRequest("Could not set photo as active.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(int userId, int id)
        {
             if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var user = await _datingRepo.GetUser(userId);

            if(!user.Photos.Any(p => p.Id == id))
                return Unauthorized();

            var photoForActive = await _datingRepo.GetPhoto(id);

            if(photoForActive.IsActive)
                return BadRequest("You can not delete the active photo.");

            if(photoForActive.PublicId != null) {
                var deleteParam = new DeletionParams(photoForActive.PublicId);
                var result = _cloudinary.Destroy(deleteParam);
                if(result.Result == "ok") {
                    _datingRepo.Delete(photoForActive);
                }
            }

            if(photoForActive.PublicId == null) {
                _datingRepo.Delete(photoForActive);
            }

            if(await _datingRepo.SaveAll()){
                return Ok();
            }

            return BadRequest("Failled to delete photo.");
        }

    }
}