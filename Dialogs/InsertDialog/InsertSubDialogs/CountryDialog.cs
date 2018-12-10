using AkaratakBot.Shared;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace AkaratakBot.Dialogs.InsertDialog.InsertSubDialogs
{
    [Serializable]
    public class CountryDialog : IDialog<SearchEntry>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.PrivateConversationData.TryGetValue("@userProfile", out _userProfile);
            this.AskForCountry(context);
        }
        public const int country_city_pager_count = 25;
        public CountryDialog(SearchEntry entry)
        {
            _userOption = entry;
        }
        private SearchEntry _userOption;
        private UserProfile _userProfile;

        private void AskForCountry(IDialogContext context)
        {
            var entry = new RangeEntry
            {
                RangeCount = country_city_pager_count,
                RangeStart = _userProfile.insertParameters.insertCountryCurrentCount
            };

            PromptDialog.Choice(
               context,
               AfterCountryChoice,
               Common.Insert.GetCountryList(context, entry),
               Resources.Insert.InsertDialog.InsertFormCountrySelection);
        }
        public async Task AfterCountryChoice(IDialogContext context, IAwaitable<SearchEntry> argument)
        {
            var message = await argument;
            if (message.searchValue == Resources.Insert.InsertDialog.InsertFormCountryNext)
            {
                _userProfile.insertParameters.insertCountryCurrentCount += country_city_pager_count;
                this.AskForCountry(context);
            }
            else if (message.searchValue == Resources.Insert.InsertDialog.InsertFormCountryReset)
            {
                _userProfile.insertParameters.insertCountryCurrentCount = 0;
                this.AskForCountry(context);
            }
            else
            {
                _userProfile.insertParameters.insertCountry = message.searchKey;
                context.PrivateConversationData.SetValue("@userProfile", _userProfile);
                this.AskForCity(context, message);
            }
        }
        private void AskForCity(IDialogContext context, SearchEntry message)
        {
            PromptDialog.Choice(context,
               AfteCityChoice, Shared.Common.Insert.GetCityList(context, message),
               Resources.Insert.InsertDialog.InsertFormCitySelection,
               Resources.BaseDialog.NotAValidOption,
               3, PromptStyle.Auto);
        }
        public async Task AfteCityChoice(IDialogContext context, IAwaitable<SearchEntry> argument)
        {
            var message = (await argument);

            _userProfile.insertParameters.insertCity = message.searchKey;
            context.PrivateConversationData.SetValue("@userProfile", _userProfile);
            this.AskForAddress(context);

        }
        public void AskForAddress(IDialogContext context)
        {
            PromptDialog.Text(context, AfterAddressEntry, Resources.Insert.InsertDialog.InsertFormAddressTextDescription);
        }

        private async Task AfterAddressEntry(IDialogContext context, IAwaitable<string> result)
        {
            _userProfile.insertParameters.insertAddress = await result;
            context.PrivateConversationData.SetValue("@userProfile", _userProfile);
            await this.AskForLocation(context);
        }
        public async Task AskForLocation(IDialogContext context)
        {
            var place = API.IOCommon.LocationManager.GeocodeUserLocation(_userProfile.insertParameters.insertAddress);
            _userProfile.insertParameters.insertLocation = API.IOCommon.LocationManager.GenerateLoactionString(place);
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