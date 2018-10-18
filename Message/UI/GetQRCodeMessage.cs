namespace Message.UI
{
    public class GetQRCodeMessage : BaseMessage 
    {
    }

    public class GetQRCodeResponseMessage : BaseMessage 
    {
        public string QRCodeBase64 { get; set; }
    }
}