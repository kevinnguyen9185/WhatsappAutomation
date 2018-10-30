using System.Collections.Generic;

namespace Message.Robot
{
    public class ContactListMessage : BaseMessage 
    {
        public bool IsGetall { get; set; }
    }

    public class ContactListResponseMessage : BaseMessage 
    {
        public bool IsGetall { get; set; }
        public List<string> Contacts { get; set; }
    }
}