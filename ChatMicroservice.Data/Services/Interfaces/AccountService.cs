using Api.DtoModels.Auth;
using ChatMicroservice.Data.Context;
using ChatMicroservice.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatMicroservice.Data.Services.Interfaces
{
    public class AccountService : IAccountService
    {
        private MyDbContext dbContext;

        public AccountService(MyDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<bool> CreateNewUser(AccountDto accountDto)
        {
            var account = dbContext.Accounts.Where(a => a.PhoneNumber == accountDto.PhoneNumber).SingleOrDefault();
            if (account != null) return false; // implement update

            account = new Account
            {
                Id = accountDto.Id,
                Firstname = accountDto.Firstname,
                Lastname = accountDto.Lastname,
                PhoneNumber = accountDto.PhoneNumber
            };

            if (accountDto.Contacts?.Count > 0)
            {
                var dtoContacts = accountDto.Contacts.Select(dto => dto.PhoneNumber).ToList();
                List<Account> contactsToAdd = dbContext.Accounts
                    .Where(a => dtoContacts.Contains(a.PhoneNumber))
                    .ToList();

                account.Contacts = contactsToAdd.Select(a => new Contact
                {
                    Account = account,
                    ContactAccountId = a.Id,
                    ContactName = accountDto.Contacts.SingleOrDefault(c => c.PhoneNumber == a.PhoneNumber)?.ContactName
                })
                .ToList();
            }

            dbContext.Accounts.Add(account);

            await dbContext.SaveChangesAsync();

            return true;
        }
    }
}
