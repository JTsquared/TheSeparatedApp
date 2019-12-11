namespace BusinessLayer.Models
{
    public interface INotification
    {
        string Title { get; set; }
        string Message { get; set; }
        string Data { get; set; }
    }
}