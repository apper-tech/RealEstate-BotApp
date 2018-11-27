using Microsoft.Bot.Builder.Dialogs;
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
                    this.AskForZipCode(context);
                    break;
            }
        }
        public void AskForPropertySize(IDialogContext context)
        {
            PromptDialog.Text(context, AfterNumberEntry, Resources.Insert.InsertDialog.InsertFormPropertySizeDescription);
        }

        public void AskForZipCode(IDialogContext context)
        {
            PromptDialog.Text(context, AfterTextEntry, Resources.Insert.InsertDialog.InsertFormZipCodeTextDescription);
        }
        public void AskForOtherDetails(IDialogContext context)
        {
            PromptDialog.Text(context, AfterTextEntry, Resources.Insert.InsertDialog.InsertFormOtherDetailsTextDescription);
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
                    _userProfile.insertParameters.insertSize = message;
                    break;
                case MiscInsertOptions.FloorCount:
                    _userProfile.insertParameters.insertFloorCount = message;
                    break;
                case MiscInsertOptions.BathroomCount:
                    _userProfile.insertParameters.insertBathRoomCount = message;
                    break;
                case MiscInsertOptions.BedRoomCount:
                    _userProfile.insertParameters.insertBedRoomCount = message;
                    break;
                case MiscInsertOptions.FloorLevel:
                    _userProfile.insertParameters.insertFloorLevel = message;
                    break;
            }

            CallBack(context, _userProfile);
        }
        public async Task AfterTextEntry(IDialogContext context, IAwaitable<string> argument)
        {
            var message = await argument;
            switch (_option.insertOptions)
            {
                case MiscInsertOptions.OtherDetails:
                    _userProfile.insertParameters.insertOtherDetails = message;
                    break;
                case MiscInsertOptions.ZipCode:
                    _userProfile.insertParameters.insertZipCode = message;
                    break;
            }
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