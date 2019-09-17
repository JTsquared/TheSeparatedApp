using System.Collections.Generic;

namespace BusinessLayer.Models
{
    public interface IMessageQueue
    {
        List<ReportMissingMsg> PollMsgQueue(ContactType contactType, Location location);

        void RemoveQueueMsg(ReportMissingMsg model, ReportMissingMsg matchedReport);
    }
}