using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using static AkaratakBot.Shared.API.IOCommon;

namespace AkaratakBot.Dialogs.InsertDialog.InsertSubDialogs
{
    [Serializable]
    public class PhoneNumberDialog : IDialog<object>
    {
        private UserProfile _userProfile;

        public async Task StartAsync(IDialogContext context)
        {
            context.PrivateConversationData.TryGetValue("@userProfile", out _userProfile);
            this.AskForPhoneNumber(context,false);
        }
        public void AskForPhoneNumber(IDialogContext context,bool error)
        {
            if (error)
                context.PostAsync(Resources.Insert.InsertDialog.InsertPhoneNumberError);
            PromptDialog.Text(context, AfterPhoneNumberChoice, Resources.Insert.InsertDialog.InsertPhoneNumber);
        }
        public async Task AfterPhoneNumberChoice(IDialogContext context,IAwaitable<string> result)
        {
            var message = await result;
            string value = string.Empty;
            if (RegexManager.Compare(message,RegexManager.PhoneRegex,out value))
            {
                _userProfile.insertParameters.insertPhoneNumber = value;
                context.PrivateConversationData.SetValue("@userProfile", _userProfile);
                context.Done(context.MakeMessage());
            }
            else
                this.AskForPhoneNumber(context, true);
        }
    }
}