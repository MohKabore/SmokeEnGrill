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
        public string ValidationCode { get; set; }
        public DateTime? ValidationDate { get; set; }
        public bool ValidatedCode { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        public ICollection<UserRole> UserRoles { get; set; }
        public int ForgotPasswordCount { get; set; }
        public int ResetPasswordCount { get; set; }
        public byte TempData { get; set; }
        public string Idnum { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int? NbChild { get; set; }
        public string BirthPlace { get; set; }
        public string Cni { get; set; }
        public string Passport { get; set; }
        public string Iddoc { get; set; }
        public string Atnum { get; set; }
        public byte Paid { get; set; }
        public DateTime InsertDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public byte? Version { get; set; }
        // prelectionné mais pas encore séléctionné apres formation
        public bool PreSelected { get; set; }
        // sélectionné apres formation mais pas encore retenu definitivement(peut etre donc dans la reserve)
        public bool Selected { get; set; }
        // selectionné et affecté
        public bool Hired { get; set; }
        // en formation ou former
        public bool Trained { get; set; }
        // compte supprimé ou activé
        public bool Active { get; set; }
        public bool Reserved { get; set; }
        public bool OnTraining { get; set; }
        public ICollection<Photo> Photos { get; set; }
        public DateTime? ForgotPasswordDate { get; set; }
        public int Nok { get; set; }

    }
}