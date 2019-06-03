using System;
using System.Collections.Generic;
using System.Text;

namespace ChatMicroservice.RabbitMQ.Publishers.Interfaces
{
    public interface IContactMQPublisher
    {
        void CreateConnection();

        void Close();

        void SendContact<T>(T contact);
    }
}
