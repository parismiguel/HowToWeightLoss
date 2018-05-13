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
    public class TaxonomyController : BaseController
    {
        /// <inheritdoc />
        public TaxonomyController(IHostingEnvironment hostingEnvironment, IOptions<UrlOptions> urlOptions, IOptions<ButterCmsOptions> siteOptions, ButterCMSClient client, IMemoryCache cache) : base(hostingEnvironment, urlOptions, siteOptions, client, cache)
        {
        }

        [Route("categories")]
        [ResponseCache(CacheProfileName = "2days")]
        public async Task<ActionResult> ListAllCategories()
        {
            var response = (await Cache.GetOrCreateAsync("categories|all", async entry =>
            {
                entry.Value = (await Client.ListCategoriesAsync()).ToList();
                entry.AbsoluteExpiration = DateTimeOffset.Now.AddDays(2);
                return (IEnumerable<Category>)entry.Value;
            })).ToList();

            var model = new CategoriesListViewModel
            {
                Categories = response,
                Count = response.Count
            };

            return View(model);
        }

        [Route("category/{slug}")]
        [Route("category/{slug}/p/{page}")]
        [ResponseCache(CacheProfileName = "2days")]
        public async Task<ActionResult> CategoryDetail(string slug, int page = 1)
        {
            var postsPerPage = 10;

            var categoryResponse = (await Cache.GetOrCreateAsync($"category|by-slug|{slug}", async entry =>
            {
                entry.Value = await Client.RetrieveCategoryAsync(slug);
                entry.AbsoluteExpiration = DateTimeOffset.Now.AddDays(2);
                return (Category) entry.Value;
            }));
            var postsResponse = await Cache.GetOrCreateAsync($"posts|by-category|{slug}|{postsPerPage}|{page}", async entry =>
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

            var model = new CategoryViewModel
            {
                Category = categoryResponse,
                Blogs = blogs
            };
            return View(model);
        }

        [Route("tags")]
        [ResponseCache(CacheProfileName = "2days")]
        public async Task<ActionResult> ListAllTags()
        {
            var response = (await Cache.GetOrCreateAsync("tags|all", async entry =>
            {
                entry.Value = (await Client.ListTagsAsync()).ToList();
                entry.AbsoluteExpiration = DateTimeOffset.Now.AddDays(1);
                return (IEnumerable<Tag>)entry.Value;
            })).ToList(); 

            var model = new TagsListViewModel
            {
                Tags = response,
                Count = response.Count
            };

            return View(model);
        }

        [Route("tag/{slug}")]
        [Route("tag/{slug}/p/{page}")]
        [ResponseCache(CacheProfileName = "2days")]
        public async Task<ActionResult> TagDetail(string slug, int page = 1)
        {
            var postsPerPage = 10;
            
            var tagResponse = (await Cache.GetOrCreateAsync($"tag|by-slug|{slug}", async entry =>
            {
                entry.Value = await Client.RetrieveTagAsync(slug);
                entry.AbsoluteExpiration = DateTimeOffset.Now.AddDays(2);
                return (Tag)entry.Value;
            }));
            var postsResponse = await Cache.GetOrCreateAsync($"posts|by-tag|{slug}|{postsPerPage}|{page}", async entry =>
            {
                entry.Value = (await Client.ListPostsAsync(page, postsPerPage, true, tagSlug: slug));
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

            var model = new TagViewModel
            {
                Tag = tagResponse,
                Blogs = blogs
            };
            return View(model);
        }
    }
}