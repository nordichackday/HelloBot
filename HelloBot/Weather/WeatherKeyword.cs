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
            var danishWeatherKeywords = new[] { "prognose", "prognose for", "prognosen for" };
            var swedishWeatheKeywords = new[] { "prognos", "prognos för", "prognosen för" };
            var finishWeatherKeywords = new[] { "ennuste", "miten on ennuste" };
            var weatherKeywords = new List<string[]>() { danishWeatherKeywords, swedishWeatheKeywords, finishWeatherKeywords };
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