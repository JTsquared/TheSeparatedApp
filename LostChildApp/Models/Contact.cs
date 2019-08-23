using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LostChild.Models
{
    public class Contact : IContact
    {
        private string phoneNumber;
        private string firstName;
        private string lastName;
        private Location location;

        public string PhoneNumber { get => phoneNumber; set => phoneNumber = value; }
        public string FirstName { get => firstName; set => firstName = value; }
        public string LastName { get => lastName; set => lastName = value; }

        public Contact(string contactPhoneNumber)
        {
            phoneNumber = contactPhoneNumber;
        }

        public Contact(string contactPhoneNumber, Location contactLocation)
        {
            phoneNumber = contactPhoneNumber;
            location = contactLocation;
        }

        public Contact(string contactPhoneNumber, string contactFirstName, string contactLastName)
        {
            phoneNumber = contactPhoneNumber;
            firstName = contactFirstName;
            lastName = contactLastName;
        }
    }
}
