using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ChatMicroservice.Data.Models
{
    public class Emoji
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Shortcode { get; set; }
        public string HexCodePoint { get; set; }

        [ForeignKey("EmojiCategory")]
        public int EmojiCategoryId { get; set; }
        public virtual EmojiCategory EmojiCategory { get; set; }
    }
}
