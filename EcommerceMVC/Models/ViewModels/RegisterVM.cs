using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EcommerceMVC.Models.ViewModels
{
    public class RegisterVM
    {

        //MaKH,MatKhau,HoTen,GioiTinh,NgaySinh?,DiaChi,DienThoai,Email,Hinh?
        //Khi lấy dữ liệu cho người dùng, không nên lấy trực tiếp EntityModel, mà phải định nghĩa các ViewModel
        [Key]
        [DisplayName("Tên Đăng nhập ")]
        [Required(ErrorMessage ="*")]
        [MaxLength(20,ErrorMessage ="Tối đa 20 kí tự")]
        public string MaKh { get; set; }
        
        [Required(ErrorMessage ="*")]
        [Display(Name = "Mật khẩu")]
        [DataType(DataType.Password)]
        public string MatKhau { get; set; }
        [Required(ErrorMessage ="*")]
        [Display(Name = "Họ tên")]
        [MaxLength(50,ErrorMessage = "Tối đa 50 kí tự")]
        public string HoTen { get; set; }
        [Display(Name = "Giới tính")]
        public bool GioiTinh { get; set; } = true;
        [DataType(DataType.Date)]
        [Display(Name ="Ngày sinh")]
        public DateTime? NgaySinh { get; set; }
        [MaxLength(60,ErrorMessage ="Tối đa 60 kí tự")]
        [Display(Name ="Địa chỉ")]
        public string DiaChi { get; set; }
        [MaxLength(24, ErrorMessage = "Tối đa 24 kí tự")]
        [Display(Name = "Điện thoại")]
        [RegularExpression(@"0[9875]\d{8}",ErrorMessage ="Chưa đúng định dạng di động Việt Nam")]
        public string DienThoai { get; set; }
        [EmailAddress(ErrorMessage ="Chưa đúng định dạng")]
        public string Email { get; set; }
        public string? Hinh { get; set; }
    }
}
