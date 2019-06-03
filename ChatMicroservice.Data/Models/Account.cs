﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ChatMicroservice.Data.Models
{
    public class Account
    {
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }

        [Required]
        public string PhoneNumber { get; set; }
        public DateTime DateRegistered { get; set; } = DateTime.Now;

        public string AvatarImageUrl { get; set; }
        public string CoverImageUrl { get; set; }

        public string DataSpaceDirName { get; set; }

        public int CallingCountryCode { get; set; }

        [InverseProperty("Account")]
        public virtual ICollection<Contact> Contacts { get; set; }
    }
}
