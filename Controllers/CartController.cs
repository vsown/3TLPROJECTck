using _3TLPROJECTck.Data;
using _3TLPROJECTck.Helper;
using _3TLPROJECTck.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace _3TLPROJECTck.Controllers
{
    public class CartController : Controller
    {
        private readonly PaypalClient _paypalClient;
        private readonly Hshop2023Context db;

        private static readonly Dictionary<string, int> ValidCoupons = new()
        {
            { "GIAM10", 10 },
            { "GIAM20", 20 },
            { "SALE50", 50 },
        };

        private const string COUPON_KEY = "AppliedCoupon";
        private const string COUPON_PCT_KEY = "CouponPercent";

        public CartController(Hshop2023Context context, PaypalClient paypalClient)
        {
            _paypalClient = paypalClient;
            db = context;
        }

        public List<CartItem> Cart =>
            HttpContext.Session.Get<List<CartItem>>(MySetting.CART_KEY) ?? new List<CartItem>();

        private int CurrentDiscountPercent =>
            int.TryParse(HttpContext.Session.GetString(COUPON_PCT_KEY), out var pct) ? pct : 0;

        // =====================================================
        // GIO HANG
        // =====================================================
        public IActionResult Index()
        {
            SetDiscountViewBag();
            return View(Cart);
        }

        public IActionResult AddToCart(int id)
        {
            var hanghoa = db.HangHoas.SingleOrDefault(p => p.MaHh == id);
            if (hanghoa == null)
            {
                TempData["Message"] = "San pham khong ton tai";
                return RedirectToAction("Index", "HangHoa");
            }

            var cart = Cart;
            var item = cart.SingleOrDefault(p => p.MaHh == id);

            if (item == null)
            {
                cart.Add(new CartItem
                {
                    MaHh = hanghoa.MaHh,
                    TenHh = hanghoa.TenHh ?? "",
                    Hinh = hanghoa.Hinh ?? "",
                    DonGia = (decimal)hanghoa.DonGia,
                    SoLuong = 1
                });
            }
            else
            {
                item.SoLuong++;
            }

            HttpContext.Session.Set(MySetting.CART_KEY, cart);
            return RedirectToAction("Index");
        }

        public IActionResult Increase(int id)
        {
            var gioHang = Cart;
            var item = gioHang.SingleOrDefault(p => p.MaHh == id);
            if (item != null) item.SoLuong++;
            HttpContext.Session.Set(MySetting.CART_KEY, gioHang);
            return RedirectToAction("Index");
        }

        public IActionResult Decrease(int id)
        {
            var gioHang = Cart;
            var item = gioHang.SingleOrDefault(p => p.MaHh == id);
            if (item != null)
            {
                item.SoLuong--;
                if (item.SoLuong <= 0) gioHang.Remove(item);
            }
            HttpContext.Session.Set(MySetting.CART_KEY, gioHang);
            return RedirectToAction("Index");
        }

        public IActionResult RemoveCart(int id)
        {
            var gioHang = Cart;
            var item = gioHang.SingleOrDefault(p => p.MaHh == id);
            if (item != null) gioHang.Remove(item);
            HttpContext.Session.Set(MySetting.CART_KEY, gioHang);
            return RedirectToAction("Index");
        }

        // =====================================================
        // COUPON - redirect ve Index
        // =====================================================
        [HttpPost]
        public IActionResult ApplyCoupon(string couponCode)
        {
            if (!string.IsNullOrEmpty(couponCode))
            {
                couponCode = couponCode.ToUpper();
                if (ValidCoupons.ContainsKey(couponCode))
                {
                    HttpContext.Session.SetString(COUPON_KEY, couponCode);
                    HttpContext.Session.SetString(COUPON_PCT_KEY, ValidCoupons[couponCode].ToString());
                }
            }
            return RedirectToAction("Index");
        }

        // =====================================================
        // COUPON AJAX - tra JSON
        // =====================================================
        [HttpPost]
        public IActionResult ApplyCouponAjax(string couponCode)
        {
            if (string.IsNullOrEmpty(couponCode))
                return Json(new { success = false, message = "Vui long nhap ma coupon." });

            couponCode = couponCode.ToUpper().Trim();

            if (!ValidCoupons.ContainsKey(couponCode))
                return Json(new { success = false, message = "Ma coupon khong hop le hoac da het han." });

            HttpContext.Session.SetString(COUPON_KEY, couponCode);
            HttpContext.Session.SetString(COUPON_PCT_KEY, ValidCoupons[couponCode].ToString());

            decimal subtotal = (decimal)Cart.Sum(x => x.ThanhTien);
            int pct = ValidCoupons[couponCode];
            decimal discount = subtotal * pct / 100m;
            decimal finalTotal = subtotal - discount;

            return Json(new
            {
                success = true,
                couponCode = couponCode,
                discountPercent = pct,
                discountAmount = discount,
                finalTotal = finalTotal
            });
        }

        public IActionResult RemoveCoupon()
        {
            HttpContext.Session.Remove(COUPON_KEY);
            HttpContext.Session.Remove(COUPON_PCT_KEY);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult RemoveCouponAjax()
        {
            HttpContext.Session.Remove(COUPON_KEY);
            HttpContext.Session.Remove(COUPON_PCT_KEY);
            return Json(new { success = true });
        }

        // =====================================================
        // CHECKOUT - GET
        // =====================================================
        [HttpGet]
        public IActionResult Checkout()
        {
            if (!User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("DangNhap", "KhachHang",
                    new { ReturnUrl = Url.Action("Checkout", "Cart") });
            }

            var cart = HttpContext.Session.Get<List<CartItem>>(MySetting.CART_KEY)
                       ?? new List<CartItem>();

            if (cart.Count == 0)
                return RedirectToAction("Index");

            SetDiscountViewBag();

            // Truyen thong tin KH xuong view de JS dien vao form khi tick checkbox
            var customerId = HttpContext.User.Claims
                .FirstOrDefault(p => p.Type == MySetting.CLAIM_CUSTOMERID)?.Value;
            var khachHang = db.KhachHangs.SingleOrDefault(kh => kh.MaKh == customerId);
            if (khachHang != null)
            {
                ViewBag.KhHoTen = khachHang.HoTen ?? "";
                ViewBag.KhDiaChi = khachHang.DiaChi ?? "";
                ViewBag.KhDienThoai = khachHang.DienThoai ?? "";
            }
            else
            {
                ViewBag.KhHoTen = "";
                ViewBag.KhDiaChi = "";
                ViewBag.KhDienThoai = "";
            }

            return View(cart);
        }

        // =====================================================
        // CHECKOUT - POST
        // =====================================================
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Checkout(CheckoutVM model)
        {
            // Xoa ModelState cua cac field se xu ly thu cong
            ModelState.Remove("GiongKhachHang");
            ModelState.Remove("HoTen");
            ModelState.Remove("DiaChi");
            ModelState.Remove("DienThoai");

            var customerId = HttpContext.User.Claims
                .FirstOrDefault(p => p.Type == MySetting.CLAIM_CUSTOMERID)?.Value;

            // Neu tick "Giong thong tin khach hang", lay thong tin tu DB
            if (model.GiongKhachHang)
            {
                var khachHang = db.KhachHangs.SingleOrDefault(kh => kh.MaKh == customerId);

                if (khachHang != null)
                {
                    model.HoTen = khachHang.HoTen;
                    model.DiaChi = khachHang.DiaChi;
                    model.DienThoai = khachHang.DienThoai;
                }
            }

            // Kiem tra ModelState
            if (!ModelState.IsValid)
            {
                SetDiscountViewBag();

                TempData["CheckoutError"] = "Vui lòng kiểm tra lại thông tin đặt hàng!";

                return View(Cart);
            }

            // Kiem tra thong tin nhan hang
            if (string.IsNullOrWhiteSpace(model.HoTen) ||
                string.IsNullOrWhiteSpace(model.DiaChi) ||
                string.IsNullOrWhiteSpace(model.DienThoai))
            {
                TempData["CheckoutError"] = "Vui lòng nhập đầy đủ thông tin nhận hàng!";

                SetDiscountViewBag();

                return View(Cart);
            }

            int discPct = CurrentDiscountPercent;

            var hoadon = new HoaDon
            {
                MaKh = customerId,
                HoTen = model.HoTen,
                DiaChi = model.DiaChi,
                DienThoai = model.DienThoai,
                NgayDat = DateTime.Now,
                CachThanhToan = "COD",
                CachVanChuyen = "GRAB",
                MaTrangThai = 1,
                GhiChu = model.GhiChu
            };

            using var transaction = db.Database.BeginTransaction();

            try
            {
                // Luu hoa don
                db.HoaDons.Add(hoadon);
                db.SaveChanges();

                // Luu chi tiet hoa don
                var cthd = Cart.Select(item => new ChiTietHd
                {
                    MaHd = hoadon.MaHd,
                    MaHh = item.MaHh,
                    SoLuong = item.SoLuong,
                    DonGia = item.DonGia,
                    GiamGia = discPct
                }).ToList();

                db.ChiTietHds.AddRange(cthd);

                db.SaveChanges();

                transaction.Commit();

                // Xoa gio hang + coupon
                HttpContext.Session.Remove(MySetting.CART_KEY);
                HttpContext.Session.Remove(COUPON_KEY);
                HttpContext.Session.Remove(COUPON_PCT_KEY);

                // Truyen thong tin don hang sang Success qua TempData (phai la string, khong duoc decimal)
                decimal tongTien = cthd.Sum(x => x.DonGia * x.SoLuong);
                decimal soTienGiam = tongTien * discPct / 100m;
                decimal thanhTien = tongTien - soTienGiam;

                TempData["HoTen"] = hoadon.HoTen ?? "";
                TempData["DiaChi"] = hoadon.DiaChi ?? "";
                TempData["DienThoai"] = hoadon.DienThoai ?? "";
                TempData["NgayDat"] = hoadon.NgayDat.ToString("dd/MM/yyyy HH:mm");
                TempData["ThanhToan"] = hoadon.CachThanhToan ?? "COD";
                TempData["VanChuyen"] = hoadon.CachVanChuyen ?? "GRAB";
                TempData["TongTien"] = thanhTien.ToString("N0");

                // CHUYEN TRANG SUCCESS
                return RedirectToAction("Success", "Cart");
            }
            catch (Exception ex)
            {
                db.Database.RollbackTransaction();

                var innerMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : ex.Message;

                ModelState.AddModelError("", "Lỗi khi đặt hàng: " + innerMessage);

                SetDiscountViewBag();

                return View(Cart);
            }
        }
        

        // =====================================================
        // SUCCESS
        // =====================================================
        [HttpGet]
        public IActionResult Success()
        {
            return View();
        }

        // =====================================================
        // VIEWBAG GIAM GIA
        // =====================================================
        private void SetDiscountViewBag()
        {
            string code = HttpContext.Session.GetString(COUPON_KEY) ?? "";
            int pct = CurrentDiscountPercent;

            decimal subtotal = (decimal)Cart.Sum(x => x.ThanhTien);
            decimal discount = subtotal * pct / 100m;

            ViewBag.CouponCode = code;
            ViewBag.DiscountPercent = pct;
            ViewBag.DiscountAmount = discount;
            ViewBag.FinalTotal = subtotal - discount;
        }
    }
}