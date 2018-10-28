using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
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
            var reply = context.MakeMessage();
            //reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            ////reply.Attachments = Shared.Common.Update.GetPropertyList(Shared.API.IOCommon.UserManager.GetUserID(_userProfile,true));
            //reply.Attachments.Add(Shared.Cards.GetHeroCard(
            //                     "test",
            //                    "test",
            //                     "test",
            //                     new CardImage(url: ("https://www.google.com/favicon.ico")),
            //                     new CardAction(ActionTypes.ImBack,"tet")
            //                     ));
            //if (reply.Attachments.Count > 0)
            //    await context.PostAsync(reply);
            //else
                await context.PostAsync("Nothing to Update");
            context.Wait<Activity>(AfterPropertyList);
        }
        public async Task AfterPropertyList(IDialogContext context,IAwaitable<Activity> argument)
        {
            var message = await argument;
            await context.PostAsync(message.Text[0].ToString());
            //show list of options to edit
            //rediect same as insert
            //save and ask 

        }
      
    }
}