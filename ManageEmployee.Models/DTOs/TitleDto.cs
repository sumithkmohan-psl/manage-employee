using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.Models.DTOs
{
    public class TitleDto
    {
        [Display(Name ="Title")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Minimum Salary")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal Min { get; set; }

        [Display(Name = "Maximum Salary")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal Max { get; set; }
    }
}
