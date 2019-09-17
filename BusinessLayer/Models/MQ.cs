using System;
using System.Collections.Generic;

namespace BusinessLayer.Models
{
    public class MQ : IMessageQueue
    {
        public List<ReportMissingMsg> PollMsgQueue(ContactType contactType, Location location)
        {
            List<ReportMissingMsg> queueResults = new List<ReportMissingMsg>();
            ContactType searchContactType = contactType == ContactType.NonFamily ? ContactType.Family : ContactType.NonFamily;
            //queueResults = MissingReportRepository.GetQueueMessages(searchContactType, location);
            return queueResults;
        }

        public void RemoveQueueMsg(ReportMissingMsg model, ReportMissingMsg matchedReport)
        {
            //TODO: make a call to queue storage to process/clear message
            //after project is in MVP state consider creating a secondary queue to push both the model message and the matched report message which could be kept until family member confirmed that their dependent has been reunited
        }
    }
}
