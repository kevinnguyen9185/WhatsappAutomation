namespace Message.UI
{
    public class LoginMessage : BaseMessage 
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class LoginResponseMessage : BaseMessage 
    {
        public string LoginToken {get;set;}
    }
}