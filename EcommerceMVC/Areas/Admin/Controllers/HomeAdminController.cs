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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System;
using X.PagedList;
using X.PagedList.Mvc.Core;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EcommerceMVC.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin")]
    public class HomeAdminController : Controller
    {
        private readonly Hshop2023Context db;
        private SignInManager<IdentityUser> signInManager;
        public HomeAdminController(Hshop2023Context context, SignInManager<IdentityUser> signInMgr)
        {
            db = context;
            signInManager = signInMgr;
        }

        [Authorize(Roles = "Admin")]
        [Route("danhmuchanghoa")]
        public IActionResult DanhMucHangHoa(int? loai,int? page)
        {
            var hangHoas = db.HangHoas.AsQueryable();
            if (loai.HasValue)
            {
                hangHoas = hangHoas.Where(p => p.MaLoai == loai.Value).AsNoTracking().OrderBy(x => x.TenHh);
            }
            var result = hangHoas.Select(p => new HangHoaThemVM
            {
                MaHh = p.MaHh,
                TenHH = p.TenHh,
                DonGia = p.DonGia ?? 0,
                Hinh = p.Hinh ?? "",
                TenLoai = p.MaLoaiNavigation.TenLoai
            });
            int pageSize = 9;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            ViewBag.pageSize = (hangHoas.Count() / pageSize)+1;
            ViewBag.pagenumber = pageNumber;
            PagedList<HangHoaThemVM> lst = new PagedList<HangHoaThemVM>(result, pageNumber, pageSize);
            return View(lst);
        }

        [Authorize(Roles = "Admin")]
        [Route("Search")]
        public IActionResult Search(string? query, int? page)
        {
            var hangHoas = db.HangHoas.AsQueryable();
            if (query != null)
            {
                hangHoas = hangHoas.Where(p => p.TenHh.Contains(query));
            }
            var result = hangHoas.Select(p => new HangHoaThemVM
            {
                MaHh = p.MaHh,
                TenHH = p.TenHh,
                DonGia = p.DonGia ?? 0,
                Hinh = p.Hinh ?? "",
                TenLoai = p.MaLoaiNavigation.TenLoai
            });
            int pageSize = 9;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            ViewBag.pageSize = (hangHoas.Count() / pageSize) + 1;
            ViewBag.pagenumber = pageNumber;
            PagedList<HangHoaThemVM> lst = new PagedList<HangHoaThemVM>(result, pageNumber, pageSize);
            return View(lst);
        }
        [Authorize(Roles = "Admin")]
        [Route("ThemSanPhamMoi")]
        [HttpGet]
        public IActionResult ThemSanPhamMoi()
        {
            ViewBag.MaNCC = db.NhaCungCaps.AsEnumerable();
            ViewBag.MaLoai = db.Loais.AsEnumerable();
            return View();
        }
        [HttpPost]
        [Route("ThemSanPhamMoi")]
        [Authorize(Roles = "Admin")]
        public IActionResult ThemSanPhamMoi(HangHoaThemVM model)
        {

            if (ModelState.IsValid)
            {
                var hanghoa = new HangHoa
                {
                    TenHh = model.TenHH ?? "",
                    MaLoai = model.MaLoai,
                    DonGia = model.DonGia,
                    NgaySx = model.NgaySx,
                    GiamGia = model.GiamGia,
                    SoLanXem = model.SoLanXem,
                    MaNcc = model.MaNcc,
                };

                if (model.MyImage != null)
                {
                    hanghoa.Hinh = MyUtil.UploadHinh(model.MyImage, "HangHoa");
                }

                db.Add(hanghoa);
                db.SaveChanges();
                return RedirectToAction("DanhMucHangHoa");
            }
            return View(model);
        }
        [Authorize(Roles = "Admin")]
        [Route("SuaSanPham")]
        [HttpGet]
        public IActionResult SuaSanPham(int MaHh)
        {
            ViewBag.MaHh= MaHh;
            var hangHoa = db.HangHoas.Include(p => p.MaLoaiNavigation)
                .Include(p => p.MaNccNavigation)
                .FirstOrDefault(x=>x.MaHh==MaHh);
            ViewBag.MaNCC = db.NhaCungCaps.AsEnumerable();
            ViewBag.MaLoai = db.Loais.AsEnumerable();
            if (hangHoa == null)
            {
                return View();
            }
            var result = new HangHoaThemVM
            {
                MaHh = MaHh,
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
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("SuaSanPham")]
        public IActionResult SuaSanPham(HangHoaThemVM model, int MaHh)
        {
            if (ModelState.IsValid)
            {
                var hangHoa = db.HangHoas.FirstOrDefault(x => x.MaHh == MaHh);
                if (hangHoa == null)
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
                db.Entry(hangHoa).State = EntityState.Modified;
                db.Update(hangHoa);
                db.SaveChanges();
                return RedirectToAction("DanhMucHangHoa");
            }
            return View(model);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("XoaSanPham")]
        public IActionResult XoaSanPham(int id)
        {
            var chitiethoadon = db.ChiTietHds.Where(x => x.MaHh == id).ToList();
            if (chitiethoadon.Count() > 0)
            {
                return RedirectToAction("DanhMucHangHoa");
            }
            db.Remove(db.HangHoas.Find(id));
            db.SaveChanges();
            return RedirectToAction("DanhMucHangHoa");
        }
        [Route("ChiTiet")]
        public IActionResult ChiTiet(int id)
        {
            var data = db.HangHoas
                .Include(p => p.MaLoaiNavigation)
                .Include(p => p.MaNccNavigation)
                .SingleOrDefault(p => p.MaHh == id);
            if (data == null)
            {
                TempData["Message"] = $"Không thấy sản phầm có mã {id}";
                return Redirect("/404");
            }
            /*var result = new HangHoaThemVM
            {
                MaHh = id,
                TenHH = data.TenHh,
                MaLoai = data.MaLoai,
                DonGia = data.DonGia,
                NgaySx = data.NgaySx,
                GiamGia = data.GiamGia,
                SoLanXem = data.SoLanXem,
                MaNcc = data.MaNcc,
                Hinh=data.Hinh,

            };*/
            return View(data);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("HoaDon")]
        public IActionResult HoaDon()
        {
            /*var customerId = User.Identity.Name;*/
            /*var chiTietHds = db.ChiTietHds
           .Include(ct => ct.MaHhNavigation)
           .Include(ct => ct.MaHdNavigation)
           .ToList();*/
            var groupedOrders = db.ChiTietHds.Include(ct => ct.MaHdNavigation)
        .AsEnumerable()
        .GroupBy(o => o.MaHd)
        .Select(g => new
        {
            MaHd = g.Key,
            Count= g.Sum(o=>o.SoLuong),
            SoMatHang= g.ToList().Count(),
            TenKH =g.Max(o=>o.MaHdNavigation.HoTen),
            TongTien=g.Max(o=>o.DonGia*o.SoLuong-o.GiamGia),
            Orders = g.ToList()
        });

            ViewBag.GroupedOrders = groupedOrders;
            return View();
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("ChiTietHoaDon")]
        public IActionResult ChiTietHoaDon(int MaHd)
        {
            var chiTietHds = db.ChiTietHds
            .Include(ct => ct.MaHhNavigation)
            .Include(ct => ct.MaHdNavigation)
            .Where(ct => ct.MaHd == MaHd)
            .ToList();
            return View(chiTietHds);
        }

    }
}
