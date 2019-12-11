using BusinessLayer.Models.Interfaces;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLayer.Models
{
    public class Customer : ICustomer
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public DateTime RegisteredDate { get; set; }
        public List<FamilyMember> FamilyMembers { get; set; }

        public Customer() { }

        public Customer(int iD, string firstName, string lastName, string email, string userName, string password, DateTime registeredDate, List<FamilyMember> familyMembers)
        {
            ID = iD;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            UserName = userName;
            Password = password;
            RegisteredDate = registeredDate;
            FamilyMembers = familyMembers;
        }

        public Customer(ICustomer custEntity)
        {
            ID = custEntity.ID;
            FirstName = custEntity.FirstName;
            LastName = custEntity.LastName;
            Email = custEntity.Email;
            UserName = custEntity.UserName;
            Password = custEntity.Password;
            RegisteredDate = custEntity.RegisteredDate;
        }

        public void SetFamilyMemberList(List<FamilyMemberEntity> familyMemberEntityList)
        {
            List<FamilyMember> family = new List<FamilyMember>();
            foreach (FamilyMemberEntity fmEntity in familyMemberEntityList)
            {
                family.Add(new FamilyMember(fmEntity));
            }
            FamilyMembers = family;
        }
    }
}
