using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.MVC.Interfaces
{
    public interface ISearchSortEx
    {
        string CurrentSortOrder { get; set; }
        string SearchFilter { get; set; }
        string BrandSortParm { get; set; }
        string NameSortParm { get; set; }
        string AbrvSortParm { get; set; }
        int SlectionPageSize { get; set; }
    }
}
