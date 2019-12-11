using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLayer.Models.Interfaces
{
    public interface ICustomer
    {
        int ID { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        string Email { get; set; }
        string UserName { get; set; }
        string Password { get; set; }
        DateTime RegisteredDate { get; set; }
    }
}
