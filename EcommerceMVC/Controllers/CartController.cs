using EcommerceMVC.Data;
using Microsoft.AspNetCore.Mvc;
using EcommerceMVC.Models;
using EcommerceMVC.Models.ViewModels;
using EcommerceMVC.Helpers;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace EcommerceMVC.Controllers
{
    public class CartController : Controller
    {
        //Đưa hàng vào giỏ => cần làm việc với database
        private readonly Hshop2023Context db;
        public CartController(Hshop2023Context context)
        {
            db = context;
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
        public IActionResult AddToCart(int id, int quantity=1)
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
        public IActionResult RemoveCart(int id)
        { 
            //Tìm kiếm sản phẩm cần xóa(tham chiếu sản phẩm cần xóa)
            var gioHang=Cart;
            var item = gioHang.SingleOrDefault(p=>p.MaHh == id);
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
            if(Cart.Count==0)
            {
                return Redirect("/");
            }    

            return View(Cart);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Checkout(CheckoutVM model)
        {
            if (Cart.Count == 0)
            {
                //HttpContext.User đại diện cho người dùng hiện đang được xác thực
                var customerId =HttpContext.User.Claims.SingleOrDefault(p => p.Type == MySetting.CLAIM_CUSTOMERID).Value;
                var khachHang = new KhachHang();
                if(model.GiongKhachHang)
                {
                    khachHang=db.KhachHangs.SingleOrDefault(kh=>kh.MaKh==customerId);
                }    
                var hoaDon=new HoaDon
                {
                    MaKh=customerId,
                    HoTen=model.HoTen?? khachHang.HoTen,//nếu null thì mặc định lấy họ tên KhachHang
                    DiaChi=model.DiaChi??khachHang.DiaChi,// nếu null thì mặc định lấy địa chỉ KhachHang
                    DienThoai = model.DienThoai ?? khachHang.DienThoai,
                    NgayDat=DateTime.Now,
                    CachThanhToan="COD",
                    CachVanChuyen="GRAB",
                    MaTrangThai=0,
                    GhiChu=model.GhiChu
                };
                
                try
                {
                    db.Database.BeginTransaction();
                    db.Add(hoaDon);//
                    db.SaveChanges();

                    //chi tiết hóa đơn
                    var cthd=new List<ChiTietHd>();
                    foreach(var item in Cart)
                    {
                        cthd.Add(new ChiTietHd
                        {
                            MaHd=hoaDon.MaHd,
                            SoLuong=item.SoLuong,
                            DonGia=item.DonGia,
                            MaHh=item.MaHh,
                            GiamGia=0
                        });
                    }
                    db.SaveChanges();
                    HttpContext.Session.Set<List<CartItem>>(MySetting.CART_KEY,new List<CartItem>());//thiết lập session là List rỗng sau khi thanh toán
                    return View("Success");
                }
                catch (Exception ex)
                {
                    db.Database.CommitTransaction();
                }

            }

            return View(Cart);
        }
    }
}
