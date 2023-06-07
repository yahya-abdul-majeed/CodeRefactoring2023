using CLabManager_API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CLabManager_API.Controllers
{
    [Route("api/[controller]")]
    public class MailController : ControllerBase
    {
        private readonly IEmailService _emailService;
        public MailController(IEmailService emailService)
        {
            _emailService = emailService; 
        }
        [HttpPost]
        public IActionResult SendEmail([FromBody]string to)
        {
            _emailService.SendEmailTo(to);
            return Ok();
        }
    }
}
