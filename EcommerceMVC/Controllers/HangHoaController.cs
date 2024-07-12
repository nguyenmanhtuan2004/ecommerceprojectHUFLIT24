using EcommerceMVC.Data;
using Microsoft.AspNetCore.Mvc;
using EcommerceMVC.Models;
using EcommerceMVC.ViewModels;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using X.PagedList.Mvc.Core;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
namespace EcommerceMVC.Controllers
{
    public class HangHoaController : Controller
    {
        private readonly Hshop2023Context db;

        public HangHoaController(Hshop2023Context context)
        {
            db=context;
        }
        public IActionResult Index(int? loai,int? page)
        {
            
            var hangHoas=db.HangHoas.AsQueryable();
            if(loai.HasValue)
            {
                hangHoas=hangHoas.Where(p => p.MaLoai==loai.Value).AsNoTracking().OrderBy(x=>x.TenHh);
            }
            var result =hangHoas.Select(p => new HangHoaVM
            {
                MaHh=p.MaHh,
                TenHH=p.TenHh,
                DonGia=p.DonGia??0,
                Hinh=p.Hinh??"",
                MoTaNgan=p.MoTaDonVi??"",
                TenLoai=p.MaLoaiNavigation.TenLoai,
                
            });
            int pageSize = 9;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            ViewBag.pageSize = hangHoas.Count()/pageSize;
            ViewBag.pagenumber = pageNumber;

            PagedList<HangHoaVM> lst=new PagedList<HangHoaVM>(result, pageNumber, pageSize);
            return View(lst);
        }
        public IActionResult Search(string? query)
        {
            var hangHoas = db.HangHoas.AsQueryable();
            if (query!=null)
            {
                hangHoas = hangHoas.Where(p => p.TenHh.Contains(query));
            }
            var result = hangHoas.Select(p => new HangHoaVM
            {
                MaHh = p.MaHh,
                TenHH = p.TenHh,
                DonGia = p.DonGia ?? 0,
                Hinh = p.Hinh ?? "",
                MoTaNgan = p.MoTaDonVi ?? "",
                TenLoai = p.MaLoaiNavigation.TenLoai
            });
            return View(result);
        }

        public IActionResult Detail(int id)
        {
            var data=db.HangHoas
                .Include(p=>p.MaLoaiNavigation)
                .SingleOrDefault(p=>p.MaHh==id);
            if(data==null)
            {
                TempData["Message"] = $"Không thấy sản phầm có mã {id}";
                return Redirect("/404");
            }
            var result = new ChiTietHangHoaVM
            {
                MaHh = data.MaHh,
                TenHH = data.TenHh,
                DonGia = data.DonGia ?? 0,
                ChiTiet = data.MoTa ?? string.Empty,
                Hinh=data.Hinh??string.Empty,
                MoTaNgan = data.MoTaDonVi ?? string.Empty,
                TenLoai = data.MaLoaiNavigation.TenLoai,
                SoLuongTon=10,//Tính sau
                DiemDanhGia = 5,//Check sau
            };
            return View(result);
        }
    }
}
