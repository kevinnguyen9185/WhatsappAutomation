namespace Message.UI
{
    public class UnPairRobotUIMessage : BaseMessage 
    {
        public string UIId { get; set; }
        public string RobotId { get; set; }
    }

    public class UnPairRobotUIResponseMessage : BaseMessage 
    {
    }
}