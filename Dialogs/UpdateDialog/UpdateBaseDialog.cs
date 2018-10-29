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
            PromptDialog.Choice<SearchEntry>(
                context,
                AfterPropertyList,
                Shared.Common.Search._GetPropertyCategoryList(context, true),
                Resources.Search.SearchDialog.SearchCategorySelection);
        }
        public async Task AfterPropertyList(IDialogContext context,IAwaitable<SearchEntry> argument)
        {
            var message = await argument;
            await context.PostAsync(message.searchKey);
            //show list of options to edit
            //rediect same as insert
            //save and ask 

        }
        private static Attachment GetProfileHeroCard()
        {
            var heroCard = new HeroCard
            {
                // title of the card  
                Title = "Suthahar Jegatheesan",
                //subtitle of the card  
                Subtitle = "Microsoft certified solution developer",
                // navigate to page , while tab on card  
                Tap = new CardAction(ActionTypes.OpenUrl, "Learn More", value: "http://www.devenvexe.com"),
                //Detail Text  
                Text = "Suthahar J is a Technical Lead and C# Corner MVP. He has extensive 10+ years of experience working on different technologies, mostly in Microsoft space. His focus areas are  Xamarin Cross Mobile Development ,UWP, SharePoint, Azure,Windows Mobile , Web , AI and Architecture. He writes about technology at his popular blog http://devenvexe.com",
                // list of  Large Image  
                Images = new List<CardImage> { new CardImage("http://csharpcorner.mindcrackerinc.netdna-cdn.com/UploadFile/AuthorImage/jssuthahar20170821011237.jpg") },
                // list of buttons   
                Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Learn More", value: "http://www.devenvexe.com"), new CardAction(ActionTypes.OpenUrl, "C# Corner", value: "http://www.c-sharpcorner.com/members/suthahar-j"), new CardAction(ActionTypes.OpenUrl, "MSDN", value: "https://social.msdn.microsoft.com/profile/j%20suthahar/") }
            };

            return heroCard.ToAttachment();
        }
    }
}