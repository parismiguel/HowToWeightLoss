using ButterCMS;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Website.Configuration;

namespace Website
{
    public class Startup
    {
        public IHostingEnvironment HostingEnvironment { get; }

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            HostingEnvironment = env;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddResponseCaching();
            services.AddMvc(options =>
            {
                options.CacheProfiles.Add("7days", new CacheProfile
                {
                    Duration = 604800,
                    Location = ResponseCacheLocation.Any,
                    VaryByQueryKeys = new[] { "*" },
                    VaryByHeader = "Accept, Accept-Language, Accept-Charset, User-Agent"
                });
                options.CacheProfiles.Add("2days", new CacheProfile
                {
                    Duration = 172800,
                    Location = ResponseCacheLocation.Any,
                    VaryByQueryKeys = new[] { "*" },
                    VaryByHeader = "Accept, Accept-Language, Accept-Charset, User-Agent"
                });
                options.CacheProfiles.Add("1days", new CacheProfile
                {
                    Duration = 86400,
                    Location = ResponseCacheLocation.Any,
                    VaryByQueryKeys = new[] { "*" },
                    VaryByHeader = "Accept, Accept-Language, Accept-Charset, User-Agent"
                });
            });
            
            services.AddSingleton<IMemoryCache>(new MemoryCache(new MemoryCacheOptions()));

            services.AddSingleton<IHostingEnvironment>(HostingEnvironment);

            services.AddOptions();
            services.Configure<UrlOptions>(Configuration.GetSection("UrlOptions"));
            services.Configure<ButterCmsOptions>(Configuration.GetSection("ButterCMSOptions"));

            services.AddScoped<ButterCMSClient>(c =>
                new ButterCMSClient(c.GetRequiredService<IOptions<ButterCmsOptions>>().Value.ApiKey));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseStatusCodePagesWithReExecute("/error/{0}");

                var options = new RewriteOptions()
                    .AddRedirectToNonWww()
                    .AddRedirectToHttpsPermanent();
                app.UseRewriter(options);
            }


            app.UseResponseCaching();
            app.UseStaticFiles();
            app.UseMvc();
        }
    }
}
