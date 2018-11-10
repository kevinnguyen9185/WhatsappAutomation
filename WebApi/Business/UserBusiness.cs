using System;
using System.Collections.Generic;
using LiteDB;
using WebApi.Business.Models;

namespace WebApi.Business
{
    public class UserBusiness: BaseBusiness
    {
        public List<Users> GetUsersAsync()
        {
            using(var db = new LiteRepository(ConnectionString))
            {
                return db.Query<Users>().ToList();
            }
        }

        public Users GetUserAsync(string username)
        {
            using(var db = new LiteRepository(ConnectionString))
            {
                return db.Query<Users>().Where(u=>u.Username==username).FirstOrDefault();
            }
        }

        public Users GetUserAsyncByToken(string token)
        {
            using(var db = new LiteRepository(ConnectionString))
            {
                return db.Query<Users>().Where(u=>u.LoginToken==token).FirstOrDefault();
            }
        }

        public void ChangePassword(string password, string username)
        {
            using(var db = new LiteRepository(ConnectionString))
            {
                var user = db.Query<Users>().Where(u=>u.Username == username).FirstOrDefault();
                if(user!=null)
                {
                    user.Password = password;
                    db.Update<Users>(user);
                }
            }
        }

        public void CreateUser(string username, string password)
        {
            using(var db = new LiteRepository(ConnectionString))
            {
                var user = db.Query<Users>().Where(u=>u.Username == username).FirstOrDefault();
                if(user == null){
                    var newuser = new Users(){
                        Username = username,
                        Password = password
                    };
                    db.Insert<Users>(newuser);
                }
            }
        }
    }
}