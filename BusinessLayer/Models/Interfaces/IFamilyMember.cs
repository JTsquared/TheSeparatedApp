using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLayer.Models.Interfaces
{
    public interface IFamilyMember
    {
        int FamilyID { get; set; }
        string FaceID { get; set; }
        string Name { get; set; }
        string DependentType { get; set; }
    }
}
