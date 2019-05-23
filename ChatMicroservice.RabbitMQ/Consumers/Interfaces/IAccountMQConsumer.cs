using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ChatMicroservice.RabbitMQ.Consumers.Interfaces
{
    public interface IAccountMQConsumer
    {
        void CreateConnection();

        void Close();

        void ConsumeMessages();
    }
}
