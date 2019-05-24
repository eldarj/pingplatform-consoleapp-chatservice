using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Ping.Commons.Dtos.Models.Auth;

namespace ChatMicroservice.Data.Services.Interfaces
{
    public interface IContactService
    {
        Task<List<ContactDto>> GetAllByUser(string phoneNumber);
    }
}
