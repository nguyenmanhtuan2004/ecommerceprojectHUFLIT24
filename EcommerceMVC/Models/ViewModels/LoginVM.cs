﻿using System.ComponentModel.DataAnnotations;

namespace EcommerceMVC.Models.ViewModels
{
    public class LoginVM
    {
        [Display(Name ="Tên đăng nhập")]
        [Required(ErrorMessage ="Chưa nhập tên")]
        [MaxLength(20,ErrorMessage ="Tối đa 20 kí tự")]
        public string UserName { get; set; }
        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "Chưa nhập mật khẩu")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string ReturnUrl { get; set; } = "/";
    }
}
