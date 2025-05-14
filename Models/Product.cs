using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PROG7311_PART2_AgriEnergyConnect.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        [StringLength(100, ErrorMessage = "Product name cannot be longer than 100 characters")]
        [Display(Name = "Product Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Category is required")]
        [StringLength(50, ErrorMessage = "Category cannot be longer than 50 characters")]
        public string Category { get; set; }

        [Required(ErrorMessage = "Production date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Production Date")]
        [DateNotInFuture(ErrorMessage = "Production date cannot be in the future")]
        public DateTime ProductionDate { get; set; }

        [ForeignKey("Farmer")]
        public int FarmerId { get; set; }

        [Display(Name = "Farmer")]
        public virtual Farmer Farmer { get; set; }
    }

    public class DateNotInFutureAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is DateTime date)
            {
                return date <= DateTime.Today;
            }
            return false;
        }
    }
}