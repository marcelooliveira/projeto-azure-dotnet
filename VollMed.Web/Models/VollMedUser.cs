using Microsoft.AspNetCore.Identity;

namespace VollMed.Web.Models
{
    public class VollMedUser : IdentityUser
    {
        public string? RefreshToken { get; set; }
        public DateTime? ExpireTime { get; set; }
    }
}
