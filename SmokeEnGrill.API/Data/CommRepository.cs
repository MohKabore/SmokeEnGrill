using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmokeEnGrill.API.Models;
using EducNotes.API.Dtos;
using System;
using System.Web;
using EducNotes.API.data;
using System.Linq;
using SmokeEnGrill.API.Helpers;

namespace SmokeEnGrill.API.Data
{
    public class CommRepository : ICommRepository
    {
        private readonly DataContext _context;
        private ICacheRepository _cache;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private int resetPwdEmailId, broadcastTokenTypeId;
        private readonly IAdminRepository _adminRepo;
        private readonly string baseUrl;
        private readonly int employeeConfirmEmailId;
        public CommRepository(DataContext context, IConfiguration config, UserManager<User> userManager,
            IMapper mapper, IAdminRepository adminRepo, ICacheRepository cache)
        {
            _context = context;
            _cache = cache;
            _config = config;
            _mapper = mapper;
            _config = config;
            _adminRepo = adminRepo;
            _userManager = userManager;
            resetPwdEmailId = _config.GetValue<int>("AppSettings:resetPwdEmailId");
            broadcastTokenTypeId = _config.GetValue<int>("AppSettings:broadcastTokenTypeId");
            baseUrl = _config.GetValue<String>("AppSettings:DefaultLink");
            employeeConfirmEmailId = _config.GetValue<int>("AppSettings:employeeConfirmEmailId");
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

        public async Task<bool> SendResetPasswordLink(User user, string token)
        {
        var template = await _context.EmailTemplates.FirstAsync(t => t.Id == resetPwdEmailId);
        var email = await SetEmailForResetPwdLink(template.Subject, template.Body, user.Id,
            user.LastName, user.FirstName, user.Gender, user.Email, token);
        _context.Add(email);

        if (!await SaveAll())
            return false;
        else
            return true;
        }

        public async Task<Email> SetEmailForResetPwdLink(string subject, string content, int userId, string lastName,
        string firstName, byte gender, string userEmail, string resetToken)
        {
        var tokens = await _adminRepo.GetTokens();

        Email newEmail = new Email();
        newEmail.EmailTypeId = 1;
        newEmail.ToAddress = userEmail;
        newEmail.FromAddress = "no-reply@smoke-grill.com";
        newEmail.Subject = subject;
        List<TokenDto> tags = GetResetPwdLinkTokenValues(tokens, userId, lastName, firstName, gender, resetToken);
        newEmail.Body = _adminRepo.ReplaceTokens(tags, content);
        newEmail.InsertUserId = 1;
        newEmail.InsertDate = DateTime.Now;
        newEmail.UpdateUserId = 1;
        newEmail.UpdateDate = DateTime.Now;
        newEmail.ToUserId = userId;

        return newEmail;
        }

        public List<TokenDto> GetResetPwdLinkTokenValues(IEnumerable<Token> tokens, int userId, string lastName,
        string firstName, byte gender, string resetToken)
        {
            var subDomain = _adminRepo.GetAppSubDomain();
            List<TokenDto> tokenValues = new List<TokenDto>();

            foreach (var token in tokens)
            {
                TokenDto td = new TokenDto();
                td.TokenString = token.TokenString;
                switch (td.TokenString)
                {
                case "<N_USER>":
                    td.Value = lastName;
                    break;
                case "<P_USER>":
                    td.Value = firstName;
                    break;
                case "<M_MME>":
                    td.Value = gender == 0 ? "Mme" : "M.";
                    break;
                case "<SUBDOMAIN>":
                    td.Value = subDomain == "" ? "" : subDomain + ".";
                    break;
                case "<CONFIRM_LINK>":
                    string url = "";
                    if (subDomain != "")
                    url = string.Format(baseUrl, subDomain + ".");
                    else
                    url = string.Format(baseUrl, "");
                    td.Value = string.Format("{0}/resetPassword?id={1}&token={2}", url, userId, HttpUtility.UrlEncode(resetToken));
                    break;
                default:
                    break;
                }

                if(td.Value != null)
                tokenValues.Add(td);
            }

            return tokenValues;
        }

        public async Task<bool> SendEmployeeConfirmEmail(User user)
        {
            List<EmailTemplate> emailTemplates = await _cache.GetEmailTemplates();

            ConfirmEmployeeEmailDto emailData = new ConfirmEmployeeEmailDto()
            {
                Id = user.Id,
                LastName = user.LastName,
                FirstName = user.FirstName,
                Cell = user.PhoneNumber,
                Gender = user.Gender,
                Email = user.Email,
                Token = await _userManager.GenerateEmailConfirmationTokenAsync(user)
            };

            var template = emailTemplates.First(t => t.Id == employeeConfirmEmailId);
            Email emailToSend = await SetDataForConfirmEmployeeEmail(emailData, template.Body, template.Subject);
            Add(emailToSend);

            return true;
        }

        public async Task<Email> SetDataForConfirmEmployeeEmail(ConfirmEmployeeEmailDto emailData, string content, string subject)
        {
        List<Setting> settings = await _cache.GetSettings();

        var tokens = await _adminRepo.GetTokens();
        var schoolName = settings.First(s => s.Name == "SchoolName").Value;
        Email newEmail = new Email();
        newEmail.EmailTypeId = 1;
        newEmail.ToAddress = emailData.Email;
        newEmail.FromAddress = "no-reply@educnotes.com";
        newEmail.Subject = subject.Replace("<NOM_ECOLE>", schoolName);
        List<TokenDto> tags = GetEmployeeEmailTokenValues(tokens, emailData);
        newEmail.Body = _adminRepo.ReplaceTokens(tags, content);
        newEmail.InsertUserId = 1;
        newEmail.InsertDate = DateTime.Now;
        newEmail.UpdateUserId = 1;
        newEmail.UpdateDate = DateTime.Now;
        newEmail.ToUserId = emailData.Id;

        return newEmail;
        }

        public List<TokenDto> GetEmployeeEmailTokenValues(List<Token> tokens, ConfirmEmployeeEmailDto emailData)
        {
        List<TokenDto> tokenValues = new List<TokenDto>();

        foreach (var token in tokens)
        {
            TokenDto td = new TokenDto();
            td.TokenString = token.TokenString;
            var subDomain = _adminRepo.GetAppSubDomain();
            string teacherId = emailData.Id.ToString();

            switch (td.TokenString)
            {
            case "<N_USER>":
                td.Value = emailData.LastName.FirstLetterToUpper();
                break;
            case "<P_USER>":
                td.Value = emailData.FirstName.FirstLetterToUpper();
                break;
            case "<M_MME>":
                td.Value = emailData.Gender == 0 ? "Mme" : "M.";
                break;
            case "<CELL_USER>":
                td.Value = emailData.Cell;
                break;
            case "<EMAIL_USER>":
                td.Value = emailData.Email;
                break;
            case "<TOKEN>":
                td.Value = emailData.Token;
                break;
            case "<ROLE>":
                td.Value = emailData.Role;
                break;
            case "<CONFIRM_LINK>":
                string url = "";
                if (subDomain != "")
                url = string.Format(baseUrl, subDomain + ".");
                else
                url = string.Format(baseUrl, "");
                td.Value = string.Format("{0}/confirmUserEmail?id={1}&token={2}", url,
                teacherId, HttpUtility.UrlEncode(emailData.Token));
                break;
            default:
                break;
            }

            if(td.Value != null)
            tokenValues.Add(td);
        }

        return tokenValues;
        }

    }
}