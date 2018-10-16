using System;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Message;
using Newtonsoft.Json;

namespace Client
{
    class Program
    {
        private static CancellationToken cts = new CancellationToken();
        private static ClientWebSocket client = new ClientWebSocket();
        static async Task Main(string[] args)
        {
            var name = Guid.NewGuid();
            Console.WriteLine("Hello World!");
            await client.ConnectAsync(new Uri($"ws://localhost:5000/RobotSocket?RobotId={name}"), cts);
            var message = new Message.SendMessage()
            {
                Sender = name.ToString(),
                MessageType = typeof(Message.Robot.ConnectMessage).FullName,
                Message = JsonConvert.SerializeObject(new Message.Robot.ConnectMessage(){
                    RobotId = name.ToString(),
                    RobotStatus = Message.Status.NotLoggedIn
                })
            };
            await SendMessageAsync(JsonConvert.SerializeObject(message));
            while(client.State == WebSocketState.Open)
            {
                await ReadMessage();
            }
            Console.ReadLine();
        }

        static async Task SendMessageAsync(string message) 
        {
            var byteMessage = Encoding.UTF8.GetBytes(message);
            var segmnet = new ArraySegment<byte>(byteMessage);
        
            await client.SendAsync(segmnet, WebSocketMessageType.Text, true, cts);
        }

        static async Task ReadMessage()
        {
            WebSocketReceiveResult result;
            var message = new ArraySegment<byte>(new byte[4096]);
            do 
            {
                result = await client.ReceiveAsync(message, cts);
                if (result.MessageType != WebSocketMessageType.Text)
                    break;
                var messageBytes = message.Skip(message.Offset).Take(result.Count).ToArray();
                string receivedMessage = Encoding.UTF8.GetString(messageBytes);
                Console.WriteLine("Received: {0}", receivedMessage);
            } 
            while (!result.EndOfMessage);
        }
    }
}