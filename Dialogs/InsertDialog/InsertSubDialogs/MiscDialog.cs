﻿using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Threading.Tasks;
using System.Web;
using static AkaratakBot.Shared.API.IOCommon;

namespace AkaratakBot.Dialogs.InsertDialog.InsertSubDialogs
{
    [Serializable]
    public class MiscDialog : IDialog<SearchEntry>
    {
        private MiscEntry _option;
        private UserProfile _userProfile;

        public MiscDialog(MiscEntry misc)
        {
            _option = misc;
            _option.insertOptions = MiscEntry.GetMiscOption(_option);
        }
        public async Task StartAsync(IDialogContext context)
        {
            context.PrivateConversationData.TryGetValue("@userProfile", out _userProfile);
            switch (_option.insertOptions)
            {
                case MiscInsertOptions.PropertySize:
                    this.AskForPropertySize(context);
                    break;
                case MiscInsertOptions.FloorCount:
                    this.AskForFloorCount(context);
                    break;
                case MiscInsertOptions.BedRoomCount:
                    this.AskForBedroomCount(context);
                    break;
                case MiscInsertOptions.BathroomCount:
                    this.AskForBathroomCount(context);
                    break;
                case MiscInsertOptions.OtherDetails:
                    this.AskForOtherDetails(context);
                    break;
                case MiscInsertOptions.FloorLevel:
                    this.AskForFloorLevel(context);
                    break;
                case MiscInsertOptions.ZipCode:
                    this.AskForZipCode(context, false);
                    break;
            }
        }
        public void AskForPropertySize(IDialogContext context)
        {
            PromptDialog.Text(context, AfterNumberEntry, Resources.Insert.InsertDialog.InsertFormPropertySizeDescription);
        }

        public void AskForZipCode(IDialogContext context, bool error)
        {
            if (error)
                context.PostAsync(Resources.Insert.InsertDialog.InsertFormZipCodeError);
            PromptDialog.Text(context, AfterTextEntry, Resources.Insert.InsertDialog.InsertFormZipCodeTextDescription);
        }
        public async void AskForOtherDetails(IDialogContext context)
        {
            PromptDialog.Confirm(context, AfterOtherDetailsSelection, Resources.Insert.InsertDialog.InsertOtherDetailsSelection);
        }

        private async Task AfterOtherDetailsSelection(IDialogContext context, IAwaitable<bool> result)
        {
            if (await result)
            {
                this.AskForOtherDetailsEntry(context);
            }
            else
            {
                _userProfile.insertParameters.insertOtherDetails = string.Empty;
                CallBack(context, _userProfile);
            }
        }
        public async void AskForOtherDetailsEntry(IDialogContext context)
        {
            PromptDialog.Text(context, AfterOtherDetailsEntry, Resources.Insert.InsertDialog.InsertFormOtherDetailsTextDescription);
        }

        private async Task AfterOtherDetailsEntry(IDialogContext context, IAwaitable<string> result)
        {
            var message = await result;
            _userProfile.insertParameters.insertOtherDetails = message;
            CallBack(context, _userProfile);
        }

        public void AskForFloorCount(IDialogContext context)
        {
            PromptDialog.Text(context, AfterNumberEntry, Resources.Insert.InsertDialog.InsertFormFloorCountDescription);
        }
        public void AskForFloorLevel(IDialogContext context)
        {
            PromptDialog.Text(context, AfterNumberEntry, Resources.Insert.InsertDialog.InsertFormFloorLevelCountDescription);
        }
        public void AskForBedroomCount(IDialogContext context)
        {
            PromptDialog.Text(context, AfterNumberEntry, Resources.Insert.InsertDialog.InsertFormBedroomCountDescription);
        }
        public void AskForBathroomCount(IDialogContext context)
        {
            PromptDialog.Text(context, AfterNumberEntry, Resources.Insert.InsertDialog.InsertFormBathroomCountDescription);
        }
        public async Task AfterNumberEntry(IDialogContext context, IAwaitable<string> argument)
        {
            var result = await argument;
            var message = CultureResourceManager.toEnglishNumber(result);
            switch (_option.insertOptions)
            {
                case MiscInsertOptions.PropertySize:
                    if (message == null)
                        this.AskForPropertySize(context);
                    else
                        _userProfile.insertParameters.insertSize = (int)message;
                    break;
                case MiscInsertOptions.FloorCount:
                    if (message == null)
                        this.AskForFloorCount(context);
                    else
                        _userProfile.insertParameters.insertFloorCount = (int)message;
                    break;
                case MiscInsertOptions.BathroomCount:
                    if (message == null)
                        this.AskForBathroomCount(context);
                    else
                        _userProfile.insertParameters.insertBathRoomCount = (int)message;
                    break;
                case MiscInsertOptions.BedRoomCount:
                    if (message == null)
                        this.AskForBedroomCount(context);
                    else
                        _userProfile.insertParameters.insertBedRoomCount = (int)message;
                    break;
                case MiscInsertOptions.FloorLevel:
                    if (message == null)
                        this.AskForFloorLevel(context);
                    else
                        _userProfile.insertParameters.insertFloorLevel = (int)message;
                    break;
            }
            if (message != null)
                CallBack(context, _userProfile);
        }
        public async Task AfterTextEntry(IDialogContext context, IAwaitable<string> argument)
        {
            var message = await argument;
            bool error = false;
            switch (_option.insertOptions)
            {
                case MiscInsertOptions.ZipCode:
                    var value = string.Empty;
                    if (message != null && RegexManager.Compare(message, RegexManager.ZipCodeRegex, out value))
                        _userProfile.insertParameters.insertZipCode = value;
                    else
                    {
                        this.AskForZipCode(context, true);
                        error = true;
                    }
                    break;
            }
            if (!error)
                CallBack(context, _userProfile);
        }
        public void CallBack(IDialogContext context, UserProfile userProfile)
        {
            context.PrivateConversationData.SetValue("@userProfile", _userProfile);
            context.Done(Shared.Common.Insert.CheckField(context, _option.insertResource));
        }
    }
    [Serializable]
    public class MiscEntry
    {
        public MiscInsertOptions insertOptions { get; set; }
        public SearchEntry insertResource { get; set; }
        public static List<MiscEntry> GetMiscEntries()
        {
            return new List<MiscEntry>
            {
                new MiscEntry
                {
                    insertOptions= MiscInsertOptions.FloorCount,
                    insertResource=new SearchEntry{searchValue=Resources.Insert.InsertDialog.InsertFieldFloorCount}
                },
                new MiscEntry
                {
                    insertOptions= MiscInsertOptions.PropertySize,
                    insertResource=new SearchEntry{searchValue=Resources.Insert.InsertDialog.InsertFieldPropertySize}
                },
                new MiscEntry
                {
                    insertOptions= MiscInsertOptions.BathroomCount,
                    insertResource=new SearchEntry{searchValue=Resources.Insert.InsertDialog.InsertFieldBathroomCount}
                },
                new MiscEntry
                {
                    insertOptions= MiscInsertOptions.BedRoomCount,
                    insertResource=new SearchEntry{searchValue=Resources.Insert.InsertDialog.InsertFieldBedroomCount}
                },
                new MiscEntry
                {
                    insertOptions= MiscInsertOptions.OtherDetails,
                    insertResource=new SearchEntry{searchValue=Resources.Insert.InsertDialog.InsertFieldOtherDetailsText}
                },
                 new MiscEntry
                {
                    insertOptions= MiscInsertOptions.FloorLevel,
                    insertResource=new SearchEntry{searchValue=Resources.Insert.InsertDialog.InsertFieldFloorLevelCount}
                },
                  new MiscEntry
                {
                    insertOptions= MiscInsertOptions.ZipCode,
                    insertResource=new SearchEntry{searchValue=Resources.Insert.InsertDialog.InsertFieldZipCodeText}
                }

            };
        }
        public static bool Contains(MiscEntry miscEntry, ResourceManager manager)
        {
            return (miscEntry.insertResource.searchKey == Shared.API.IOCommon.CultureResourceManager.GetKey(
                manager, Shared.Common.Insert.CheckField(null, miscEntry.insertResource, true).searchValue, true));

        }
        public static MiscInsertOptions GetMiscOption(MiscEntry entry)
        {
            foreach (var item in GetMiscEntries())
                if (item.insertResource == entry.insertResource)
                    return item.insertOptions;
            return MiscInsertOptions.Default;
        }
    }
    [Serializable]
    public enum MiscInsertOptions
    {
        PropertySize,
        FloorCount,
        BathroomCount,
        BedRoomCount,
        OtherDetails,
        FloorLevel,
        ZipCode,
        Default
    }
}