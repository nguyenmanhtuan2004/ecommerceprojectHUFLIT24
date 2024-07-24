using System;
using System.Collections.Generic;

namespace EcommerceMVC.Data
{
    public partial class NhanVien
    {
        public NhanVien()
        {
            HoaDons = new HashSet<HoaDon>();
        }

        public string MaNv { get; set; } = null!;
        public string HoTen { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? MatKhau { get; set; }

        public virtual ICollection<HoaDon> HoaDons { get; set; }
    }
}
