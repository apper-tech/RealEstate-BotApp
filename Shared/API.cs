using AkaratakBot.EntityModel;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Location;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Resources;
using System.Web;
using System.Web.Configuration;

namespace AkaratakBot.Shared
{
    public class API
    {
        public class IOCommon
        {
            public class PhotoManager
            {
                private static string _photoHomePath = ConfigurationManager.AppSettings["PhotosRootDialog"];
                private const char _photoSpliter = '|';
                public static IEnumerable<Attachment> ValidateUserPhotos(IEnumerable<Attachment> attachments)
                {
                    var res = new List<Attachment>();
                    foreach (var item in attachments)
                        if (item.ContentType.Contains("image"))
                            res.Add(item);
                    return res;
                }

                public static string DownloadUserInsertPhotos(IActivity activity, IEnumerable<Attachment> attachments)
                {

                    string userPhotosNames = string.Empty;
                    var userID = UserManager.CheckEmulatorTelegramData((Activity)activity);
                    var contextServer = HttpContext.Current.Server;
                    var userPhotoPath = contextServer.MapPath($"{_photoHomePath}/_temp/") + userID.callback_query.from.id;
                    if (!Directory.Exists(userPhotoPath))
                        Directory.CreateDirectory(userPhotoPath);

                    foreach (var item in Directory.GetFiles(userPhotoPath))
                        File.Delete(item);
                    int count = 1;
                    foreach (var item in attachments)
                        using (var client = new WebClient())
                        {

                            var photoLocalPath = $"{userPhotoPath}\\p{count}.jpg";
                            client.DownloadFile($"{item.ContentUrl}", photoLocalPath);
                            userPhotosNames += $"{photoLocalPath}{_photoSpliter}"; count++;
                        }


                    return userPhotosNames;
                }
                private static string GeneratePhotoName()
                {
                    int length = 15;
                    Random random = new Random();
                    string name = new string((from s in Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", length)
                                              select s[random.Next(s.Length)]).ToArray());
                    return name;
                }
                public static string UploadPhotoToHost(string photoPath)
                {
                    string _photoNames = string.Empty;
                    var ftpUsername = WebConfigurationManager.AppSettings["AkaratakFtpUsername"];
                    var ftpPassword = WebConfigurationManager.AppSettings["AkaratakFtpPassword"];
                    var ftpUrl = WebConfigurationManager.AppSettings["AkaratakFtpUrl"];



                    foreach (var item in photoPath.Split(_photoSpliter))
                    {
                        FileInfo file = new FileInfo(item);
                        string _photopath = GeneratePhotoName() + file.Extension;
                        using (WebClient client = new WebClient())
                        {
                            client.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
                            client.UploadFile($"{ftpUrl}/{_photopath}", WebRequestMethods.Ftp.UploadFile, file.FullName);
                        }
                        _photoNames += _photopath + "|";
                    }
                    return _photoNames;
                }
                public static string UploadPhotoToHost(string photoPath,string oldPhotoPath)
                {
                    var ftpUsername = WebConfigurationManager.AppSettings["AkaratakFtpUsername"];
                    var ftpPassword = WebConfigurationManager.AppSettings["AkaratakFtpPassword"];
                    var ftpUrl = WebConfigurationManager.AppSettings["AkaratakFtpUrl"];

                    using (WebClient client = new WebClient())
                    {
                        client.Credentials = new NetworkCredential(ftpUsername, ftpPassword);

                        var def = oldPhotoPath.Split(_photoSpliter).Count() - photoPath.Split(_photoSpliter).Count();
                        var photoNames = new List<string>();
                        photoNames.AddList(oldPhotoPath.Split(_photoSpliter));

                        if (def < 0)
                            for (int i = 0; i < Convert.ToUInt32(def); i++)
                                photoNames.Add(GeneratePhotoName() + ".jpg");

                        foreach (var oldPhoto in photoNames)
                            foreach (var newPhoto in photoPath.Split(_photoSpliter))
                                client.UploadFile($"{ftpUrl}/{oldPhoto}", WebRequestMethods.Ftp.UploadFile, new FileInfo(newPhoto).FullName);
                        
                    }

                    return oldPhotoPath;
                }
            }
            public class UserManager
            {
                public static TelegramData CheckEmulatorTelegramData(Activity activity)
                {
                    TelegramData userID = null;
                    var contextServer = HttpContext.Current.Server;
                    if (activity.ChannelId != "emulator")
                        userID = JsonConvert.DeserializeObject<TelegramData>(activity.ChannelData.ToString());
                    else
                        userID = JsonConvert.DeserializeObject<TelegramData>(File.ReadAllText(contextServer.MapPath("~/_root/_test/test.json")));
                    return userID;
                }
                public static string GetUserID(UserProfile userProfile,bool emulator)
                {

                    if (!emulator && userProfile.telegramData.callback_query != null)
                        using (AkaratakModel model = new AkaratakModel())
                            return model.Users.Where(x => x.Telegram_ID == userProfile.telegramData.callback_query.from.id).FirstOrDefault().User_ID;
                    else
                        return WebConfigurationManager.AppSettings["AkaratakBotUserID"];
                }
            }
            public class CultureResourceManager
            {
                public class UserLocationResourceManager : LocationResourceManager
                {
                    private readonly ResourceManager resourceManager;

                    /// <summary>
                    /// The <see cref="AddressSeparator"/> resource string.
                    /// </summary>
                    public override string AddressSeparator => this.GetResource(nameof(Resources.Shared.LocationResourceManager.AddressSeparator));

                    /// <summary>
                    /// The <see cref="AddToFavoritesAsk"/> resource string.
                    /// </summary>
                    public override string AddToFavoritesAsk => this.GetResource(nameof(Resources.Shared.LocationResourceManager.AddToFavoritesAsk));

                    /// <summary>
                    /// The <see cref="AddToFavoritesRetry"/> resource string.
                    /// </summary>
                    public override string AddToFavoritesRetry => this.GetResource(nameof(Resources.Shared.LocationResourceManager.AddToFavoritesRetry));

                    /// <summary>
                    /// The <see cref="AskForEmptyAddressTemplate"/> resource string.
                    /// </summary>
                    public override string AskForEmptyAddressTemplate => this.GetResource(nameof(Resources.Shared.LocationResourceManager.AskForEmptyAddressTemplate));

                    /// <summary>
                    /// The <see cref="AskForPrefix"/> resource string.
                    /// </summary>
                    public override string AskForPrefix => this.GetResource(nameof(Resources.Shared.LocationResourceManager.AskForPrefix));

                    /// <summary>
                    /// The <see cref="AskForTemplate"/> resource string.
                    /// </summary>
                    public override string AskForTemplate => this.GetResource(nameof(Resources.Shared.LocationResourceManager.AskForTemplate));

                    /// <summary>
                    /// The <see cref="CancelCommand"/> resource string.
                    /// </summary>
                    public override string CancelCommand => this.GetResource(nameof(Resources.Shared.LocationResourceManager.CancelCommand));

                    /// <summary>
                    /// The <see cref="CancelPrompt"/> resource string.
                    /// </summary>
                    public override string CancelPrompt => this.GetResource(nameof(Resources.Shared.LocationResourceManager.CancelPrompt));

                    /// <summary>
                    /// The <see cref="ConfirmationAsk"/> resource string.
                    /// </summary>
                    public override string ConfirmationAsk => this.GetResource(nameof(Resources.Shared.LocationResourceManager.ConfirmationAsk));

                    /// <summary>
                    /// The <see cref="ConfirmationInvalidResponse"/> resource string.
                    /// </summary>
                    public override string ConfirmationInvalidResponse => this.GetResource(nameof(Resources.Shared.LocationResourceManager.ConfirmationInvalidResponse));

                    /// <summary>
                    /// The <see cref="Country"/> resource string.
                    /// </summary>
                    public override string Country => this.GetResource(nameof(Resources.Shared.LocationResourceManager.Country));

                    /// <summary>
                    /// The <see cref="DeleteCommand"/> resource string.
                    /// </summary>
                    public override string DeleteCommand => this.GetResource(nameof(Resources.Shared.LocationResourceManager.DeleteCommand));

                    /// <summary>
                    /// The <see cref="DeleteFavoriteAbortion"/> resource string.
                    /// </summary>
                    public override string DeleteFavoriteAbortion => this.GetResource(nameof(Resources.Shared.LocationResourceManager.DeleteFavoriteAbortion));

                    /// <summary>
                    /// The <see cref="DeleteFavoriteConfirmationAsk"/> resource string.
                    /// </summary>
                    public override string DeleteFavoriteConfirmationAsk => this.GetResource(nameof(Resources.Shared.LocationResourceManager.DeleteFavoriteConfirmationAsk));

                    /// <summary>
                    /// The <see cref="DialogStartBranchAsk"/> resource string.
                    /// </summary>
                    public override string DialogStartBranchAsk => this.GetResource(nameof(Resources.Shared.LocationResourceManager.DialogStartBranchAsk));

                    /// <summary>
                    /// The <see cref="DuplicateFavoriteNameResponse"/> resource string.
                    /// </summary>
                    public override string DuplicateFavoriteNameResponse => this.GetResource(nameof(Resources.Shared.LocationResourceManager.DuplicateFavoriteNameResponse));

                    /// <summary>
                    /// The <see cref="EditCommand"/> resource string.
                    /// </summary>
                    public override string EditCommand => this.GetResource(nameof(Resources.Shared.LocationResourceManager.EditCommand));

                    /// <summary>
                    /// The <see cref="EditFavoritePrompt"/> resource string.
                    /// </summary>
                    public override string EditFavoritePrompt => this.GetResource(nameof(Resources.Shared.LocationResourceManager.EditFavoritePrompt));

                    /// <summary>
                    /// The <see cref="EnterNewFavoriteLocationName"/> resource string.
                    /// </summary>
                    public override string EnterNewFavoriteLocationName => this.GetResource(nameof(Resources.Shared.LocationResourceManager.EnterNewFavoriteLocationName));

                    /// <summary>
                    /// The <see cref="FavoriteAddedConfirmation"/> resource string.
                    /// </summary>
                    public override string FavoriteAddedConfirmation => this.GetResource(nameof(Resources.Shared.LocationResourceManager.FavoriteAddedConfirmation));

                    /// <summary>
                    /// The <see cref="FavoriteDeletedConfirmation"/> resource string.
                    /// </summary>
                    public override string FavoriteDeletedConfirmation => this.GetResource(nameof(Resources.Shared.LocationResourceManager.FavoriteDeletedConfirmation));

                    /// <summary>
                    /// The <see cref="FavoriteEdittedConfirmation"/> resource string.
                    /// </summary>
                    public override string FavoriteEdittedConfirmation => this.GetResource(nameof(Resources.Shared.LocationResourceManager.FavoriteEdittedConfirmation));

                    /// <summary>
                    /// The <see cref="FavoriteLocations"/> resource string.
                    /// </summary>
                    public override string FavoriteLocations => this.GetResource(nameof(Resources.Shared.LocationResourceManager.FavoriteLocations));

                    /// <summary>
                    /// The <see cref="HelpCommand"/> resource string.
                    /// </summary>
                    public override string HelpCommand => this.GetResource(nameof(Resources.Shared.LocationResourceManager.HelpCommand));

                    /// <summary>
                    /// The <see cref="HelpMessage"/> resource string.
                    /// </summary>
                    public override string HelpMessage => this.GetResource(nameof(Resources.Shared.LocationResourceManager.HelpMessage));

                    /// <summary>
                    /// The <see cref="InvalidFavoriteLocationSelection"/> resource string.
                    /// </summary>
                    public override string InvalidFavoriteLocationSelection => this.GetResource(nameof(Resources.Shared.LocationResourceManager.InvalidFavoriteLocationSelection));

                    /// <summary>
                    /// The <see cref="InvalidFavoriteNameResponse"/> resource string.
                    /// </summary>
                    public override string InvalidFavoriteNameResponse => this.GetResource(nameof(Resources.Shared.LocationResourceManager.InvalidFavoriteNameResponse));


                    /// <summary>
                    /// The <see cref="InvalidLocationResponse"/> resource string.
                    /// </summary>
                    public override string InvalidLocationResponse => this.GetResource(nameof(Resources.Shared.LocationResourceManager.InvalidLocationResponse));

                    /// <summary>
                    /// The <see cref="InvalidLocationResponseFacebook"/> resource string.
                    /// </summary>
                    public override string InvalidLocationResponseFacebook => this.GetResource(nameof(Resources.Shared.LocationResourceManager.InvalidLocationResponseFacebook));

                    /// <summary>
                    /// The <see cref="InvalidStartBranchResponse"/> resource string.
                    /// </summary>
                    public override string InvalidStartBranchResponse => this.GetResource(nameof(Resources.Shared.LocationResourceManager.InvalidStartBranchResponse));

                    /// <summary>
                    /// The <see cref="LocationNotFound"/> resource string.
                    /// </summary>
                    public override string LocationNotFound => this.GetResource(nameof(Resources.Shared.LocationResourceManager.LocationNotFound));

                    /// <summary>
                    /// The <see cref="Locality"/> resource string.
                    /// </summary>
                    public override string Locality => this.GetResource(nameof(Resources.Shared.LocationResourceManager.Locality));

                    /// <summary>
                    /// The <see cref="MultipleResultsFound"/> resource string.
                    /// </summary>
                    public override string MultipleResultsFound => this.GetResource(nameof(Resources.Shared.LocationResourceManager.MultipleResultsFound));

                    /// <summary>
                    /// The <see cref="NoFavoriteLocationsFound"/> resource string.
                    /// </summary>
                    public override string NoFavoriteLocationsFound => this.GetResource(nameof(Resources.Shared.LocationResourceManager.NoFavoriteLocationsFound));

                    /// <summary>
                    /// The <see cref="OtherComand"/> resource string.
                    /// </summary>
                    public override string OtherComand => this.GetResource(nameof(Resources.Shared.LocationResourceManager.OtherComand));

                    /// <summary>
                    /// The <see cref="OtherLocation"/> resource string.
                    /// </summary>
                    public override string OtherLocation => this.GetResource(nameof(Resources.Shared.LocationResourceManager.OtherLocation));

                    /// <summary>
                    /// The <see cref="PostalCode"/> resource string.
                    /// </summary>
                    public override string PostalCode => this.GetResource(nameof(Resources.Shared.LocationResourceManager.PostalCode));

                    /// <summary>
                    /// The <see cref="Region"/> resource string.
                    /// </summary>
                    public override string Region => this.GetResource(nameof(Resources.Shared.LocationResourceManager.Region));

                    /// <summary>
                    /// The <see cref="ResetCommand"/> resource string.
                    /// </summary>
                    public override string ResetCommand => this.GetResource(nameof(Resources.Shared.LocationResourceManager.ResetCommand));

                    /// <summary>
                    /// The <see cref="ResetPrompt"/> resource string.
                    /// </summary>
                    public override string ResetPrompt => this.GetResource(nameof(Resources.Shared.LocationResourceManager.ResetPrompt));

                    /// <summary>
                    /// The <see cref="SelectFavoriteLocationPrompt"/> resource string.
                    /// </summary>
                    public override string SelectFavoriteLocationPrompt => this.GetResource(nameof(Resources.Shared.LocationResourceManager.SelectFavoriteLocationPrompt));

                    /// <summary>
                    /// The <see cref="SelectLocation"/> resource string.
                    /// </summary>
                    public override string SelectLocation => this.GetResource(nameof(Resources.Shared.LocationResourceManager.SelectLocation));

                    /// <summary>
                    /// The <see cref="SingleResultFound"/> resource string.
                    /// </summary>
                    public override string SingleResultFound => this.GetResource(nameof(Resources.Shared.LocationResourceManager.SingleResultFound));

                    /// <summary>
                    /// The <see cref="StreetAddress"/> resource string.
                    /// </summary>
                    public override string StreetAddress => this.GetResource(nameof(Resources.Shared.LocationResourceManager.StreetAddress));

                    /// <summary>
                    /// The <see cref="TitleSuffix"/> resource string.
                    /// </summary>
                    public override string TitleSuffix => this.GetResource(nameof(Resources.Shared.LocationResourceManager.TitleSuffix));

                    /// <summary>
                    /// The <see cref="TitleSuffixFacebook"/> resource string.
                    /// </summary>
                    public override string TitleSuffixFacebook => this.GetResource(nameof(Resources.Shared.LocationResourceManager.TitleSuffixFacebook));

                    internal UserLocationResourceManager()
                    {
                        this.resourceManager = Resources.Shared.LocationResourceManager.ResourceManager;
                    }

                    private string GetResource(string name)
                    {
                        return this.resourceManager.GetString(name) ??
                               Resources.Shared.LocationResourceManager.ResourceManager.GetString(name);
                    }
                }
                public static bool Contains(ResourceManager resourceManager, string value, bool allCulture)
                {
                    if (allCulture)
                        foreach (var item in LanguageOption.CreateListOption())
                        {
                            var entry = resourceManager.GetResourceSet(new CultureInfo(item.Locale), true, true)
                                .OfType<DictionaryEntry>()
                                .FirstOrDefault(dictionaryEntry => dictionaryEntry.Value.ToString() == value);
                            if (entry.Key != null && !string.IsNullOrEmpty(entry.Key.ToString()))
                                return true;
                        }
                    return string.IsNullOrEmpty(resourceManager.GetString(value));

                }
                public static string GetKey(ResourceManager resourceManager, string value, bool allCulture)
                {
                    if (allCulture)
                        foreach (var item in LanguageOption.CreateListOption())
                        {
                            var _entry = resourceManager.GetResourceSet(new CultureInfo(item.Locale), true, true)
                                .OfType<DictionaryEntry>()
                                .FirstOrDefault(dictionaryEntry => dictionaryEntry.Value.ToString() == value);
                            if (_entry.Key != null && !string.IsNullOrEmpty(_entry.Key.ToString()))
                                return _entry.Key.ToString();
                        }
                    return string.Empty;

                }
            }
        }
      
    }
    public static class Extenstions
    {
        public static void AddList(this List<string> list, string[] itemsToAdd)
        {
            foreach (var item in itemsToAdd)
                if(!string.IsNullOrEmpty(item))
                list.Add(item);
        }
    }
}