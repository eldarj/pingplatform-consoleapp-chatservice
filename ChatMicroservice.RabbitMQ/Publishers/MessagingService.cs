using ChatMicroservice.Data.Context;
using ChatMicroservice.Data.Models;
using ChatMicroservice.RabbitMQ.Publishers.Interfaces;
using Microsoft.EntityFrameworkCore;
using Ping.Commons.Dtos.Models.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatMicroservice.RabbitMQ.Publishers
{
    public class MessagingService : IMessagingService
    {
        private readonly MyDbContext dbContext;
        public MessagingService(MyDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<bool> SaveMessage(MessageDto newMessageDto)
        {
            Account sender = await dbContext.Accounts
                .Where(a => a.PhoneNumber == newMessageDto.Sender)
                .SingleOrDefaultAsync();

            if (sender == null) return false;

            Account receiver = await dbContext.Accounts
                .Where(a => a.PhoneNumber == newMessageDto.Receiver)
                .SingleOrDefaultAsync();

            if (receiver == null) return false;

            Message message = new Message
            {
                SenderAccountId = sender.Id,
                ReceiverAccountId = receiver.Id,
                Text = newMessageDto.Text
            };

            dbContext.Messages.Add(message);
            await dbContext.SaveChangesAsync();
            return true;
        }
    }
}
