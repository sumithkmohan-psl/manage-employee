using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageEmployee.Models.Entities
{
    public class Title
    {
        public string Name { get; set; } = string.Empty;
        public decimal Min { get; set; }
        public decimal Max { get; set; }
    }
}
