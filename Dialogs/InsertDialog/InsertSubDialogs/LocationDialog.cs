using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Location;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;

namespace AkaratakBot.Dialogs.InsertDialog.InsertSubDialogs
{
    [Serializable]
    public class LocationDialog : IDialog<SearchEntry>
    {
        string _apiKey = WebConfigurationManager.AppSettings["BingMapsApiKey"];
        private SearchEntry _userOption;
        private UserProfile _userProfile;
        public async Task StartAsync(IDialogContext context)
        {
            context.PrivateConversationData.TryGetValue("@userProfile", out _userProfile);
            this.AskForLocation(context);
        }
        public LocationDialog(SearchEntry entry)
        {
            _userOption = entry;
        }
        public void AskForLocation(IDialogContext context)
        {
            var prompt = Resources.Insert.InsertDialog.InsertFormLocationLatLngDescription;

            context.Call( new Microsoft.Bot.Builder.Location.LocationDialog(
                _apiKey,
                context.Activity.ChannelId,
                prompt, 
                LocationOptions.ReverseGeocode,
                LocationRequiredFields.StreetAddress), AfterLocationChoice);
           
        }
        public async Task AfterLocationChoice(IDialogContext context,IAwaitable<Place> argument)
        {
            Place place = await argument;
            if (place != null)
            {
                var lat = ((GeoCoordinates)place.Geo).Latitude;
                var lng= ((GeoCoordinates)place.Geo).Longitude;
                _userProfile.insertParameters.insertLocation = $"{lat},{lng},9";
                context.PrivateConversationData.SetValue("@userProfile", _userProfile);
                context.Done(Shared.Common.Insert.CheckField(context, _userOption));
            }
        }
    }
}