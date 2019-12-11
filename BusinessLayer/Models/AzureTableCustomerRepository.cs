using System;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BusinessLayer.Models
{
    class AzureTableCustomerRepository : ICustomerRepository
    {
        private CloudStorageAccount _cloudStorageAccount;
        private CloudTableClient _cloudTableClient;
        private CloudTable _cloudTable;

        public AzureTableCustomerRepository()
        {
            _cloudStorageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=theseparatedapptable;AccountKey=wG1nxodjz9Sgx12NSMKYjuaHvLUbAAiWXg+Y0ymQPpGldUv5GSpj/zLeEnXVPSx6L3kRU/amfUQUcKpjKljriw==;EndpointSuffix=core.windows.net");
            _cloudTableClient = _cloudStorageAccount.CreateCloudTableClient();
            _cloudTable = _cloudTableClient.GetTableReference("customers");
        }

        public AzureTableCustomerRepository(CloudStorageAccount cloudStorageAccount, CloudTableClient cloudTableClient, CloudTable cloudTable)
        {
            _cloudStorageAccount = cloudStorageAccount;
            _cloudTableClient = cloudTableClient;
            _cloudTable = cloudTable;
        }

        public void CreateCustomer(Customer customer)
        {
            AddCustomerToStorageAsync(new CustomerEntity(customer)).GetAwaiter().GetResult();

            foreach(FamilyMember member in customer.FamilyMembers)
            {
                AddFamilyMemberToStorageAsync(new FamilyMemberEntity(member)).GetAwaiter().GetResult();
            }
        }

        public async Task AddCustomerToStorageAsync(CustomerEntity customer)
        {
            _cloudTable = _cloudTableClient.GetTableReference("customers");
            TableOperation tableOperation = TableOperation.Insert(customer);
            var results = await _cloudTable.ExecuteAsync(tableOperation);
        }

        public async Task AddFamilyMemberToStorageAsync(FamilyMemberEntity familyMember)
        {
            _cloudTable = _cloudTableClient.GetTableReference("familyMembers");
            TableOperation tableOperation = TableOperation.Insert(familyMember);
            var results = await _cloudTable.ExecuteAsync(tableOperation);
        }

        public Customer GetCustomer(string username, string password)
        {
            Customer customer = null;
            try
            {
                _cloudTable = _cloudTableClient.GetTableReference("customers");
                var customerQuery = GetCustomerAsync(username, password);
                customer = new Customer(customerQuery.Result);
                _cloudTable = _cloudTableClient.GetTableReference("familyMembers");
                var familyMemberQuery = GetFamilyMembersAsync(customer.ID);
                //List<FamilyMemberEntity> familyMembers = familyMemberQuery.Result;
                customer.SetFamilyMemberList(familyMemberQuery.Result);
            }
            catch (Exception) { }

            return customer;
        }

        public async Task<CustomerEntity> GetCustomerAsync(string username, string password)
        {
            CustomerEntity customer = new CustomerEntity();

            string usernameFilter = TableQuery.GenerateFilterCondition("Username", QueryComparisons.Equal, username);
            string passwordFilter = TableQuery.GenerateFilterCondition("Password", QueryComparisons.Equal, password);
            string credentialsFilter = TableQuery.CombineFilters(usernameFilter, TableOperators.And, passwordFilter);

            TableQuery<CustomerEntity> customerQuery = new TableQuery<CustomerEntity>().Where(credentialsFilter);

            var continuationToken = default(TableContinuationToken);

            var queryResult = await _cloudTable.ExecuteQuerySegmentedAsync(customerQuery, continuationToken);
            customer = queryResult.Results.Find(x => x.PartitionKey != "");

            return customer;
        }

        private async Task<List<FamilyMemberEntity>> GetFamilyMembersAsync(int id)
        {
            List<FamilyMemberEntity> familyMemberList = new List<FamilyMemberEntity>();

            string familyIDFilter = TableQuery.GenerateFilterConditionForInt("FamilyID", QueryComparisons.Equal, id);

            TableQuery<FamilyMemberEntity> familyMemberQuery = new TableQuery<FamilyMemberEntity>().Where(familyIDFilter);

            var continuationToken = default(TableContinuationToken);

            do
            {
                var queryResult = await _cloudTable.ExecuteQuerySegmentedAsync(familyMemberQuery, continuationToken);
                familyMemberList.AddRange(queryResult.Results);
                continuationToken = queryResult.ContinuationToken;
            } while (continuationToken != null);

            return familyMemberList;
        }
    }
}
