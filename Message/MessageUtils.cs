using Newtonsoft.Json;

namespace Message
{
    public class Utils
    {
        public static string CreateSendMessage<T>(string sender, object message)
        {
            var mess = new SendMessage()
            {
                Sender = sender,
                MessageType = typeof(T).Name,
                Message = JsonConvert.SerializeObject(message)
            };
            return JsonConvert.SerializeObject(mess);
        }
    }
}