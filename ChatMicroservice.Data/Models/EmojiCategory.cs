using System;
using System.Collections.Generic;
using System.Text;

namespace ChatMicroservice.Data.Models
{
    public class EmojiCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Emoji> Emojis { get; set; }
    }
}
