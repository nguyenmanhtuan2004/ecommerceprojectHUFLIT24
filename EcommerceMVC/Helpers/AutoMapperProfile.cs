using AutoMapper;
using EcommerceMVC.Data;
using EcommerceMVC.Models.ViewModels;

namespace EcommerceMVC.Helpers
{
    public class AutoMapperProfile:Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<RegisterVM, KhachHang>();
                /*.ForMember(kh=>kh.HoTen,option=>option.MapFrom(RegisterVM=>RegisterVM.HoTen))
                .ReverseMap();//map 2 chiều*/
            //cột cùng tên sẽ tự động map qua
            //những cột khác tên thì cần chỉ rõ
        }
    }
}
