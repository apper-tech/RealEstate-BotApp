using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using static AkaratakBot.Shared.API.IOCommon;

namespace AkaratakBot.Dialogs.UpdateDialog.UpdateSubDialogs
{
    [Serializable]
    public class PhotoDialog : IDialog<SearchEntry>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.PrivateConversationData.TryGetValue("@userProfile", out _userProfile);
            this.AskFoPhoto(context,false);
        }
        private SearchEntry _userOption;
        private UserProfile _userProfile;
        public PhotoDialog(SearchEntry entry)
        {
            _userOption = entry;
        }
        public async Task AskFoPhoto(IDialogContext context,bool error)
        {
            var reply = context.MakeMessage();
            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            var emulator = context.Activity.ChannelId == "emulator";
            reply.Attachments = Shared.Common.Update.GetPhotoList(_userProfile.updateParameters.updatePropertyID);
            uint minCount = (uint)reply.Attachments.Count - 4; 
            if (reply.Attachments.Count == 0)
            {
                await context.PostAsync(reply);
                reply = context.MakeMessage();
            }
            else
            {
                reply.Attachments.Add(Shared.Cards.GetButtonCard($"Add ({minCount}) More","add"));
                await context.PostAsync(reply);
            }
            context.Wait<Activity>(AfterPhotoChoice);
        }
        public async Task AfterPhotoChoice(IDialogContext context, IAwaitable<Activity> argument)
        {
            //var photos = PhotoManager.ValidateUserPhotos((await argument).Take(4));
            //if (photos.Count() > 0)
            //    _userProfile.updateParameters.updatePhotoPath = PhotoManager.DownloadUserInsertPhotos(context.Activity, photos);
            //else
            //    this.AskFoPhoto(context,true);
            //context.PrivateConversationData.SetValue("@userProfile", _userProfile);
            var message = await argument;
            if(message.Text =="add")
            {
                //ask for images 
                //save localy
                //get ids and add to userprofile
            }
            else
            {
                //check if image id excist
                //true :replace image by id
                //false: we ask again
            }

            context.Done(Shared.Common.Insert.CheckField(context, _userOption));
        }
    }
}