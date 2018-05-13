using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Website.Controllers
{
    public class ErrorController : Controller
    {
        [Route("error")]
        [Route("error/{code}")]
        public IActionResult Index(HttpStatusCode code = 0)
        {

            switch (code)
            {
                case HttpStatusCode.NotFound:
                    return View("404");

                case HttpStatusCode.Forbidden:
                case HttpStatusCode.Unauthorized:
                    return View("Unauthorized");

                case HttpStatusCode.BadRequest:
                case HttpStatusCode.BadGateway:
                case HttpStatusCode.GatewayTimeout:
                case HttpStatusCode.InternalServerError:
                default:
                    return View();
            }
        }
    }
}