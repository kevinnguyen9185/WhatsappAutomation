using System;
using System.Collections.Generic;
using LiteDB;
using Message.UI;
using Server.Business.Models;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<int> UpsertContactsAsync(List<Contact> lstNewContact, bool isGetAll)
        {
            var noSuccess = 0;
            
                foreach(var newcontact in lstNewContact)
                {
                    Contact existedRecord = null;
                    using(var db=new LiteRepository(ConnectionString))
                    {
                        existedRecord = db.Query<Contact>()
                            .Where(c=>c.UserId == newcontact.UserId && c.ContactName.ToLower().Trim() == newcontact.ContactName.ToLower().Trim())
                            .FirstOrDefault();
                    }
                    
                    using(var db=new LiteRepository(ConnectionString))
                    {
                        if(existedRecord!=null)
                        {
                            //Do update
                            var updatedSchedule = newcontact;
                            updatedSchedule._id = existedRecord._id;
                            if(isGetAll)
                            {
                                updatedSchedule.IsRecent = existedRecord.IsRecent;
                            }
                            if(db.Update<Contact>(updatedSchedule))
                            {
                                noSuccess ++;
                            }
                        }
                        else
                        {
                            //Do insert
                            var insertSchedule = newcontact;
                            insertSchedule._id = Guid.NewGuid().ToString();
                            if(db.Insert<Contact>(new Contact{
                                _id = insertSchedule._id,
                                ContactName = insertSchedule.ContactName,
                                UserId = insertSchedule.UserId,
                                IsRecent = insertSchedule.IsRecent
                        
                            })!=null)
                            {
                                noSuccess ++;
                            }
                        }
                    }
                }
            return noSuccess;
        }
    }
}