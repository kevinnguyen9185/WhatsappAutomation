using System.Threading.Tasks;
using Message;
using Message.Robot;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using System.Net.WebSockets;
using Message.UI;
using Server.WebSocketManager;
using Server.Business;
using System.Collections.Generic;
using Server.Business.Models;
using System;

namespace Server.ServerSocket
{
    public class MessageHandler
    {
        private readonly ILogger<MessageHandler> _logger;
        private readonly ServerSocketHandler _serverSocketHandler;
        private ServerConnection _currentConn;
        public MessageHandler(WebSocketHandler serverSocketHandler)
        {
            _logger = new LoggerFactory().AddConsole().CreateLogger<MessageHandler>();
            _serverSocketHandler = (ServerSocketHandler)serverSocketHandler;
        }

        public async Task ProcessMessage(SendMessage mess)
        {
            var messageNotProcessed = false;
            _currentConn = (ServerConnection)_serverSocketHandler.Connections.Find(p=>((ServerConnection)p).ConnectionId == mess.Sender);
            switch(mess.MessageType)
            {
                case "LoginMessage":
                    var loginMessage = JsonConvert.DeserializeObject<LoginMessage>(mess.Message);
                    //Check username and password against the database
                    var loginToken = new LoginBusiness().IsLogin(loginMessage.UserName, loginMessage.Password);
                    if(!string.IsNullOrEmpty(loginToken))
                    {
                        await _currentConn.SendMessageAsync(Message.Utils.CreateSendMessage<LoginResponseMessage>(mess.Sender, new LoginResponseMessage(){LoginToken=loginToken}));
                    }
                    else
                    {
                        _currentConn.WebSocket.Abort();
                    }
                    break;
                case "GetQRCodeMessage":
                    var robotConn = await GetDestinationConnectionFromSource(mess.Sender);
                    if (robotConn != null && robotConn.WebSocket.State == WebSocketState.Open)
                    {
                        _logger.LogInformation($"Get QR code for {mess.Sender}");
                        await robotConn.SendMessageAsync(Message.Utils.CreateSendMessage<GetQRCodeMessage>(robotConn.ConnectionId, new GetQRCodeMessage()));
                    }
                    else
                    {
                        messageNotProcessed = true;
                    }
                    break;
                case "GetQRCodeResponseMessage":
                    var qrSource = await GetSourceConnectionFromDestination(mess.Sender);
                    if (qrSource != null && qrSource.WebSocket.State == WebSocketState.Open)
                    {
                        _logger.LogInformation($"Send QR code back to UI from robot {mess.Sender}");
                        await qrSource.SendMessageAsync(Message.Utils.CreateSendMessage<GetQRCodeResponseMessage>(qrSource.ConnectionId, new GetQRCodeResponseMessage(){
                            QRCodeBase64 = mess.Message
                        }));
                    }
                    else
                    {
                        messageNotProcessed = true;
                    }
                    break;
                case "PairRobotUIMessage":
                    //Check existing
                    var existingPair = PairController.FindPairBySource(mess.Sender);
                    if(existingPair!=null)
                    {
                        var existingDesConn = (ServerConnection)_serverSocketHandler.Connections.Find(p=>p.ConnectionId == existingPair.DestinationId);
                        if(existingDesConn!=null && existingDesConn.WebSocket.State == WebSocketState.Open)
                        {
                            _logger.LogInformation($"Pair existing Robot UI {mess.Sender}-{existingPair.DestinationId}");
                            await _currentConn.SendMessageAsync(Message.Utils.CreateSendMessage<PairRobotUIResponseMessage>(
                                mess.Sender, 
                                new PairRobotUIResponseMessage(){
                                    RobotConnId = existingPair.DestinationId
                                })
                            );
                        }
                    }
                    else {
                        var freeRobot = (ServerConnection)_serverSocketHandler.Connections.Find(p => {
                            var robot = (ServerConnection)p;
                            return robot.ConnectionType == "robot" 
                            && PairController.FindPairByDestination(robot.ConnectionId)==null
                            && robot.WebSocket.State == WebSocketState.Open;
                        });
                        if (freeRobot != null && _currentConn != null 
                            && freeRobot.WebSocket.State == WebSocketState.Open && _currentConn.WebSocket.State == WebSocketState.Open)
                        {
                            var pairResult = PairController.Pair(mess.Sender, freeRobot.ConnectionId);
                            if (pairResult)
                            {
                                _logger.LogInformation($"Pair Robot UI {mess.Sender}-{freeRobot.ConnectionId}");
                                await _currentConn.SendMessageAsync(Message.Utils.CreateSendMessage<PairRobotUIResponseMessage>(
                                    mess.Sender, 
                                    new PairRobotUIResponseMessage(){
                                        RobotConnId = freeRobot.ConnectionId
                                    })
                                );
                                break;
                            }
                        }
                        else
                        {
                            messageNotProcessed = true;
                        }
                        await _currentConn.SendMessageAsync(Message.Utils.CreateSendMessage<ErrorMessage>(mess.Sender, new ErrorMessage(){Message="pair failed"}));
                    }
                    
                    break;
                case "UnPairRobotUIMessage":
                    var unpairMess = JsonConvert.DeserializeObject<UnPairRobotUIMessage>(mess.Message);
                    var result = PairController.UnPair(
                        PairController.FindPair(unpairMess.UIId, unpairMess.RobotId)
                    );
                    _logger.LogInformation($"UnPair between {unpairMess.UIId} and {unpairMess.RobotId} successfully");
                    await _currentConn.SendMessageAsync(Message.Utils.CreateSendMessage<UnPairRobotUIResponseMessage>(mess.Sender, result));
                    break;
                case "LoginStatusResponseMessage":
                    var checkSttSource = await GetSourceConnectionFromDestination(mess.Sender);
                    if (checkSttSource != null && checkSttSource.WebSocket.State == WebSocketState.Open)
                    {
                        await checkSttSource.SendMessageAsync(JsonConvert.SerializeObject(mess));
                    }
                    break;
                case "ContactListMessage":
                    var contactRobotConn = await GetDestinationConnectionFromSource(mess.Sender);
                    if (contactRobotConn != null && contactRobotConn.WebSocket.State == WebSocketState.Open)
                    {
                        _logger.LogInformation($"Get contact list for {mess.Sender}");
                        await contactRobotConn.SendMessageAsync(Message.Utils.CreateSendMessage<ContactListMessage>(
                            contactRobotConn.ConnectionId, 
                            JsonConvert.DeserializeObject<ContactListMessage>(mess.Message)
                        ));
                    }
                    else
                    {
                        messageNotProcessed = true;
                    }
                    break;
                case "ContactListResponseMessage":
                    var contactSource = await GetSourceConnectionFromDestination(mess.Sender);
                    if (contactSource != null && contactSource.WebSocket.State == WebSocketState.Open)
                    {
                        var contactListResponseMessage = JsonConvert.DeserializeObject<ContactListResponseMessage>(mess.Message);
                        var lstContact = new List<Contact>();

                        contactListResponseMessage.Contacts.ForEach(contactModel=>{
                            var contactDb = new Contact(){
                                UserId = contactSource.ConnectionId,
                                ContactName = contactModel,
                                IsRecent = !contactListResponseMessage.IsGetall
                            };
                            lstContact.Add(contactDb);
                        });
                        //Update all received contacts
                        await new ScheduleBusiness().UpsertContactsAsync(lstContact, contactListResponseMessage.IsGetall);

                        _logger.LogInformation($"Send contact list back to UI {contactSource.ConnectionId} from robot {mess.Sender}");
                        await contactSource.SendMessageAsync(Message.Utils.CreateSendMessage<ContactListResponseMessage>(
                            contactSource.ConnectionId, 
                            JsonConvert.DeserializeObject<ContactListResponseMessage>(mess.Message)));
                    }
                    else
                    {
                        messageNotProcessed = true;
                    }
                    break;
                case "SendChatMessage":
                    var chatMessRobotConn = await GetDestinationConnectionFromSource(mess.Sender);
                    if (chatMessRobotConn != null && chatMessRobotConn.WebSocket.State == WebSocketState.Open)
                    {
                        _logger.LogInformation($"Send chat message for {mess.Sender}");
                        await chatMessRobotConn.SendMessageAsync(Message.Utils.CreateSendMessage<SendChatMessage>(
                            chatMessRobotConn.ConnectionId, 
                            JsonConvert.DeserializeObject<SendChatMessage>(mess.Message)
                        ));
                    }
                    else
                    {
                        messageNotProcessed = true;
                    }
                    break;
                case "SendChatResponseMessage":
                    var chatMessSource = await GetSourceConnectionFromDestination(mess.Sender);
                    if (chatMessSource != null && chatMessSource.WebSocket.State == WebSocketState.Open)
                    {
                        _logger.LogInformation($"Chat response to UI {chatMessSource.ConnectionId} from robot {mess.Sender}");
                        await chatMessSource.SendMessageAsync(Message.Utils.CreateSendMessage<SendChatResponseMessage>(
                            chatMessSource.ConnectionId, 
                            JsonConvert.DeserializeObject<SendChatResponseMessage>(mess.Message)));

                        //Log sent status
                        
                    }
                    else
                    {
                        messageNotProcessed = true;
                    }
                    break;
                case "TakeRobotScreenShot":
                    var takeSreenShotMess = JsonConvert.DeserializeObject<TakeRobotScreenShot>(mess.MessageType);
                    robotConn = (ServerConnection)_serverSocketHandler.Connections.Find(p=>((ServerConnection)p).ConnectionId == takeSreenShotMess.Robotid);
                    if (robotConn!=null)
                    {
                        await robotConn.SendMessageAsync(Message.Utils.CreateSendMessage<TakeRobotScreenShot>(
                            mess.Sender,
                            takeSreenShotMess));
                    }
                    break;
                default:
                    break;
            }
            if(messageNotProcessed)
            {
                await _currentConn.SendMessageAsync(Utils.CreateSendMessage<ErrorMessage>(_currentConn.ConnectionId, new ErrorMessage(){Message= "Message not processed"}));
            }
        }

        private async Task<ServerConnection> GetSourceConnectionFromDestination(string sender)
        {
            var pair = PairController.FindPairByDestination(sender);
            var conn = pair!=null?(ServerConnection)await _serverSocketHandler.GetConnectionByIdAsync(pair.SourceId):null;
            while (conn!=null && conn.WebSocket.State != WebSocketState.Open && pair != null)
            {
                PairController.UnPair(pair);
                pair = PairController.FindPairByDestination(sender);
                conn = pair!=null?(ServerConnection)await _serverSocketHandler.GetConnectionByIdAsync(pair.SourceId):null;
            }
            return conn;
        }

        private async Task<ServerConnection> GetDestinationConnectionFromSource(string sender)
        {
            var pair = PairController.FindPairBySource(sender);
            var conn = pair!=null?(ServerConnection)await _serverSocketHandler.GetConnectionByIdAsync(pair.DestinationId):null;
            while (conn!=null && conn.WebSocket.State != WebSocketState.Open && pair != null)
            {
                PairController.UnPair(pair);
                pair = PairController.FindPairBySource(sender);
                conn = pair!=null?(ServerConnection)await _serverSocketHandler.GetConnectionByIdAsync(pair.DestinationId):null;
            }
            return conn;
        }
    } 
}