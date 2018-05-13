using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ButterCMS;
using ButterCMS.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Website.Configuration;
using Website.Models;

namespace Website.Controllers
{
    public class AuthorController : BaseController
    {
        /// <inheritdoc />
        public AuthorController(IHostingEnvironment hostingEnvironment, IOptions<UrlOptions> urlOptions, IOptions<ButterCmsOptions> siteOptions, ButterCMSClient client, IMemoryCache cache) : base(hostingEnvironment, urlOptions, siteOptions, client, cache)
        {
        }

        [Route("authors")]
        [ResponseCache(CacheProfileName = "2days")]
        public async Task<ActionResult> ListAllAuthors()
        {

            var response = (await Cache.GetOrCreateAsync("authors|all", async entry =>
            {
                entry.Value = (await Client.ListAuthorsAsync()).ToList();
                entry.AbsoluteExpiration = DateTimeOffset.Now.AddDays(2);
                return (IEnumerable<Author>)entry.Value;
            })).ToList();
            
            var model = new AuthorListViewModel
            {
                Authors = response,
                Count = response.Count
            };

            return View(model);
        }

        [Route("author/{slug}")]
        [Route("author/{slug}/p/{page}")]
        [ResponseCache(CacheProfileName = "2days")]
        public async Task<ActionResult> AuthorDetail(string slug, int page = 1)
        {
            var postsPerPage = 10;
            
            var authorResponse = (await Cache.GetOrCreateAsync($"author|by-slug|{slug}", async entry =>
            {
                entry.Value = await Client.RetrieveAuthorAsync(slug);
                entry.AbsoluteExpiration = DateTimeOffset.Now.AddDays(2);
                return (Author)entry.Value;
            }));
            var postsResponse = await Cache.GetOrCreateAsync($"posts|by-author|{slug}|{postsPerPage}|{page}", async entry =>
            {
                entry.Value = (await Client.ListPostsAsync(page, postsPerPage, true, categorySlug: slug));
                entry.AbsoluteExpiration = DateTimeOffset.Now.AddDays(2);
                return (PostsResponse)entry.Value;
            });
            
            var blogs = new BlogListViewModel
            {
                Posts = postsResponse.Data,
                Count = postsResponse.Meta.Count,
                NextPage = postsResponse.Meta.NextPage,
                CurrentPage = page,
                PreviousPage = postsResponse.Meta.PreviousPage,
                TotalPages = Convert.ToInt32(Math.Floor(decimal.Divide(postsResponse.Meta.Count, postsPerPage)))
            };

            var model = new AuthorViewModel
            {
                Author = authorResponse,
                Blogs = blogs
            };
            return View(model);
        }
    }
}