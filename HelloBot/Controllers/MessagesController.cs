﻿using System;
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
                if (message.Text == "what do we want?") return message.CreateReplyMessage("CAAAAANDYYYYYYY!!!");
                if (message.Text != null && message.Text.Contains("wit"))
                {
                    var response = WitAi.DoStuff(message.Text.Split(new[] {"wit"}, StringSplitOptions.None)[1], message.From.Name);
                }

                if (message.Text != null && message.Text.ToLower().Contains("pepe"))
                {
                    var m = message.CreateReplyMessage("As requested:");
                    m.Attachments = new List<Attachment> {new Attachment {ContentUrl = "http://i.imgur.com/HWqeKN1.png" , ContentType = "image/png", FallbackText = "Pepe the frog"}};
                    return m;
                }
                return message.CreateReplyMessage($"Hi {message.From.Name}! If you grow tired of talking with me just say goodbye!"); 
               
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