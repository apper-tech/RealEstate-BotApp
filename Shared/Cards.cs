using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AkaratakBot.Shared
{
    public class Cards
    {
        public static ThumbnailCard GetThumbnailCard(string title, string subtitle, string text, string imageurl)
        {
            var heroCard = new ThumbnailCard
            {
                Title = title,
                Subtitle = subtitle,
                Text = text,
                Images = new List<CardImage> { new CardImage(imageurl) },
            };

            return heroCard;
        }
        public static Attachment GetHeroCard(string title, string subtitle, string text, CardImage cardImage, CardAction cardAction)
        {
            var heroCard = new HeroCard
            {
                Title = title,
                Subtitle = subtitle,
                Text = text,
                Images = new List<CardImage>() { cardImage },
                Buttons = new List<CardAction>() { cardAction },
            };

            return heroCard.ToAttachment();
        }
        public static Attachment GetCancelCard()
        {
            return new HeroCard
            {
                Buttons = new List<CardAction>
                {
                    new CardAction
                    {
                        Text=Resources.Insert.InsertDialog.InsertCancel,
                        DisplayText=Resources.Insert.InsertDialog.InsertCancel,
                        Title=Resources.Insert.InsertDialog.InsertCancel,
                        Type=ActionTypes.MessageBack,
                        Value="Cancel"
                    }
                }
            }.ToAttachment();
        }       public static Attachment GetButtonCard(string text, string value)
        {
            return new HeroCard
            {
                Buttons = new List<CardAction>
                {
                    new CardAction
                    {
                        Text=text,
                        DisplayText=text,
                        Title=text,
                        Type=ActionTypes.MessageBack,
                        Value=value
                    }
                }
            }.ToAttachment();
        }
    }

}