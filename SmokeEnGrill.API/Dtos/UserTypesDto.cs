using EducNotes.API.Models;
using SmokeEnGrill.API.Models;

namespace EducNotes.API.Dtos
{
    public class UserTypesDto
    {
        public UserType UserType { get; set; }
        public int Total { get; set; }
        public int TotalActive { get; set; }
    }
}