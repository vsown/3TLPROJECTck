using System.ComponentModel.DataAnnotations;

namespace _3TLPROJECTck.ViewModels
{
    public class RegisterVM
    {
        [Display(Name = "Tên Đăng Nhập")]
        [Required(ErrorMessage = "*")]
        [MaxLength(20, ErrorMessage = "Tối đa 20 ký tự")]
        public string MaKh { get; set; }

        [Display(Name = "Họ Tên")]
        [Required(ErrorMessage = "*")]
        [MaxLength(50, ErrorMessage = "Tối đa 50 ký tự")]
        public string HoTen { get; set; }

        public bool GioiTinh { get; set; } = true;

        [Display(Name = "Ngày Sinh")]
        [DataType(DataType.Date)]
        public DateTime? NgaySinh { get; set; }

        [MaxLength(60, ErrorMessage = "Tối đa 60 ký tự")]

        [Required(ErrorMessage = "Không được để trống địa chỉ")]
        public string? DiaChi { get; set; }

        [Required(ErrorMessage = "Không được để trống số điện thoại")]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Số điện thoại phải gồm 10 số và bắt đầu bằng 0")]
        public string? DienThoai { get; set; }


        [Required(ErrorMessage = "Không được để trống email")]
        [RegularExpression(
    @"^[a-zA-Z0-9._%+-]+@gmail\.com$",
    ErrorMessage = "Email phải đúng định dạng và là gmail.com"
)]
        public string? Email { get; set; }

        [Display(Name = "Mật Khẩu")]
        [Required(ErrorMessage = "*")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Mật khẩu tối thiểu 6 ký tự")]
        public string? MatKhau { get; set; }

        [Display(Name = "Xác Nhận Mật Khẩu")]
        [Required(ErrorMessage = "*")]
        [DataType(DataType.Password)]
        [Compare("MatKhau", ErrorMessage = "Mật khẩu không khớp")]
        public string? XacNhanMatKhau { get; set; }

        // Lưu tên file ảnh vào DB
        public string? Hinh { get; set; }

        // Nhận file upload từ form
        [Display(Name = "Hình Đại Diện")]
        [DataType(DataType.Upload)]
        public IFormFile? HinhFile { get; set; }
    }
}