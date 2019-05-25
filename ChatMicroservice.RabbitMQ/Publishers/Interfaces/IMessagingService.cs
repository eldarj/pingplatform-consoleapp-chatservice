using Ping.Commons.Dtos.Models.Chat;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ChatMicroservice.RabbitMQ.Publishers.Interfaces
{
    public interface IMessagingService
    {
        Task<bool> SaveMessage(MessageDto newMessageDto);
    }
}
