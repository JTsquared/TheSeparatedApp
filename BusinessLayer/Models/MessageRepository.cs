using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage;
using Newtonsoft.Json;

namespace BusinessLayer.Models
{
    public class MessageRepository : IMessageRepository
    {
        private CloudStorageAccount _cloudStorageAccount;
        private CloudTableClient _cloudTableClient;
        private CloudTable _cloudTable;

        public MessageRepository()
        {
            _cloudStorageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=lostchildstoragegroup;AccountKey=clGdbIKEAkQx1OB2dZfazxCLdNLkqO490nX+SnE+FAlUCY6fNcOrfG0MAXaFT+An38sO1/0nuMVkIH2ZYE4cMw==;EndpointSuffix=core.windows.net");
            _cloudTableClient = _cloudStorageAccount.CreateCloudTableClient();
            _cloudTable = _cloudTableClient.GetTableReference("theseparatedapptable");
        }

        public MessageRepository(CloudStorageAccount cloudStorageAccount, CloudTableClient cloudTableClient, CloudTable cloudTable)
        {
            _cloudStorageAccount = cloudStorageAccount;
            _cloudTableClient = cloudTableClient;
            _cloudTable = cloudTable;
        }

        public List<ReportMissingMsgAdaptor> GetMessagesByLocation(ContactType contactType, Location location, double searchRadius)
        {
            var tableSegmentResults = GetMessagesByLocationAsync(contactType, location, searchRadius);
            return tableSegmentResults.Result;
        }

        public async Task<List<ReportMissingMsgAdaptor>> GetMessagesByLocationAsync(ContactType contactType, Location location, double searchRadius)
        {
            //make sure that the contact type search is opposite the contact type of the caller. i.e. if caller has contact type family the contact type for search should be non-family
            ContactType searchContactType = contactType == ContactType.NonFamily ? ContactType.Family : ContactType.NonFamily;

            List<ReportMissingMsgAdaptor> messageList = new List<ReportMissingMsgAdaptor>();

            //setup search radius filter for azure storage table to only pull records within a certain distance of report location
            string longitudeMinusFilter = TableQuery.GenerateFilterConditionForDouble("Longitude", QueryComparisons.GreaterThan, location.Coordinates.Longitude - searchRadius);
            string longitudePlusFilter = TableQuery.GenerateFilterConditionForDouble("Longitude", QueryComparisons.LessThan, location.Coordinates.Longitude + searchRadius);
            string latitudeMinusFilter = TableQuery.GenerateFilterConditionForDouble("Latitude", QueryComparisons.GreaterThan, location.Coordinates.Latitude - searchRadius);
            string latitudePlusFilter = TableQuery.GenerateFilterConditionForDouble("Latitude", QueryComparisons.LessThan, location.Coordinates.Latitude + searchRadius);

            string longitudePlusMinus = TableQuery.CombineFilters(longitudeMinusFilter, TableOperators.Or, longitudePlusFilter);
            string latitudePlusMinus = TableQuery.CombineFilters(latitudeMinusFilter, TableOperators.Or, latitudePlusFilter);

            string locSearchRadiusFilter = TableQuery.CombineFilters(longitudePlusMinus, TableOperators.And, latitudePlusMinus);

            TableQuery<ReportMissingMsgAdaptor> query = new TableQuery<ReportMissingMsgAdaptor>().Where(locSearchRadiusFilter);

            var continuationToken = default(TableContinuationToken);

            do
            {
                var queryResult = await _cloudTable.ExecuteQuerySegmentedAsync(query, continuationToken);
                messageList.AddRange(queryResult.Results);
                continuationToken = queryResult.ContinuationToken;
            } while (continuationToken != null);

            return messageList;     
        }

        public void AddMessageToStorage(ReportMissingMsgAdaptor reportMessage)
        {
            AddMessageToStorageAsync(reportMessage).GetAwaiter().GetResult();
        }

        public async Task AddMessageToStorageAsync(ReportMissingMsgAdaptor reportMessage)
        {
            TableOperation tableOperation = TableOperation.Insert(reportMessage);
            var results = await _cloudTable.ExecuteAsync(tableOperation);
        }

        public void RemoveMessageFromStorage(ReportMissingMsgAdaptor reportMessage)
        {
            //TODO: make a call to queue storage to process/clear message
            //after project is in MVP state consider creating a secondary queue to push both the model message and the matched report message which could be kept until family member confirmed that their dependent has been reunited
        }
    }
}
