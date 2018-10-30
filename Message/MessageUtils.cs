using Newtonsoft.Json;

namespace Message
{
    public class Utils
    {
        public static string CreateSendMessage<T>(string sender, object message, string type="web")
        {
            var mess = new SendMessage()
            {
                Sender = sender,
                MessageType = typeof(T).Name,
                Message = JsonConvert.SerializeObject(message),
                Type = type
            };
            return JsonConvert.SerializeObject(mess);
        }
    }
}