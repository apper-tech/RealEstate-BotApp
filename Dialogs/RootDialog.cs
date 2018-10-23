using AkaratakBot.Shared;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Location;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;

namespace AkaratakBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private List<string> _options;

        public async Task StartAsync(IDialogContext context)
        {
            context.PrivateConversationData.SetValue(SettingsDialog.BaseDialog.UserLanguageToken, "en-US");
            context.Wait<Activity>(MessageReceivedAsync);
        }
        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<Activity> argument)
        {
            var message = await argument;
            this.ShowOptions(context);
        }
        private void ShowOptions(IDialogContext context)
        {
            _options = new List<string>() {
                      Resources.Search.SearchDialog.Search,
                      Resources.Settings.SettingsDialog.Settings,
                      Resources.Insert.InsertDialog.Insert,
                    //"Test Cards",
                    "Test Channel Data"
            };
            PromptDialog.Choice(context, this.OnOptionSelected, _options, Resources.BaseDialog.Greetings, "Not a valid option", 3);
        }
        UserProfile _userProfile;
        private async Task OnOptionSelected(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                string optionSelected = await result;
                _userProfile = _userProfile == null ? new UserProfile()
                {
                    searchParameters = new SearchParameters(),
                    insertParameters = new InsertParameters(),
                    settingsParameters = new SettingsParameters(),
                    telegramData = GetUserTelegramData(context)
                } : _userProfile;
                context.PrivateConversationData.SetValue("@userProfile", _userProfile);

                if (optionSelected == Resources.Search.SearchDialog.Search)
                    context.Call(new SearchDialogs.BaseDialog(), this.ResumeAfterOptionDialog);
                if (optionSelected == Resources.Settings.SettingsDialog.Settings)
                    context.Call(new SettingsDialog.BaseDialog(), this.ResumeAfterOptionDialog);
                if (optionSelected == Resources.Insert.InsertDialog.Insert)
                    context.Call(new InsertDialog.BaseDialog(), this.ResumeAfterOptionDialog);

                if (optionSelected == "Test Cards")
                {
                    context.Call(new Test_Dialogs.TestCarouselCardsDialog(), this.ResumeAfterOptionDialog);
                }
                if (optionSelected == "Test Channel Data")
                {
                    string id= _userProfile.telegramData.callback_query != null? _userProfile.telegramData.callback_query.from.id.ToString():"emulator";
                    await context.PostAsync($"User ID: {id}");
                }

            }
            catch (TooManyAttemptsException ex)
            {
                await context.PostAsync($"Ooops! Too many attempts :(. But don't worry, I'm handling that exception and you can try again!");

                context.Wait<Activity>(this.MessageReceivedAsync);
            }
        }
        private async Task ResumeAfterOptionDialog(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                var message = await result;
                this.ShowOptions(context);
            }
            catch (Exception ex)
            {
                await context.PostAsync($"Failed with message: {ex.Message}");
            }
        }
        public TelegramData GetUserTelegramData(IDialogContext context)
        {
            var message = context.Activity;
            string data = message.ChannelData.ToString();
            return JsonConvert.DeserializeObject<TelegramData>(data);
        }

    }
}