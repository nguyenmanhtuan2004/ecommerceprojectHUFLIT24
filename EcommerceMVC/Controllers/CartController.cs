using EcommerceMVC.Data;
using Microsoft.AspNetCore.Mvc;
using EcommerceMVC.Models;
using EcommerceMVC.Models.ViewModels;
using EcommerceMVC.Helpers;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using ECommerceMVC.Helpers;
using Microsoft.EntityFrameworkCore;

namespace EcommerceMVC.Controllers
{
	public class CartController : Controller
	{
		//Đưa hàng vào giỏ => cần làm việc với database
		private readonly Hshop2023Context db;
        private readonly PaypalClient _paypalClient;

        private UserManager<IdentityUser> userManager;
		private SignInManager<IdentityUser> signInManager;
		public CartController(Hshop2023Context context, UserManager<IdentityUser> userMgr,
		SignInManager<IdentityUser> signInMgr, PaypalClient paypalClient)
		{
			db = context;
			userManager = userMgr;
			signInManager = signInMgr;
			_paypalClient = paypalClient;
		}

		public List<CartItem> Cart
		{
			get
			{
				return HttpContext.Session.Get<List<CartItem>>(MySetting.CART_KEY) ?? new List<CartItem>();
			}
		}
		public IActionResult Index()
		{
			return View(Cart);
		}
		public IActionResult AddToCart(int id, int quantity = 1)
		{
			var gioHang = Cart;
			var item = gioHang.SingleOrDefault(p => p.MaHh == id);
			if (item == null) //nếu mặt hàng đó chưa có trong giỏ thì thêm vào
			{
				var hangHoa = db.HangHoas.SingleOrDefault(p => p.MaHh == id);
				if (hangHoa == null)
				{
					TempData["Message"] = $"Không tìm thấy hàng hóa có mã {id}";
					return Redirect("/404");
				}
				item = new CartItem
				{
					MaHh = hangHoa.MaHh,
					TenHH = hangHoa.TenHh,
					DonGia = hangHoa.DonGia ?? 0,
					Hinh = hangHoa.Hinh ?? string.Empty,
					SoLuong = quantity
				};
				gioHang.Add(item);
			}
			else//nếu mặt hàng đã có trong giỏ rồi thì thêm số lượng
			{
				item.SoLuong += quantity;
			}
			HttpContext.Session.Set(MySetting.CART_KEY, gioHang);
			return RedirectToAction("Index");//chuyển tới index để hiển thị
		}

		public IActionResult Save(int id,int quantity = 1)
		{
            var gioHang = Cart;
            gioHang.ForEach(cart => {
                if (cart.MaHh==id)
                {
                    cart.SoLuong= quantity; // Giảm giá 10% cho các sản phẩm có số lượng lớn hơn 1
                }
            });
            HttpContext.Session.Set(MySetting.CART_KEY, gioHang);
            
            return RedirectToAction("Index");
        }
		public IActionResult RemoveCart(int id)
		{
			//Tìm kiếm sản phẩm cần xóa(tham chiếu sản phẩm cần xóa)
			var gioHang = Cart;
			var item = gioHang.SingleOrDefault(p => p.MaHh == id);
			if (item != null)
			{
				gioHang.Remove(item);
				HttpContext.Session.Set(MySetting.CART_KEY, gioHang);
			}
			return RedirectToAction("Index");//vẫn xử lí trên server
		}
		[Authorize]
		[HttpGet]
		public IActionResult Checkout()
		{
			if (Cart.Count == 0)
			{
				return Redirect("/");
			}

			return View(Cart);
		}

		[Authorize]
		[HttpPost]
		public IActionResult Checkout(CheckoutVM model)
		{
			if (ModelState.IsValid)
			{
				var customerId = User.Identity.Name;
				var khachHang = new KhachHang();
				var hoadon = new HoaDon();
				if (model.GiongKhachHang)
				{
					if(customerId==null)
					{
                        return View(Cart);
                    }	
					khachHang = db.KhachHangs.SingleOrDefault(kh => kh.MaKh == customerId);
				}

				hoadon = new HoaDon
				{
					MaKh = customerId,
					HoTen = model.HoTen ?? khachHang.HoTen,
					DiaChi = model.DiaChi ?? khachHang.DiaChi,
					DienThoai = model.DienThoai ?? khachHang.DienThoai,
					NgayDat = DateTime.Now,
					CachThanhToan = "COD",
					CachVanChuyen = "GRAB",
					MaTrangThai = 0,
					GhiChu = model.GhiChu
				};

				db.Database.BeginTransaction();
				try
				{
					db.Database.CommitTransaction();
					db.Add(hoadon);
					db.SaveChanges();

					var cthds = new List<ChiTietHd>();
					foreach (var item in Cart)
					{
						cthds.Add(new ChiTietHd
						{
							MaHd = hoadon.MaHd,
							SoLuong = item.SoLuong,
							DonGia = item.DonGia,
							MaHh = item.MaHh,
							GiamGia = 0
						});
					}
					db.AddRange(cthds);
					db.SaveChanges();

					HttpContext.Session.Set<List<CartItem>>(MySetting.CART_KEY, new List<CartItem>());

					return View("Success");
				}
				catch
				{
					db.Database.RollbackTransaction();
				}
			}

			return View(Cart);
		}

        [Authorize]
        [HttpGet]
        public IActionResult LichSuMuaHang()
		{
            var customerId = User.Identity.Name;
            var chiTietHds = db.ChiTietHds
           .Include(ct => ct.MaHhNavigation)
           .Include(ct => ct.MaHdNavigation)
		   .Where(ct=>ct.MaHdNavigation.MaKh==customerId)
           .ToList();
            return View(chiTietHds);
		}
        
    }
}