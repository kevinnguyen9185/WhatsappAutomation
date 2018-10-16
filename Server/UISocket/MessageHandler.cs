using System;
using System.Threading.Tasks;
using Message;
using Message.Robot;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace Server.UISocket 
{
    public class MessageHandler
    {
        private readonly ILogger<MessageHandler> _logger;
        public MessageHandler()
        {
            _logger = new LoggerFactory().AddConsole().CreateLogger<MessageHandler>();
        }

        public async Task ProcessMessage(SendMessage mess)
        {
            switch(mess.MessageType)
            {
                case "GetQRCode":
                    //var connectMessage = JsonConvert.DeserializeObject<GetQRCode>(mess.Message);
                    _logger.LogInformation($"Get QR code for {mess.Sender}");
                    break;
                default:
                    break;
            }
        }
    } 
}