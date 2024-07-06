
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EcommerceMVC.Areas.Admin.Models.ViewModels
{
    public class HangHoaThemVM
    {
        public int? MaHh { get; set; }

        [DisplayName("Tên Hàng Hóa")]
        public string TenHH { get; set; }
        [DisplayName("Mã Loại")]
        public int MaLoai { get; set; }
        public string? TenLoai { get; set; }
        public string? Hinh { get; set; }
        public double? DonGia { get; set; }
        [DisplayName("Ngày Sản Xuất")]
        public DateTime NgaySx { get; set; }
        [DisplayName("Giảm Giá")]
        public double GiamGia { get; set; }
        [DisplayName("Số Lần Xem")]
        public int SoLanXem { get; set; }
        [DisplayName("Mã Nhà Cung Cấp")]
        public string MaNcc { get; set; }
        [DisplayName("Hình Sản Phẩm")]
        public IFormFile? MyImage { set; get; }
    }
}

