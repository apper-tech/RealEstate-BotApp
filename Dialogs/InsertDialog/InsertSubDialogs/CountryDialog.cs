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
                _userProfile.insertParameters.insertCountryCurrentCount += country_city_pager_count;
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
               Resources.Search.SearchDialog.SearchTypeSelection, Resources.BaseDialog.NotAValidOption,
               3, PromptStyle.Auto);
        }
        public async Task AfteCityChoice(IDialogContext context, IAwaitable<SearchEntry> argument)
        {
            var message = (await argument);

            _userProfile.insertParameters.insertCity = message.searchKey;
            context.PrivateConversationData.SetValue("@userProfile", _userProfile);

            context.Done(Shared.Common.Insert.CheckField(context, _userOption));

        }
    }
}