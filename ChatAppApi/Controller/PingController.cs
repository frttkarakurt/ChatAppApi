using Microsoft.AspNetCore.Mvc;

namespace ChatAppApi.Controllers // 'Controller' yerine 'Controllers' yapıldı
{
    [ApiController]
    [Route("api/[controller]")]
    public class PingController : ControllerBase
    {
        [HttpGet]
        public string Get() => "Pong!";
    }
}