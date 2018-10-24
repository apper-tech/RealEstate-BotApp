using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AkaratakBot.Shared
{
    [Serializable]
    public class From
    {
        public int id { get; set; }
        public bool is_bot { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string username { get; set; }
        public string language_code { get; set; }
    }
    [Serializable]
    public class Chat
    {
        public int id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string username { get; set; }
        public string type { get; set; }
    }
    [Serializable]
    public class Message
    {
        public int message_id { get; set; }
        public Chat chat { get; set; }
        public int date { get; set; }
        public string text { get; set; }
    }
    [Serializable]
    public class CallbackQuery
    {
        public string id { get; set; }
        public From from { get; set; }
        public Message message { get; set; }
        public string chat_instance { get; set; }
        public string data { get; set; }
    }
    [Serializable]
    public class TelegramData
    {
        public int update_id { get; set; }
        public CallbackQuery callback_query { get; set; }
    }
}