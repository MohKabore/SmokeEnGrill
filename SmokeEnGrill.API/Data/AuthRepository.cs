using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using EducNotes.API.data;
using EducNotes.API.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SmokeEnGrill.API.Dtos;
using SmokeEnGrill.API.Models;

namespace SmokeEnGrill.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        private readonly IConfiguration _config;
        private readonly IEmailSender _emailSender;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly ICacheRepository _cache;
        string password;

        public AuthRepository(DataContext context, IConfiguration config, IEmailSender emailSender,
            UserManager<User> userManager, IMapper mapper, ICacheRepository cache)
        {
            _context = context;
            _config = config;
            _cache = cache;
            _emailSender = emailSender;
            _mapper = mapper;
            _config = config;
            password = _config.GetValue<String>("AppSettings:defaultPassword");
            _userManager = userManager;
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

        public async Task<User> Login(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == username);

            if (user == null)
                return null;

            // if(!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            // return null;

            return user;
        }

        public async Task<bool> UserNameExist(string userName, int currentUserId)
        {
        List<User> users = await _cache.GetUsers();

        var user = users.Where(u => u.Id != currentUserId)
                        .FirstOrDefault(e => e.UserName.ToLower() == userName.ToLower());

        if (user != null)
        {
            return true;
        }
        return false;
        }

        public async Task<User> GetUserByEmailAndLogin(string username, string email)
        {
        List<User> users = await _cache.GetUsers();
        return users.FirstOrDefault(u => u.Email.ToUpper() == email.ToUpper() &&
            u.UserName.ToUpper() == username.ToUpper());
        }

    public async Task<bool> EmailExist(string email)
    {
      var user = await _context.Users.FirstOrDefaultAsync(e => e.Email == email);
      if (user != null)
        return true;
      return
      false;
    }

        public async Task<bool> UserExists(string username)
        {
            if (await _context.Users.AnyAsync(x => x.UserName == username))
                return true;

            return false;
        }

        public async Task<User> GetUserById(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User> GetUser(int id, bool isCurrentUser)
        {
            var query = _context.Users.AsQueryable();
            // .Include(c => c.Class)
            // .Include(p => p.Photos).AsQueryable();

            if (isCurrentUser)
                query = query.IgnoreQueryFilters();

            var user = await query.FirstOrDefaultAsync(u => u.Id == id);

            return user;
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email.ToUpper() == email.ToUpper());
        }

        public async Task<bool> UserNameExist(string userName)
        {
            var user = await _context.Users.FirstOrDefaultAsync(e => e.UserName == userName.ToLower());
            if (user != null)
                return true;
            return
            false;
        }


        private string ResetPasswordContent(string code)
        {
            return "<b>RLE 2020</b> a bien enrgistré votre demande de réinitialisation de mot de passe !<br>" +
                "Vous pouvez utiliser le lien suivant pour réinitialiser votre mot de passe: <br>" +
                " <a href=" + _config.GetValue<String>("AppSettings:DefaultResetPasswordLink") + code + "/>cliquer ici</a><br>" +
                "Si vous n'utilisez pas ce lien dans les 3 heures, il expirera." +
                "Pour obtenir un nouveau lien de réinitialisation de mot de passe, visitez" +
                " <a href=" + _config.GetValue<String>("AppSettings:DefaultforgotPasswordLink") + "/>réinitialiser son mot de passe</a>.<br>" +
                "Merci,";

        }
    }
}