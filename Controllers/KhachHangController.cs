
using _3TLPROJECTck.Data;
using _3TLPROJECTck.Helper;
using _3TLPROJECTck.Helpers;
using _3TLPROJECTck.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace _3TLPROJECTck.Controllers
{
    public class KhachHangController : Controller
    {
        private readonly Hshop2023Context db;
        private readonly IMapper _mapper;

        public KhachHangController(Hshop2023Context context, IMapper mapper)
        {
            db = context;
            _mapper = mapper;
        }

        #region Register
        [HttpGet]
        public IActionResult DangKy()
        {
            return View();
        }

        [HttpPost]
        public IActionResult DangKy(RegisterVM model, IFormFile? Hinh)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var khachhang = _mapper.Map<KhachHang>(model);

                    khachhang.RandomKey = MyUtil.GenerateRandomKey();
                    khachhang.MatKhau = model.MatKhau.ToMd5Hash(khachhang.RandomKey);
                    khachhang.HieuLuc = true;
                    khachhang.VaiTro = 0;

                    if (Hinh != null)
                    {
                        khachhang.Hinh = MyUtil.UploadHinh(Hinh, "KhachHang");
                    }

                    db.Add(khachhang);
                    db.SaveChanges();

                    TempData["Success"] = "Đăng ký tài khoản thành công!";

                    return RedirectToAction("DangNhap", "KhachHang");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Đã xảy ra lỗi: " + ex.Message);
                }
            }

            return View(model);
        }
        #endregion

        #region Login
        [HttpGet]
        public IActionResult DangNhap(string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DangNhap(LoginVM model, string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            if (ModelState.IsValid)
            {
                var khachhang = db.KhachHangs.SingleOrDefault(kh => kh.MaKh == model.UserName);

                if (khachhang == null)
                {
                    ModelState.AddModelError("loi", "Tên đăng nhập không tồn tại");
                }
                else if (!khachhang.HieuLuc)
                {
                    ModelState.AddModelError("loi", "Tài khoản không còn hiệu lực. Vui lòng liên hệ Admin");
                }
                else if (khachhang.MatKhau != model.Password.ToMd5Hash(khachhang.RandomKey))
                {
                    ModelState.AddModelError("loi", "Mật khẩu không đúng");
                }
                else
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Email, khachhang.Email ?? ""),
                        new Claim(ClaimTypes.Name, khachhang.HoTen ?? ""),
                        new Claim("CustomerID", khachhang.MaKh),
                        new Claim(MySetting.CLAIM_CUSTOMERID, khachhang.MaKh),
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                    await HttpContext.SignInAsync(claimsPrincipal);
                    TempData["Success"] = "Đăng nhập thành công!";

                    return Url.IsLocalUrl(ReturnUrl)
                        ? Redirect(ReturnUrl)
                        : RedirectToAction("Index", "HangHoa"); // ✅ sửa lỗi RedirectToAction
                }
            }
            ModelState.AddModelError("", "Sai tài khoản hoặc mật khẩu");
            return View(model); // ✅ trả về model để giữ lại dữ liệu đã nhập
        }
        #endregion

        #region Logout
        [Authorize]
        
        public async Task<IActionResult> DangXuat()
        {
            await HttpContext.SignOutAsync();

            TempData["Success"] = "Đăng xuất thành công!";

            return RedirectToAction("DangNhap", "KhachHang");
        }
        #endregion

        [Authorize]
        public IActionResult Profile()
        {
            var maKh = User.FindFirstValue(MySetting.CLAIM_CUSTOMERID);
            var khachhang = db.KhachHangs.SingleOrDefault(kh => kh.MaKh == maKh);
            if (khachhang == null)
            {
                return RedirectToAction("DangNhap");
            }
            return View(khachhang);
        }

    }
}