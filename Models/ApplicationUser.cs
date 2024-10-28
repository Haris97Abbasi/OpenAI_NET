using Microsoft.AspNetCore.Identity;

namespace OpenAiProject.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? Name { get; set; }
    }
}
