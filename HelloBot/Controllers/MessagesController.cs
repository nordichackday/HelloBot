using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Utilities;
using Newtonsoft.Json;

namespace HelloBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<Message> Post([FromBody]Message message)
        {
            if (message.Type == "Message")
            {
                if (message.Text != null && message.Text.Contains("wit"))
                {
                    var response = WitAi.Init(message.Text.Split(new[] { "wit" }, StringSplitOptions.None)[1], message.From.Name);
                }
                if (message.Text == null)
                    return message.CreateReplyMessage("Why do you null me so?");

                if (message.Text == "what do we want?") return message.CreateReplyMessage("CAAAAANDYYYYYYY!!!");
                if (message.Text.ToLower().Contains("mr.roboto"))
                    return
                        message.CreateReplyMessage(
                            "Hello @mr.roboto , goddag og tak for tippet. Hvordan er vejret og hvad er nyheder?");
                if (message.Text.ToLower().Contains("pepe"))
                {
                    var m = message.CreateReplyMessage("As requested:");
                    m.Attachments = new List<Attachment> {new Attachment {ContentUrl = "http://i.imgur.com/HWqeKN1.png" , ContentType = "image/png", FallbackText = "Pepe the frog"}};
                    return m;
                }
                var channelResponse = "";
                if (message.Text.ToLower().Contains("nrk"))
                    channelResponse+="Kanskje du finner svaret her http://nrk.no eller \n";
                if (message.Text.ToLower().Contains("yle"))
                    channelResponse += "Kanske du hittar svaret här / ehkä löydät vastauksen täältä http://yle.fi tai \n";
                if (message.Text.ToLower().Contains("svt"))
                    channelResponse += "Kanske du hittar svaret här http://svt.se eller \n";
                if (message.Text.ToLowerInvariant().Contains("ruv") )
                    channelResponse += "Kannski þú munt finna svarið hér http://ruv.is eða \n";

                if (!string.IsNullOrEmpty(channelResponse))
                    return message.CreateReplyMessage(channelResponse + "here http://google.com");

                if ( message.Text.ToLower().Contains("tv"))
                {

                    var response = "Der vises følgende på DRs tv kanaler netop nu:\n";
                    var tzInfo = TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time");
                    
                    foreach (var nowNext in MUClient.GetNowNextForAllActiveDRChannels().Where(nn=>nn.Now != null))
                    {
                        
                        response +=
                            $"\nPå {MUClient.GetChannel(nowNext.ChannelSlug).Title} vises der {nowNext.Now.Title}, som startede {TimeZoneInfo.ConvertTimeFromUtc(nowNext.Now.StartTime,tzInfo):HH:mm}\n\n" +
                            $"Se live her på https://www.dr.dk/tv/live/{nowNext.ChannelSlug}/ \n";

                        if (nowNext.Now.ProgramCardHasPrimaryAsset)
                            response +=
                                $"Det kan ses ondemand allerede nu her på https://www.dr.dk/tv/se/{nowNext.Now.ProgramCard.SeriesSlug}/{nowNext.Now.ProgramCard.Slug} \n";
                        else if (nowNext.Now.SeriesHasProgramCardWithPrimaryAsset)
                            response +=
                               $"Det seneste afsnit kan ses på https://www.dr.dk/tv/se/{nowNext.Now.ProgramCard.SeriesSlug}/ \n ";
                        
                        
                    }
                    return message.CreateReplyMessage(response);
                }
                var danishWeatherKeywords = new[] {"vejret", "vejret i", "hvordan er vejret", "hvordan er vejret i"};
                var norwegianWeatheKeywords = new[] { "vær", "vær i", "hvordan er været", "hvordan er været i"};
                var swedishWeatheKeywords = new[] { "väder", "väder i", "hur är vädret", "hur är vädret i" };
                var finishWeatherKeywords = new[] {"miten sää", "sää"};
                var weatherKeywords = new List<string[]>() {danishWeatherKeywords, norwegianWeatheKeywords, swedishWeatheKeywords, finishWeatherKeywords };
                string triggerWeatherKeyword = null;
                foreach (var weatherKeywordCollection in weatherKeywords)
                {
                    foreach (var weatherKeyword in weatherKeywordCollection.OrderByDescending(x => x.Length))
                    {
                        if (message.Text.ToLower().Contains(weatherKeyword + " "))
                        {
                            triggerWeatherKeyword = weatherKeyword;
                            break;
                        }
                    }
                    
                }
                if (triggerWeatherKeyword != null)
                {
                    var suggestion = message.Text.Substring(triggerWeatherKeyword.Length).TrimStart().Replace("?", string.Empty).TrimEnd();
                    var cities = Weather.WeatherClient.GetCity(suggestion);
                    if (cities.Any())
                    {
                        var tuple = Weather.WeatherClient.GetCurrentWeather(cities);
                        var city = tuple.Item1;
                        var nowNext = tuple.Item2;
                        return message.CreateReplyMessage($"I {city.Name} er det: {nowNext.T} grader. {nowNext.Prosa}.");

                    }
                    else
                    {
                        return
                            message.CreateReplyMessage(
                                $"Jeg kender ingen byer der hedder {suggestion}, beklager. :pepe:");
                    }
                }
                return message.CreateReplyMessage($"Hi {message.From.Name}! Here is a Chuck Noris fact: \n {Jokes.Random()}. \n \n If you grow tired of talking with me just say goodbye!"); 

               
            }
            else
            {
                return HandleSystemMessage(message);
            }
        }

        private Message HandleSystemMessage(Message message)
        {
            if (message.Type == "Ping")
            {
                Message reply = message.CreateReplyMessage();
                reply.Type = "Ping";
                return reply;
            }
            else if (message.Type == "DeleteUserData")
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == "BotAddedToConversation")
            {
            }
            else if (message.Type == "BotRemovedFromConversation")
            {
            }
            else if (message.Type == "UserAddedToConversation")
            {
            }
            else if (message.Type == "UserRemovedFromConversation")
            {
            }
            else if (message.Type == "EndOfConversation")
            {
            }

            return null;
        }
    }
}