using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
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
        string password;

        public AuthRepository(DataContext context, IConfiguration config, IEmailSender emailSender,
            UserManager<User> userManager, IMapper mapper)
        {
            _context = context;
            _config = config;
            _emailSender = emailSender;
            _mapper = mapper;
            _config = config;
            password = _config.GetValue<String>("AppSettings:defaultPassword");
            _userManager = userManager;
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

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i]) return false;
                }
            }
            return true;
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            // user.PasswordHash = passwordHash;
            // user.PasswordSalt = passwordSalt;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
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

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> SendEmail(EmailFormDto emailFormDto)
        {
            try
            {
                await _emailSender.SendEmailAsync(emailFormDto.toEmail, emailFormDto.subject, emailFormDto.content);
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
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

        public async Task<bool> SendResetPasswordLink(string email, string code)
        {
            var emailform = new EmailFormDto
            {
                toEmail = email,
                subject = "Réinitialisation de mot passe ",
                //content ="Votre code de validation: "+ "<b>"+code.ToString()+"</b>"
                content = ResetPasswordContent(code)
            };
            try
            {
                var res = await SendEmail(emailform);
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }

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