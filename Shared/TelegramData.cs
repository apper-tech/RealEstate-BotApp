using Microsoft.Bot.Builder.Dialogs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;

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
    public class From2
    {
        public int id { get; set; }
        public bool is_bot { get; set; }
        public string first_name { get; set; }
        public string username { get; set; }
    }
    [Serializable]
    public class Message
    {
        public int message_id { get; set; }
        public From from { get; set; }
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
    public class TelegramData2
    {
        public int update_id { get; set; }
        public CallbackQuery callback_query { get; set; }
    }
    [Serializable]
    public class TelegramData
    {
        public int update_id { get; set; }
        public Message message { get; set; }
        private static TelegramData ConvertTelegramFormat(string json)
        {
            var data = JsonConvert.DeserializeObject<TelegramData2>(json.ToString());
            var telegramData = new TelegramData();
            telegramData.update_id = data.update_id;
            telegramData.message = data.callback_query.message;
            return telegramData;
        }
        public static TelegramData GetUserTelegramData(IDialogContext context)
        {
            var message = context.Activity;
            StringWriter wr = new StringWriter();
            var jsonWriter = new JsonTextWriter(wr);
            jsonWriter.StringEscapeHandling = StringEscapeHandling.EscapeNonAscii;
            new JsonSerializer().Serialize(jsonWriter, message.ChannelData);
            var data = JsonConvert.DeserializeObject<TelegramData>(wr.ToString());
            if (data.message != null)
                return data;
            else
                return ConvertTelegramFormat(wr.ToString());
        }
    }
}