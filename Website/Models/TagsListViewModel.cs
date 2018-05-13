using System.Collections.Generic;
using ButterCMS.Models;

namespace Website.Models
{
    public class TagsListViewModel
    {
        public IEnumerable<Tag> Tags { get; set; }
        public int Count { get; set; }
    }
}