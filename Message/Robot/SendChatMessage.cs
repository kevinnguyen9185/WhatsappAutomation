using System.Collections.Generic;

namespace Message.Robot
{
    public class SendChatMessage : BaseMessage 
    {
        public string ContactName { get; set; }
        public string ChatMessage { get; set; }
        public List<string> ImagePaths { get; set; }
    }

    public class SendChatResponseMessage : BaseMessage 
    {
        public bool IsSent { get; set; }
        public string ContactName { get; set; }
    }
}