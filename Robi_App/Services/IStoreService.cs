using Robi_App.Models;
using Robi_App.Models.ViewModels;

namespace Robi_App.Services
{
    public interface IStoreService
    {
        IEnumerable <Store> GetStores(bool tracking);    
        Store ? GetStoreById(int id , bool tracking);
        void AddStore(Store store);
        void UpdateStore(Store store);
        void DeleteStore(Store store);
        bool StoreExists(int id);

        ShowStoreInvoicesVM ShowStoreInvoicesVM(int id);  
    }
}
