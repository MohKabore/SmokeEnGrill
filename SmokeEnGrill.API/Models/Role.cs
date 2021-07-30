using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using SmokeEnGrill.API.Models;

namespace EducNotes.API.Models
{
  public class Role : IdentityRole<int>
  {
    public ICollection<UserRole> UserRoles { get; set; }
  }
}