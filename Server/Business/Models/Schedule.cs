using System;
using System.Collections.Generic;

namespace Server.Business.Models
{
    public class Schedule
    {
        public string _id {get;set;}
        public string Username { get; set; }
        public string[] Contacts { get; set; }
        public string ChatMessage { get; set; }
        public string[] PathImages { get; set; }
        public DateTime WillSendDate { get; set; }
        public bool IsSent { get; set; }
        public List<ScheduleSentResult> ScheduleSentResults { get; set; }
    }

    public class ScheduleSentResult
    {
        public string ContactName { get; set; }
        public bool IsSent { get; set; }
    }
}