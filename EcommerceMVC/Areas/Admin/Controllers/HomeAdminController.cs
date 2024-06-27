using EcommerceMVC.Data;
using EcommerceMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EcommerceMVC.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin")]
    public class HomeAdminController : Controller
    {
        private readonly Hshop2023Context db;
        public HomeAdminController(Hshop2023Context context)
        {
            db = context;
        }

        [Route("")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("danhmuchanghoa")]
        public IActionResult DanhMucHangHoa(int? loai)
        {
            var hangHoas = db.HangHoas.AsQueryable();
            if (loai.HasValue)
            {
                hangHoas = hangHoas.Where(p => p.MaLoai == loai.Value);
            }
            var result = hangHoas.Select(p => new HangHoaVM
            {
                MaHh = p.MaHh,
                TenHH = p.TenHh,
                DonGia = p.DonGia ?? 0,
                Hinh = p.Hinh ?? "",
                TenLoai = p.MaLoaiNavigation.TenLoai
            });
            return View(result);
        }

        [Route("ThemSanPhamMoi")]
        [HttpGet]
        public IActionResult ThemSanPhamMoi()
        {
            return View();
        }
        [HttpPost]
        [Route("ThemSanPhamMoi")]
        public IActionResult ThemSanPhamMoi(HangHoaVM model)
        {
            if (ModelState.IsValid)
            { 
                return RedirectToAction("DanhMucHangHoa");
            }
            return View();
        }


    }
}
