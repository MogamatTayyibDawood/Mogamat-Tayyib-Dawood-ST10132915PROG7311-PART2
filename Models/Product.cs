using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;

namespace PROG7311_PART2_AgriEnergyConnect.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public string Category { get; set; }

        [Required(ErrorMessage = "Production date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Production Date")]
        [CustomValidation(typeof(Product), nameof(ValidateProductionDate))]
        public DateTime ProductionDate { get; set; }

        [Required(ErrorMessage = "Farmer is required")]
        [HiddenInput(DisplayValue = false)]
        public int FarmerId { get; set; }

        [ForeignKey("FarmerId")]
        public Farmer Farmer { get; set; }




        public static ValidationResult? ValidateProductionDate(DateTime date, ValidationContext context)
        {
            if (date > DateTime.Today)
            {
                return new ValidationResult("Production date cannot be in the future.");
            }
            return ValidationResult.Success;
        }
    }
}
