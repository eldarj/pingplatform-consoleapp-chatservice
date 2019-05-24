using Api.DtoModels.Auth;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ChatMicroservice.Data.Services.Interfaces
{
    public interface IAccountService
    {
        Task<bool> CreateNewUser(AccountDto accountDto);
    }
}
