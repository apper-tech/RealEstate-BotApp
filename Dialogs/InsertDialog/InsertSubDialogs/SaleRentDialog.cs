using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

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
            PromptDialog.Number(context, AfterSalePriceChoice, Resources.Insert.InsertDialog.InsertFormSalePriceCountDescription);
        }
        public void AskFoRentPrice(IDialogContext context)
        {
            PromptDialog.Number(context, AfterRentPriceChoice, Resources.Insert.InsertDialog.InsertFormRentPriceCountDescription);
        }
        public async Task AfterSalePriceChoice(IDialogContext context, IAwaitable<double> argument)
        {
            _userProfile.insertParameters.insertSalePrice =(int) await argument;
            context.PrivateConversationData.SetValue("@userProfile", _userProfile);
            this.AskFoRentPrice(context);
        }
        public async Task AfterRentPriceChoice(IDialogContext context, IAwaitable<double> argument)
        {
            _userProfile.insertParameters.insertRentPrice =(int) await argument;
            context.PrivateConversationData.SetValue("@userProfile", _userProfile);
            context.Done(Shared.Common.Insert.CheckField(context, _userOption));
        }
    }
}