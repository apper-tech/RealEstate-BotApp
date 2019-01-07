using AkaratakBot.Shared;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Location;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using static AkaratakBot.Shared.API.IOCommon;

namespace AkaratakBot.Dialogs.UpdateDialog.UpdateSubDialogs
{
    [Serializable]
    public class LocationDialog : IDialog<SearchEntry>
    {

        private SearchEntry _userOption;
        private UserProfile _userProfile;
        public async Task StartAsync(IDialogContext context)
        {
            context.PrivateConversationData.TryGetValue("@userProfile", out _userProfile);
            this.AskForAddress(context);
        }
        public LocationDialog(SearchEntry entry)
        {
            _userOption = entry;
        }
        public void AskForAddress(IDialogContext context)
        {
            PromptDialog.Text(context, AfterAddressEntry, Resources.Update.UpdateDialog.UpdateFormAddressTextDescription);
        }

        private async Task AfterAddressEntry(IDialogContext context, IAwaitable<string> result)
        {
            var message = await result;
            string value = string.Empty;
            if (RegexManager.Compare(message, RegexManager.AddressRegex, out value))
            {
                _userProfile.updateParameters.updateAddress = value;
                context.PrivateConversationData.SetValue("@userProfile", _userProfile);
                await this.AskForLocation(context);
            }
            else
            {
                await context.PostAsync(Resources.Insert.InsertDialog.InsertFormAddressError);
                this.AskForAddress(context);
            }
        }
        public async Task AskForLocation(IDialogContext context)
        {
            var place = API.IOCommon.LocationManager.GeocodeUserLocation(_userProfile.updateParameters.updateAddress);
            _userProfile.updateParameters.updateLocation = API.IOCommon.LocationManager.GenerateLoactionString(place);
            context.PrivateConversationData.SetValue("@userProfile", _userProfile);
            var replay = context.MakeMessage();
            replay.Attachments.Add(API.IOCommon.LocationManager.GenerateImageByLocation(place));
            await context.PostAsync(replay);
            PromptDialog.Confirm(context, AfterLocationEntry, Resources.Insert.InsertDialog.InsertFormConfirmLocation);
        }
        private async Task AfterLocationEntry(IDialogContext context, IAwaitable<bool> result)
        {
            if (await result)
                context.Done(Common.Insert.CheckField(context, _userOption));
            else
            {
                _userProfile.insertParameters.insertLocation = string.Empty;
                context.PrivateConversationData.SetValue("@userProfile", _userProfile);
                this.AskForAddress(context);
            }
        }
    }
}