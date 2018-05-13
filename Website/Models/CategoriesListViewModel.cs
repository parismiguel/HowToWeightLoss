using System.Collections.Generic;
using ButterCMS.Models;

namespace Website.Models
{
    public class CategoriesListViewModel
    {
        public List<Category> Categories { get; set; }
        public int Count { get; set; }
    }
}