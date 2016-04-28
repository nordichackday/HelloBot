using System;
using System.Collections.Generic;
using System.Linq;
using DR.Common.RESTClient;

namespace HelloBot.Weather
{
    public class RequestClient
    {
        private static readonly IJsonClient JsonClient = new JsonClient(true) { BaseURL = "http://www.dr.dk/tjenester/drvejret/" };

        public IEnumerable<City> GetCity(string suggestion)
        {
            return JsonClient.Get<City[]>($"/Suggestions?query={suggestion}&maxChoices=50");
        }

        public Tuple<City, IEnumerable<WeatherNowNextNext>> GetCurrentWeather(IEnumerable<City> cities)
        {
            var city = cities.First();
            var forecasts = JsonClient.Get<WeatherObject[]>($"/BoxedFromCenter/0/0/6/{city.Name}/{city.Id}");
            if (forecasts != null && forecasts.Any())
            {
                var detailed = forecasts.First().Detailed;
                if (detailed != null && detailed.Any())
                {
                    return new Tuple<City, IEnumerable<WeatherNowNextNext>>(city, detailed.First().WeatherNowNextNext);
                }
            }

            return null;
        }
    }
}