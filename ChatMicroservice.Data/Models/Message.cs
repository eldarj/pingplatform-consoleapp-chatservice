using System;
using System.Collections.Generic;
using System.Text;

namespace ChatMicroservice.Data.Models
{
    public class Message
    {
        public int Id { get; set; }
        public DateTime DateSent { get; set; } = DateTime.UtcNow;
        public string Text { get; set; }

        public int SenderAccountId { get; set; }
        public Account SenderAccount { get; set; }

        public int ReceiverAccountId { get; set; }
        public Account ReceiverAccount { get; set; }
    }
}
