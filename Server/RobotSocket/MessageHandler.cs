using System;
using System.Threading.Tasks;
using Message;
using Message.Robot;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace Server.RobotSocket 
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
                case "Message.Robot.ConnectMessage":
                    var connectMessage = JsonConvert.DeserializeObject<ConnectMessage>(mess.Message);
                    //_logger.LogInformation(connectMessage.RobotStatus.ToString());
                    break;
                case "Message.Robot.DisconnectMessage":
                    var disconnectMessage = JsonConvert.DeserializeObject<DisconnectMessage>(mess.Message);
                    //_logger.LogInformation(disconnectMessage.RobotStatus.ToString());
                    break;
                default:
                    break;
            }
        }
    } 
}