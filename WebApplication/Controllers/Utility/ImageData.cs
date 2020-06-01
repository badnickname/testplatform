using Microsoft.AspNetCore.Http;

namespace WebApplication.Controllers
{
    public class ImageData
    {
        public IFormFile Image { get; set; }
    }
}