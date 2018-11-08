using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApi.Business;
using WebApi.ViewModels;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILogger<LoginController> _logger;
        public LoginController(ILogger<LoginController> logger)
        {
            _logger = logger;
        }
        // GET api/values/5
        [HttpPost]
        public IActionResult Login([FromBody] User user)
        {
            _logger.LogInformation($"Call login for user {user.Username}");
            return Ok(
                new {
                    LoginToken = new LoginBusiness().IsLogin(user.Username, user.Password)
                });
        }
    }
}
