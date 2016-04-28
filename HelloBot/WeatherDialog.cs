using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;

namespace HelloBot
{
    [Serializable]
    public class WeatherDialog : IDialog<object>
    {
        private City _city;
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }
        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<Message> argument)
        {
            var message = await argument;

            if (message.Text.ToLower().StartsWith("hvordan er vejret i"))
            {
                var suggestion = message.Text.Substring("hvordan er vejret i ".Length);
                var cities = await Request<City[]>($"http://www.dr.dk/tjenester/drvejret/Suggestions?query={suggestion}&maxChoices=50");
                if (cities.Any())
                {
                    if (cities.Count() > 1)
                    {
                        _city = cities.First();
                        //PromptDialog.Choice(context, (dialogContext, result) =>);
                    }
                    else
                    {
                        _city = cities.First();
                        var url = $"http://www.dr.dk/tjenester/drvejret/BoxedFromCenter/0/0/6/{_city.Name}/{_city.Id}";
                        var forecasts = await Request<RootObject[]>(url);
                        var nowNext = forecasts.First().Detailed.First().WeatherNowNextNext.First();
                        await context.PostAsync($"I {_city.Name} er det {nowNext.Prosa}");
                    }

                }
            }
            else
            {
                await context.PostAsync("Jeg forstår ikke hvad du skriver");
                context.Wait(MessageReceivedAsync);
            }
        }

        public async Task CitySuggestion(IDialogContext context, ResumeAfter<char> argument)
        {

        }

        private async Task<T> Request<T>(string url)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var responseAsString = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<T>(responseAsString);

                    return result;
                }
            }

            throw new Exception("something went wrong");
        }
    }
}