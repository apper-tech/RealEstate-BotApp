using AkaratakBot.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AkaratakBot
{
    [Serializable]
    public class UserProfile
    {
        public SearchParameters searchParameters { get; set; }
        public SettingsParameters settingsParameters { get; set; }
        public InsertParameters insertParameters { get; set; }
        public UpdateParameters updateParameters { get; set; }
        public TelegramData telegramData { get; set; }
    }
}