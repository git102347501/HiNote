using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System.UserProfile;

namespace HiNote.Common
{
    public class LanguageHelper
    {
        public static string GetCurLanguage()
        {
            var languages = GlobalizationPreferences.Languages;
            if (languages.Count > 0)
            {
                List<string> lLang = new List<string>();
                lLang.Add("zh-cn、zh、zh-Hans、zh-hans-cn、zh-sg、zh-hans-sg");
                lLang.Add("zh-hk、zh-Hant、zh-mo、zh-tw、zh-hant-hk、zh-hant-mo、zh-hant-tw");
                lLang.Add("de、de-at、de-ch、de-de、de-lu、de-li");
                lLang.Add("en-us、en、en-au、en-ca、en-gb、en-ie、en-in、en-nz、en-sg、en-za、en-bz、en-hk、en-id、en-jm、en-kz、en-mt、en-my、en-ph、en-pk、en-tt、en-vn、en-zw、en-053、en-021、en-029、en-011、en-018、en-014");
                lLang.Add("es、es-cl、es-co、es-es、es-mx、es-ar、es-bo、es-cr、es-do、es-ec、es-gt、es-hn、es-ni、es-pa、es-pe、es-pr、es-py、es-sv、es-us、es-uy、es-ve、es-019、es-419");
                lLang.Add("fr、fr-be、fr-ca、fr-ch、fr-fr、fr-lu、fr-015、fr-cd、fr-ci、fr-cm、fr-ht、fr-ma、fr-mc、fr-ml、fr-re、frc-latn、frp-latn、fr-155、fr-029、fr-021、fr-011");
                lLang.Add("hi、hi-in");
                lLang.Add("it、it-it、it-ch");
                lLang.Add("ja、ja-jp");
                lLang.Add("pt、pt-pt、pt-br");
                lLang.Add("ru、ru-ru");
                for (int i = 0; i < lLang.Count; i++)
                {
                    if (lLang[i].ToLower().Contains(languages[0].ToLower()))
                    {
                        string temp = lLang[i].ToLower();
                        string[] tempArr = temp.Split('、');

                        return tempArr[0];
                    }
                    else
                        return "zh-Hans-CN";
                }
            }
            return "zh-Hans-CN";
        }
    }
}
