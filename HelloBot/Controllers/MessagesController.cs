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
                           var init = WitAi.Init(message.Text.Split(new[] { "wit" }, StringSplitOptions.None)[1], message.From.Name);

                           while (init.type.ToLower() != "stop")
                           {
                               if (init.type.ToLower() == "merge")
                               {
                                   init = WitAi.DoAction(message.From.Name, "merge");
                               }
                               else if (init.type.ToLower() == "action")
                               {
                                   init = WitAi.DoAction(message.From.Name, init.action);
                               }
                               else if (init.type.ToLower() == "msg")
                               {
                                   init = WitAi.DoType(message.From.Name, init.type);
                               }
                               if (!string.IsNullOrEmpty(init.msg)) msg += init.msg + "\n";
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

                  var keywordr = new Weather.Keyword();

                  var triggerWeatherKeyword = keywordr.FindOrDefault(message.Text);
                  if (triggerWeatherKeyword != null)
                  {
                      var weatherClient = new Weather.RequestClient();
                      var i = message.Text.IndexOf(triggerWeatherKeyword, StringComparison.InvariantCultureIgnoreCase) + triggerWeatherKeyword.Length;
                      var suggestion = message.Text.Substring(i).TrimStart().Replace("?", string.Empty).TrimEnd();
                      var cities = weatherClient.GetCity(suggestion);
                      if (cities.Any())
                      {
                          var tuple = weatherClient.GetCurrentWeather(cities);
                          var city = tuple.Item1;
                          var nowNext = tuple.Item2;
                          var now = nowNext.First();
                          var next = nowNext.Skip(1).First();
                          var nextNext = nowNext.Skip(2).First();
                          return Chain.Return($"> I {city.Name} er det lige nu {now.T}°. {now.Prosa}. \n \n > I {next.Dtt.ToLower()} forventer vi {next.T}°. {next.Prosa}. \n \n > Og i {nextNext.Dtt.ToLower()} {nextNext.T}°. {nextNext.Prosa}.");

                      }

                      else
                      {
                          return
                              Chain.Return(
                                  $"Jeg kender ingen byer der hedder {suggestion}, beklager. :pepe:");
                      }
                  }

                  var prognoseWeatherKeyword = keywordr.FindPrognoseOrDefault(message.Text);
                  if (prognoseWeatherKeyword != null)
                  {
                      var weatherClient = new Weather.RequestClient();
                      var i = message.Text.IndexOf(prognoseWeatherKeyword, StringComparison.InvariantCultureIgnoreCase) + prognoseWeatherKeyword.Length;
                      var suggestion = message.Text.Substring(i).TrimStart().Replace("?", string.Empty).TrimEnd();
                      var cities = weatherClient.GetCity(suggestion);
                      if (cities.Any())
                      {
                          var tuple = weatherClient.GetPrognoseWeather(cities);
                          var city = tuple.Item1;
                          var nowNext = tuple.Item2;
                          var s = $"> Prognose for {city.Name}. \n \n";
                          s = nowNext.Skip(1).Select(x => $"> {x.Dtt}: {x.T}°. {x.Prosa}.\n \n").Aggregate(s, (current, t) => current + t);
                          return Chain.Return(s);

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