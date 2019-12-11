using BusinessLayer.Models.Interfaces;

namespace BusinessLayer.Models
{
    public class FamilyMember : IFamilyMember
    {
        public int FamilyID { get; set; }
        public string FaceID { get; set; }
        public string Name { get; set; }
        public string DependentType { get; set; }

        public FamilyMember() { }

        public FamilyMember(IFamilyMember fmEntity)
        {
            FamilyID = fmEntity.FamilyID;
            FaceID = fmEntity.FaceID;
            Name = fmEntity.Name;
            DependentType = fmEntity.DependentType;
        }
    }
}