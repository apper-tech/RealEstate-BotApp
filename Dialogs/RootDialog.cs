﻿using AkaratakBot.Shared;
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
            _userProfile = _userProfile == null ? new UserProfile()
            {
                searchParameters = new SearchParameters(),
                insertParameters = new InsertParameters(),
                settingsParameters = new SettingsParameters(),
                updateParameters = new UpdateParameters(),
                telegramData = TelegramData.GetUserTelegramData(context)
            } : _userProfile;
            context.PrivateConversationData.SetValue("@userProfile", _userProfile);
            context.PrivateConversationData.SetValue(SettingsDialog.BaseDialog.UserLanguageToken, "en-US");
            context.Wait<Activity>(MessageReceivedAsync);
        }
        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<Activity> argument)
        {
            var message = await argument;
            await ShowOptions(context, message);
        }
        private async Task ShowOptions(IDialogContext context, Activity activity)
        {
            _options = new List<string>() {
                      Resources.Search.SearchDialog.Search,//Search
                      Resources.Settings.SettingsDialog.Settings,//Settings
                      Resources.Insert.InsertDialog.Insert,//Insert
                      //"Test PList"
                    //"Test Channel Data",
                    //"Test Date"
            };
            bool emulator = context.Activity.ChannelId == "emulator";
            if (_userProfile.telegramData.message == null)
            {
                _userProfile.telegramData = TelegramData.GetUserTelegramData(context);
                context.PrivateConversationData.SetValue("@userProfile", _userProfile);
            }
            var update = Common.Update.CheckUserHasProperty(_userProfile, emulator) ?//Update
                      Resources.Update.UpdateDialog.Update : string.Empty;
            if (update != string.Empty)
                _options.Add(update);
            if (_options.Contains(activity.Text))
                await RedirectUserInput(context, activity.Text);
            else
                PromptDialog.Choice(context, this.OnOptionSelected, _options, Resources.BaseDialog.Greetings, Resources.BaseDialog.NotAValidOption, 3);
        }
        UserProfile _userProfile;
        private async Task OnOptionSelected(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                await RedirectUserInput(context, await result);
            }
            catch (TooManyAttemptsException ex)
            {
                await context.PostAsync($"Ooops! Too many attempts :(. But don't worry, I'm handling that exception and you can try again!");

                context.Wait<Activity>(this.MessageReceivedAsync);
            }
        }
        private async Task RedirectUserInput(IDialogContext context, string input)
        {
            string optionSelected = input;


            if (optionSelected == Resources.Search.SearchDialog.Search)
                context.Call(new SearchDialogs.BaseDialog(), this.ResumeAfterOptionDialog);
            if (optionSelected == Resources.Settings.SettingsDialog.Settings)
                context.Call(new SettingsDialog.BaseDialog(), this.ResumeAfterOptionDialog);
            if (optionSelected == Resources.Insert.InsertDialog.Insert)
                context.Call(new InsertDialog.BaseDialog(), this.ResumeAfterOptionDialog);
            if (optionSelected == Resources.Update.UpdateDialog.Update)
                context.Call(new UpdateDialog.BaseDialog(), this.ResumeAfterOptionDialog);


            if (optionSelected == "Test Cards")
            {
                context.Call(new Test_Dialogs.TestCarouselCardsDialog(), this.ResumeAfterOptionDialog);
            }
            if (optionSelected == "Test PList")
            {
                var reply = context.MakeMessage();
                reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                var emulator = context.Activity.ChannelId == "emulator";
                reply.Attachments = Shared.Common.Update.GetPropertyList(Shared.API.IOCommon.UserManager.GetUserID(_userProfile, emulator));
                await context.PostAsync(reply);
                //API.IOCommon.PhotoManager.UploadPhotoToHost(@"C:\Users\mcs3d\Source\Repos\RealEstate-BotApp\_root\_images\_temp\123456789\p1.jpg", new EntityModel.Property());
            }
        }
        private async Task ResumeAfterOptionDialog(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                var message = await result;
                await ShowOptions(context, new Activity { Text = "None" });
            }
            catch (Exception ex)
            {
                API.IOCommon.Logger.Log(ex);
            }
        }


    }
}