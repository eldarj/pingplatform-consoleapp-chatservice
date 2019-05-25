using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Ping.Commons.Dtos.Models.Auth;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ChatMicroservice.SignalR.ClientServices
{
    public class ChatHubClient
    {
        private readonly ILogger logger;
        private readonly HubConnection hubConnectionChat;
        public ChatHubClient(ILogger<ChatHubClient> logger)
        {
            this.logger = logger;

            // Setup SignalR Hub connection
            hubConnectionChat = new HubConnectionBuilder()
                .WithUrl("https://localhost:44380/chathub?groupName=chatMicroservice")
                .Build();

            Task.Run(async () =>
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
            });
        }

        public void ContactRegisteredOnPing(string phoneNumber, ContactDto contactDto) {
            hubConnectionChat.SendAsync("AddContactSuccess", phoneNumber, contactDto);
        }
    }
}
