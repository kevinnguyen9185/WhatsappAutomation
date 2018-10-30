using System;

namespace WebApi.Business.Models
{
    public class Users:Base
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public DateTime LastDateLogin { get; set; }
        public string LoginToken { get; set; }
    }
}