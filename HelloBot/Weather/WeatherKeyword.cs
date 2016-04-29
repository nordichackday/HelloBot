using System.Collections.Generic;
using System.Linq;

namespace HelloBot.Weather
{
    public class Keyword
    {
        public string FindOrDefault(string message)
        {
            var danishWeatherKeywords = new[] { "vejret", "vejret i", "hvordan er vejret", "hvordan er vejret i" };
            var norwegianWeatheKeywords = new[] { "vær", "vær i", "hvordan er været", "hvordan er været i" };
            var swedishWeatheKeywords = new[] { "väder", "väder i", "hur är vädret", "hur är vädret i" };
            var finishWeatherKeywords = new[] { "miten sää", "sää" };
            var weatherKeywords = new List<string[]>() { danishWeatherKeywords, norwegianWeatheKeywords, swedishWeatheKeywords, finishWeatherKeywords };
            foreach (var weatherKeywordCollection in weatherKeywords)
            {
                foreach (var weatherKeyword in weatherKeywordCollection.OrderByDescending(x => x.Length))
                {
                    if (message.ToLower().Contains(weatherKeyword + " "))
                    {
                        return weatherKeyword;
                    }
                }
            }

            return null;
        }

        public string FindPrognoseOrDefault(string message)
        {
            var danishWeatherKeywords = new[] { "prognose for", "prognose" };
            var weatherKeywords = new List<string[]>() { danishWeatherKeywords };
            foreach (var weatherKeywordCollection in weatherKeywords)
            {
                foreach (var weatherKeyword in weatherKeywordCollection.OrderByDescending(x => x.Length))
                {
                    if (message.ToLower().Contains(weatherKeyword + " "))
                    {
                        return weatherKeyword;
                    }
                }
            }

            return null;
        }
    }
}