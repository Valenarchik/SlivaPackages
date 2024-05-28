using System;
using System.Net;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace Localization
{
    public static class GoogleTranslateUtility
    {
        public static string Translate(LocalizationSettings settings, string text, string translationTo = "en")
        {
            if (string.IsNullOrEmpty(text))
            {
                Debug.LogError("The text for translation was not found!");
                return null;
            }

            var url = string.Format("https://translate.google." + settings.domainAutoLocalization + "/translate_a/single?client=gtx&dt=t&sl={0}&tl={1}&q={2}",
                "auto", translationTo, WebUtility.UrlEncode(text));
            var webRequest = UnityWebRequest.Get(url);
            webRequest.SendWebRequest();
            while (!webRequest.isDone)
            {

            }
            var response = webRequest.downloadHandler.text;

            try
            {
                var jsonArray = JArray.Parse(response);
                response = jsonArray[0][0][0].ToString();
            }
            catch
            {
                response = "process error";
                Debug.LogError("The process is not completed! Most likely, you made too many requests. In this case, the Google Translate API blocks access to the translation for a while.  Please try again later. Do not translate the text too often, so that Google does not consider your actions as spam");
            }

            return response;
        }
    }
}