using System; 
using System.Collections.Generic; 
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Message;
using Message.Robot;
using Message.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Server.Business;
using Server.WebSocketManager;
 
namespace Server.ServerSocket
{ 
    public class ServerSocketHandler : WebSocketHandler , IDisposable
    { 
        public readonly ILogger<ServerSocketHandler> _logger;
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        public ServerSocketHandler()
        {
            _logger = new LoggerFactory().AddConsole().CreateLogger<ServerSocketHandler>();
            Task.Run(async () => await ClientHealthCheck(cancellationTokenSource.Token));
            Task.Run(async () => await PairHealthCheck(cancellationTokenSource.Token));
            Task.Run(async () => await ScheduleCheck(cancellationTokenSource.Token));
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
            var logintoken = context.Request.Query["logintoken"];
            if (!string.IsNullOrEmpty(name)) 
            {
                //Check phone & loginToken valid
                if(type=="robot" || (new LoginBusiness().IsTokenValid(name, logintoken) && !string.IsNullOrEmpty(logintoken)))
                {
                    var connection = Connections.FirstOrDefault(m => ((ServerConnection)m).ConnectionId == name); 

                    if (connection == null)
                    { 
                        return await CreateConnectionAsync(name, type,logintoken, context);
                    } 
                    else 
                    {
                        Connections.Remove(connection);
                        connection.WebSocket.Abort();
                        connection = await CreateConnectionAsync(name, type, logintoken, context);
                    }
                    return connection; 
                }
            }
            return null; 
        }

        private async Task<WebSocketConnection> CreateConnectionAsync(string name, string type, string logintoken, HttpContext context)
        {
            var webSocket = await context.WebSockets.AcceptWebSocketAsync(); 

            var connection = new ServerConnection(this) 
            {
                ConnectionId = name, 
                WebSocket = webSocket,
                ConnectionType = type
            };

            Connections.Add(connection); 
            await connection.SendMessageAsync(Message.Utils.CreateSendMessage<LoginResponseMessage>(name, new LoginResponseMessage(){LoginToken=logintoken}));
            _logger.LogInformation($"{name} connected successfully");
            return connection;
        }

        public override async Task ClientHealthCheck(CancellationToken cancellationToken)
        {
            while (true)
            {   
                var removeConnList = new List<WebSocketConnection>();
                Connections.ForEach(async (c) => {
                    try
                    {
                        await c.SendMessageAsync(Utils.CreateSendMessage<SendMessage>(c.ConnectionId, "connection health check"));
                    }
                    catch(Exception ex)
                    {
                        removeConnList.Add(c);
                    }
                });
                
                removeConnList.ForEach(r => {
                    Connections.Remove(r);
                });
                removeConnList.Clear();
                await Task.Delay(5000, cancellationToken);
                if (cancellationToken.IsCancellationRequested)
                    break;
            }
        }

        public async Task PairHealthCheck(CancellationToken cancellationToken)
        {
            while (true)
            {   
                List<PairRobotAndUI> tempPairList = new List<PairRobotAndUI>();
                if (PairController.PairList!=null)
                {
                    PairController.PairList.ForEach(async (p) => {
                    var sConn = await GetConnectionByIdAsync(p.SourceId);
                    var dConn = await GetConnectionByIdAsync(p.DestinationId);
                    if (dConn == null || dConn.WebSocket.State != WebSocketState.Open)
                        {
                            tempPairList.Add(p);
                            if (sConn != null && sConn.WebSocket.State == WebSocketState.Open)
                            {
                                await sConn.SendMessageAsync(Utils.CreateSendMessage<ErrorMessage>(sConn.ConnectionId, new ErrorMessage(){Message = "orphaned"}));
                            }
                            // if (dConn != null && dConn.WebSocket.State == WebSocketState.Open)
                            // {
                            //     await dConn.SendMessageAsync(Utils.CreateSendMessage<ErrorMessage>(dConn.ConnectionId, new ErrorMessage(){Message = "orphaned"}));
                            // }
                        }
                    
                    });
                }
                tempPairList.ForEach(p=>{
                    PairController.UnPair(p);
                    _logger.LogInformation($"Health check removed {p.SourceId}-{p.DestinationId}");
                });
                tempPairList.Clear();
                await Task.Delay(5000, cancellationToken);
                if (cancellationToken.IsCancellationRequested)
                    break;
            }
        }

        public async Task ScheduleCheck(CancellationToken cancellationToken)
        {
            while(true)
            {
                try
                {
                    var scheduleList = new ScheduleBusiness().CheckUnsentSchedule();
                    if(scheduleList.Count>0 && PairController.PairList!=null)
                    {
                        var usersInPair = PairController.PairList.Select(r=>r.SourceId);
                        //Find matching with existing pair
                        var scheduleInPair = scheduleList.Where(r=>usersInPair.Contains(r.Username)).ToList();
                        foreach(var schedule in scheduleInPair)
                        {
                            _logger.LogInformation($"Processing schedule {schedule._id}");
                            var robotPair = PairController.PairList.Where(r=>r.SourceId == schedule.Username).FirstOrDefault();
                            if(robotPair!=null)
                            {
                                var conn = await GetConnectionByIdAsync(robotPair.DestinationId);
                                foreach (var contact in schedule.Contacts)
                                {
                                    await Task.Delay(1000);
                                    await conn.SendMessageAsync(
                                        Utils.CreateSendMessage<SendChatMessage>(
                                            conn.ConnectionId,
                                            new SendChatMessage(){
                                                ChatMessage = schedule.ChatMessage,
                                                ContactName = contact,
                                                ImagePaths = schedule.PathImages.ToList()
                                            })
                                    );
                                }
                                schedule.IsSent = true;
                                new ScheduleBusiness().UpdateSchedule(schedule);
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex.Message);
                }
                await Task.Delay(30000, cancellationToken);
                if (cancellationToken.IsCancellationRequested)
                    break;
            }
        }

        public void Dispose()
        {
            cancellationTokenSource.Cancel();
        }
    }
} 
