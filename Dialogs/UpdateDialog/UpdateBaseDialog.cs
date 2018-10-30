using AkaratakBot.Dialogs.UpdateDialog.UpdateSubDialogs;
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
            _optionList = Shared.Common.Update.CreateForm(context);
            await this.ShowPropertyList(context);
        }
        UserProfile _userProfile;
        List<SearchEntry> _optionList;
        public async Task ShowPropertyList(IDialogContext context)
        {
            var reply = context.MakeMessage();
            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            reply.Attachments = Shared.Common.Update.GetPropertyList(Shared.API.IOCommon.UserManager.GetUserID(_userProfile, false));
            if (reply.Attachments.Count > 0)
                await context.PostAsync(reply);
            else
                await context.PostAsync(Resources.Search.SearchDialog.SearchEmptyResult);
            context.Wait<Activity>(AfterPropertyList);
        }
        public async Task AfterPropertyList(IDialogContext context, IAwaitable<Activity> argument)
        {
            var message = await argument;

            if (Shared.Common.Update.GetPropertyList(_userProfile)
                .Where(x => x.PropertyID == int.Parse(message.Text))
                .ToList().Count > 0)
            {
                _userProfile.updateParameters.updatePropertyID = int.Parse(message.Text);
                await this.ShowProgress(context);
            }
            else
                await this.ShowPropertyList(context);
        }
        public async Task ShowProgress(IDialogContext context)
        {
            PromptDialog.Choice(context,
          OnOptionSelected, _optionList,
          Resources.Update.UpdateDialog.UpdateFormHeader, Resources.BaseDialog.NotAValidOption,
          3, PromptStyle.Auto);
        }
        public async Task OnOptionSelected(IDialogContext context, IAwaitable<SearchEntry> argument)
        {
            var message = await argument;
           // var entry = new MiscEntry { insertResource = message };

            if (CheckUpdateFieldResource(message, Resources.Update.UpdateDialog.UpdateCancel))
                context.Done(context.MakeMessage());

            else if (CheckUpdateFieldResource(message, Resources.Update.UpdateDialog.UpdateFieldCategoryType))
                context.Call(new CategoryDialog(message), InsertOptionCallback);
        }
        public bool CheckUpdateFieldResource(SearchEntry message, string resource)
        {
            return message.searchKey == Shared.API.IOCommon.CultureResourceManager.GetKey(Resources.Update.UpdateDialog.ResourceManager,
                resource, true);
        }
        public async Task InsertOptionCallback(IDialogContext context, IAwaitable<SearchEntry> argument)
        {
            var message = await argument;

            _optionList[_optionList.FindIndex(x => x.searchKey == message.searchKey)] = message;
            if (Shared.Common.Update.CheckForm(context, _optionList))
                if (_optionList.Where(x => x.searchKey == "UpdateKey").ToList().Count == 0)
                    _optionList.Add(new SearchEntry { searchKey = "UpdateKey", searchValue = Resources.Update.UpdateDialog.Update + " ✔" });

            await AskForDone(context);
        }
        public async Task AskForDone(IDialogContext context)
        {
            PromptDialog.Confirm(context, AfterDoneChoice, Resources.Update.UpdateDialog.UpdateDoneConfirm);
        }
        public async Task AfterDoneChoice(IDialogContext context, IAwaitable<bool> argument)
        {
            if (await argument)
            {
                //update with current settings
            }
            else
                await this.ShowProgress(context);
        }
    }
}