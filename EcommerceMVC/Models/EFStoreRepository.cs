using EcommerceMVC.Data;
using EcommerceMVC.Models;
namespace EcommerceMVC.Models
{
    public class EFStoreRepository : IStoreRepository
    {
        private Hshop2023Context context;
        public EFStoreRepository(Hshop2023Context ctx)
        {
            context = ctx;
        }
        public IQueryable<HangHoa> Products => context.HangHoas;

        public void CreateProduct(HangHoa p)
        {
            context.Add(p);
            context.SaveChanges();
        }
        public void DeleteProduct(HangHoa p)
        {
            context.Remove(p);
            context.SaveChanges();
        }
        public void SaveProduct(HangHoa p)
        {
            context.SaveChanges();
        }
    }
}
