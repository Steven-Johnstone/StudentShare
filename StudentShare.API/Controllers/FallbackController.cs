using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace StudentShare.API.Controllers
{
    public class FallbackController : Controller
    {
        public IActionResult Index()
        { // tells the API server what to do if it cant find a route - goes to a fallback of index.html
            return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(),
                "wwwroot", "index.html"), "text/HTML");
        }
    }
}