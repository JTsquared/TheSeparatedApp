﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLayer.Models
{
    public class LostChildRepository : ILostDependentRepository
    {

        public ReportMissingMsg GetLostChildAlert()
        {
            throw new NotImplementedException();
        }

        public ReportMissingMsg SendLostChildAlert()
        {
            throw new NotImplementedException();
        }
    }
}