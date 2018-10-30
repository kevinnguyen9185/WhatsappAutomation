using System;
using LiteDB;
using WebApi.Business.Models;

namespace WebApi.Business
{
    public class LoginBusiness: BaseBusiness
    {
        public string IsLogin(string username, string passwowrd)
        {
            using(var db = new LiteRepository(ConnectionString))
            {
                var user = db.Query<Users>()
                    .Where(x=>x.Username == username && x.Password == passwowrd)
                    .FirstOrDefault();
                if (user != null)
                {
                    user.LastDateLogin = DateTime.Now;
                    user.LoginToken = Guid.NewGuid().ToString();
                    //Update db
                    db.Update<Users>(user);
                    return user.LoginToken;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}