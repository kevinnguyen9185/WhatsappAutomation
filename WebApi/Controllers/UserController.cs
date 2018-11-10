using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApi.Business;
using WebApi.Business.Models;
using WebApi.ViewModels;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }
        // GET api/values/5
        [HttpGet]
        [Route("List")]
        public IActionResult ListUserAsync()
        {
            if(IsAuthorized())
            {
                return Ok(
                        new {
                            Users = new UserBusiness().GetUsersAsync()
                        });
            }
            return Unauthorized();
        }

        [HttpPost]
        [Route("Changepassword")]
        public IActionResult ChangepasswordAsync([FromBody] User updatedUser)
        {
            if(IsAuthorized())
            {
                new UserBusiness().ChangePassword(updatedUser.Password, updatedUser.Username);
                return Ok();
            }
            return Unauthorized();
        }

        [HttpPost]
        [Route("Newuser")]
        public IActionResult NewuserAsync([FromBody] User newUser)
        {
            if(IsAuthorized())
            {
                new UserBusiness().CreateUser(newUser.Username, newUser.Password);
                return Ok();
            }
            return Unauthorized();
        }

        public bool IsAuthorized()
        {
            var loginToken = Request.Headers.FirstOrDefault(f=>f.Key=="tk").Value;
            if(!string.IsNullOrEmpty(loginToken))
            {
                var user = new UserBusiness().GetUserAsyncByToken(loginToken);
                if(user!=null && user.Username == "admin")
                {
                    return true;
                }
            }
            return false;
        }
    }
}
