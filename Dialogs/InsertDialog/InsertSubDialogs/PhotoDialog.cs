using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using static AkaratakBot.Shared.API.IOCommon;

namespace AkaratakBot.Dialogs.InsertDialog.InsertSubDialogs
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
        public void AskFoPhoto(IDialogContext context,bool error)
        {
            if (error)
                context.PostAsync(Resources.Insert.InsertDialog.InsertPhotosErrorMessage);

            PromptDialog.Attachment(context, AfterPhotoChoice, Resources.Insert.InsertDialog.InsertFormPropertyPhotoDescription);
        }
        public async Task AfterPhotoChoice(IDialogContext context, IAwaitable<IEnumerable<Attachment>> argument)
        {
            var photos = PhotoManager.ValidateUserPhotos((await argument).Take(4));
            if (photos.Count() > 0)
                _userProfile.insertParameters.insertPhotoPath = PhotoManager.DownloadUserInsertPhotos(context.Activity, photos);
            else
                this.AskFoPhoto(context,true);
            context.PrivateConversationData.SetValue("@userProfile", _userProfile);
            context.Done(Shared.Common.Insert.CheckField(context, _userOption));
        }
    }
}