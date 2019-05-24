using ChatMicroservice.Data.Services.Interfaces;
using ChatMicroservice.RabbitMQ.Consumers.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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

        private readonly HubConnection hubConnectionAuth;
        public SignalRClientService(
            ILogger<SignalRClientService> logger,
            IApplicationLifetime applicationLifetime)
        {
            this.logger = logger;
            this.appLifetime = applicationLifetime;

            // Setup SignalR Hub connection
            hubConnectionAuth = new HubConnectionBuilder()
                .WithUrl("https://localhost:44380/authhub?groupName=chatMicroservice")
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
                await hubConnectionAuth.StartAsync().ContinueWith(t =>
                {
                    if (t.IsFaulted)
                    {
                        logger.LogError("-- Couln't connect to signalR ChatHub (OnStarted)");
                        return;
                    }
                    logger.LogInformation("ChatMicroservice connected to ChatHub successfully (OnStarted)");
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
