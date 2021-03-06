﻿using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using ChatMicroservice.SignalRServices.Interfaces;

using Ping.Commons.Settings;
using Ping.Commons.SignalR.Base;
using Ping.Commons.Dtos.Models.Auth;
using Ping.Commons.Dtos.Models.Wrappers.Response;
using ChatMicroservice.Data.Services.Interfaces;
using ChatMicroservice.RabbitMQ.Publishers.Interfaces;
using Newtonsoft.Json;
using Ping.Commons.Dtos.Models.Chat;
using Ping.Commons.Dtos.Models.Emojis;
using Ping.Commons.Dtos.Models.Various;

namespace ChatMicroservice.SignalRServices
{
    public class ChatHubClientService : BaseHubClientService, IChatHubClientService
    {
        private static readonly string HUB_ENDPOINT = "chathub";

        private readonly ILogger logger;
        private readonly IContactService contactService;
        private readonly IMessagingService messagingService;
        private readonly IEmojiService emojiService;
        private readonly IContactMQPublisher contactMQPublisher;

        public ChatHubClientService(IOptions<GatewayBaseSettings> gatewayBaseOptions,
            IOptions<SecuritySettings> securityOptions,
            IContactService contactService,
            IMessagingService messagingService,
            IEmojiService emojiService,
            IContactMQPublisher contactMQPublisher,
            ILogger<ChatHubClientService> logger)
            : base(gatewayBaseOptions, securityOptions, HUB_ENDPOINT)
        {
            this.logger = logger;

            this.contactMQPublisher = contactMQPublisher;

            this.emojiService = emojiService;
            this.contactService = contactService;
            this.messagingService = messagingService;
        }

        public async void Connect()
        {
            await hubConnection.StartAsync().ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    logger.LogInformation("-- Couln't connect to SignalR ChatHub (OnStarted)");
                    return;
                }
                logger.LogInformation("ChatMicroservice connected to ChatHub successfully (OnStarted)");
                RegisterHandlers();
            });
        }

        public void RegisterHandlers()
        {
            hubConnection.On<string>("GetEmojis", async (phoneNumber) => 
            {
                logger.LogInformation($"[{phoneNumber}] - requesting Emojis.");

                //List<EmojiCategoryDto> emojis = new List<EmojiCategoryDto> { 
                //    new EmojiCategoryDto
                //    {
                //        Name = "Category1",
                //        Emojis = new List<EmojiDto>
                //        {
                //            new EmojiDto
                //            {
                //                Category = "Category1",
                //                Decimal = "15",
                //                Hex = "0x1F600",
                //                Name = "smiley",
                //                Unicode = "15921"
                //            },
                //            new EmojiDto
                //            {
                //                Category = "Category1",
                //                Decimal = "16",
                //                Hex = "0x1F604",
                //                Name = "smiley2",
                //                Unicode = "2159195"
                //            }
                //        }
                //    }
                //};
                List<EmojiCategoryDto> emojis = await emojiService.GetEmojis();

                await hubConnection.SendAsync("EmojisResponse", phoneNumber, emojis);
            });

            hubConnection.On<string, ContactDto>("AddContact", async (phoneNumber, newContactDto) =>
            {
                logger.LogInformation($"[{phoneNumber}] - requesting to add new contact {newContactDto.ContactPhoneNumber}.");

                ResponseDto<ContactDto> response = await contactService.AddContact(phoneNumber, newContactDto);
                if (response != null && response.Success)
                {
                    logger.LogInformation($"[{phoneNumber}] - added new contact (Success) {newContactDto.ContactPhoneNumber}.");
                    contactMQPublisher.SendContact(response.Dto);
                }
                else
                {
                    logger.LogError($"[{phoneNumber}] - couldn't add new contact (Fail) {newContactDto.ContactPhoneNumber} within account [{phoneNumber}]. " +
                        $"Returning error message");
                }

                await hubConnection.SendAsync("AddContactResponse", phoneNumber, response);
            });

            hubConnection.On<string, ContactDto>("UpdateContact", async (phoneNumber, updateContactDto) =>
            {
                logger.LogError($"[{phoneNumber}] - requesting contact update {updateContactDto.ContactPhoneNumber} within account [{phoneNumber}].");

                ResponseDto<ContactDto> response = await contactService.UpdateContact(phoneNumber, updateContactDto);
                if (response != null && response.Success)
                {
                    logger.LogError($"[{phoneNumber}] - updated contact (Success) {updateContactDto.ContactPhoneNumber} within account [{phoneNumber}].");
                    contactMQPublisher.SendContact(response.Dto);
                }
                else
                {
                    logger.LogError($"[{phoneNumber}] - couldn't update contact (Fail) {updateContactDto.ContactPhoneNumber} within account [{phoneNumber}].");
                }

                await hubConnection.SendAsync("UpdateContactResponse", phoneNumber, response);
            });

            hubConnection.On<string>("RequestContacts", async (phoneNumber) =>
            {
                logger.LogInformation($"[{phoneNumber}] requesting Contacts for account: {phoneNumber}.");

                List<ContactDto> contacts = await contactService.GetAllByUser(phoneNumber);
                if (contacts != null)
                {
                    logger.LogInformation($"[{phoneNumber}] - returning list of contacts (Success): {JsonConvert.SerializeObject(contacts)}");
                    await hubConnection.SendAsync("RequestContactsSuccess", phoneNumber, contacts);
                    return;
                }

                logger.LogError($"[{phoneNumber}] - contacts couldn't be retrieved (Fail). Returning error message");
                await hubConnection.SendAsync("RequestContactsFail", phoneNumber,
                    $"Couldn't load contacts, for account: {phoneNumber}, requested by: {phoneNumber}");
            });

            hubConnection.On<string, int>("LoadMessages", async (phoneNumber, pageNumber) =>
            {
                logger.LogInformation($"[{phoneNumber}] requesting more messages, page: {pageNumber}.");

                //PagedList<MessageDto> pagedMessages = await contactService.GetMessages(pageNumber);
                //if (pagedMessages != null)
                //{
                //    logger.LogInformation($"[{phoneNumber}] - returning page {pageNumber} of messages (Success): {JsonConvert.SerializeObject(pagedMessages)}");
                //    await hubConnection.SendAsync("LoadMessagesSuccess", phoneNumber, pagedMessages);
                //    return;
                //}

                //logger.LogError($"[{phoneNumber}] - page {pageNumber} of messages couldn't be retrieved (Fail). Returning error message");
                //await hubConnection.SendAsync("LoadMessagesFail", phoneNumber,
                //    $"Couldn't load page {pageNumber} of messages, for account: {phoneNumber}, requested by: {phoneNumber}");
            });

            hubConnection.On<MessageDto>("SendMessage", async (newMessageDto) =>
            {
                logger.LogInformation($"[{newMessageDto.Sender}] sending message to: {newMessageDto.Receiver}.");

                if (await messagingService.SaveMessage(newMessageDto))
                {
                    logger.LogInformation($"[{newMessageDto.Sender}] - message sent (Success) to: {newMessageDto.Receiver}.");
                    return;
                }

                logger.LogError($"[{newMessageDto.Sender}] - SendMessage couldn't be executed (Fail).");
            });
        }
    }
}
