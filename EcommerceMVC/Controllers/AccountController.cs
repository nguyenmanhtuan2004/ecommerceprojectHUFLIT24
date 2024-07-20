using AutoMapper;
using EcommerceMVC.Data;
using EcommerceMVC.Helpers;
using EcommerceMVC.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EcommerceMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly Hshop2023Context db;
        private readonly IMapper _mapper;
        private UserManager<IdentityUser> userManager;
        private SignInManager<IdentityUser> signInManager;

        //tư tưởng net core:muốn xài gì thì phải inject nó vô
        public AccountController(Hshop2023Context context,IMapper mapper, UserManager<IdentityUser> userMgr,
        SignInManager<IdentityUser> signInMgr) 
        {
            db= context;
            _mapper = mapper;
            userManager = userMgr;
            signInManager = signInMgr;
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
        public async Task<IActionResult> DangKy(RegisterVM model)//Hình truyền riêng nên phải có tham số Hinh
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

                   
                    db.Add(khachHang);
                    db.SaveChanges();//chức năng thêm khách hàng
                    IdentityUser newUser = new() { UserName = model.MaKh, Email = model.Email };
                    IdentityResult result = await userManager.CreateAsync(newUser, model.MatKhau);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(newUser, "Customer");
                        return RedirectToAction("Index", "HangHoa");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                   /* return RedirectToAction("Index", "HangHoa");*/
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
        public ViewResult Login(string returnUrl)
        {
            return View(new LoginVM
            {
                ReturnUrl = returnUrl
            });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM loginModel)
        {
            if (ModelState.IsValid)
            {
                IdentityUser user =
                await userManager.FindByNameAsync(loginModel.UserName);
                if (user != null)
                {
                      
                    await signInManager.SignOutAsync();
                    if ((await signInManager.PasswordSignInAsync(user,
                    loginModel.Password, false, false)).Succeeded)
                    {
                        if (!user.UserName.Equals("Admin"))
                        {
                            return RedirectToAction("Index", "HangHoa");
                        }
                        else
                        return Redirect("/Admin");
                    }
                }
                ModelState.AddModelError("", "Invalid name or password");
            }
            return View(loginModel);
        }
        [Authorize]
        public async Task<RedirectResult> DangXuat(string returnUrl = "/")
        {
            await signInManager.SignOutAsync();
            return Redirect(returnUrl);
        }

        #endregion
        [Authorize]
        //Thuộc tính này cho biết rằng chỉ những người dùng đã xác thực mới có thể truy cập phương thức hành động này.
        public IActionResult Profile()
        {
            return View();
        }

    }
}
