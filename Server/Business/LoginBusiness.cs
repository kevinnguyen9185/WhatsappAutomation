using System;
using LiteDB;
using Message.UI;
using Server.Business.Models;

namespace Server.Business
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

        public bool IsTokenValid(string username, string logintoken)
        {
            using(var db = new LiteRepository(ConnectionString))
            {
                return db.Query<Users>()
                    .Where(x=>x.Username == username && x.LoginToken == logintoken)
                    .FirstOrDefault()!=null;
            }
        }
    }
}