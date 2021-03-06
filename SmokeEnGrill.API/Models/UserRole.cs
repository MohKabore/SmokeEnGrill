using EducNotes.API.Models;
using Microsoft.AspNetCore.Identity;

namespace SmokeEnGrill.API.Models
{
  public class UserRole : IdentityUserRole<int>
  {
    public User User { get; set; }
    public Role Role { get; set; }
  }
}