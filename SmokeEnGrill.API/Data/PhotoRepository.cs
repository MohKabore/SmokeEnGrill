using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using EducNotes.API.data;
using EducNotes.API.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SmokeEnGrill.API.Helpers;
using SmokeEnGrill.API.Models;

namespace SmokeEnGrill.API.Data
{
    public class PhotoRepository : IPhotoRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly ICacheRepository _cache;
        private readonly IAdminRepository _adminRepo;
        private Cloudinary _cloudinary;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private readonly IConfiguration _config;
        private int broadcastTokenTypeId;
        public PhotoRepository(DataContext context, IMapper mapper, UserManager<User> userManager, IAdminRepository adminRepo,
            ICacheRepository cache, IConfiguration config, IOptions<CloudinarySettings> cloudinaryConfig)
        {
            _userManager = userManager;
            _mapper = mapper;
            _context = context;
            _cloudinaryConfig = cloudinaryConfig;
            _cache = cache;

            _cloudinaryConfig = cloudinaryConfig;
            Account acc = new Account(
                _cloudinaryConfig.Value.CloudName,
                _cloudinaryConfig.Value.ApiKey,
                _cloudinaryConfig.Value.ApiSecret
            );
            _cloudinary = new Cloudinary(acc);
        }

        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public async void AddAsync<T>(T entity) where T : class
        {
            await _context.AddAsync(entity);
        }

        public void Update<T>(T entity) where T : class
        {
            _context.Update(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public void DeleteAll<T>(List<T> entities) where T : class
        {
            _context.RemoveRange(entities);
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Photo> GetPhoto(int id)
        {
            var photo = await _context.Photos.IgnoreQueryFilters()
                .FirstOrDefaultAsync(p => p.Id == id);

            return photo;
        }

        public async Task<Boolean> AddUserPhoto(User user, IFormFile photoFile)
        {
            Boolean photoOK = true;

            if(photoFile.Length > 0)
            {
                var uploadResult = new ImageUploadResult();
                using (var stream = photoFile.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(photoFile.Name, stream),
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                    };
                    var subdomain = _adminRepo.GetAppSubDomain();
                    if (subdomain != "")
                        uploadParams.Folder = subdomain + "/";
                    else
                        uploadParams.Folder = "localDemo/";

                    uploadResult = _cloudinary.Upload(uploadParams);
                    if (uploadResult.StatusCode == HttpStatusCode.OK)
                    {
                        Photo photo = new Photo();
                        photo.Url = uploadResult.SecureUri.ToString();
                        photo.PublicId = uploadResult.PublicId;
                        photo.UserId = user.Id;
                        photo.DateAdded = DateTime.Now;
                        if (user.Photos.Any(u => u.IsMain))
                        {
                            var oldPhoto = await _context.Photos.FirstAsync(p => p.UserId == user.Id && p.IsMain == true);
                            oldPhoto.IsMain = false;
                            Update(oldPhoto);
                        }
                        photo.IsMain = true;
                        photo.IsApproved = true;
                        Add(photo);
                    }
                    else
                    {
                        photoOK = false;
                    }
                }
            }

            return photoOK;
        }

        public Boolean DeletePhotoFromCloudinary(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            var result = _cloudinary.Destroy(deleteParams);
            if (result.Result != "ok")
            {
                return false;
            }

            return true;
        }

        public async Task<ErrorDto> DeletePhoto(int userId, int id)
        {
            ErrorDto error = new ErrorDto();
            error.NoError = true;

            // var user = await GetUser(userId, true);
            var photoFromRepo = await GetPhoto(id);

            if (photoFromRepo.IsMain)
            {
                error.NoError = false;
                error.Message = "You cannot delete your main photo";
            }

            if (photoFromRepo.PublicId != null)
            {
                var deleteParams = new DeletionParams(photoFromRepo.PublicId);
                var result = _cloudinary.Destroy(deleteParams);
                if (result.Result == "ok")
                {
                Delete(photoFromRepo);
                }
            }

            if (photoFromRepo.PublicId == null)
            {
                Delete(photoFromRepo);
            }

            if (await SaveAll())
            {
                error.NoError = true;
                return error;
            }

            error.NoError = false;
            error.Message = "failed to delete the photo";
            return error;
        }

        public async Task<Photo> GetMainPhotoForUser(int userId)
        {
            return await _context.Photos.Where(u => u.UserId == userId).FirstOrDefaultAsync(p => p.IsMain);
        }

    }
}