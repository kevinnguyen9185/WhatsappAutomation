using System; 
using System.Collections.Generic; 
using System.Linq; 
using System.Threading.Tasks; 
using Server.WebSocketManager; 
using Newtonsoft.Json; 
using Message;
using Microsoft.Extensions.Logging;

namespace Server.UISocket 
{ 
    public class UIConnection : WebSocketConnection 
    {
        private MessageHandler _messHandler;
        private readonly ILogger<UIConnection> _logger;
        public UIConnection(WebSocketHandler handler) : base(handler) 
        {
            _logger = new LoggerFactory().AddConsole().CreateLogger<UIConnection>();
            _messHandler = new MessageHandler();
        } 
 
        public string UIId { get; set; }
 
        public override async Task ReceiveAsync(string message) 
        { 
            try
            {
                var receiveMessage = JsonConvert.DeserializeObject<SendMessage>(message); 
                var receiver = Handler.Connections.FirstOrDefault(m => ((UIConnection)m).UIId == receiveMessage.Sender); 
    
                if (receiver != null)
                {
                    await _messHandler.ProcessMessage(receiveMessage);
                }
                else
                { 
                    var sendMessage = JsonConvert.SerializeObject(new SendMessage 
                    { 
                        Sender = UIId, 
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
