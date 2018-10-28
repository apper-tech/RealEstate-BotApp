using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace AkaratakBot.Dialogs.UpdateDialog
{
    [Serializable]
    public class BaseDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.PrivateConversationData.TryGetValue("@userProfile", out _userProfile);
            await this.ShowPropertyList(context);
        }
        UserProfile _userProfile;
        public async Task ShowPropertyList(IDialogContext context)
        {

        }
        public async Task AfterPropertyList(IDialogContext context,IAwaitable<object> argument)
        {
            //show list of options to edit
            //rediect same as insert
            //save and ask 

        }
      
    }
}