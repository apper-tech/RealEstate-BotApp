using AkaratakBot.Dialogs.InsertDialog.InsertSubDialogs;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace AkaratakBot.Dialogs.InsertDialog
{
    [Serializable]
    public class BaseDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {

            context.PrivateConversationData.TryGetValue("@userProfile", out _userProfile);
            _optionList = Shared.Common.Insert.CreateForm(context);
            await this.ShowProgress(context);
        }
        UserProfile _userProfile;
        List<SearchEntry> _optionList;
        public bool CheckInsertFieldResource(SearchEntry message, string resource)
        {
            return message.searchKey == Shared.Common.CultureResourceManager.GetKey(Resources.Insert.InsertDialog.ResourceManager,
                resource, true);
        }

        public async Task ShowProgress(IDialogContext context)
        {
            PromptDialog.Choice(context,
                AfterInsertChoice, _optionList,
                Resources.Insert.InsertDialog.InsertFormHeader, Resources.BaseDialog.NotAValidOption,
                3, PromptStyle.Auto);
        }
        public async Task AfterInsertChoice(IDialogContext context, IAwaitable<SearchEntry> argument)
        {
            var message = await argument;
            var entry = new MiscEntry { insertResource = message };
            
            if (CheckInsertFieldResource(message, Resources.Insert.InsertDialog.InsertFieldCategoryType))
                context.Call(new CategoryDialog(message), InsertOptionCallback);

            else if (CheckInsertFieldResource(message, Resources.Insert.InsertDialog.InsertFieldCountryCity))
                context.Call(new CountryDialog(message), InsertOptionCallback);

            else if (CheckInsertFieldResource(message, Resources.Insert.InsertDialog.InsertFieldGardenGarageChoice))
                context.Call(new GardenGarageDialog(message), InsertOptionCallback);

            else if (CheckInsertFieldResource(message, Resources.Insert.InsertDialog.InsertFieldSaleRentPriceCount))
                context.Call(new SaleRentDialog(message), InsertOptionCallback);

            else if (CheckInsertFieldResource(message, Resources.Insert.InsertDialog.InsertFieldPhotoSelection))
                context.Call(new PhotoDialog(message), InsertOptionCallback);

            else if (CheckInsertFieldResource(message, Resources.Insert.InsertDialog.InsertFieldLocationLatLng))
                context.Call(new LocationDialog(message), InsertOptionCallback);

            else if (MiscEntry.Contains(entry, Resources.Insert.InsertDialog.ResourceManager))
                context.Call(new MiscDialog(entry), InsertOptionCallback);
            
            else if(message.searchKey=="InsertKey")
            {
                context.Call(new PhoneNumberDialog(), InsertPhoneCallback);
            }
        }
        public async Task InsertOptionCallback(IDialogContext context, IAwaitable<SearchEntry> argument)
        {
            var message = await argument;

            _optionList[_optionList.FindIndex(x => x.searchKey == message.searchKey)] = message;
            if (Shared.Common.Insert.CheckForm(context, _optionList))
                _optionList.Add(new SearchEntry { searchKey = "InsertKey", searchValue = Resources.Insert.InsertDialog.Insert+ " ✔" });

            await this.ShowProgress(context);
        }
        public async Task InsertPhoneCallback(IDialogContext context,IAwaitable<object> result)
        {
            context.PrivateConversationData.TryGetValue("@userProfile", out _userProfile);
            if (Shared.Common.Insert.InsertForm(context, _userProfile))
                await context.PostAsync(Resources.Insert.InsertDialog.InsertDone);
            else
                await context.PostAsync("Error");
            context.Done(context.MakeMessage());
        }
    }
}