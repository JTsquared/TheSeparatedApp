using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Models
{
    public interface IMessageRepository
    {
        List<ReportMissingMsgAdaptor> GetMessagesByLocation(ContactType contactType, Coordinates coordinates, double searchRadius);
        void RemoveMessageFromStorage(ReportMissingMsgAdaptor reportMessage);
        void AddMessageToStorage(ReportMissingMsgAdaptor reportMessage);
    }
}