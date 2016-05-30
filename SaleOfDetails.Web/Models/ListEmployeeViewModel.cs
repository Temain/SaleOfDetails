using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SaleOfDetails.Web.Models
{
    public class ListEmployeeViewModel
    {
        public IEnumerable<EmployeeViewModel> Employees { get; set; }
        public int EmployeesCount { get; set; }
        public int PagesCount { get; set; }
        public int SelectedPage { get; set; }
    }
}