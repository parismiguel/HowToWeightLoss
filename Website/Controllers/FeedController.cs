using System.IO;
using System.Threading.Tasks;
using System.Xml;
using ButterCMS;
using Microsoft.AspNetCore.Mvc;

namespace Website.Controllers
{
    public class FeedController : Controller
    {
        public ButterCMSClient Client { get; }

        public FeedController(ButterCMSClient client)
        {
            Client = client;
        }

        [Route("feeds/rss")]
        [ResponseCache(CacheProfileName = "1days")]
        public async Task<ActionResult> ShowRss()
        {
            var xmlDoc = await Client.GetRSSFeedAsync();
            using (var sw = new StringWriter())
            {
                using (var xw = XmlWriter.Create(sw))
                {
                    xmlDoc.WriteTo(xw);
                    xw.Flush();
                    return Content(sw.GetStringBuilder().ToString(), "text/xml");
                }
            }
        }

        [Route("feeds/atom")]
        [ResponseCache(CacheProfileName = "1days")]
        public async Task<ActionResult> ShowAtom()
        {
            var xmlDoc = await Client.GetAtomFeedAsync();
            using (var sw = new StringWriter())
            {
                using (var xw = XmlWriter.Create(sw))
                {
                    xmlDoc.WriteTo(xw);
                    xw.Flush();
                    return Content(sw.GetStringBuilder().ToString(), "text/xml");
                }
            }
        }

        [Route("sitemap")]
        [Route("sitemap.xml")]
        [ResponseCache(CacheProfileName = "1days")]
        public async Task<ActionResult> ShowSitemap()
        {
            var xmlDoc = await Client.GetSitemapAsync();
            using (var sw = new StringWriter())
            {
                using (var xw = XmlWriter.Create(sw))
                {
                    xmlDoc.WriteTo(xw);
                    xw.Flush();
                    return Content(sw.GetStringBuilder().ToString(), "text/xml");
                }
            }
        }
    }
}