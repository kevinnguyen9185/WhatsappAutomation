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
using Microsoft.Extensions.CommandLineUtils;
using Serilog;
using Serilog.Events;

namespace Client
{
    //How the robot works?
    //1. The robot running websapp without login
    //2. The UI send username/password to the robot (The username should be the phone number.)
    //3. Check login success or fail
    //4. If login success, check Robot information on Server (If Username/Password matched, do the pair (Sending the username/password to Robot))
    //5. If login success, if no robot matched, check free robot.
    //6. If no robot found, remove the pair
    class Program
    {
        private static CancellationTokenSource _cts  = new CancellationTokenSource();
        private static ClientWebSocket client = new ClientWebSocket();
        private static LandingPage _langdingPage;
        private static ChatPage _chatPage;
        private static string _robotConnId = Guid.NewGuid().ToString();
        private static readonly AutoResetEvent _closing = new AutoResetEvent(false);
        private static string _baseWebsocketAddress = "localhost:5552";
        public static string SharedSelFolder = "";
        static async Task Main(string[] args)
        {
            // await Test();
            // return;
            var app = new CommandLineApplication();
            var selFolderOtions = app.Option("--selfolder <selfolder>", "add folder", CommandOptionType.SingleValue);
            app.OnExecute(() => {
                if(selFolderOtions.HasValue()){
                    Program.SharedSelFolder = selFolderOtions.Value();
                    Console.WriteLine($"Set selfolder to {Program.SharedSelFolder}");
                }
                return 0;
            });
            app.Execute(args);
            //Handle Ctrl C
            Console.CancelKeyPress += new ConsoleCancelEventHandler((sender, e)=>{
                Bootstrap.ChromeDriver.Quit();
                _cts.Cancel();
                Console.WriteLine("Quit the driver then close...");
                _closing.Set();
            });
            //Loging
            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.RollingFile(
                Path.Combine(Directory.GetCurrentDirectory(), "Logs/Client-{Date}.log"),
                fileSizeLimitBytes: 1_000_000,
                shared: true,
                flushToDiskInterval: TimeSpan.FromSeconds(2)
            )
            .CreateLogger();

            //Handle global exception
            System.AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler((sender, e)=>{
                Console.WriteLine(e.ExceptionObject.ToString());
                Bootstrap.ChromeDriver.Quit();
                _cts.Cancel();
                Console.WriteLine("Quit the driver then close...");
                _closing.Set();
            });
            //Main logic goes here
            _langdingPage = new LandingPage("WhatsApp");
            _chatPage = new ChatPage("WhatsApp");
            Console.WriteLine("Client started...");
            client.Options.SetBuffer(1024*1024, 1024*1024);
            await client.ConnectAsync(new Uri($"ws://{_baseWebsocketAddress}/ServerSocket?connId={_robotConnId}&type=robot"), _cts.Token);
            var tReadMessage = ReadMessage(_cts.Token);
            var tCheckLoginStt = CheckLoginStatus(_cts.Token);
            await Task.WhenAll(tReadMessage, tCheckLoginStt);
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
            _chatPage = new ChatPage("WhatsApp");
            _chatPage.GoTo("https://web.whatsapp.com/");
            await Task.Delay(15000);
            await _chatPage.SendWhatsappMessByFindingContact("Toby", "test thoi Bi oi", new string[]{});
            Console.ReadLine();
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
                        },
                        "robot"
                    ));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error when check login status");
                    Log.Error($"Error when check login status {ex.Message}");
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
                    try
                    {
                        var message = await ReadMessage();
                        await ProcessMessage(message);
                    }
                    catch(Exception ex)
                    {
                        Console.Write(ex.Message);
                        Log.Error($"{ex.Message}");
                    }
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
                        await SendMessageAsync(Utils.CreateSendMessage<GetQRCodeResponseMessage>(
                            _robotConnId,
                            new GetQRCodeResponseMessage(){
                                QRCodeBase64 = imgContent
                            },
                            "robot"
                        ));
                    }
                    break;
                case "ErrorMessage":
                    var errMess = JsonConvert.DeserializeObject<ErrorMessage>(receiveMessage.Message);
                    await ProcessErrorMessage(errMess.Message);
                    break;
                case "ContactListMessage":
                    var contactCmd = JsonConvert.DeserializeObject<ContactListMessage>(receiveMessage.Message);
                    if(contactCmd.IsGetall)
                    {
                        var contacts = await _chatPage.GetContactListAll();
                        await SendMessageAsync(Utils.CreateSendMessage<ContactListResponseMessage>(
                            _robotConnId,
                            new ContactListResponseMessage(){
                                IsGetall = true,
                                Contacts = contacts
                            },
                            "robot"
                        ));
                    }
                    else
                    {
                        var contacts = await _chatPage.GetContactList();
                        await SendMessageAsync(Utils.CreateSendMessage<ContactListResponseMessage>(
                            _robotConnId,
                            new ContactListResponseMessage(){
                                IsGetall = false,
                                Contacts = contacts
                            },
                            "robot"
                        ));
                    }
                    break;
                case "SendChatMessage":
                    var sendchatMessage = JsonConvert.DeserializeObject<SendChatMessage>(receiveMessage.Message);
                    if(_chatPage.IsLogin)
                    {
                        await _chatPage.SendWhatsappMessByFindingContact(
                            sendchatMessage.ContactName, 
                            sendchatMessage.ChatMessage, 
                            sendchatMessage.ImagePaths.ToArray()
                        );
                    }
                    await SendMessageAsync(Utils.CreateSendMessage<SendChatResponseMessage>(
                        _robotConnId,
                        new SendChatResponseMessage(),
                        "robot"
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