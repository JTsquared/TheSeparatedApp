using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLayer.Models
{
    public enum ContactType { Family = 1, NonFamily }

    public class Contact : IContact
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public ContactType ContactType { get; set; }

        public Contact() { }

        public Contact(string contactPhoneNumber, ContactType contactType)
        {
            PhoneNumber = contactPhoneNumber;
            ContactType = contactType;
        }

        public Contact(string contactPhoneNumber, ContactType contactType, string contactName)
        {
            PhoneNumber = contactPhoneNumber;
            ContactType = contactType;
            Name = contactName;
        }
    }
}
