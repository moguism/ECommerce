using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckoutController : ControllerBase
    {

        private readonly Settings _settings;

        public CheckoutController(Settings settings)
        {
            _settings = settings;
        }






    }
}
