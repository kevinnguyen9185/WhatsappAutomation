namespace Message.UI
{
    public class LoginStatusMessage : BaseMessage 
    {
        
    }

    public class LoginStatusResponseMessage : BaseMessage 
    {
        public string Status { get; set; }
    }
}