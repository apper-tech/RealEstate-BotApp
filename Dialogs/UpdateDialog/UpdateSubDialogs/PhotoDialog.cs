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
            await this.ShowPhotoList(context, false);
        }
        private SearchEntry _userOption;
        private UserProfile _userProfile;
        public PhotoDialog(SearchEntry entry)
        {
            _userOption = entry;
        }
        public async Task ShowPhotoList(IDialogContext context, bool error)
        {
            var reply = context.MakeMessage();
            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            var emulator = context.Activity.ChannelId == "emulator";
            reply.Attachments = Shared.Common.Update.GetPhotoList(_userProfile.updateParameters.updatePropertyID);

            var minCount = int.Parse((reply.Attachments.Count - 4).ToString().Remove(0, 1));
            _userProfile.updateParameters.PhotoParameters = new PhotoParameters { MinCount = minCount };
            context.PrivateConversationData.SetValue("@userProfile", _userProfile);

            if (minCount == 0)
            {
                await context.PostAsync(reply);
                reply = context.MakeMessage();
            }
            else
            {
                reply.Attachments.Add(Shared.Cards.GetButtonCard($"Add ({minCount}) More", "add"));
                await context.PostAsync(reply);
            }
            context.Wait<Activity>(AfterPhotoListChoice);
        }
        public async Task AfterPhotoListChoice(IDialogContext context, IAwaitable<Activity> argument)
        {
            var message = await argument;
            if (message.Text == "add")
            {
                _userProfile.updateParameters.PhotoParameters.Add = true;
                context.PrivateConversationData.SetValue("@userProfile", _userProfile);
            }
            else
            {
                _userProfile.updateParameters.PhotoParameters.Add = false;
                _userProfile.updateParameters.PhotoParameters.Photos = new List<UploadPhoto>()
                {
                    new UploadPhoto{PublicId = message.Text}
                };
                context.PrivateConversationData.SetValue("@userProfile", _userProfile);
            }
            this.AskFoPhoto(context, false);
        }
        public void AskFoPhoto(IDialogContext context, bool error)
        {
            if (error)
                context.PostAsync(Resources.Insert.InsertDialog.InsertPhotosErrorMessage);

            PromptDialog.Attachment(context, AfterPhotoChoice, Resources.Update.UpdateDialog.UpdateFormPropertyNewPhotoDescription);
        }
        public async Task AfterPhotoChoice(IDialogContext context, IAwaitable<IEnumerable<Attachment>> argument)
        {

            var photos = PhotoManager.ValidateUserPhotos((await argument).Take(_userProfile.updateParameters.PhotoParameters.MinCount));
            if (photos.Count() > 0)
                _userProfile.updateParameters.PhotoParameters.Photos = PhotoManager.DownloadUserUpdatePhotos(context.Activity, photos,_userProfile);
            else
                this.AskFoPhoto(context, true);
            context.PrivateConversationData.SetValue("@userProfile", _userProfile);
            context.Done(Shared.Common.Insert.CheckField(context, _userOption));
        }
    }
}