using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DR.Common.RESTClient;

namespace HelloBot
{
    public static class Jokes
    {
        private static readonly IJsonClient JsonClient = new JsonClient(true) { BaseURL = "http://api.icndb.com/jokes/random" };

        private class Joke
        {
            public JokeValue Value { get; set; }

            public class JokeValue
            {
                public string Joke { get; set; }
            }
        }
        public static string Random()
        {
            return HttpUtility.HtmlDecode(JsonClient.Get<Joke>("").Value.Joke);
        }
        
    }
}