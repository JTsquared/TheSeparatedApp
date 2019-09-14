namespace LostFamily.Models
{
    public interface IContact
    {
        string Name { get; set; }
        string PhoneNumber { get; set; }
        ContactType ContactType { get; set; }
    }
}