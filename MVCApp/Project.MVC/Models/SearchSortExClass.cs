using Project.MVC.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.MVC.Models
{
    public class SearchSortExClass : ISearchSortEx
    {
        public string CurrentSortOrder { get; set; }
        public string SearchFilter { get; set; }
        public string BrandSortParm { get; set; }
        public string NameSortParm { get; set; }
        public string AbrvSortParm { get; set; }
        public int SlectionPageSize { get; set; }

    }
}
