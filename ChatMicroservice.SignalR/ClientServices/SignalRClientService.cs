using ChatMicroservice.Data.Services.Interfaces;
using ChatMicroservice.RabbitMQ.Consumers.Interfaces;
using ChatMicroservice.RabbitMQ.Publishers.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ping.Commons.Dtos.Models.Auth;
using Ping.Commons.Dtos.Models.Chat;
using Ping.Commons.Dtos.Models.Wrappers.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatMicroservice.SignalR.ClientServices
{
    public class SignalRClientService : IHostedService
    {
        private readonly ILogger logger;
        private readonly IApplicationLifetime appLifetime;
        private readonly IContactService contactService;
        private readonly IMessagingService messagingService;

        private readonly HubConnection hubConnectionChat;

        private readonly IContactMQPublisher contactMQPublisher;
        public SignalRClientService(
            ILogger<SignalRClientService> logger,
            IApplicationLifetime applicationLifetime,
            IContactService contactService,
            IContactMQPublisher contactMQPublisher,
            IMessagingService messagingService)
        {
            this.logger = logger;
            this.appLifetime = applicationLifetime;

            this.contactService = contactService;
            this.messagingService = messagingService;

            this.contactMQPublisher = contactMQPublisher;

            // Setup SignalR Hub connection
            hubConnectionChat = new HubConnectionBuilder()
                .WithUrl("https://localhost:44380/chathub?groupName=chatMicroservice")
                .Build();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            appLifetime.ApplicationStarted.Register(OnStarted);
            appLifetime.ApplicationStopping.Register(OnStopping);
            appLifetime.ApplicationStopped.Register(OnStopped);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private async void OnStarted()
        {
            logger.LogInformation("Starting ChatMicroservice (OnStarted)");
            // Connect to hub
            try
            {
                await hubConnectionChat.StartAsync().ContinueWith(t =>
                {
                    if (t.IsFaulted)
                    {
                        logger.LogError("-- Couln't connect to signalR ChatHub (OnStarted)");
                        return;
                    }
                    logger.LogInformation("ChatMicroservice connected to ChatHub successfully (OnStarted)");
                });

                hubConnectionChat.On<string, string, ContactDto>("AddContact", async (appId, phoneNumber, newContactDto) =>
                {
                    logger.LogInformation($"-- {appId} adding a new contact [{newContactDto.ContactPhoneNumber}] for account: {newContactDto.PhoneNumber}.");

                    ResponseDto<ContactDto> response = await contactService.AddContact(phoneNumber, newContactDto);
                    if (response != null)
                    {
                        logger.LogInformation($"-- Returning new contact data.");
                        await hubConnectionChat.SendAsync("AddContactResponse", appId, response);
                        if (response.Dto != null)
                        {
                            contactMQPublisher.SendCreatedContact(response.Dto);
                        }
                        return;
                    }

                    logger.LogError($"-- Request couldn't be executed - returning error message.");
                    await hubConnectionChat.SendAsync("AddContactFail", appId,
                        $"Couldn't add new contact [{newContactDto.ContactPhoneNumber}], for account by number: {newContactDto.PhoneNumber}, requested by: {appId}");
                });

                hubConnectionChat.On<string, string>("RequestContacts", async (appId, phoneNumber) =>
                {
                    logger.LogInformation($"-- {appId} requesting Contacts for account: {phoneNumber}.");
                    
                    List<ContactDto> contacts = await contactService.GetAllByUser(phoneNumber);
                    if (contacts != null)
                    {
                        logger.LogInformation($"-- Returning list of contacts.");
                        await hubConnectionChat.SendAsync("RequestContactsSuccess", appId, contacts);
                        return;
                    }

                    logger.LogError($"-- Request couldn't be executed - returning error message.");
                    await hubConnectionChat.SendAsync("RequestContactsFail", appId,
                        $"Couldn't load contacts, for account by number: {phoneNumber}, requested by: {appId}");
                });

                hubConnectionChat.On<MessageDto>("SendMessage", async (newMessageDto) =>
                {
                    logger.LogInformation($"-- {newMessageDto.Sender} sending message to: {newMessageDto.Receiver}.");

                    if (await messagingService.SaveMessage(newMessageDto))
                    {
                        logger.LogInformation($"-- Message saved successfully.");
                        return;
                    }

                    logger.LogError($"-- Request couldn't be executed.");
                });
            }
            catch (Exception e)
            {
                logger.LogInformation("ChatMicroservice couldn't be started (OnStarted)");
                return;
            }
            // Perform on-started activites here

        }

        private void OnStopping()
        {
            logger.LogInformation("Stopping ChatMicroservice (OnStopping)");
            // Perform on-stopping activities here
        }

        private void OnStopped()
        {
            logger.LogInformation("ChatMicroservice stopped (OnStopped)");
            // Perform post-stopped activities here
        }
    }
}
