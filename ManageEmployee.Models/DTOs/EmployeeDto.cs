using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageEmployee.Models.DTOs
{
    public class EmployeeDto
    {
        public int EmployeeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SSN { get; set; } = string.Empty;

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime DOB { get; set; }
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Zip { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime JoinDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", NullDisplayText = "N/A")]
        public DateTime? ExitDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal Salary { get; set; }
        public string Title { get; set; } = string.Empty;

        [Display(Name="Address")]
        public string FullAddress =>
        $"{Address},<br/> {City},<br/> {State} {Zip}";
    }
}
