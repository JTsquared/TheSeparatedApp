using BusinessLayer.Models.Interfaces;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLayer.Models
{
    public class CustomerEntity : TableEntity, ICustomer
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public DateTime RegisteredDate { get; set; }

        public CustomerEntity() { }

        public CustomerEntity(Customer customer)
        {
            ID = customer.ID;
            FirstName = customer?.FirstName ?? string.Empty;
            LastName = customer?.LastName ?? string.Empty;
            Email = customer?.Email ?? string.Empty;
            UserName = customer?.UserName ?? string.Empty;
            Password = customer?.Password ?? string.Empty;
            RegisteredDate = customer?.RegisteredDate ?? DateTime.MinValue;

            this.PartitionKey = UserName;
            this.RowKey = Password;
        }
        public CustomerEntity(int iD, string firstName, string lastName, string email, string userName, string password, DateTime registeredDate)
        {
            ID = iD;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            UserName = userName;
            Password = password;
            RegisteredDate = registeredDate;

            this.PartitionKey = userName;
            this.RowKey = password;
        }

    }

    public class FamilyMemberEntity : TableEntity, IFamilyMember
    {
        public int FamilyID { get; set; }
        public string FaceID { get; set; }
        public string Name { get; set; }
        public string DependentType { get; set; }

        public FamilyMemberEntity() { }

        public FamilyMemberEntity(FamilyMember familyMember)
        {
            FamilyID = familyMember?.FamilyID ?? 0;
            FaceID = familyMember?.FaceID ?? string.Empty;
            Name = familyMember?.Name ?? string.Empty;
            DependentType = familyMember?.DependentType ?? string.Empty;

            this.PartitionKey = FamilyID.ToString();
            this.RowKey = FaceID;
        }
        public FamilyMemberEntity(int familyID, string faceID, string name, string dependentType)
        {
            FamilyID = familyID;
            FaceID = faceID;
            Name = name;
            DependentType = dependentType;

            this.PartitionKey = familyID.ToString();
            this.RowKey = faceID;
        }
        
    }
}
