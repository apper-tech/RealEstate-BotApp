using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace AkaratakBot.Dialogs.UpdateDialog.UpdateSubDialogs
{
    [Serializable]
    public class GardenGarageDialog : IDialog<SearchEntry>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.PrivateConversationData.TryGetValue("@userProfile", out _userProfile);
            this.AskForGarden(context);
        }
        private SearchEntry _userOption;
        private UserProfile _userProfile;
        public GardenGarageDialog(SearchEntry entry)
        {
            _userOption = entry;
        }
        public void AskForGarden(IDialogContext context)
        {
            PromptDialog.Confirm(context, AfterGardenChoice, Resources.Insert.InsertDialog.InsertGardenSelection);
        }
        public void AskForGarage(IDialogContext context)
        {
            PromptDialog.Confirm(context, AfterGarageChoice, Resources.Insert.InsertDialog.InsertGarageSelection);
        }
        public async Task AfterGardenChoice(IDialogContext context, IAwaitable<bool> argument)
        {
            _userProfile.updateParameters.updateHasGarden = await argument;
            context.PrivateConversationData.SetValue("@userProfile", _userProfile);
            this.AskForGarage(context);
        }
        public async Task AfterGarageChoice(IDialogContext context, IAwaitable<bool> argument)
        {
            _userProfile.updateParameters.updateHasGarage = await argument;
            context.PrivateConversationData.SetValue("@userProfile", _userProfile);
            context.Done(Shared.Common.Insert.CheckField(context, _userOption));
        }
    }
}