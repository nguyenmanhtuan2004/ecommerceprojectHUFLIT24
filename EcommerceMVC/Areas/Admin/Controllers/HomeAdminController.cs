using EcommerceMVC.Data;
using EcommerceMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using EcommerceMVC.Areas.Admin.Models.ViewModels;
using EcommerceMVC.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;
using Microsoft.Extensions.Hosting;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static NuGet.Packaging.PackagingConstants;

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
            var result = hangHoas.Select(p => new HangHoaThemVM
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
        public IActionResult ThemSanPhamMoi(HangHoaThemVM model)
        {
            if (ModelState.IsValid)
            {
                var hanghoa = new HangHoa
                {
                    TenHh=model.TenHH??"",
                    MaLoai=model.MaLoai,
                    DonGia=model.DonGia,
                    NgaySx=model.NgaySx,
                    GiamGia=model.GiamGia,
                    SoLanXem=model.SoLanXem,
                    MaNcc=model.MaNcc,
                };
        
                if (model.MyImage != null)
                {
                    hanghoa.Hinh = MyUtil.UploadHinh(model.MyImage,"HangHoa");
                }

                db.Add(hanghoa);
                db.SaveChanges();
                return RedirectToAction("DanhMucHangHoa");
            }
            return View(model);
        }

        [Route("SuaSanPham")]
        [HttpGet]
        public IActionResult SuaSanPham(int MaHh)
        {
            ViewBag.MaHh = MaHh;
            var hangHoa=db.HangHoas.Find(MaHh);
            if (hangHoa == null)
            {
                return View();
            }    
            var result = new HangHoaThemVM
            {
                MaHh=MaHh,
                TenHH = hangHoa.TenHh,
                MaLoai = hangHoa.MaLoai,
                DonGia = hangHoa.DonGia,
                NgaySx = hangHoa.NgaySx,
                GiamGia = hangHoa.GiamGia,
                SoLanXem = hangHoa.SoLanXem,
                MaNcc = hangHoa.MaNcc,
             
            };

            return View(result);
        }
        [HttpPost]
        [Route("SuaSanPham")]
        public IActionResult SuaSanPham(HangHoaThemVM model,int MaHh)
        {
            if (ModelState.IsValid)
            {
                var hangHoa=db.HangHoas.FirstOrDefault(x=>x.MaHh==MaHh);
                if(hangHoa == null)
                {
                    return View(model);
                }    
                hangHoa.TenHh = model.TenHH;
                hangHoa.MaLoai = model.MaLoai;
                hangHoa.DonGia = model.DonGia;
                hangHoa.NgaySx = model.NgaySx;
                hangHoa.GiamGia = model.GiamGia;
                hangHoa.SoLanXem = model.SoLanXem;
                hangHoa.MaNcc = model.MaNcc;

                if (model.MyImage != null)
                {
                    hangHoa.Hinh = MyUtil.UploadHinh(model.MyImage, "HangHoa");       
                }
                db.Entry(hangHoa).State=EntityState.Modified;
                db.Update(hangHoa);
                db.SaveChanges();
                return RedirectToAction("DanhMucHangHoa");
            }
            return View(model);
        }

        [HttpPost]
        [Route("XoaSanPham")]
        public IActionResult XoaSanPham(int id)
        {
            var chitiethoadon = db.ChiTietHds.Where(x=>x.MaHh == id).ToList();
            if (chitiethoadon.Count()>0)
            {
                return RedirectToAction("DanhMucHangHoa");
            }
            db.Remove(db.HangHoas.Find(id));
            db.SaveChanges();
            return RedirectToAction("DanhMucHangHoa");
        }
    }
}
