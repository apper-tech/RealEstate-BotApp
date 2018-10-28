using AkaratakBot.EntityModel;
using Microsoft.Bot.Builder.Dialogs;
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
                            userPhotosNames += photoLocalPath + "|"; count++;
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



                    foreach (var item in photoPath.Split('|'))
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
}