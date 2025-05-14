using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PROG7311_PART2_AgriEnergyConnect.Models
{
    public class Farmer
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
        [Display(Name = "Farmer Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Contact number is required")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Contact number must be 10 digits")]
        [Display(Name = "Contact Number")]
        public string ContactNumber { get; set; }

        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}