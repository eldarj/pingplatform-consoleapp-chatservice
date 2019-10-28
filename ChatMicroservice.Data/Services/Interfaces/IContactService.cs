using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Ping.Commons.Dtos.Models.Auth;
using Ping.Commons.Dtos.Models.Chat;
using Ping.Commons.Dtos.Models.Wrappers.Response;

namespace ChatMicroservice.Data.Services.Interfaces
{
    public interface IContactService
    {
        Task<List<ContactDto>> GetAllByUser(string phoneNumber);
        Task<ResponseDto<ContactDto>> AddContact(string phoneNumber, ContactDto contactDto);
        Task<ResponseDto<ContactDto>> UpdateContact(string phoneNumber, ContactDto contactDto);
    }
}
