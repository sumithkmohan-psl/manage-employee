using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageEmployee.Models.DTOs
{
    public class EmployeeListViewModel
    {
        public IEnumerable<EmployeeDto> EmployeeList { get; set; }
        public string Keyword { get; set; }
    }
}
