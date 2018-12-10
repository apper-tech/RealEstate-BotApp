using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Threading.Tasks;
using System.Web;
using static AkaratakBot.Shared.API.IOCommon;

namespace AkaratakBot.Dialogs.UpdateDialog.UpdateSubDialogs
{
    [Serializable]
    public class MiscDialog : IDialog<SearchEntry>
    {
        private MiscEntry _option;
        private UserProfile _userProfile;

        public MiscDialog(MiscEntry misc)
        {
            _option = misc;
            _option.userOptions = MiscEntry.GetMiscOption(_option);
        }
        public async Task StartAsync(IDialogContext context)
        {
            context.PrivateConversationData.TryGetValue("@userProfile", out _userProfile);
            switch (_option.userOptions)
            {
                case MiscOptions.PropertySize:
                    this.AskForPropertySize(context);
                    break;
                case MiscOptions.FloorCount:
                    this.AskForFloorCount(context);
                    break;
                case MiscOptions.BedRoomCount:
                    this.AskForBedroomCount(context);
                    break;
                case MiscOptions.BathroomCount:
                    this.AskForBathroomCount(context);
                    break;
                case MiscOptions.Address:
                    this.AskForAddress(context);
                    break;
                case MiscOptions.OtherDetails:
                    this.AskForOtherDetails(context);
                    break;
                case MiscOptions.FloorLevel:
                    this.AskForFloorLevel(context);
                    break;
                case MiscOptions.ZipCode:
                    this.AskForZipCode(context);
                    break;
            }
        }
        public void AskForPropertySize(IDialogContext context)
        {
            PromptDialog.Text(context, AfterNumberEntry, Resources.Update.UpdateDialog.UpdateFormPropertySizeDescription);
        }
        public void AskForAddress(IDialogContext context)
        {
            PromptDialog.Text(context, AfterTextEntry, Resources.Update.UpdateDialog.UpdateFormAddressTextDescription);
        }
        public void AskForZipCode(IDialogContext context)
        {
            PromptDialog.Text(context, AfterTextEntry, Resources.Update.UpdateDialog.UpdateFormZipCodeTextDescription);
        }
        public void AskForOtherDetails(IDialogContext context)
        {
            PromptDialog.Text(context, AfterTextEntry, Resources.Update.UpdateDialog.UpdateFormOtherDetailsTextDescription);
        }
        public void AskForFloorCount(IDialogContext context)
        {
            PromptDialog.Text(context, AfterNumberEntry, Resources.Update.UpdateDialog.UpdateFormFloorCountDescription);
        }
        public void AskForFloorLevel(IDialogContext context)
        {
            PromptDialog.Text(context, AfterNumberEntry, Resources.Update.UpdateDialog.UpdateFormFloorLevelCountDescription);
        }
        public void AskForBedroomCount(IDialogContext context)
        {
            PromptDialog.Text(context, AfterNumberEntry, Resources.Update.UpdateDialog.UpdateFormBedroomCountDescription);
        }
        public void AskForBathroomCount(IDialogContext context)
        {
            PromptDialog.Text(context, AfterNumberEntry, Resources.Update.UpdateDialog.UpdateFormBathroomCountDescription);
        }
        public async Task AfterNumberEntry(IDialogContext context, IAwaitable<string> argument)
        {
            var result = await argument;
            var message = CultureResourceManager.toEnglishNumber(result);
            switch (_option.userOptions)
            {
                case MiscOptions.PropertySize:
                    if (message == null)
                        this.AskForPropertySize(context);
                    else
                        _userProfile.insertParameters.insertSize = (int)message;
                    break;
                case MiscOptions.FloorCount:
                    if (message == null)
                        this.AskForFloorCount(context);
                    else
                        _userProfile.insertParameters.insertFloorCount = (int)message;
                    break;
                case MiscOptions.BathroomCount:
                    if (message == null)
                        this.AskForBathroomCount(context);
                    else
                        _userProfile.insertParameters.insertBathRoomCount = (int)message;
                    break;
                case MiscOptions.BedRoomCount:
                    if (message == null)
                        this.AskForBedroomCount(context);
                    else
                        _userProfile.insertParameters.insertBedRoomCount = (int)message;
                    break;
                case MiscOptions.FloorLevel:
                    if (message == null)
                        this.AskForFloorLevel(context);
                    else
                        _userProfile.insertParameters.insertFloorLevel = (int)message;
                    break;
            }

            CallBack(context, _userProfile);
        }
        public async Task AfterTextEntry(IDialogContext context, IAwaitable<string> argument)
        {
            var message = await argument;
            switch (_option.userOptions)
            {
                case MiscOptions.Address:
                    _userProfile.updateParameters.updateAddress = message;
                    break;
                case MiscOptions.OtherDetails:
                    _userProfile.updateParameters.updateOtherDetails = message;
                    break;
                case MiscOptions.ZipCode:
                    _userProfile.updateParameters.updateZipCode = message;
                    break;
            }
            CallBack(context, _userProfile);
        }
        public void CallBack(IDialogContext context, UserProfile userProfile)
        {
            context.PrivateConversationData.SetValue("@userProfile", _userProfile);
            context.Done(Shared.Common.Insert.CheckField(context, _option.userResource));
        }
    }
    [Serializable]
    public class MiscEntry
    {
        public MiscOptions userOptions { get; set; }
        public SearchEntry userResource { get; set; }

        public static List<MiscEntry> GetMiscEntries()
        {
            return new List<MiscEntry>
            {
                new MiscEntry
                {
                    userOptions= MiscOptions.FloorCount,
                    userResource=new SearchEntry{searchValue=Resources.Update.UpdateDialog.UpdateFieldFloorCount}
                },
                new MiscEntry
                {
                    userOptions= MiscOptions.PropertySize,
                    userResource=new SearchEntry{searchValue=Resources.Update.UpdateDialog.UpdateFieldPropertySize}
                },
                new MiscEntry
                {
                    userOptions= MiscOptions.BathroomCount,
                    userResource=new SearchEntry{searchValue=Resources.Update.UpdateDialog.UpdateFieldBathroomCount}
                },
                new MiscEntry
                {
                    userOptions= MiscOptions.BedRoomCount,
                    userResource=new SearchEntry{searchValue=Resources.Update.UpdateDialog.UpdateFieldBedroomCount}
                },
                new MiscEntry
                {
                    userOptions= MiscOptions.Address,
                    userResource=new SearchEntry{searchValue=Resources.Update.UpdateDialog.UpdateFieldAddressText}
                },
                new MiscEntry
                {
                    userOptions= MiscOptions.OtherDetails,
                    userResource=new SearchEntry{searchValue=Resources.Update.UpdateDialog.UpdateFieldOtherDetailsText}
                },
                 new MiscEntry
                {
                    userOptions= MiscOptions.FloorLevel,
                    userResource=new SearchEntry{searchValue=Resources.Update.UpdateDialog.UpdateFieldFloorLevelCount}
                },
                  new MiscEntry
                {
                    userOptions= MiscOptions.ZipCode,
                    userResource=new SearchEntry{searchValue=Resources.Update.UpdateDialog.UpdateFieldZipCodeText}
                }

            };
        }
        public static bool Contains(MiscEntry miscEntry, ResourceManager manager)
        {
            return (miscEntry.userResource.searchKey == Shared.API.IOCommon.CultureResourceManager.GetKey(
                manager, Shared.Common.Insert.CheckField(null, miscEntry.userResource, true).searchValue, true));

        }
        public static MiscOptions GetMiscOption(MiscEntry entry)
        {
            foreach (var item in GetMiscEntries())
                if (item.userResource == entry.userResource)
                    return item.userOptions;
            return MiscOptions.Default;
        }
    }
    [Serializable]
    public enum MiscOptions
    {
        PropertySize,
        FloorCount,
        BathroomCount,
        BedRoomCount,
        Address,
        OtherDetails,
        FloorLevel,
        ZipCode,
        Default
    }

}