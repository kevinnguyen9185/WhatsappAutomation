using System;
using System.Collections.Generic;
using LiteDB;
using Message.UI;
using Server.Business.Models;

namespace Server.Business
{
    public class ScheduleBusiness: BaseBusiness
    {
        public List<Schedule> CheckUnsentSchedule()
        {
            using(var db = new LiteRepository(ConnectionString))
            {
                return db.Query<Schedule>()
                    .Where(x=>x.IsSent==false && x.WillSendDate<=DateTime.Now).ToList();
            }
        }

        public void UpdateSchedule(Schedule schedule)
        {
            using(var db = new LiteRepository(ConnectionString))
            {
                db.Update<Schedule>(schedule);
            }
        }
    }
}