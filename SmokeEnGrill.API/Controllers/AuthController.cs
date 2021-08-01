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
using EducNotes.API.data;
using EducNotes.API.Dtos;
using System.Linq;

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
    private readonly ISmokeEnGrillRepository _repo;
    private DataContext _context;
    private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
    private Cloudinary _cloudinary;
    int parentRoleId, memberRoleId, moderatorRoleId, adminRoleId, teacherRoleId;
    int teacherTypeId, parentTypeId, studentTypeId, adminTypeId;
    int parentIsncrTypeId, schoolInscrTypeId, updateAccountEmailId, resetPwdEmailId;
    public ICacheRepository _cache { get; }

    public AuthController(IConfiguration config, IMapper mapper, ISmokeEnGrillRepository repo,
      UserManager<User> userManager, SignInManager<User> signInManager, DataContext context,
      IOptions<CloudinarySettings> cloudinaryConfig, ICacheRepository cache)
    {
      _cache = cache;
      _context = context;
      _repo = repo;
      _config = config;
      _mapper = mapper;
      _userManager = userManager;
      _signInManager = signInManager;
      teacherTypeId = _config.GetValue<int>("AppSettings:teacherTypeId");
      parentTypeId = _config.GetValue<int>("AppSettings:parentTypeId");
      adminTypeId = _config.GetValue<int>("AppSettings:adminTypeId");
      studentTypeId = _config.GetValue<int>("AppSettings:studentTypeId");
      parentRoleId = _config.GetValue<int>("AppSettings:parentRoleId");
      memberRoleId = _config.GetValue<int>("AppSettings:memberRoleId");
      moderatorRoleId = _config.GetValue<int>("AppSettings:moderatorRoleId");
      adminRoleId = _config.GetValue<int>("AppSettings:adminRoleId");
      teacherRoleId = _config.GetValue<int>("AppSettings:teacherRoleId");
      parentIsncrTypeId = _config.GetValue<int>("AppSettings:parentInscTypeId");
      schoolInscrTypeId = _config.GetValue<int>("AppSettings:schoolInscTypeId");
      updateAccountEmailId = _config.GetValue<int>("AppSettings:updateAccountEmailId");
      resetPwdEmailId = _config.GetValue<int>("AppSettings:resetPwdEmailId");

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
        user.Validated = true;
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

    [HttpPost("ForgotPassword")]
    public async Task<IActionResult> ForgotPassword(UserForResetPwdDto userData)
    {
      bool resetOK = false;
      var username = userData.Username;
      var email = userData.Email;
      // find user with username and email as inputs
      var user = await _repo.GetUserByEmailAndLogin(username, email);
      if (user != null)
      {
        if (!user.EmailConfirmed)
          return BadRequest("compte pas encore activé....");
        // send 'token for password reset' email
        string resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        if (await _repo.SendResetPasswordLink(user, resetToken))
        {
          // envoi effectué
          user.ForgotPasswordCount += 1;
          user.ForgotPasswordDate = DateTime.Now;
          if (await _repo.SaveAll())
          {
            resetOK = true;
            return Ok(resetOK);
          }
          else
          {
            return BadRequest("problème pour envoyer le email");
          }
        }

        return BadRequest("problème pour envoyer le email");
      }
      else
      {
        return Ok(resetOK);
      }
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
    {
      // RollbarLocator.RollbarInstance.Configure(new RollbarConfig("13e83bd7d4794996b0535c4990db269e"));
      // RollbarLocator.RollbarInstance.Info("Rollbar is configured properly.");

      // verification de l'existence du userName
      if (await _repo.UserNameExist(userForLoginDto.Username.ToLower(), 0))
      {
        var user = await _userManager.FindByNameAsync(userForLoginDto.Username.ToLower());
        if (!user.AccountDataValidated)
          return BadRequest("l'utilisateur n'a pas confirmé son email/mobile.");

        var result = await _signInManager.CheckPasswordSignInAsync(user, userForLoginDto.Password, true);

        if (result.Succeeded)
        {
          var appUser = await _userManager.Users.Include(p => p.Photos)
            .FirstOrDefaultAsync(u => u.NormalizedUserName == userForLoginDto.Username.ToUpper());

          var userToReturn = _mapper.Map<UserForListDto>(appUser);

          //get school settings
          var settings = await _repo.GetSettings();

          return Ok(new
          {
            token = await GenerateJwtToken(appUser),
            user = userToReturn,
            settings
          });
        }

        var lockedOut = false;
        if (result.IsLockedOut)
        {
          lockedOut = true;
          return Ok(new
          {
            userName = user.LastName + " " + user.FirstName,
            lockedOut
          });
        }
        // else
        // {
        //   return BadRequest("login ou mot de passe incorrect...");
        // }
      }

      var loginPwdFailed = true;
      return Ok(new
      {
        failed = loginPwdFailed
      });
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

    [HttpPost("LastValidation")]
    public async Task<IActionResult> LastValidation(int id, UserForUpdateDto userForUpdateDto)
    {
      var userToUpdate = await _userManager.FindByNameAsync(userForUpdateDto.UserName);
      if (userToUpdate != null)
      {
        userToUpdate.EmailConfirmed = true;
        _repo.Update(userToUpdate);

        if (await _repo.SaveAll())
          return base.Ok(new
          {
            token = GenerateJwtToken(userToUpdate).Result,
            user = _mapper.Map<UserForDetailedDto>(userToUpdate)
          });

        return BadRequest("impossible de mettre a jour ce compte");

      }
      return BadRequest("Compte introuvable");
    }

    [HttpGet("GetAllCities")]
    public async Task<IActionResult> GetAllCities()
    {
      return Ok(await _repo.GetAllCities());
    }

    [HttpGet("GetDistrictsByCityId/{id}")]
    public async Task<IActionResult> GetAllGetDistrictsByCityIdCities(int id)
    {
      return Ok(await _repo.GetAllGetDistrictsByCityIdCities(id));
    }

    [HttpGet("{email}/VerifyEmail")]
    public async Task<IActionResult> VerifyEmail(string email)
    {
      return Ok(await _repo.EmailExist(email));
    }

    [HttpGet("VerifyUserName/{currentUserId}/{userName}")]
    public async Task<IActionResult> VerifyUserName(string userName, int currentUserId)
    {
      return Ok(await _repo.UserNameExist(userName, currentUserId));
    }

    [HttpPost("{id}/setUserAccountData")]
    public async Task<IActionResult> setUserAccountData(int id, UserDataToUpdateDto userData)
    {
      // List<EmailTemplate> emailTemplates = await _cache.GetEmailTemplates();

      var user = await _repo.GetUser(id, false);
      if (user != null)
      {
        var newPassword = _userManager.PasswordHasher.HashPassword(user, userData.Pwd);
        user.UserName = userData.UserName.ToLower();
        user.NormalizedUserName = userData.UserName.ToUpper();
        user.PasswordHash = newPassword;
        user.AccountDataValidated = true;
        if (user.UserTypeId != studentTypeId)
          user.Validated = true;
        user.ValidationDate = DateTime.Now;
        var res = await _userManager.UpdateAsync(user);
        if (res.Succeeded)
        {
          // var template = emailTemplates.First(t => t.Id == updateAccountEmailId);
          // var email = await _repo.SetEmailForAccountUpdated(template.Subject, template.Body,
          //   user.LastName, user.Gender, user.Email, user.Id);
          // _context.Add(email);

          var userToReturn = _mapper.Map<UserForListDto>(user);
          await _cache.LoadUsers();
          return Ok(new {
            user = userToReturn
          });
        }

        return BadRequest("problème pour mettre à jour les données");
      }

      return NotFound();
    }

    [HttpPost("{userId}/AddPhotoForUser")]
    public async Task<IActionResult> AddPhotoForUser(int userId, [FromForm] PhotoForCreationDto photoForCreationDto)
    {
      var user = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(a => a.Id == userId);
      // user.TempData = 2;
      var file = photoForCreationDto.File;
      var uploadResult = new ImageUploadResult();

      if (file.Length > 0)
      {
        using (var stream = file.OpenReadStream())
        {
          var uploadParams = new ImageUploadParams()
          {
            File = new FileDescription(file.Name, stream),
            Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
          };

          uploadResult = _cloudinary.Upload(uploadParams);
        }
      }

      photoForCreationDto.Url = uploadResult.SecureUri.ToString();
      photoForCreationDto.PublicId = uploadResult.PublicId;

      var photo = _mapper.Map<Photo>(photoForCreationDto);

      if (!user.Photos.Any(u => u.IsMain))
      {
        photo.IsMain = true;
        photo.IsApproved = true;
      }
      user.Photos.Add(photo);

      if (await _repo.SaveAll())
        return Ok();

      return BadRequest("Could not add the photo");
    }

    [HttpGet("GetUser/{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
      var user = await _repo.GetUser(id, false);
      return Ok(user);
    }

    [HttpGet("LoginPageInfos")]
    public async Task<IActionResult> GetLoginPageInfos()
    {
      var infos = await _context.LoginPageInfos.OrderBy(o => o.Title).ToListAsync();
      return Ok(infos);
    }

    // [HttpPost("AddFile")]
    // public async Task<IActionResult> AddFile([FromForm]PhotoForCreationDto photoForCreationDto)
    // {
    //     var file = photoForCreationDto.File;

    //     var uploadResult = new ImageUploadResult();

    //     if (file.Length > 0)
    //     {
    //         using (var stream = file.OpenReadStream())
    //         {
    //             var uploadParams = new ImageUploadParams()
    //             {
    //                 File = new FileDescription(file.Name, stream)
    //             };

    //             uploadResult = _cloudinary.Upload(uploadParams);
    //         }
    //     }

    //     photoForCreationDto.Url = uploadResult.SecureUri.ToString();
    //     photoForCreationDto.PublicId = uploadResult.PublicId;


    //     var fichier = _mapper.Map<Fichier>(photoForCreationDto);
    //     fichier.Description = photoForCreationDto.File.FileName;
    //     _repo.Add(fichier);

    //     if (await _repo.SaveAll())
    //         return Ok();

    //     return BadRequest("Could not add the photo");
    // }

    // [HttpGet("GetAllFiles")]
    // public async Task<IActionResult> GetAllFiles()
    // {
    //     return Ok(await _context.Fichiers.ToListAsync());
    // }
  }
}