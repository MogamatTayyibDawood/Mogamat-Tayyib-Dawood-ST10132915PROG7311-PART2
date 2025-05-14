using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PROG7311_PART2_AgriEnergyConnect.Models
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "User Role")]
        public string Role { get; set; }

        [Display(Name = "Farmer Name")]
        public string? FarmerName { get; set; }

        [Display(Name = "Contact Number")]
        public string? ContactNumber { get; set; }

        public List<SelectListItem> RoleItems { get; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "Farmer", Text = "Farmer" },
            new SelectListItem { Value = "Employee", Text = "Employee" }
        };
    }
}