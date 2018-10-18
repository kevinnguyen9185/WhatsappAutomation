using System; 
using System.Collections.Generic; 
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks; 
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Server.WebSocketManager; 
 
namespace Server.RobotSocket
{ 
    public class RobotSocketHandler : WebSocketHandler 
    { 
        public readonly ILogger<RobotSocketHandler> _logger;
        public RobotSocketHandler()
        {
            _logger = new LoggerFactory().AddConsole().CreateLogger<RobotSocketHandler>();
        }
        protected override int BufferSize { get => 1024 * 4; }

        public override Task<WebSocketConnection> GetConnectionByIdAsync(string connId)
        {
            return Task.FromResult<WebSocketConnection>(Connections.FirstOrDefault(m => ((RobotConnection)m).ConnectionId == connId)); 
        }

        public override async Task<WebSocketConnection> OnConnected(HttpContext context) 
        { 
            var name = context.Request.Query["connId"];
            if (!string.IsNullOrEmpty(name)) 
            { 
                var connection = Connections.FirstOrDefault(m => ((RobotConnection)m).ConnectionId == name); 
 
                if (connection == null) 
                { 
                    var webSocket = await context.WebSockets.AcceptWebSocketAsync(); 
 
                    connection = new RobotConnection(this) 
                    { 
                        ConnectionId = name, 
                        WebSocket = webSocket 
                    }; 
 
                    Connections.Add(connection); 
                    
                    _logger.LogInformation($"{name} connected successfully");
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
 
                        connection = new RobotConnection(this) 
                        { 
                            ConnectionId = name, 
                            WebSocket = webSocket 
                        }; 
    
                        Connections.Add(connection); 
                    }
                }
                return connection; 
            }

            return null; 
        } 
    } 
} 
