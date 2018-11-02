namespace Server.Business.Models
{
    public class Contact
    {
        public string _id {get;set;}
        public string UserId { get; set; }
        public string ContactName { get; set; }
        public string PhoneNo { get; set; }
        public bool IsRecent { get; set; }
    }
}