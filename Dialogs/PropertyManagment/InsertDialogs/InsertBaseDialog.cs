using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace AkaratakBot.Dialogs.InsertDialogs
{
    [Serializable]
    public class BaseDialog : IDialog<object>
    {

        public async Task StartAsync(IDialogContext context)
        {
            context.PrivateConversationData.TryGetValue("@userProfile", out _userProfile);
            await this.ShowProgress(context);
        }
        UserProfile _userProfile;
        List<string> formList;
        public async Task ShowProgress(IDialogContext context)
        {

        }
    }
}