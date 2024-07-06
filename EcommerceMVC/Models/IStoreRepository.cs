using EcommerceMVC.Areas.Admin.Models.ViewModels;
using EcommerceMVC.Data;

namespace EcommerceMVC.Models
{
    public interface IStoreRepository
    {
        IQueryable<HangHoa> Products { get; }

        void SaveProduct(HangHoa p);
        void CreateProduct(HangHoa p);
        void DeleteProduct(HangHoa p);
    }

}
