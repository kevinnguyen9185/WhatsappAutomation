using System.Collections.Generic;

namespace Message.Robot
{
    public class ContactListMessage : BaseMessage 
    {
    }

    public class ContactListResponseMessage : BaseMessage 
    {
        public List<string> Contacts { get; set; }
    }
}