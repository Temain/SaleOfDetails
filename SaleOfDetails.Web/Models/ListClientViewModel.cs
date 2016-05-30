using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SaleOfDetails.Web.Models
{
    public class ListViewModel<T> where T : class
    {
        public IEnumerable<T> Items { get; set; }
        public int ItemsCount { get; set; }
        public int PagesCount { get; set; }
        public int SelectedPage { get; set; }
    }
}