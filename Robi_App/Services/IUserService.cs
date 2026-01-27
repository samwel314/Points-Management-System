using Robi_App.Models.ViewModels;

namespace Robi_App.Services
{
    public interface IUserService
    {
        Task<IEnumerable <CustomerProfileVM>> GetAllCustomers();
    }
}
