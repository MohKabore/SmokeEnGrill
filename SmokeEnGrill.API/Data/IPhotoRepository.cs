using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EducNotes.API.Dtos;
using Microsoft.AspNetCore.Http;
using SmokeEnGrill.API.Models;

namespace SmokeEnGrill.API.Data
{
    public interface IPhotoRepository
    {
        void Add<T>(T entity) where T : class;
        void Update<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        void DeleteAll<T>(List<T> entities) where T : class;
        Task<bool> SaveAll();
        Task<Photo> GetPhoto(int id);
        Boolean DeletePhotoFromCloudinary(string publicId);
        Task<ErrorDto> DeletePhoto(int userId, int id);
        Task<Boolean> AddUserPhoto(User user, IFormFile photoFile);
    }
}