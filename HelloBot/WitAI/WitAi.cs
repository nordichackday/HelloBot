using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace HelloBot.WitAI
{
    public class WitAi
    {
        private static readonly string baseurl = "https://api.wit.ai/converse?v=20160330&session_id=";
        public static RootObject Init(string msg, string username)
        {
            using (var httpClient = new HttpClient())
            {
                var uri = baseurl + username + "&q=" + msg;
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "TGTM7ZQAJRZLVNGS5WU7MERPJUSRDZET");
                var result = JsonConvert.DeserializeObject<RootObject>(httpClient.PostAsync(new Uri(uri), null).Result.Content.ReadAsStringAsync().Result);
                return result;
            }
        }

        public static RootObject DoType(string username, string type)
        {
            using (var httpClient = new HttpClient())
            {
                var uri = baseurl + username + "&type=" + type;
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "TGTM7ZQAJRZLVNGS5WU7MERPJUSRDZET");
                var result = httpClient.PostAsync(new Uri(uri), null).Result.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<RootObject>(result);
            }

        }
        public static RootObject DoAction(string username, string action)
        {
            using (var httpClient = new HttpClient())
            {
                var uri = baseurl + username + "&type=action&action=" + action;
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "TGTM7ZQAJRZLVNGS5WU7MERPJUSRDZET");
                var result = httpClient.PostAsync(new Uri(uri), null).Result.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<RootObject>(result);
            }

        }
    }
    public class Value
    {
        public string type { get; set; }
        public string value { get; set; }
        public bool suggested { get; set; }
    }

    public class Location
    {
        public string body { get; set; }
        public Value value { get; set; }
        public int start { get; set; }
        public int end { get; set; }
        public string entity { get; set; }
    }

    public class Entities
    {
        public List<Location> location { get; set; }
    }

    public class RootObject
    {
        public string type { get; set; }
        public Entities entities { get; set; }
        public double confidence { get; set; }
        public string action { get; set; }
        public string msg { get; set; }
    }

}