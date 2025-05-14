namespace PROG7311_PART2_AgriEnergyConnect.Models
{
    using Microsoft.AspNetCore.Identity;

    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; } 
    }
}
