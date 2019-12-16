using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLayer.Models
{
    public enum ContactType { Family = 1, NonFamily }
    public enum NotificationTypePreference { PushNotification = 1, Email }

    public class Contact : IContact
    {
        [Required]
        public string Name { get; set; }
        [Required(ErrorMessage = "You must enter a telphone number")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }
        [Required]
        public ContactType ContactType { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        public NotificationTypePreference NotificationTypePreference { get; set; }

        public Contact()
        {
            //NotificationTypePreference = NotificationTypePreference.PushNotification;
        }

        public Contact(string contactPhoneNumber, ContactType contactType, NotificationTypePreference preference)
        {
            PhoneNumber = contactPhoneNumber;
            ContactType = contactType;
            NotificationTypePreference = preference;
            //NotificationTypePreference = NotificationTypePreference.PushNotification;
        }

        public Contact(string contactPhoneNumber, ContactType contactType, string contactName, NotificationTypePreference preference)
        {
            PhoneNumber = contactPhoneNumber;
            ContactType = contactType;
            Name = contactName;
            NotificationTypePreference = preference;
            //NotificationTypePreference = NotificationTypePreference.PushNotification;
        }

        public void SetPreferredNotificationType(NotificationTypePreference notificationType)
        {
            NotificationTypePreference = notificationType;
        }
    }
}
