using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AkaratakBot
{
    [Serializable]
    public class SettingsParameters
    {
        public System.Globalization.CultureInfo userLanguage { set; get; }

    }
    [Serializable]
    public class LanguageOption
    {
        public string Locale { get; set; }
        public string Text { get; set; }
        public LanguageOption()
        {
            Locale = "";
            Text = "";
        }

        public override string ToString()
        {
            return this.Text;
        }
        public static bool Contains(LanguageOption languageOption)
        {
            foreach (var item in CreateListOption())
                if (languageOption.Text == item.Text)
                    return true;
            return false;
        }
        public static List<LanguageOption> CreateListOption()
        {
            return new List<LanguageOption>() {
                new LanguageOption(){Text="English",Locale="en-US" },
                new LanguageOption(){Text="العربية",Locale="ar-SA" },
                new LanguageOption(){Text="Türk",Locale="tr-TR" },
            };
        }
    }
}