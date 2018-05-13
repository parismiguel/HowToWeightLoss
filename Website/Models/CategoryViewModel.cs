using ButterCMS.Models;

namespace Website.Models
{
    public class CategoryViewModel
    {
        public Category Category{ get; set; }
        public BlogListViewModel Blogs { get; set; }
    }
}
