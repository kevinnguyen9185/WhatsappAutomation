using System;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Client.Automation;
using Message;
using Newtonsoft.Json;

namespace Client
{
    class Program
    {
        private static CancellationToken cts = new CancellationToken();
        private static ClientWebSocket client = new ClientWebSocket();
        private static LandingPage _langdingPage;
        private static string _robotConnId = Guid.NewGuid().ToString();
        static async Task Main(string[] args)
        {
            _langdingPage = new LandingPage();
            Console.WriteLine("Client started...");
            client.Options.SetBuffer(1024*1024, 1024*1024);
            await client.ConnectAsync(new Uri($"ws://localhost:5000/ServerSocket?connId={_robotConnId}&type=robot"), cts);
            while(client.State == WebSocketState.Open)
            {
                var message = await ReadMessage();
                await ProcessMessage(message);
            }
            Bootstrap.ChromeDriver.Dispose();
        }

        static async Task SendMessageAsync(string message) 
        {
            Console.WriteLine(message);
            var byteMessage = Encoding.UTF8.GetBytes(message);
            var segmnet = new ArraySegment<byte>(byteMessage);
        
            await client.SendAsync(segmnet, WebSocketMessageType.Text, true, cts);
        }

        static async Task<string> ReadMessage()
        {
            WebSocketReceiveResult result;
            var message = new ArraySegment<byte>(new byte[1024*1024]);
            string receivedMessage = "";
            do 
            {
                result = await client.ReceiveAsync(message, cts);
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
                    var imgContent = _langdingPage.GetQRCodeImage();
                    await SendMessageAsync(JsonConvert.SerializeObject(new SendMessage(){
                        Sender = _robotConnId,
                        MessageType = "GetQRCodeResponseMessage",
                        Message = imgContent
                    }));
                    break;
                default:
                    break;
            }
        }
    }                                                                  
}