using System; 
using System.Collections.Generic; 
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Message;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Server.WebSocketManager;
 
namespace Server.ServerSocket
{ 
    public class ServerSocketHandler : WebSocketHandler 
    { 
        public readonly ILogger<ServerSocketHandler> _logger;
        public ServerSocketHandler()
        {
            _logger = new LoggerFactory().AddConsole().CreateLogger<ServerSocketHandler>();
        }
        protected override int BufferSize { get => 1024 * 1024; }

        public override Task<WebSocketConnection> GetConnectionByIdAsync(string connId)
        {
            return Task.FromResult<WebSocketConnection>(Connections.FirstOrDefault(m => ((ServerConnection)m).ConnectionId == connId)); 
        }

        public override async Task<WebSocketConnection> OnConnected(HttpContext context)
        { 
            var name = context.Request.Query["connId"];
            var type = context.Request.Query["type"];
            if (!string.IsNullOrEmpty(name)) 
            { 
                var connection = Connections.FirstOrDefault(m => ((ServerConnection)m).ConnectionId == name); 
 
                if (connection == null) 
                { 
                    return await CreateConnectionAsync(name, type, context);
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
                        connection = await CreateConnectionAsync(name, type, context);
                    }
                }
 
                return connection; 
            }

            return null; 
        }

        private async Task<WebSocketConnection> CreateConnectionAsync(string name, string type, HttpContext context)
        {
            var webSocket = await context.WebSockets.AcceptWebSocketAsync(); 

            var connection = new ServerConnection(this) 
            {
                ConnectionId = name, 
                WebSocket = webSocket,
                ConnectionType = type
            };

            Connections.Add(connection); 
            _logger.LogInformation($"{name} connected successfully");
            await connection.SendMessageAsync(Message.Utils.CreateSendMessage<ResponseMessage>(name, "ok"));
            return connection;
        }
    }
} 
