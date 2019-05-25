using ChatMicroservice.Data.Context;
using ChatMicroservice.Data.Models;
using ChatMicroservice.Data.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Ping.Commons.Dtos.Models.Auth;
using Ping.Commons.Dtos.Models.Chat;
using Ping.Commons.Dtos.Models.Wrappers.Response;
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

        public async Task<ResponseDto<ContactDto>> AddContact(string phoneNumber, ContactDto newContactDto)
        {
            Account account = await dbContext.Accounts
                .Where(a => a.PhoneNumber == newContactDto.PhoneNumber)
                .SingleOrDefaultAsync();

            if (account == null) return null;

            Account contactAccount = await dbContext.Accounts
                .Where(a => a.PhoneNumber == newContactDto.ContactPhoneNumber)
                .SingleOrDefaultAsync();

            if (contactAccount == null)
            {
                return new ResponseDto<ContactDto>
                {
                    Message = "It seems like this contact isn't a Ping user.",
                    MessageCode = "CONTACT_DOESNT_EXIST"
                };
            }

            Contact contact = new Contact
            {
                Account = account,
                AccountId = account.Id,
                ContactAccountId = contactAccount.Id,
                ContactName = !String.IsNullOrWhiteSpace(newContactDto.ContactName) ?
                    newContactDto.ContactName :
                    contactAccount.Firstname + " " + contactAccount.Lastname // fix this - we receive this from the user
            };

            dbContext.Contacts.Add(contact);
            await dbContext.SaveChangesAsync();
            return new ResponseDto<ContactDto>
            {
                Dto = new ContactDto
                {
                    DateAdded = contact.DateAdded,
                    AccountId = contact.AccountId,
                    PhoneNumber = account.PhoneNumber,
                    ContactAccountId = contact.ContactAccountId,
                    ContactName = contact.ContactName,
                    ContactPhoneNumber = contact.ContactAccount.PhoneNumber,
                    AvatarImageUrl = contact.ContactAccount.AvatarImageUrl,
                    CoverImageUrl = contact.ContactAccount.CoverImageUrl
                },
                Message = "New contact added successfully.",
                MessageCode = "CONTACT_ADDED_SUCCESSFULLY"
            };
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
                    AccountId = account.Id,
                    PhoneNumber = account.PhoneNumber,
                    ContactAccountId = c.ContactAccountId,
                    ContactName = c.ContactName,
                    ContactPhoneNumber = c.ContactAccount.PhoneNumber,
                    AvatarImageUrl = c.ContactAccount.AvatarImageUrl,
                    CoverImageUrl = c.ContactAccount.CoverImageUrl,
                    Messages = dbContext.Messages
                        .Where(m => (m.SenderAccountId == account.Id && m.ReceiverAccountId == c.ContactAccountId) ||
                            (m.SenderAccountId == c.ContactAccountId && m.ReceiverAccountId == c.AccountId))
                        .OrderByDescending(m => m.DateSent)
                        .Select(m => new MessageDto
                        {
                            Sender = m.SenderAccount.PhoneNumber,
                            Receiver = m.ReceiverAccount.PhoneNumber,
                            Text = m.Text,
                            Ticks = m.DateSent.Ticks
                        })
                        .ToList()
                })
                .ToListAsync();
        }
    }
}
