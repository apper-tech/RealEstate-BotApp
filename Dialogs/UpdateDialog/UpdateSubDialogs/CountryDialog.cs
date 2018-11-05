using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace AkaratakBot.Dialogs.UpdateDialog.UpdateSubDialogs
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
            var entry = new RangeEntry {
                RangeCount = country_city_pager_count,
                RangeStart = _userProfile.insertParameters.insertCountryCurrentCount };

            PromptDialog.Choice(
               context,
               AfterCountryChoice,
               Shared.Common.Insert.GetCountryList(context, entry),
               Resources.Search.SearchDialog.SearchCategorySelection);
        }
        public async Task AfterCountryChoice(IDialogContext context, IAwaitable<SearchEntry> argument)
        {
            var message = await argument;
            if (message.searchValue == Resources.Insert.InsertDialog.InsertFormCountryNext)
            {
                _userProfile.updateParameters.updateCountryCurrentCount += country_city_pager_count;
                this.AskForCountry(context);
            }
            else
            {
                _userProfile.updateParameters.updateCountry = message.searchKey;
                context.PrivateConversationData.SetValue("@userProfile", _userProfile);
                this.AskForCity(context, message);
            }
        }
        private void AskForCity(IDialogContext context, SearchEntry message)
        {
            PromptDialog.Choice(context,
               AfteCityChoice, Shared.Common.Insert.GetCityList(context, message),
               Resources.Search.SearchDialog.SearchTypeSelection, Resources.BaseDialog.NotAValidOption,
               3, PromptStyle.Auto);
        }
        public async Task AfteCityChoice(IDialogContext context, IAwaitable<SearchEntry> argument)
        {
            var message = (await argument);

            _userProfile.updateParameters.updateCity = message.searchKey;
            context.PrivateConversationData.SetValue("@userProfile", _userProfile);

            context.Done(Shared.Common.Insert.CheckField(context, _userOption));

        }
    }
}