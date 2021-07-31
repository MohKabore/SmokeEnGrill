using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace SmokeEnGrill.API.Models
{
    public class User : IdentityUser<int>
    {
        public User()
        {
            Active = true;
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public byte Gender { get; set; }
        public string SecondPhoneNumber { get; set; }
        public int DistrictId { get; set; }
        public District District { get; set; }
        public DateTime? ValidationDate { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        public int ForgotPasswordCount { get; set; }
        public int ResetPasswordCount { get; set; }
        public string Idnum { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int NbChild { get; set; } = 0;
        public string Cni { get; set; }
        public string Passport { get; set; }
        public string Iddoc { get; set; }
        public byte Paid { get; set; }
        public bool Hired { get; set; }
        public bool Trained { get; set; }
        public bool Active { get; set; }
        public bool OnTraining { get; set; }
        public DateTime? ForgotPasswordDate { get; set; }
        public int UserTypeId { get; set; }
        public UserType UserType { get; set; }
        public DateTime InsertDate { get; set; }
        public int InsertUserId { get; set; }
        public User InsertUser { get; set; }
        public DateTime UpdateDate { get; set; }
        public int UpdateUserId { get; set; }
        public User UpdateUser { get; set; }
        public string Version { get; set; }
        public Boolean Validated { get; set; }
        public Boolean AccountDataValidated { get; set; }
        public ICollection<UserRole> UserRoles { get; set; }
        public ICollection<Photo> Photos { get; set; }
    }
}