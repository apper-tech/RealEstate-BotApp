using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace AkaratakBot.Dialogs.SettingsDialog
{
    [Serializable]
    public class BaseDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {

            context.PrivateConversationData.TryGetValue("@userProfile", out _userProfile);

            this.ShowSettingsMenu(context);
        }
        UserProfile _userProfile;
        public const string UserLanguageToken = "ULTN";
        private void ShowSettingsMenu(IDialogContext context)
        {
            PromptDialog.Choice(
               context,
               AfterSettingChoice,
               new string[] {
                   Resources.Settings.SettingsDialog.SettingsLanguage,
                   Resources.Settings.SettingsDialog.SettingsReturn
               },
               Resources.Settings.SettingsDialog.SettingsMenu);
        }
        public async Task AfterSettingChoice(IDialogContext context, IAwaitable<string> argument)
        {
            var message = await argument;

            if (message == Resources.Settings.SettingsDialog.SettingsLanguage)
            { this.ShowLanguageMenu(context); }
            else if (Shared.Common.CultureResourceManager.Contains(Resources.Settings.SettingsDialog.ResourceManager, message, true))
            { context.Done(context.MakeMessage()); }
        }
        public async Task AfterLanguageChoice(IDialogContext context, IAwaitable<LanguageOption> argument)
        {
            var optionSelected = await argument;
            context.PrivateConversationData.SetValue(UserLanguageToken, optionSelected.Locale);
            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(optionSelected.Locale);
            this.ShowSettingsMenu(context);
        }
        private void ShowLanguageMenu(IDialogContext context)
        {
            PromptDialog.Choice(
               context,
               AfterLanguageChoice,
               LanguageOption.CreateListOption(),
               "🌐");
        }
    }
}