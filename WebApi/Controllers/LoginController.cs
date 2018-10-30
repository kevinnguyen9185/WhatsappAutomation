using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApi.Business;
using WebApi.ViewModels;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        // GET api/values/5
        [HttpPost]
        public IActionResult Login([FromBody] User user)
        {
            return Ok(
                new {
                    LoginToken = new LoginBusiness().IsLogin(user.Username, user.Password)
                });
        }
    }
}
