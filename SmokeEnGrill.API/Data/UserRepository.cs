using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EducNotes.API.data;
using EducNotes.API.Dtos;
using EducNotes.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SmokeEnGrill.API.Helpers;
using SmokeEnGrill.API.Models;

namespace SmokeEnGrill.API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private int resetPwdEmailId, broadcastTokenTypeId;
        private readonly IAdminRepository _adminRepo;
        private readonly IPhotoRepository _photoRepo;
        private readonly ICacheRepository _cache;
        private readonly string baseUrl;
        public UserRepository(DataContext context, IConfiguration config, UserManager<User> userManager,
            IMapper mapper, IAdminRepository adminRepo, ICacheRepository cache, IPhotoRepository photoRepo)
        {
            _context = context;
            _config = config;
            _mapper = mapper;
            _config = config;
            _cache = cache;
            _adminRepo = adminRepo;
            _photoRepo = photoRepo;
            _userManager = userManager;
        }

        public void Add<T>(T entity) where T : class
        {
            throw new System.NotImplementedException();
        }

        public void Delete<T>(T entity) where T : class
        {
            throw new System.NotImplementedException();
        }

        public void DeleteAll<T>(List<T> entities) where T : class
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> SaveAll()
        {
            throw new System.NotImplementedException();
        }

        public void Update<T>(T entity) where T : class
        {
            throw new System.NotImplementedException();
        }

        public string GetUserIDNumber(int userId, string lastName, string firstName)
        {
            int randomVal = 300631;
            int val = userId * 2 + randomVal;
            // string idNum = lastName.Substring(0, 1).ToUpper() + firstName.Substring(0,1).ToUpper() + val.ToString().To5Digits();
            string idNum = val.ToString().To5Digits();
            return idNum;
        }

        public async Task<bool> EditUserAccount(UserAccountForEditDto user)
        {
            List<User> users = await _cache.GetUsers();
            bool resultStatus = false;
            using (var identityContextTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    User appUser = users.FirstOrDefault(u => u.Id == user.Id);
                    appUser.LastName = user.LastName;
                    appUser.FirstName = user.FirstName;
                    appUser.Gender = user.Gender;
                    var dateArray = user.strDateOfBirth.Split("/");
                    int year = Convert.ToInt32(dateArray[2]);
                    int month = Convert.ToInt32(dateArray[1]);
                    int day = Convert.ToInt32(dateArray[0]);
                    DateTime birthDay = new DateTime(year, month, day);
                    appUser.DateOfBirth = birthDay;
                    appUser.SecondPhoneNumber = user.SecondPhoneNumber;
                    if (user.CityId > 0)
                        appUser.CityId = user.CityId;
                    if (user.DistrictId > 0)
                        appUser.DistrictId = user.DistrictId;

                    //add user photo
                    var photoFile = user.PhotoFile;
                    if (photoFile != null)
                    {
                        resultStatus = await _photoRepo.AddUserPhoto(appUser, photoFile);
                    }
                    else
                    {
                        resultStatus = true;
                    }

                    if (photoFile != null && await SaveAll())
                    {
                        var result = await _userManager.UpdateAsync(appUser);
                        if (result.Succeeded)
                        {
                            // fin de la transaction
                            identityContextTransaction.Commit();
                            await _cache.LoadUsers();
                            resultStatus = true;
                        }
                        else
                        {
                            identityContextTransaction.Rollback();
                            resultStatus = false;
                        }
                    }
                    else
                    {
                        var result = await _userManager.UpdateAsync(appUser);
                        if (result.Succeeded)
                        {
                            // fin de la transaction
                            identityContextTransaction.Commit();
                            await _cache.LoadUsers();
                            resultStatus = true;
                        }
                        else
                        {
                            identityContextTransaction.Rollback();
                            resultStatus = false;
                        }
                    }
                }
                catch
                {
                    identityContextTransaction.Rollback();
                    return resultStatus = false;
                }
            }
            return resultStatus;
        }

        public async Task<bool> AddEmployee(EmployeeForEditDto user)
        {
            List<User> employees = await _cache.GetEmployees();
            List<UserRole> userRoles = await _cache.GetUserRoles();

            using(var identityContextTransaction = _context.Database.BeginTransaction())
            {
                string publicId = "";
                Boolean photoExists = false;
                try
                {
                    User appUser = new User();

                    var dateArray = user.strDateOfBirth.Split("/");
                    int year = Convert.ToInt32(dateArray[2]);
                    int month = Convert.ToInt32(dateArray[1]);
                    int day = Convert.ToInt32(dateArray[0]);
                    DateTime birthDay = new DateTime(year, month, day);

                    //is it a new user
                    if (user.Id == 0)
                    {
                        var userToSave = _mapper.Map<User>(user);
                        string strDoB = user.strDateOfBirth;
                        userToSave.DateOfBirth = birthDay;
                        var code = Guid.NewGuid();
                        userToSave.UserName = code.ToString();
                        userToSave.Validated = false;
                        userToSave.EmailConfirmed = false;

                        var result = await _userManager.CreateAsync(userToSave, Guid.NewGuid().ToString());
                        if (result.Succeeded)
                        {
                        appUser = await _userManager.Users.Include(i => i.Photos)
                                                            .FirstOrDefaultAsync(u => u.NormalizedUserName == userToSave.UserName.ToUpper());
                        appUser.IdNum = GetUserIDNumber(appUser.Id, appUser.LastName, appUser.FirstName);
                        await _userManager.UpdateAsync(appUser);
                        }
                    }
                    else
                    {
                        appUser = employees.First(u => u.Id == user.Id);
                        appUser.LastName = user.LastName;
                        appUser.FirstName = user.FirstName;
                        appUser.Gender = user.Gender;
                        appUser.DateOfBirth = birthDay;
                        appUser.PhoneNumber = user.PhoneNumber;
                        appUser.SecondPhoneNumber = user.SecondPhoneNumber;
                        appUser.Email = user.Email;
                        appUser.DistrictId = user.DistrictId;
                        Update(appUser);

                        //delete previous employee roles
                        List<UserRole> prevRoles = userRoles.Where(c => c.UserId == appUser.Id).ToList();
                        DeleteAll(prevRoles);
                    }

                    if (user.Roles != null)
                    {
                        List<string> rolesList = user.Roles.Split(",").ToList();
                        if (rolesList.Count() > 0)
                        {
                            List<Role> roles = await _context.Roles.ToListAsync();
                            IEnumerable<string> roleNames = roles.Where(r => rolesList.Contains(r.Name)).Select(r => r.Name);
                            var result = _userManager.AddToRolesAsync(appUser, roleNames);
                            if(!result.Result.Succeeded)
                            {
                                identityContextTransaction.Rollback();
                                return false;
                            }
                        }
                    }

                    //add user photo
                    var photoFile = user.PhotoFile;
                    if(photoFile != null)
                    {
                        Boolean photoOK = await _photoRepo.AddUserPhoto(appUser, photoFile);
                    }

                    // send the mail to update userName/pwd - add to Email table
                    if(appUser.Email != null)
                    {
                    }

                    await SaveAll();
                    identityContextTransaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    var dd = ex.Message;
                    identityContextTransaction.Rollback();
                    if(photoExists)
                        _photoRepo.DeletePhotoFromCloudinary(publicId);
                    return false;
                }
                finally
                {
                    await _cache.LoadUsers();
                    await _cache.LoadUserRoles();
                    await _cache.LoadPhotos();
                }
            }
        }

    }
}