using System;

namespace WebApi.ViewModels
{
    public class Schedule
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string[] Contacts { get; set; }
        public string ChatMessage { get; set; }
        public string[] PathImages { get; set; }
        public DateTime WillSendDate { get; set; }
    }
}