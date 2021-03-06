using System;
using System.Collections.Generic;
using EducNotes.API.Models;
using SmokeEnGrill.API.Models;

namespace EducNotes.API.Dtos
{
    public class UserForDetailedDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string idNum { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public Boolean EmailConfirmed { get; set; }
        public int ClassLevelId { get; set; }
        public string ClassLevelName { get; set; }
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public byte Gender { get; set; }
        public string strGender { get; set; }
        public string strDateOfBirth { get; set; }
        public int Age { get; set; }
        public string KnownAs { get; set; }
        public string PhotoUrl { get; set; }
          public string PhoneNumber { get; set; }
          public Boolean PhoneNumberConfirmed { get; set; }
        public string SecondPhoneNumber { get; set; }
        public string UserTypeName { get; set; }
        public int UserTypeId { get; set; }
        public bool ValidatedCode { get; set; }
        public bool Validated { get; set; }
        public bool AccountDataValidated { get; set; }
        public int DistrictId { get; set; }
        public District District { get; set; }
        public string DistrictName { get; set; }
        public int CityId { get; set; }
        public City City { get; set; }
        public string CityName { get; set; }
        public double Avg { get; set; }
        public int NbAbsences { get; set; }
        public int NbLateArrivals { get; set; }
        public string strCreated { get; set; }
        private ICollection<Photo> photos;
        public ICollection<PhotosForDetailedDto> Photos { get; set; }
    }
}