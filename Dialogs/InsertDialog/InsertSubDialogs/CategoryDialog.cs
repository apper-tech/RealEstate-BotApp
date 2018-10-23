using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace AkaratakBot.Dialogs.InsertDialog.InsertSubDialogs
{
    [Serializable]
    public class CategoryDialog : IDialog<SearchEntry>
    {
        public async Task StartAsync(IDialogContext context)
        {           
            context.PrivateConversationData.TryGetValue("@userProfile", out _userProfile);
            this.AskForCategory(context);
        }
        private SearchEntry _userOption;
        private UserProfile _userProfile;
        public CategoryDialog(SearchEntry entry)
        {
            _userOption = entry;
        }
        private void AskForCategory(IDialogContext context)
        {
            PromptDialog.Choice<SearchEntry>(
               context,
               AfterCategoryChoice,
               Shared.Common.Insert.GetCategoryList(context),
               Resources.Search.SearchDialog.SearchCategorySelection);
        }
        private void AskForPropertyType(IDialogContext context, SearchEntry message)
        {
            PromptDialog.Choice<SearchEntry>(
               context,
               AfterTypeChoice,
               Shared.Common.Insert.GetTypeList(context, message),
              Resources.Search.SearchDialog.SearchTypeSelection);
        }
        public async Task AfterCategoryChoice(IDialogContext context, IAwaitable<SearchEntry> argument)
        {
            var message = await argument;

            _userProfile.insertParameters.insertCategory = message.searchKey ;
            context.PrivateConversationData.SetValue("@userProfile", _userProfile);
            this.AskForPropertyType(context, message);
        }
        public async Task AfterTypeChoice(IDialogContext context, IAwaitable<SearchEntry> argument)
        {
            var message = (await argument);

            _userProfile.insertParameters.insertType = message.searchKey;
            context.PrivateConversationData.SetValue("@userProfile", _userProfile);

            context.Done(Shared.Common.Insert.CheckField(context, _userOption));

        }
    }

}
