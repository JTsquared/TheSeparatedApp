using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LostChild.Models
{
    interface ILostChildRepository
    {
        LostChildAlert GetLostChildAlert();
        LostChildAlert SendLostChildAlert();
    }
}
