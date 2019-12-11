namespace BusinessLayer.Models
{
    public interface ICustomerRepository
    {
        Customer GetCustomer(string username, string password);
        void CreateCustomer(Customer customer);

    }
}