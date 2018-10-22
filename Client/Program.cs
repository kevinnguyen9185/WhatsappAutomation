using System;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Client.Automation;
using Message;
using Message.UI;
using Newtonsoft.Json;
using Message.Robot;
using System.IO;

namespace Client
{
    class Program
    {
        private static CancellationTokenSource _cts  = new CancellationTokenSource();
        private static ClientWebSocket client = new ClientWebSocket();
        private static LandingPage _langdingPage;
        private static ChatPage _chatPage;
        private static string _robotConnId = Guid.NewGuid().ToString();
        static async Task Main(string[] args)
        {
            _langdingPage = new LandingPage("WhatsApp");
            _chatPage = new ChatPage("WhatsApp");
            Console.WriteLine("Client started...");
            client.Options.SetBuffer(1024*1024, 1024*1024);
            await client.ConnectAsync(new Uri($"ws://localhost:5000/ServerSocket?connId={_robotConnId}&type=robot"), _cts.Token);
            var tReadMessage = ReadMessage(_cts.Token);
            var tCheckLoginStt = CheckLoginStatus(_cts.Token);
            await Task.WhenAll(tReadMessage, tCheckLoginStt);
            //await Test();
            //Console.ReadLine();
            Bootstrap.ChromeDriver.Dispose();
        }

        private static async Task Test()
        {
            // Byte[] bytes = File.ReadAllBytes("/Users/kevinng/Desktop/Test3.png");
            // String file = Convert.ToBase64String(bytes);
            // Console.WriteLine(file.Substring(0,100));

                        // _langdingPage = new LandingPage("WhatsApp");
            // _chatPage = new ChatPage("WhatsApp");
            // Console.WriteLine(_langdingPage.GetQRCodeImage());
            // while(true)
            // {
            //     if(_chatPage.IsLogin)
            //     {
            //         var contacts = _chatPage.GetContactList();
            //         var tobyContact = contacts.FirstOrDefault(c => c=="Toby");
            //         await _chatPage.SendWhatsappMess(
            //             tobyContact, 
            //             $"test thoi Bi oi {DateTime.Now.Ticks.ToString()}", 
            //             new string[5]{
            //                 "/Users/kevinng/Desktop/Test1.png",
            //                 "/Users/kevinng/Desktop/Test2.png",
            //                 "/Users/kevinng/Desktop/Test3.png",
            //                 "/Users/kevinng/Desktop/Test4.png",
            //                 "/Users/kevinng/Desktop/Test5.png"
            //             });
            //         break;
            //     }
            //     await Task.Delay(1000);
            // }
            // _chatPage = new ChatPage("WhatsApp");
            // _chatPage.GoTo("file:///Users/kevinng/Desktop/WhatsApp.htm");
            // var contacts = _chatPage.GetContactList();
            // var tobyContact = contacts.FirstOrDefault(c => c=="Toby");
            // await _chatPage.SendWhatsappMess(tobyContact, "test thoi Bi oi");
        }

        static async Task CheckLoginStatus(CancellationToken cts)
        {
            while(true)
            {
                try
                {
                    //Console.WriteLine(isLogin);
                    await SendMessageAsync(Utils.CreateSendMessage<LoginStatusResponseMessage>(_robotConnId,
                        new LoginStatusResponseMessage(){
                            Status = _chatPage.IsLogin.ToString()
                        }
                    ));
                }
                catch
                {
                    Console.WriteLine("Error when check login status");
                }
                await Task.Delay(1000, cts);
                if (cts.IsCancellationRequested)
                    break;
            }
        }

        static async Task ReadMessage(CancellationToken cts)
        {
            while(true)
            {
                if(client.State == WebSocketState.Open)
                {
                    var message = await ReadMessage();
                    await ProcessMessage(message);
                }
                await Task.Delay(100, cts);
                if (cts.IsCancellationRequested)
                    break;
            } 
        }

        static async Task SendMessageAsync(string message) 
        {
            var byteMessage = Encoding.UTF8.GetBytes(message);
            var segmnet = new ArraySegment<byte>(byteMessage);
        
            await client.SendAsync(segmnet, WebSocketMessageType.Text, true, _cts.Token);
        }

        static async Task<string> ReadMessage()
        {
            WebSocketReceiveResult result;
            var message = new ArraySegment<byte>(new byte[1024*1024]);
            string receivedMessage = "";
            do 
            {
                result = await client.ReceiveAsync(message, _cts.Token);
                if (result.MessageType != WebSocketMessageType.Text)
                    break;
                var messageBytes = message.Skip(message.Offset).Take(result.Count).ToArray();
                receivedMessage += Encoding.UTF8.GetString(messageBytes);
            } 
            while (!result.EndOfMessage);

            return receivedMessage;
        }

        static async Task ProcessMessage(string message)
        {
            var receiveMessage = JsonConvert.DeserializeObject<SendMessage>(message); 
            switch (receiveMessage.MessageType)
            {
                case "GetQRCodeMessage":
                    //Get QR code and return
                    if(!_chatPage.IsLogin)
                    {
                        var imgContent = _langdingPage.QRCode;
                        await SendMessageAsync(JsonConvert.SerializeObject(new SendMessage(){
                            Sender = _robotConnId,
                            MessageType = "GetQRCodeResponseMessage",
                            Message = imgContent
                        }));
                    }
                    break;
                case "ErrorMessage":
                    var errMess = JsonConvert.DeserializeObject<ErrorMessage>(receiveMessage.Message);
                    await ProcessErrorMessage(errMess.Message);
                    break;
                case "ContactListMessage":
                    var contacts = _chatPage.GetContactList();
                    await SendMessageAsync(Utils.CreateSendMessage<ContactListResponseMessage>(
                        _robotConnId,
                        new ContactListResponseMessage(){
                            Contacts = contacts
                        }
                    ));
                    break;
                case "SendChatMessage":
                    var sendchatMessage = JsonConvert.DeserializeObject<SendChatMessage>(receiveMessage.Message);
                    if(_chatPage.IsLogin)
                    {
                        await _chatPage.SendWhatsappMess(
                            sendchatMessage.ContactName, 
                            sendchatMessage.ChatMessage, 
                            sendchatMessage.ImagePaths.ToArray()
                        );
                    }
                    await SendMessageAsync(Utils.CreateSendMessage<SendChatResponseMessage>(
                        _robotConnId,
                        new SendChatResponseMessage()
                    ));
                    break;
                default:
                    break;
            }
        }

        static async Task ProcessErrorMessage(string message)
        {
            switch(message)
            {
                case "orphaned":
                    _langdingPage.RefreshPage();
                    break;
                default:
                    break;
            }
        }
    }                                                                  
}