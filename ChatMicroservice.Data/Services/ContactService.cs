using ChatMicroservice.Data.Context;
using ChatMicroservice.Data.Models;
using ChatMicroservice.Data.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Ping.Commons.Dtos.Models.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatMicroservice.Data.Services
{
    public class ContactService : IContactService
    {
        private readonly MyDbContext dbContext;
        public ContactService(MyDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<ContactDto>> GetAllByUser(string phoneNumber)
        {
            Account account = await dbContext.Accounts
                .Where(a => a.PhoneNumber == phoneNumber)
                .SingleOrDefaultAsync();

            if (account == null)
            {
                return null;
            }

            return await dbContext.Contacts
                .Where(c => c.AccountId == account.Id)
                .Select(c => new ContactDto
                {
                    DateAdded = c.DateAdded,
                    ContactName = c.ContactName,
                    AvatarImageUrl = c.ContactAccount.AvatarImageUrl,
                    CoverImageUrl = c.ContactAccount.CoverImageUrl,
                    PhoneNumber = c.ContactAccount.PhoneNumber,
                    ContactAccountId = c.ContactAccountId
                })
                .ToListAsync();
        }
    }
}
