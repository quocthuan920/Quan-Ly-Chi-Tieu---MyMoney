﻿using MyMoney.Application.Common.CurrencyConversion.Models;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace MyMoney.Application.Common.CurrencyConversion
{
    public static class RequestHelper
    {
        public const string BASE_URL = "https://free.currconv.com/api/v7/";

        public static List<Currency> GetAllCurrencies(string apiKey)
        {
            string url = $"{BASE_URL}currencies?apiKey={apiKey}";

            string jsonString = GetResponse(url);

            JToken[] data = JObject.Parse(jsonString)["results"].ToArray();
            return data.Select(item => item.First.ToObject<Currency>()).ToList();
        }

        public static double ExchangeRate(string from, string to, string apiKey)
        {
            string url = $"{BASE_URL}convert?q={from}_{to}&compact=ultra&apiKey={apiKey}";

            string jsonString = GetResponse(url);
            return JObject.Parse(jsonString).First.First["val"].ToObject<double>();
        }

        private static string GetResponse(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip;

            using var response = (HttpWebResponse)request.GetResponse();
            Stream stream = response.GetResponseStream();
            using var reader = new StreamReader(stream);

            string jsonString = reader.ReadToEnd();


            return jsonString;
        }
    }
}
