using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LostFamily.Models
{
    public interface ILostDependentRepository
    {
        ReportMissingMsg GetLostChildAlert();
        ReportMissingMsg SendLostChildAlert();
    }
}
