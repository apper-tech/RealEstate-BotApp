using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace AkaratakBot.Dialogs.Test_Dialogs
{
    [Serializable]
    public class TestCarouselCardsDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedAsync);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var reply = context.MakeMessage();
            reply.Type = "message";
            reply.Attachments = new List<Attachment>();

            List<CardAction> cardButtons = new List<CardAction>();
            CardAction cityBtn1 = new CardAction()
            {
                Value = "cleveland",
                Type = "postBack",
                Title = "Cleveland"
            };
            cardButtons.Add(cityBtn1);

            CardAction cityBtn2 = new CardAction()
            {
                Value = "columbus",
                Type = "postBack",
                Title = "Columbus"
            };
            cardButtons.Add(cityBtn2);

            HeroCard plCard = new HeroCard()
            {
                Title = "Please select what city you are in?",
                Buttons = cardButtons
            };
            Attachment plAttachment = plCard.ToAttachment();
            reply.Attachments.Add(plAttachment);
            await context.PostAsync(reply);
            context.Done(result);
        }
        private List<Attachment> TestMultiCard()
        {
            return new List<Attachment>(){ Shared.Cards.GetHeroCard(
                             "test Title",
                              "test Subtitle",
                             "test Description",
                             new CardImage(url: "https://insurancemarket.ng/images/thumbnails/649/220/detailed/3/26e178f.png"),
                             new CardAction(ActionTypes.OpenUrl,
                             "View Details",
                             value: "https://www.akaratak.com/Search/")
                             ) ,
            Shared.Cards.GetHeroCard(
                             "test Title",
                              "test Subtitle",
                             "test Description",
                             new CardImage(url: "https://insurancemarket.ng/images/thumbnails/649/220/detailed/3/26e178f.png"),
                             new CardAction(ActionTypes.OpenUrl,
                             "View Details",
                             value: "https://www.akaratak.com/Search/")
                             ),
            Shared.Cards.GetHeroCard(
                             "test Title",
                              "test Subtitle",
                             "test Description",
                             new CardImage(url: "https://insurancemarket.ng/images/thumbnails/649/220/detailed/3/26e178f.png"),
                             new CardAction(ActionTypes.OpenUrl,
                             "View Details",
                             value: "https://www.akaratak.com/Search/")
                             ),
            Shared.Cards.GetHeroCard(
                             "test Title",
                              "test Subtitle",
                             "test Description",
                             new CardImage(url: "https://insurancemarket.ng/images/thumbnails/649/220/detailed/3/26e178f.png"),
                             new CardAction(ActionTypes.OpenUrl,
                             "View Details",
                             value: "https://www.akaratak.com/Search/")
                             ), Shared.Cards.GetHeroCard(
                             "test Title",
                              "test Subtitle",
                             "test Description",
                             new CardImage(url: "https://insurancemarket.ng/images/thumbnails/649/220/detailed/3/26e178f.png"),
                             new CardAction(ActionTypes.OpenUrl,
                             "View Details",
                             value: "https://www.akaratak.com/Search/")
                             ), Shared.Cards.GetHeroCard(
                             "test Title",
                              "test Subtitle",
                             "test Description",
                             new CardImage(url: "https://insurancemarket.ng/images/thumbnails/649/220/detailed/3/26e178f.png"),
                             new CardAction(ActionTypes.OpenUrl,
                             "View Details",
                             value: "https://www.akaratak.com/Search/")
                             ), Shared.Cards.GetHeroCard(
                             "test Title",
                              "test Subtitle",
                             "test Description",
                             new CardImage(url: "https://insurancemarket.ng/images/thumbnails/649/220/detailed/3/26e178f.png"),
                             new CardAction(ActionTypes.OpenUrl,
                             "View Details",
                             value: "https://www.akaratak.com/Search/")
                             ), Shared.Cards.GetHeroCard(
                             "test Title",
                              "test Subtitle",
                             "test Description",
                             new CardImage(url: "https://insurancemarket.ng/images/thumbnails/649/220/detailed/3/26e178f.png"),
                             new CardAction(ActionTypes.OpenUrl,
                             "View Details",
                             value: "https://www.akaratak.com/Search/")
                             ), Shared.Cards.GetHeroCard(
                             "test Title",
                              "test Subtitle",
                             "test Description",
                             new CardImage(url: "https://insurancemarket.ng/images/thumbnails/649/220/detailed/3/26e178f.png"),
                             new CardAction(ActionTypes.OpenUrl,
                             "View Details",
                             value: "https://www.akaratak.com/Search/")
                             ), Shared.Cards.GetHeroCard(
                             "test Title",
                              "test Subtitle",
                             "test Description",
                             new CardImage(url: "https://insurancemarket.ng/images/thumbnails/649/220/detailed/3/26e178f.png"),
                             new CardAction(ActionTypes.OpenUrl,
                             "View Details",
                             value: "https://www.akaratak.com/Search/")
                             )};
        }
    }
}