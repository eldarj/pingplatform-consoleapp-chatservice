using ChatMicroservice.Data.Context;
using ChatMicroservice.Data.Models;
using ChatMicroservice.Data.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Ping.Commons.Dtos.Models.Emojis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatMicroservice.Data.Services
{
    public class EmojiService : IEmojiService
    {
        private readonly MyDbContext dbContext;
        public EmojiService(MyDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<EmojiCategoryDto>> GetEmojis()
        {
            return await dbContext.EmojiCategories.Select(emojiCategory => new EmojiCategoryDto
            {
                Name = emojiCategory.Name,
                Emojis = emojiCategory.Emojis.Select(emoji => new EmojiDto
                {
                    Category = emojiCategory.Name,
                    Name = emoji.Name,
                    Shortcode = emoji.Shortcode,
                    HexCodePoint = emoji.HexCodePoint,
                })
                .ToList()
            })
            .ToListAsync();
        }
    }
}
