using AkaratakBot.EntityModel;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Location;
using Microsoft.Bot.Connector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Web.Configuration;
using System.ComponentModel.Design;
using System.Web;
using System.Reflection;

namespace AkaratakBot.Shared
{
    public class Common
    {
        public class Search
        {
            public static string _logoImageUrl = ConfigurationManager.AppSettings["AkaratakLogoImageUrl"];
            public static string _tokenGet = ConfigurationManager.AppSettings["AkaratakServiceToken"];
            public static List<SearchEntry> _GetPropertyCategoryList(IDialogContext context, bool showCount)
            {
                List<SearchEntry> entries = new List<SearchEntry>();
                ResourceManager resourceManager = new ResourceManager(typeof(Resources.Shared.Property_Category));
                ResourceSet resourceSet = resourceManager.GetResourceSet(new CultureInfo(context.PrivateConversationData.GetValueOrDefault<string>("ULTN") != null ? context.PrivateConversationData.GetValueOrDefault<string>("ULTN") : "en-US"), true, true);
                foreach (DictionaryEntry item in resourceSet)
                    entries.Add(new SearchEntry() { searchKey = item.Key.ToString(), searchValue = item.Value.ToString() });
                if (showCount)
                    using (var model = new AkaratakModel())
                    {
                        for (int i = 0; i < entries.Count; i++)
                        {
                            string _category = entries[i].searchKey;
                            entries[i].searchValue += $" ( { model.Properties.Where(c => c.Property_Category.Cat_Name == _category).Count() } )";
                        }
                    }
                return entries;
            }
            public static List<SearchEntry> _GetPropertyTypeList(IDialogContext context, SearchEntry category, bool showCount)
            {
                List<SearchEntry> types = new List<SearchEntry>();
                using (var model = new AkaratakModel())
                {
                    var L2EQuery = from item in model.Property_Type
                                   where item.Property_Category.Cat_Name == category.searchKey
                                   select item;

                    var queryResult = L2EQuery.ToList();
                    CultureInfo culture = new CultureInfo(context.PrivateConversationData.GetValueOrDefault<string>(Dialogs.SettingsDialog.BaseDialog.UserLanguageToken));
                    ResourceManager resourceManager = new ResourceManager(typeof(Resources.Shared.Property_Type));
                    ResourceSet resourceSet = resourceManager.GetResourceSet(culture != null ? culture : new CultureInfo("en-US"), true, true);
                    var result = from first in resourceSet.Cast<DictionaryEntry>()
                                 join second in queryResult
                                 on first.Key.ToString() equals second.Property_Type_Name
                                 into matches
                                 where matches.Any()
                                 select first;
                    foreach (var item in result)
                        types.Add(new SearchEntry() { searchKey = item.Key.ToString(), searchValue = item.Value.ToString() });
                    if (showCount)
                        for (int i = 0; i < types.Count; i++)
                        {
                            string _category = category.searchKey;
                            string _type = types[i].searchKey;

                            types[i].searchValue += $@" ( { model.Properties.Where(c => c.Property_Category.Cat_Name == _category
                            && c.Property_Type.Property_Type_Name == _type).Count() } )";
                        }
                }
                return types;
            }
            public static List<SearchEntry> _GetPropertyGardenCount(IDialogContext context, SearchParameters parameters, bool showCount)
            {
                using (var model = new AkaratakModel())
                {
                    var L2EQuery = from item in model.Properties
                                   where item.Property_Category.Cat_Name == parameters.searchCategory &&
                                         item.Property_Type.Property_Type_Name == parameters.searchType
                                   select item;

                    var queryResult = L2EQuery.ToList();
                    if (queryResult.Count == 0 || !showCount)
                        return SearchEntry.CreateChoiceList();

                    int _countYes = 0;
                    foreach (var item in queryResult)
                        _countYes += item.Has_Garden == true ? 1 : 0;

                    var _optionList = SearchEntry.CreateChoiceList();

                    _optionList.Where(i => i.searchKey == SearchChoice.Yes.ToString()).FirstOrDefault().searchValue += $" ({_countYes} ) ";
                    _optionList.Where(i => i.searchKey == SearchChoice.No.ToString()).FirstOrDefault().searchValue += $" ({queryResult.Count - _countYes} ) ";
                    _optionList.Where(i => i.searchKey == SearchChoice.Both.ToString()).FirstOrDefault().searchValue += $" ({queryResult.Count} ) ";

                    return _optionList;
                }
            }
            public static List<SearchEntry> _GetPropertyGarageCount(IDialogContext context, SearchParameters parameters, bool showCount)
            {
                using (var model = new AkaratakModel())
                {
                    var L2EQuery = from item in model.Properties
                                   where item.Property_Category.Cat_Name == parameters.searchCategory &&
                                         item.Property_Type.Property_Type_Name == parameters.searchType
                                   select item;

                    var queryResult = L2EQuery.ToList();
                    if (queryResult.Count == 0 || !showCount)
                        return SearchEntry.CreateChoiceList();

                    int _countYes = 0;
                    foreach (var item in queryResult)
                        _countYes += item.Has_Garage == true ? 1 : 0;

                    var _optionList = SearchEntry.CreateChoiceList();

                    _optionList.Where(i => i.searchKey == SearchChoice.Yes.ToString()).FirstOrDefault().searchValue += $" ({_countYes} ) ";
                    _optionList.Where(i => i.searchKey == SearchChoice.No.ToString()).FirstOrDefault().searchValue += $" ({queryResult.Count - _countYes} ) ";
                    _optionList.Where(i => i.searchKey == SearchChoice.Both.ToString()).FirstOrDefault().searchValue += $" ({queryResult.Count} ) ";

                    return _optionList;
                }
            }
            public static IList<Attachment> _GetSearchResults(SearchParameters searchParameters)
            {
                IList<Attachment> attachments = new List<Attachment>();
                using (var context = new AkaratakModel())
                {
                    int cat_id = (from cat in context.Property_Category where cat.Cat_Name == searchParameters.searchCategory select cat.Cat_ID).FirstOrDefault();
                    var typ_id = (from typ in context.Property_Type where typ.Cat_ID == cat_id && typ.Property_Type_Name == searchParameters.searchType select typ.Property_Type_ID).FirstOrDefault();
                    var L2EQuery = from item in context.Properties
                                   where item.Property_Category_ID == cat_id &&
                                   item.Property_Type_ID == typ_id &&
                                   item.Has_Garage == searchParameters.searchHasGarage &&
                                   item.Has_Garden == searchParameters.searchHasGarden
                                   select item;
                    try
                    {
                        var result = L2EQuery.ToList().Take(searchParameters.searchMaxCount);
                        foreach (var item in result)
                            attachments.Add(Cards.GetHeroCard(
                                 searchParameters.searchType,
                                 item.Address,
                                 item.Other_Details,
                                 new CardImage(url: (_ConstructPropertyImageUrl(item))),
                                 new CardAction(ActionTypes.OpenUrl,
                                Resources.Search.SearchDialog.SearchViewDetails,
                                 value: _ConstructPropertyDetailsUrl(item))
                                 ));
                    }
                    catch (Exception ex)
                    {
                        API.IOCommon.Logger.Log(ex);
                    }

                }
                return attachments;
            }
            public static EntityModel.Property_Category _GetPropertyCategoryByName(SearchEntry category)
            {
                using (var context = new AkaratakModel())
                {
                    Property_Category cat_id = (from cat in context.Property_Category where cat.Cat_Name == category.searchKey select cat).FirstOrDefault();
                    return cat_id;
                }
            }
            public static EntityModel.Property_Type _GetPropertyTypeByName(SearchEntry type)
            {
                using (var context = new AkaratakModel())
                {
                    Property_Type typ_id = (from typ in context.Property_Type where typ.Property_Type_Name == type.searchKey select typ).FirstOrDefault();
                    return typ_id;
                }
            }
            public static string _ConstructPropertyDetailsUrl(Property property)
            {
                return "https://www.akaratak.com/Details/" +
                    property.PropertyID + "/" +
                    property.Country.Country_Name + "/" +
                    property.City.City_Name + "/" +
                    property.Property_Category.Cat_Name + "/" +
                    property.Property_Type.Property_Type_Name + "/" +
                    property.Address;
            }
            public static string _ConstructPropertyImageUrl(Property property)
            {
                if (!string.IsNullOrEmpty(property.Property_Photo))
                    return "https://www.akaratak.com/RealEstate/PropertyImage/" + property.Property_Photo.Split('|')[0];
                else if (property.Property_Photos.Count > 0)
                    return Uri.EscapeUriString(property.Property_Photos.ToList()[0].Photo_Url);
                else
                    return HttpContext.Current.Server.MapPath("~/_root/_images/_default/defslider.jpg");
            }
        }
        public class Insert
        {
            public static List<SearchEntry> CreateForm(IDialogContext context, UserProfile userProfile, out bool skipEntry)
            {
                var activity = (Activity)context.Activity;
                var locale = !string.IsNullOrEmpty(activity.Locale)
                    && !(activity.Locale.Contains("en-US") 
                    || activity.Locale.Contains("en")) 
                    ? "." + activity.Locale : string.Empty;
                var resxPath = WebConfigurationManager.AppSettings["InsertResourceLocation"];
                string resxFilename = HttpContext.Current.Server.MapPath($"{resxPath}{locale}.resx");

                List<SearchEntry> entries = GetFormEntriesFromResource(resxFilename);
                bool entry = false;

                if (userProfile.insertParameters != null)
                    entries = CheckCreateFormContinue(entries, userProfile, out entry);
                skipEntry = entry;
                return entries;
            }
            private static List<SearchEntry> CheckCreateFormContinue(List<SearchEntry> entries, UserProfile userProfile, out bool entry)
            {
                entry = true;
                var insertParams = userProfile.insertParameters;

                //category
                if (!string.IsNullOrEmpty(insertParams.insertCategory) && !string.IsNullOrEmpty(insertParams.insertType))
                    entries.Remove(entries.Where(x => x.searchKey.Contains(nameof(Resources.Insert.InsertDialog.InsertFieldCategoryType))).FirstOrDefault());
                //country
                if (!string.IsNullOrEmpty(insertParams.insertCountry) && !string.IsNullOrEmpty(insertParams.insertCity))
                    entries.Remove(entries.Where(x => x.searchKey.Contains(nameof(Resources.Insert.InsertDialog.InsertFieldCountryCity))).FirstOrDefault());
                //bathroom
                if (insertParams.insertBedRoomCount > 0)
                    entries.Remove(entries.Where(x => x.searchKey.Contains(nameof(Resources.Insert.InsertDialog.InsertFieldBedroomCount))).FirstOrDefault());
                //bedroom
                if (insertParams.insertBathRoomCount > 0)
                    entries.Remove(entries.Where(x => x.searchKey.Contains(nameof(Resources.Insert.InsertDialog.InsertFieldBathroomCount))).FirstOrDefault());
                //floor count
                if (insertParams.insertFloorCount > 0)
                    entries.Remove(entries.Where(x => x.searchKey.Contains(nameof(Resources.Insert.InsertDialog.InsertFieldFloorCount))).FirstOrDefault());
                //floor
                if (insertParams.insertFloorLevel > 0)
                    entries.Remove(entries.Where(x => x.searchKey.Contains(nameof(Resources.Insert.InsertDialog.InsertFieldFloorLevelCount))).FirstOrDefault());
                //size
                if (insertParams.insertSize > 0)
                    entries.Remove(entries.Where(x => x.searchKey.Contains(nameof(Resources.Insert.InsertDialog.InsertFieldPropertySize))).FirstOrDefault());
                //sale/rent
                if (insertParams.insertSalePrice > 0 || insertParams.insertRentPrice > 0)
                    entries.Remove(entries.Where(x => x.searchKey.Contains(nameof(Resources.Insert.InsertDialog.InsertFieldSaleRentPriceCount))).FirstOrDefault());
                //zipcode
                if (!string.IsNullOrEmpty(insertParams.insertZipCode))
                    entries.Remove(entries.Where(x => x.searchKey.Contains(nameof(Resources.Insert.InsertDialog.InsertFieldZipCodeText))).FirstOrDefault());
                //other
                if (!string.IsNullOrEmpty(insertParams.insertOtherDetails))
                    entries.Remove(entries.Where(x => x.searchKey.Contains(nameof(Resources.Insert.InsertDialog.InsertFieldOtherDetailsText))).FirstOrDefault());
                //location
                if (!string.IsNullOrEmpty(insertParams.insertLocation))
                    entries.Remove(entries.Where(x => x.searchKey.Contains(nameof(Resources.Insert.InsertDialog.InsertFormLocationLatLng))).FirstOrDefault());
                //photo
                if (!string.IsNullOrEmpty(insertParams.insertPhotoPath))
                    entries.Remove(entries.Where(x => x.searchKey.Contains(nameof(Resources.Insert.InsertDialog.InsertFieldPhotoSelection))).FirstOrDefault());
                return entries;
            }
            private static List<SearchEntry> GetFormEntriesFromResource(string resxFilename)
            {
                List<SearchEntry> entries = new List<SearchEntry>();
                ResXResourceReader rr = new ResXResourceReader(resxFilename);
                rr.UseResXDataNodes = true;
                IDictionaryEnumerator dict = rr.GetEnumerator();
                while (dict.MoveNext())
                {
                    ResXDataNode node = (ResXDataNode)dict.Value;
                    if (node.Name.Contains("InsertField"))
                        entries.Add(new SearchEntry
                        {
                            searchKey = node.Name,
                            searchValue = node.GetValue((ITypeResolutionService)null).ToString(),
                            searchOrder = !string.IsNullOrEmpty(node.Comment) ? int.Parse(node.Comment) : 0
                        });
                }
                entries = entries.OrderBy(x => x.searchOrder).ToList();
                return entries;
            }
            public static SearchEntry CheckField(IDialogContext context, SearchEntry entry)
            {
                entry.searchValid = true;
                if (!entry.searchValue.Contains("✅"))
                    entry.searchValue += "  ✅";

                return entry;
            }
            public static SearchEntry CheckField(IDialogContext context, SearchEntry entry, bool clear)
            {
                if (entry.searchValue.Contains("✅"))
                    entry.searchValue = entry.searchValue.Replace("  ✅", "");
                return entry;
            }
            public static bool CheckForm(IDialogContext context, List<SearchEntry> entries)
            {
                foreach (var item in entries)
                    if (!item.searchValid)
                        return false;
                return true;
            }
            public static bool InsertForm(IDialogContext context, UserProfile userProfile)
            {
                using (AkaratakModel model = new AkaratakModel())
                {
                    UserProfile profile = null;
                    if (userProfile.telegramData.message != null)
                        profile = userProfile;
                    else
                        context.PrivateConversationData.TryGetValue("@userProfile", out profile);


                    var property = GenerateProperty(profile);
                    model.Properties.Add(property);
                    try
                    {
                        model.SaveChanges();
                        userProfile.insertParameters = new InsertParameters();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        API.IOCommon.Logger.Log(ex);
                        return false;
                    }

                }
            }
            private static Property GenerateProperty(UserProfile userProfile)
            {
                var param = userProfile.insertParameters;
                var insertProperty = new Property
                {
                    Property_Size = param.insertSize,
                    Address = param.insertAddress,
                    City_ID = int.Parse(param.insertCity),
                    Country_ID = int.Parse(param.insertCountry),
                    Property_Category_ID = int.Parse(param.insertCategory),
                    Property_Type_ID = int.Parse(param.insertType),
                    Date_Added = DateTime.UtcNow,
                    Expire_Date = DateTime.UtcNow.AddMonths(3).Date,
                    Floor = param.insertFloorLevel,
                    Has_Garage = param.insertHasGarage,
                    Has_Garden = param.insertHasGarden,
                    Location = param.insertLocation,
                    Num_Floors = param.insertFloorCount,
                    Num_Bedrooms = param.insertBedRoomCount,
                    Num_Bathrooms = param.insertBathRoomCount,
                    Other_Details = param.insertOtherDetails,
                    Rent_Price = param.insertRentPrice,
                    Sale_Price = param.insertSalePrice,
                    User_ID = userProfile.telegramData.message != null ?
                    CreateOrGetUserID(userProfile) :
                    WebConfigurationManager.AppSettings["AkaratakBotUserID"],
                    Zip_Code = param.insertZipCode

                };
                API.IOCommon.PhotoManager.UploadPhotoToHost(param.insertPhotoPath, insertProperty);
                return insertProperty;
            }
            private static string CreateOrGetUserID(UserProfile userProfile)
            {
                From from = userProfile.telegramData.message.from;
                using (AkaratakModel model = new AkaratakModel())
                {

                    try
                    {
                        var userID = model.Users.Where(x => x.Telegram_ID == from.id).FirstOrDefault();
                        if (userID != null)
                            return userID.User_ID;
                        else
                        {
                            model.Users.Add(new User
                            {
                                Telegram_ID = from.id,
                                User_ID = Guid.NewGuid().ToString(),
                                First_Name = from.first_name,
                                Last_Name = from.last_name,
                                Phone_Num = userProfile.insertParameters.insertPhoneNumber,
                                Email = from.first_name + "@" + from.last_name
                            });
                            model.SaveChanges();
                            return model.Users.Where(x => x.Telegram_ID == from.id).FirstOrDefault().User_ID;
                        }
                    }
                    catch (Exception ex)
                    {
                        API.IOCommon.Logger.Log(ex);
                        return string.Empty;
                    }
                }
            }
            public static List<SearchEntry> GetCategoryList(IDialogContext context)
            {
                List<SearchEntry> entries = new List<SearchEntry>();
                ResourceManager resourceManager = new ResourceManager(typeof(Resources.Shared.Property_Category));
                ResourceSet resourceSet = resourceManager.GetResourceSet(new CultureInfo(context.PrivateConversationData.GetValueOrDefault<string>("ULTN") != null ? context.PrivateConversationData.GetValueOrDefault<string>("ULTN") : "en-US"), true, true);
                foreach (DictionaryEntry item in resourceSet)
                    entries.Add(new SearchEntry() { searchKey = item.Key.ToString(), searchValue = item.Value.ToString() });
                using (var model = new AkaratakModel())
                {
                    for (int i = 0; i < entries.Count; i++)
                    {
                        string _category = entries[i].searchKey;
                        entries[i].searchKey = model.Property_Category.Where(c => c.Cat_Name == _category).FirstOrDefault().Cat_ID.ToString();
                    }
                }
                return entries;
            }
            public static List<SearchEntry> GetTypeList(IDialogContext context, SearchEntry entry)
            {
                List<SearchEntry> entries = new List<SearchEntry>();
                using (var model = new AkaratakModel())
                {
                    int cat_id = int.Parse(entry.searchKey);
                    var L2EQuery = from item in model.Property_Type
                                   where item.Property_Category.Cat_ID == cat_id
                                   select item;

                    var queryResult = L2EQuery.ToList();
                    CultureInfo culture = new CultureInfo(context.PrivateConversationData.GetValueOrDefault<string>(Dialogs.SettingsDialog.BaseDialog.UserLanguageToken));
                    ResourceManager resourceManager = new ResourceManager(typeof(Resources.Shared.Property_Type));
                    ResourceSet resourceSet = resourceManager.GetResourceSet(culture != null ? culture : new CultureInfo("en-US"), true, true);
                    var result = from first in resourceSet.Cast<DictionaryEntry>()
                                 join second in queryResult
                                 on first.Key.ToString() equals second.Property_Type_Name
                                 into matches
                                 where matches.Any()
                                 select first;
                    foreach (var item in result)
                        entries.Add(new SearchEntry() { searchKey = queryResult.Find(x => x.Property_Type_Name == item.Key.ToString()).Property_Type_ID.ToString(), searchValue = item.Value.ToString() });
                }
                return entries;
            }
            public static List<SearchEntry> GetCountryList(IDialogContext context, RangeEntry range)
            {
                List<SearchEntry> countries = new List<SearchEntry>();
                using (var model = new AkaratakModel())
                {
                    var L2EQuery = from item in model.Countries
                                   select item;

                    var queryResult = L2EQuery.ToList();

                    foreach (var item in queryResult)
                        countries.Add(new SearchEntry()
                        {
                            searchKey = item.Country_ID.ToString(),
                            searchValue = item.Country_Name.ToString() /*+ $" ({item.Country_Native_Name} )"*/
                        });

                }
                if (range.RangeStart + range.RangeCount < countries.Count - 1)
                {
                    countries = countries.GetRange(range.RangeStart, range.RangeCount);
                    countries.Add(new SearchEntry()
                    {
                        searchKey = string.Empty,
                        searchValue = Resources.Insert.InsertDialog.InsertFormCountryNext
                    });
                }
                else
                {
                    countries = countries.GetRange(range.RangeStart, countries.Count - range.RangeStart);
                    countries.Add(new SearchEntry()
                    {
                        searchKey = string.Empty,
                        searchValue = Resources.Insert.InsertDialog.InsertFormCountryReset
                    });
                }
                return countries;
            }
            public static List<SearchEntry> GetCityList(IDialogContext context, SearchEntry country)
            {
                List<SearchEntry> cities = new List<SearchEntry>();
                using (var model = new AkaratakModel())
                {
                    var country_id = int.Parse(country.searchKey);
                    var L2EQuery = from item in model.Cities
                                   where item.Country_ID == country_id
                                   select item;

                    var queryResult = L2EQuery.ToList();

                    foreach (var item in queryResult)
                        cities.Add(new SearchEntry() { searchKey = item.City_ID.ToString(), searchValue = item.City_Name.ToString() + $" ({item.City_Native_Name} )" });

                }
                return cities;
            }
        }
        public class Update
        {
            public static List<SearchEntry> CreateForm(IDialogContext context)
            {
                List<SearchEntry> entries = new List<SearchEntry>();
                ResourceManager resourceManager = new ResourceManager(typeof(Resources.Update.UpdateDialog));
                ResourceSet resourceSet = resourceManager.GetResourceSet(new CultureInfo(context.PrivateConversationData.GetValueOrDefault<string>("ULTN") != null ?
                    context.PrivateConversationData.GetValueOrDefault<string>("ULTN") : "en-US"), true, true);
                foreach (DictionaryEntry item in resourceSet)
                    if (item.Key.ToString().Contains("UpdateField"))
                        entries.Add(new SearchEntry { searchKey = item.Key.ToString(), searchValue = item.Value.ToString() });

                entries.Add(new SearchEntry { searchKey = "UpdateCancel", searchValue = Resources.Update.UpdateDialog.UpdateCancel });

                return entries;
            }
            public static bool CheckForm(IDialogContext context, List<SearchEntry> entries)
            {
                foreach (var item in entries)
                    if (item.searchValue.Contains("✅"))
                        return true;
                return false;
            }
            public static bool CheckUserHasProperty(UserProfile userProfile, bool emulator)
            {
                var id = API.IOCommon.UserManager.GetUserID(userProfile, emulator);
                if (id==null || id==string.Empty)
                    return false;
                using (AkaratakModel model = new AkaratakModel())
                {
                    var properties = model.Properties.Where(x => x.User_ID == id).ToList();
                    return properties.Count > 0;
                }
            }
            public static IList<Attachment> GetPropertyList(string UserID)
            {
                IList<Attachment> attachments = new List<Attachment>();
                using (var context = new AkaratakModel())
                {
                    var L2EQuery = from item in context.Properties
                                   where item.User_ID == UserID
                                   select item;
                    try
                    {
                        var result = L2EQuery.ToList();
                        foreach (var item in result)
                            attachments.Add(Cards.GetHeroCard(
                                 item.Address,
                                 item.Zip_Code,
                                 item.Other_Details,
                                 new CardImage(url: (Search._ConstructPropertyImageUrl(item))),
                                 new CardAction(
                                     type: ActionTypes.ImBack,
                                     title: "Update This",
                                     displayText: "Update this",
                                     value: item.PropertyID.ToString())
                                 ));
                    }
                    catch (Exception ex)
                    {

                        API.IOCommon.Logger.Log(ex);
                    }

                }
                return attachments;
            }
            public static IList<Property> GetPropertyList(UserProfile profile,bool emulator)
            {
                using (var context = new AkaratakModel())
                {
                    var userID = API.IOCommon.UserManager.GetUserID(profile, emulator);
                    var L2EQuery = from item in context.Properties
                                   where item.User_ID == userID
                                   select item;

                    var result = L2EQuery.ToList();
                    return result;
                }
            }
            public static IList<Attachment> GetPhotoList(int propertyID)
            {
                IList<Attachment> attachments = new List<Attachment>();
                using (var context = new AkaratakModel())
                {
                    var L2EQuery = from item in context.Property_Photos
                                   where item.Property_ID == propertyID
                                   select item;
                    try
                    {
                        var result = L2EQuery.ToList();
                        foreach (var item in result)
                            attachments.Add(Cards.GetHeroCard(
                                 string.Empty,
                                 string.Empty,
                                 string.Empty,
                                 new CardImage(url: item.Photo_Url),
                                 new CardAction(
                                     type: ActionTypes.ImBack,
                                     title: "Update This",
                                     displayText: "Update this",
                                     value: item.Public_Id)
                                 ));
                    }
                    catch (Exception ex)
                    {

                        API.IOCommon.Logger.Log(ex);
                    }

                }
                return attachments;
            }

            public static bool UpdateForm(UserProfile userProfile, List<SearchEntry> options)
            {
                using (AkaratakModel model = new AkaratakModel())
                {
                    Property propertyToUpdate = model.Properties.Where(x => x.PropertyID == userProfile.updateParameters.updatePropertyID).FirstOrDefault();

                    var updateParameters = userProfile.updateParameters;

                    foreach (var item in options)
                    {
                        switch (item.searchKey)
                        {
                            case "UpdateFieldBathroomCount":
                                propertyToUpdate.Num_Bathrooms = updateParameters.updateBathRoomCount;
                                break;
                            case "UpdateFieldBedroomCount":
                                propertyToUpdate.Num_Bedrooms = updateParameters.updateBedRoomCount;
                                break;
                            case "UpdateFieldCategoryType":
                                propertyToUpdate.Property_Category_ID = int.Parse(updateParameters.updateCategory);
                                propertyToUpdate.Property_Type_ID = int.Parse(updateParameters.updateType);
                                break;
                            case "UpdateFieldCountryCity":
                                propertyToUpdate.Country_ID = int.Parse(updateParameters.updateCountry);
                                propertyToUpdate.City_ID = int.Parse(updateParameters.updateCity);
                                break;
                            case "UpdateFieldFloorCount":
                                propertyToUpdate.Num_Floors = updateParameters.updateFloorCount;
                                break;
                            case "UpdateFieldFloorLevelCount":
                                propertyToUpdate.Floor = updateParameters.updateFloorLevel;
                                break;
                            case "UpdateFieldGardenGarageChoice":
                                propertyToUpdate.Has_Garage = updateParameters.updateHasGarage;
                                propertyToUpdate.Has_Garden = updateParameters.updateHasGarden;

                                break;
                            case "UpdateFieldLocationLatLng":
                                propertyToUpdate.Location = updateParameters.updateLocation;
                                break;
                            case "UpdateFieldOtherDetailsText":
                                propertyToUpdate.Other_Details = updateParameters.updateOtherDetails;
                                break;
                            case "UpdateFieldPropertySize":
                                propertyToUpdate.Property_Size = updateParameters.updateSize;
                                break;
                            case "UpdateFieldSaleRentPriceCount":
                                propertyToUpdate.Sale_Price = updateParameters.updateSalePrice;
                                propertyToUpdate.Rent_Price = updateParameters.updateRentPrice;
                                break;
                            case "UpdateFieldZipCodeText":
                                propertyToUpdate.Zip_Code = updateParameters.updateZipCode;
                                break;
                            case "UpdateFieldAddressText":
                                propertyToUpdate.Address = updateParameters.updateAddress;
                                break;
                            case "UpdateFieldPhotoSelection":
                                API.IOCommon.PhotoManager.UploadPhotoToHost(userProfile.updateParameters.PhotoParameters, propertyToUpdate);
                                break;
                        }
                    }
                    try
                    {
                        model.SaveChanges();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        API.IOCommon.Logger.Log(ex);
                        return false;
                    }

                }

            }
        }
    }
}