using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SmokeEnGrill.API.Data;
using SmokeEnGrill.API.Models;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Account = CloudinaryDotNet.Account;
using Microsoft.Extensions.Options;
using SmokeEnGrill.API.Helpers;
using SmokeEnGrill.API.Dtos;
using System.IdentityModel.Tokens.Jwt;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SmokeEnGrill.API.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IAuthRepository _repo;
        private readonly DataContext _context;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private Cloudinary _cloudinary;


        public AuthController(IConfiguration config, IMapper mapper, IAuthRepository repo,
              UserManager<User> userManager, SignInManager<User> signInManager, DataContext context,
              IOptions<CloudinarySettings> cloudinaryConfig)
        {
            _context = context;
            _repo = repo;
            _config = config;
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
            _cloudinaryConfig = cloudinaryConfig;
            Account acc = new Account(
            _cloudinaryConfig.Value.CloudName,
            _cloudinaryConfig.Value.ApiKey,
            _cloudinaryConfig.Value.ApiSecret
        );

            _cloudinary = new Cloudinary(acc);
        }


        [HttpPost("{id}/setPassword/{password}")] // edition du mot de passe apres validation du code
        public async Task<IActionResult> setPassword(int id, string password)
        {
            var user = await _repo.GetUser(id, false);
            if (user != null)
            {
                bool emailCOnfirmed = user.EmailConfirmed;
                var newPassword = _userManager.PasswordHasher.HashPassword(user, password);
                user.PasswordHash = newPassword;
                user.ValidatedCode = true;
                user.EmailConfirmed = true;
                if (!emailCOnfirmed)
                    user.ValidationDate = DateTime.Now;
                var res = await _userManager.UpdateAsync(user);

                if (res.Succeeded)
                {
                    var mail = new EmailFormDto();
                    if (emailCOnfirmed)
                    {
                        // Envoi de mail pour la confirmation de la mise a jour du mot de passe
                        mail.subject = "mot de passe modifié";
                        mail.content = "bonjour <b> " + user.LastName + " " + user.FirstName + "</b>, votre nouveau mot de passe a bien eté enregistré...";
                        mail.toEmail = user.Email;
                    }
                    else
                    {
                        mail.subject = "Compte confirmé";
                        mail.content = "<b> " + user.LastName + " " + user.FirstName + "</b>, votre compte a bien été enregistré";
                        mail.toEmail = user.Email;
                    }
                    await _repo.SendEmail(mail);
                    var userToReturn = _mapper.Map<UserForListDto>(user);
                    return Ok(new
                    {
                        token = await GenerateJwtToken(user),
                        user = userToReturn

                    });
                }
                return BadRequest("impossible de terminé l'action");
            }
            return NotFound();
        }


        [HttpGet("{email}/ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            // recherche du compte de l 'email
            var user = await _repo.GetUserByEmail(email);
            if (user != null)
            {
                if (!user.EmailConfirmed)
                    return BadRequest("compte pas encore activé....");
                // envoi du mail pour le reset du password
                user.ValidationCode = Guid.NewGuid().ToString();
                if (await _repo.SendResetPasswordLink(user.Email, user.ValidationCode))
                {
                    // envoi effectuer
                    user.ForgotPasswordCount += 1;
                    user.ForgotPasswordDate = DateTime.Now;
                    await _repo.SaveAll();
                    return Ok();

                }
                return BadRequest("echec d'envoi du mail");
            }
            return BadRequest("email non trouvé. Veuillez réessayer");

        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {

            // verification de l'existence du userName 
            if (await _repo.UserNameExist(userForLoginDto.Username.ToLower()))
            {
                var user = await _userManager.FindByNameAsync(userForLoginDto.Username.ToLower());
                if (!user.ValidatedCode)
                    return BadRequest("Compte non validé pour l'instant...");

                var result = await _signInManager
                .CheckPasswordSignInAsync(user, userForLoginDto.Password, false);

                if (result.Succeeded)
                {
                    var appUser = await _userManager.Users.Include(p => p.Photos)
                        .FirstOrDefaultAsync(u => u.NormalizedUserName ==
                            userForLoginDto.Username.ToUpper());

                    var userToReturn = _mapper.Map<UserForListDto>(appUser);


                    return Ok(new
                    {
                        token = await GenerateJwtToken(appUser),
                        user = userToReturn
                    });
                }

                return BadRequest("login ou mot de passe incorrecte...");
            }
            return BadRequest("login ou mot de passe incorrecte...");

        }

        [HttpGet("ResetPassword/{code}")]
        public async Task<IActionResult> ResetPassword(string code)
        {
            var user = await _repo.GetUserByCode(code);
            if (user != null)
            {
                if (DateTime.Now.AddHours(-3) >= user.ForgotPasswordDate.Value) // le delai des 3 Heures a expiré
                    return BadRequest("Désolé ce lien a expiré....");
                else
                    return Ok(new
                    {
                        user = _mapper.Map<UserForListDto>(user)
                    });

            }
            return BadRequest("lien introuvable");
        }

        [HttpGet("{userName}/VerifyUserName")]
        public async Task<IActionResult> VerifyUserName(string userName)
        {
            return Ok(await _repo.UserNameExist(userName));
        }


        [HttpPost("{id}/setLoginPassword")] // edition du mot de passe apres validation du code
        public async Task<IActionResult> setLoginPassword(int id, LoginFormDto loginForDto)
        {
            var user = await _repo.GetUser(id, false);
            if (user != null)
            {
                var newPassword = _userManager.PasswordHasher.HashPassword(user, loginForDto.Password);
                user.UserName = loginForDto.UserName.ToLower();
                user.NormalizedUserName = loginForDto.UserName.ToUpper();
                user.PasswordHash = newPassword;
                user.ValidatedCode = true;
                user.EmailConfirmed = true;
                user.Active = true;
                user.PreSelected = true;
                user.Trained = true;
                user.Selected = true;
                user.OnTraining = true;
                user.Hired = true;
                user.ValidationDate = DateTime.Now;
                var res = await _userManager.UpdateAsync(user);
                var roleName = "";
                // if (user.TypeEmpId == 1)
                //     roleName = "admin";
                // else if (user.TypeEmpId == 11)
                //     roleName = "AgentHotline";
                // else
                // {
                //     var role = await _context.Roles.FirstOrDefaultAsync(a => a.Id == user.TypeEmpId);
                //     roleName = role.Name;
                // }
                var appUser = await _userManager.Users
                    .FirstOrDefaultAsync(u => u.NormalizedUserName == user.UserName);
                _userManager.AddToRoleAsync(appUser, roleName).Wait();

                if (res.Succeeded)
                {

                    var mail = new EmailFormDto();
                    mail.subject = "Compte confirmé";
                    mail.content = "<b> " + user.LastName + " " + user.FirstName + "</b>, votre compte a bien été enregistré";
                    mail.toEmail = user.Email;
                    await _repo.SendEmail(mail);
                    var userToReturn = _mapper.Map<UserForListDto>(user);
                    return Ok(new
                    {
                        token = await GenerateJwtToken(user),
                        user = userToReturn

                    });
                }
                return BadRequest("impossible de terminé l'action");
            }
            return NotFound();
        }

        [HttpGet("emailValidation/{code}")]
        public async Task<IActionResult> emailValidation(string code)
        {

            // int id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = await _repo.GetUserByCode(code);

            if (user != null)
            {
                if (user.EmailConfirmed == true)
                    return BadRequest("cet compte a déja été confirmé...");
                return Ok(new
                {
                    user = _mapper.Map<UserForListDto>(user)
                });
            }

            return BadRequest("ce lien ,'existe pas");

        }

        [HttpPost("SavePreInscription")]
        public async Task<IActionResult> SavePreInscription(UserForRegisterDto userForRegisterDto)
        {
            var userName = Guid.NewGuid();
            var userToCreate = _mapper.Map<User>(userForRegisterDto);
            userToCreate.UserName = userName.ToString();
            userToCreate.ValidationCode = userName.ToString();
            var result = await _userManager.CreateAsync(userToCreate, "password");

            // var userToReturn = _mapper.Map<UserForDetailedDto>(userToCreate);

            if (result.Succeeded)
            {
                return Ok(userToCreate.Id);
            }

            return BadRequest(result.Errors);

        }

        [HttpPost("AddUser/{insertUserId}")]
        public async Task<IActionResult> AddUser(UserForRegisterDto userForRegisterDto, int insertUserId)
        {
            var userName = Guid.NewGuid();
            var userToCreate = _mapper.Map<User>(userForRegisterDto);
            userToCreate.Version = 2;
            userToCreate.PreSelected = true;

            userToCreate.UserName = userName.ToString();
            userToCreate.ValidationCode = userName.ToString();
            var result = await _userManager.CreateAsync(userToCreate, "password");


            // var userToReturn = _mapper.Map<UserForDetailedDto>(userToCreate);

            if (result.Succeeded)
            {
                var userId = userToCreate.Id;
                if (userId < 10)
                {
                    userToCreate.Idnum = "0000" + userId;
                }
                else if (userId >= 10 && userId < 100)
                    userToCreate.Idnum = "000" + userId;
                else if (userId >= 100 && userId < 1000)
                    userToCreate.Idnum = "00" + userId;
                else if (userId >= 1000 && userId < 10000)
                    userToCreate.Idnum = "0" + userId;
                else
                    userToCreate.Idnum = userId.ToString();
                await _repo.SaveAll();

                return Ok(userToCreate.Id);
            }

            return BadRequest(result.Errors);

        }



        private async Task<string> GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenhandler = new JwtSecurityTokenHandler();

            var token = tokenhandler.CreateToken(tokenDescriptor);

            return tokenhandler.WriteToken(token);
        }

        private string GenerateJwtTokenForEmail(User user)
        {
            var claims = new List<Claim>
                    {
                                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                                new Claim(ClaimTypes.Name, user.UserName)
                            };



            var key = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(2),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    }
}