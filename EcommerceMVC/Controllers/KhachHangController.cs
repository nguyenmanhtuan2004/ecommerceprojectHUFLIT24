using AutoMapper;
using EcommerceMVC.Data;
using EcommerceMVC.Helpers;
using EcommerceMVC.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EcommerceMVC.Controllers
{
    public class KhachHangController : Controller
    {
        private readonly Hshop2023Context db;
        private readonly IMapper _mapper;
        //tư tưởng net core:muốn xài gì thì phải inject nó vô
        public KhachHangController(Hshop2023Context context,IMapper mapper) 
        {
            db= context;
            _mapper = mapper;
        }
        #region Register
        [HttpGet]
        public IActionResult DangKy()
        {

            return View();
        }
        //Randomkey:Hệ thống tự sinh ngẫu nhiên khi đăng ký, đổi mật khẩu
        //matkhau trong db:hash(matkhau người dùng nhập+salt key/Randomkey)
        [HttpPost]
        public IActionResult DangKy(RegisterVM model, IFormFile Hinh)//Hình truyền riêng nên phải có tham số Hinh
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var khachHang = _mapper.Map<KhachHang>(model);//map cái model sang kiểu KhachHang
                    khachHang.RandomKey = MyUtil.GenerateRandomKey();//randomkey tự động sinh ra
                    khachHang.MatKhau = model.MatKhau.ToMd5Hash(khachHang.RandomKey);
                    khachHang.HieuLuc = true;//sẽ cử lí khi dùng mail để active
                    khachHang.VaiTro = 0;

                    if (Hinh != null)
                    {
                        khachHang.Hinh = MyUtil.UploadHinh(Hinh, "KhachHang");
                    }
                    db.Add(khachHang);
                    db.SaveChanges();//chức năng thêm khách hàng
                    return RedirectToAction("Index", "HangHoa");
                }
                catch (Exception ex)
                {
                    var mess = $"{ex.Message} shh";
                }

            }
            return View();
        }
        #endregion

        #region Login
        [HttpGet]
        public IActionResult DangNhap(string? ReturnUrl)//chạy chính xác trang đăng nhập thì cho vô luôn
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> DangNhap(LoginVM model,string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            if (ModelState.IsValid)
            {
                var khachHang=db.KhachHangs.SingleOrDefault(kh=>kh.MaKh==model.UserName);
                if (khachHang == null)
                {
                    ModelState.AddModelError("loi", "không có khách hàng này");

                }
                else 
                {
                    if (!khachHang.HieuLuc)
                    {
                        ModelState.AddModelError("loi", "Tài khoản đã bị khóa. Vui lòng liên hệ Admin");
                    }
                    else {
                        if(khachHang.MatKhau!=model.Password.ToMd5Hash(khachHang.RandomKey))
                        {
                            ModelState.AddModelError("loi", "Sai thông tin đăng nhập");
                        }    
                        else
                        {
                            //phần này đọc document phần Authentication asp.net core của microsoft
                            //Khai báo xác thực (Authentication) người dùng
                            var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Email,khachHang.Email),//lưu trữ địa chỉ email người dùng
                                new Claim(ClaimTypes.Name,khachHang.HoTen),
                                new Claim(MySetting.CLAIM_CUSTOMERID,khachHang.MaKh),//được dùng để xác định khách hàng
                                //claim - role động  (phân quyền), thay đổi tùy theo ngữ cảnh
                                new Claim(ClaimTypes.Role,"Customer")//gán vai trò "Customer"

                            };
                            var claimsIdentity=new ClaimsIdentity(claims,CookieAuthenticationDefaults.AuthenticationScheme);
                            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);//đại diện cho người dùng đã được xác định và chứa tất cả các claim của họ.

                            await HttpContext.SignInAsync(claimsPrincipal);

                            if (Url.IsLocalUrl(ReturnUrl))//đăng nhập thành công
                            {
                                return Redirect(ReturnUrl);//chuyển tới trang yêu cầu trước đó
                            }
                            else
                            {
                                return Redirect("/");
                            }
                        }    
                    }
                }
            }
            return View();
        }
        #endregion
        [Authorize]
        //Thuộc tính này cho biết rằng chỉ những người dùng đã xác thực mới có thể truy cập phương thức hành động này.
        public IActionResult Profile()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> DangXuat()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }
    }
}
