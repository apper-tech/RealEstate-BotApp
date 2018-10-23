using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace AkaratakBot.Dialogs.SearchDialogs
{
    [Serializable]
    public class BaseDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {

            context.PrivateConversationData.TryGetValue("@userProfile", out _userProfile);
            this.AskForCategory(context);
        }
        UserProfile _userProfile;
        private void AskForCategory(IDialogContext context)
        {
            PromptDialog.Choice<SearchEntry>(
               context,
               AfterCategoryChoice,
               Shared.Common.Search._GetPropertyCategoryList(context,true),
               Resources.Search.SearchDialog.SearchCategorySelection);
        }
        private void AskForPropertyType(IDialogContext context, SearchEntry message)
        {
            PromptDialog.Choice<SearchEntry>(
               context,
               AfterTypeChoice,
               Shared.Common.Search._GetPropertyTypeList(context,message, true),
              Resources.Search.SearchDialog.SearchTypeSelection);
        }
        private void AskForGarden(IDialogContext context)
        {
            PromptDialog.Choice<SearchEntry>(
               context,
               AfterGardenChoice,
               Shared.Common.Search._GetPropertyGardenCount(context, _userProfile.searchParameters, true),
              Resources.Search.SearchDialog.SearchGardenSelection);

        }
        private void AskForGarage(IDialogContext context)
        {
            PromptDialog.Choice<SearchEntry>(
                context,
                AfterGarageChoice,
                Shared.Common.Search._GetPropertyGarageCount(context, _userProfile.searchParameters, true),
               Resources.Search.SearchDialog.SearchGarageSelection);

        }
        private void AskForContinue(IDialogContext context)
        {
            PromptDialog.Confirm(
               context,
               AfterContinueChoice,Resources.Search.SearchDialog.SearchContinue);

        }
        private void ShowSearchResults(IDialogContext context)
        {
            var reply = context.MakeMessage();
            _userProfile.searchParameters.searchMaxCount = 10;
            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            reply.Attachments = Shared.Common.Search._GetSearchResults(_userProfile.searchParameters);
            if (reply.Attachments.Count > 0)
                context.PostAsync(reply);
            else
                context.PostAsync(Resources.Search.SearchDialog.SearchEmptyResult);
            this.AskForContinue(context);
        }
        public async Task AfterCategoryChoice(IDialogContext context, IAwaitable<SearchEntry> argument)
        {
            var message = await argument;

            _userProfile.searchParameters.searchCategory = message.searchKey;
            context.PrivateConversationData.SetValue("@userProfile", _userProfile);
            this.AskForPropertyType(context, message);

        }
        public async Task AfterTypeChoice(IDialogContext context, IAwaitable<SearchEntry> argument)
        {
            var message = (await argument).searchKey.Replace(" ", "_");

            _userProfile.searchParameters.searchType = message;
            context.PrivateConversationData.SetValue("@userProfile", _userProfile);
            this.AskForGarden(context);

        }
        public async Task AfterGardenChoice(IDialogContext context, IAwaitable<SearchEntry> argument)
        {
            var message = await argument;
            _userProfile.searchParameters.searchHasGarden = message.searchChoice == SearchChoice.Yes ? true : false;
            context.PrivateConversationData.SetValue("@userProfile", _userProfile);
            this.AskForGarage(context);
        }
        public async Task AfterGarageChoice(IDialogContext context, IAwaitable<SearchEntry> argument)
        {
            var message = await argument;
            _userProfile.searchParameters.searchHasGarage = message.searchChoice == SearchChoice.Yes ? true : false;
            context.PrivateConversationData.SetValue("@userProfile", _userProfile);
            this.ShowSearchResults(context);
        }
        public async Task AfterContinueChoice(IDialogContext context, IAwaitable<bool> argument)
        {
            var option = await argument;
            if (option)
                this.AskForCategory(context);
            else
                context.Done(option);
        }
    }
}