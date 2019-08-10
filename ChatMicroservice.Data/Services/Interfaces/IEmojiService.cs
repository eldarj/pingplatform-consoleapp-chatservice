using Ping.Commons.Dtos.Models.Emojis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ChatMicroservice.Data.Services.Interfaces
{
    public interface IEmojiService
    {
        Task<List<EmojiCategoryDto>> GetEmojis();
    }
}
