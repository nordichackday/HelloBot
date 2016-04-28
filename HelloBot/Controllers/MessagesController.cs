using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Configuration;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using HelloBot.WitAI;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Utilities;
using Newtonsoft.Json;

namespace HelloBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {

        public static readonly IDialog<string> dialog = Chain.PostToChain()

            .Switch(
                new Case<Message, IDialog<string>>(message =>
                {
                    var regex = new Regex("pepe", RegexOptions.IgnoreCase);
                    return regex.Match(message.Text).Success;
                }, (context, txt) =>
                {
                    return Chain.From(() => new PromptDialog.PromptConfirm("Are you sure you want to Pepe?",
                        "Didn't get that!", 3)).ContinueWith<bool, string>(async (ctx, res) =>
                        {
                            string reply;
                            if (await res)
                            {
                                reply = "Pepe time :pepe:";
                            }
                            else
                            {
                                reply = "No pepe...";
                            }
                            return Chain.Return(reply);
                        });
                }),
                  new Case<Message, IDialog<string>>(message =>
                  {
                      var regex = new Regex("wit", RegexOptions.IgnoreCase);
                      return regex.Match(message.Text).Success;
                  }, (context, message) =>
                  {
                      var msg = "";
                      var name = message.From.Name;
                      var init = WitAi.Init(message.Text.Split(new[] { "wit" }, StringSplitOptions.None)[1], name);

                      while (init.type.ToLower() != "stop")
                      {
                          if (init.type.ToLower() == "merge")
                          {
                              init = WitAi.DoAction(name, init.action);
                              msg += init.msg;

                          }
                          else if (init.type.ToLower() == "action")
                          {
                              init = WitAi.DoAction(name, init.action);


                          }
                          else if (init.type.ToLower() == "msg")
                          {

                              init = WitAi.DoType(name, init.type);
                              msg += init.msg;

                          }
                      }
                      return Chain.Return(msg);
                  })
            ,
                 /*  new Case<Message, IDialog<string>>(message =>
                   {
                       var regex = new Regex("what do we want", RegexOptions.IgnoreCase);
                       return regex.Match(message.Text).Success;
                   }, (context, message) =>
                   {
                       var msg = "*";
                       return Chain.Return(msg);
                   }),*/
              new DefaultCase<Message, IDialog<string>>((context, message) =>
              {


                  if (message.Text == "what do we want?") return Chain.Return("CAAAAANDYYYYYYY!!!");
                  if (message.Text.ToLower().Contains("mr.roboto"))
                      return
                          Chain.Return(
                              "Hello @mr.roboto , goddag og tak for tippet. Hvordan er vejret og hvad er nyheder?");
            
                  var channelResponse = "";
                  if (message.Text.ToLower().Contains("nrk"))
                      channelResponse += "Kanskje du finner svaret her http://nrk.no eller \n";
                  if (message.Text.ToLower().Contains("yle"))
                      channelResponse += "Kanske du hittar svaret här / ehkä löydät vastauksen täältä http://yle.fi tai \n";
                  if (message.Text.ToLower().Contains("svt"))
                      channelResponse += "Kanske du hittar svaret här http://svt.se eller \n";
                  if (message.Text.ToLowerInvariant().Contains("ruv"))
                      channelResponse += "Kannski þú munt finna svarið hér http://ruv.is eða \n";

                  if (!string.IsNullOrEmpty(channelResponse))
                      return Chain.Return(channelResponse + "here http://google.com");

                  if (message.Text.ToLower().Contains("tv"))
                  {

                      var response = "Der vises følgende på DRs tv kanaler netop nu:\n";
                      var tzInfo = TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time");

                      foreach (var nowNext in MUClient.GetNowNextForAllActiveDRChannels().Where(nn => nn.Now != null))
                      {

                          response +=
                              $"\nPå {MUClient.GetChannel(nowNext.ChannelSlug).Title} vises der {nowNext.Now.Title}, som startede {TimeZoneInfo.ConvertTimeFromUtc(nowNext.Now.StartTime, tzInfo):HH:mm}\n\n" +
                              $"Se live her på https://www.dr.dk/tv/live/{nowNext.ChannelSlug}/ \n";

                          if (nowNext.Now.ProgramCardHasPrimaryAsset)
                              response +=
                                  $"Det kan ses ondemand allerede nu her på https://www.dr.dk/tv/se/{nowNext.Now.ProgramCard.SeriesSlug}/{nowNext.Now.ProgramCard.Slug} \n";
                          else if (nowNext.Now.SeriesHasProgramCardWithPrimaryAsset)
                              response +=
                                 $"Det seneste afsnit kan ses på https://www.dr.dk/tv/se/{nowNext.Now.ProgramCard.SeriesSlug}/ \n ";


                      }
                      return Chain.Return(response);
                  }
                  var danishWeatherKeywords = new[] { "vejret", "vejret i", "hvordan er vejret", "hvordan er vejret i" };
                  var norwegianWeatheKeywords = new[] { "vær", "vær i", "hvordan er været", "hvordan er været i" };
                  var swedishWeatheKeywords = new[] { "väder", "väder i", "hur är vädret", "hur är vädret i" };
                  var finishWeatherKeywords = new[] { "miten sää", "sää" };
                  var weatherKeywords = new List<string[]>() { danishWeatherKeywords, norwegianWeatheKeywords, swedishWeatheKeywords, finishWeatherKeywords };
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
                          return Chain.Return($"I {city.Name} er det: {nowNext.T} grader. {nowNext.Prosa}.");

                      }
                      else
                      {
                          return
                              Chain.Return(
                                  $"Jeg kender ingen byer der hedder {suggestion}, beklager. :pepe:");
                      }
                  }
                  var name = message.From.Name;
                  string reply = $"Hi {name}! Here is a Chuck Noris fact: \n {Jokes.Random()}. \n \n If you grow tired of talking with me just say goodbye!";
                  return Chain.Return(reply);
              }))
          .Unwrap()
          .PostToUser();

        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<Message> Post([FromBody]Message message)
        {
            if (message.Type == "Message")
                return await Conversation.SendAsync(message, () => dialog);
            else
                return HandleSystemMessage(message);
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