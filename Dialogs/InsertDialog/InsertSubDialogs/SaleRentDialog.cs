using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using static AkaratakBot.Shared.API.IOCommon;

namespace AkaratakBot.Dialogs.InsertDialog.InsertSubDialogs
{
    [Serializable]
    public class SaleRentDialog : IDialog<SearchEntry>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.PrivateConversationData.TryGetValue("@userProfile", out _userProfile);
            this.AskForSalePrice(context);
        }
        private SearchEntry _userOption;
        private UserProfile _userProfile;
        public SaleRentDialog(SearchEntry entry)
        {
            _userOption = entry;
        }
        public void AskForSalePrice(IDialogContext context)
        {
            PromptDialog.Text(context, AfterSalePriceChoice, Resources.Insert.InsertDialog.InsertFormSalePriceCountDescription);
        }
        public void AskFoRentPrice(IDialogContext context)
        {
            PromptDialog.Text(context, AfterRentPriceChoice, Resources.Insert.InsertDialog.InsertFormRentPriceCountDescription);
        }
        public async Task AfterSalePriceChoice(IDialogContext context, IAwaitable<string> argument)
        {
            var message = await argument;
            var num = CultureResourceManager.toEnglishNumber(message);
            if (num != null)
            {
                _userProfile.insertParameters.insertSalePrice = (int)num;
                context.PrivateConversationData.SetValue("@userProfile", _userProfile);
                this.AskFoRentPrice(context);
            }
            else
                this.AskForSalePrice(context);
        }
        public async Task AfterRentPriceChoice(IDialogContext context, IAwaitable<string> argument)
        {
            var message = await argument;
            var num = CultureResourceManager.toEnglishNumber(message);
            if (num != null)
            {
                _userProfile.insertParameters.insertRentPrice = (int)num;
                context.PrivateConversationData.SetValue("@userProfile", _userProfile);
                context.Done(Shared.Common.Insert.CheckField(context, _userOption));
            }
            else
                this.AskFoRentPrice(context);
        }
    }
}