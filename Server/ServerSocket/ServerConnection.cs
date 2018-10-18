using System; 
using System.Collections.Generic; 
using System.Linq; 
using System.Threading.Tasks; 
using Server.WebSocketManager; 
using Newtonsoft.Json; 
using Message;
using Microsoft.Extensions.Logging;

namespace Server.ServerSocket 
{ 
    public class ServerConnection : WebSocketConnection
    {
        private MessageHandler _messHandler;
        private readonly ILogger<ServerConnection> _logger;

        public ServerConnection(WebSocketHandler handler) : base(handler) 
        {
            _logger = new LoggerFactory().AddConsole().CreateLogger<ServerConnection>();
            _messHandler = new MessageHandler(Handler);
        }
 
        public override async Task ReceiveAsync(string message) 
        {
            try
            {
                var receiveMessage = JsonConvert.DeserializeObject<SendMessage>(message); 
                var receiver = Handler.Connections.FirstOrDefault(m => ((ServerConnection)m).ConnectionId == receiveMessage.Sender); 
    
                if (receiver != null)
                {
                    await _messHandler.ProcessMessage(receiveMessage);
                }
                else
                { 
                    var sendMessage = JsonConvert.SerializeObject(new SendMessage 
                    { 
                        Sender = ConnectionId, 
                        MessageType = "ErrorMessage",
                        Message = "Can not seed to " + receiveMessage.Sender 
                    }); 
    
                    await SendMessageAsync(sendMessage); 
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            
        }
    } 
}
