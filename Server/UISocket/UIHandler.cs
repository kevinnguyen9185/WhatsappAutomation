using System; 
using System.Collections.Generic; 
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks; 
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Server.WebSocketManager;
 
namespace Server.UISocket
{ 
    public class UIHandler : WebSocketHandler 
    { 
        public readonly ILogger<UIHandler> _logger;
        public UIHandler()
        {
            _logger = new LoggerFactory().AddConsole().CreateLogger<UIHandler>();
        }
        protected override int BufferSize { get => 1024 * 4; } 
 
        public override async Task<WebSocketConnection> OnConnected(HttpContext context) 
        { 
            var name = context.Request.Query["UIId"];
            if (!string.IsNullOrEmpty(name)) 
            { 
                var connection = Connections.FirstOrDefault(m => ((UIConnection)m).UIId == name); 
 
                if (connection == null) 
                { 
                    var webSocket = await context.WebSockets.AcceptWebSocketAsync(); 
 
                    connection = new UIConnection(this) 
                    { 
                        UIId = name, 
                        WebSocket = webSocket 
                    }; 
 
                    Connections.Add(connection); 
                    
                    _logger.LogInformation($"{name} connected successfully");
                    await connection.SendMessageAsync("ok");
                } 
                else 
                {
                    if(connection.WebSocket.State == WebSocketState.Open)
                    {
                        _logger.LogInformation($"{name} already connected");
                    }
                    else
                    {
                        Connections.Remove(connection);
                        var webSocket = await context.WebSockets.AcceptWebSocketAsync(); 
 
                        connection = new UIConnection(this) 
                        { 
                            UIId = name, 
                            WebSocket = webSocket 
                        }; 
    
                        Connections.Add(connection); 
                        _logger.LogInformation($"{name} Rconnected successfully");
                        await connection.SendMessageAsync("ok");
                    }
                }
 
                return connection; 
            }

            return null; 
        } 
    } 
} 
