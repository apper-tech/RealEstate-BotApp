using AkaratakBot.Shared;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Location;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace AkaratakBot.Dialogs.Test_Dialogs
{
    [Serializable]
    public class TestChannelDataDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            //context.Call(new Location.API.Dialogs.LocationDialog.LocationBaseDialog(context.Activity), AfterLocation);
            context.Done(context.MakeMessage());
            
        }
        public async Task AfterLocation(IDialogContext context,IAwaitable<Activity> argument)
        {

        }
    }
}