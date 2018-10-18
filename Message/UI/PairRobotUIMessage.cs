namespace Message.UI
{
    public class PairRobotUIMessage : BaseMessage 
    {
    }

    public class PairRobotUIResponseMessage : BaseMessage 
    {
        public string RobotConnId { get; set; }
    }
}