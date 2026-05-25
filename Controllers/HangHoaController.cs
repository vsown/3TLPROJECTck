using _3TLPROJECTck.Data;
using _3TLPROJECTck.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _3TLPROJECTck.Controllers
{
    public class HangHoaController : Controller
    {
        private readonly Hshop2023Context db;

        public HangHoaController(Hshop2023Context context)
        {
            db = context;
        }

        public IActionResult Index(int? loai, int page = 1,
    double minGia = 0, double maxGia = 10000000,
    string? size = null, string? mauSac = null, string? sapXep = null)
        {
            int pageSize = 9;

            var hangHoas = db.HangHoas.AsQueryable();

            if (loai.HasValue)
                hangHoas = hangHoas.Where(h => h.MaLoai == loai.Value);

            // Filter giá
            hangHoas = hangHoas.Where(h => h.DonGia >= minGia && h.DonGia <= maxGia);

           

            // Sắp xếp
            hangHoas = sapXep switch
            {
                "name_az" => hangHoas.OrderBy(h => h.TenHh),
                "name_za" => hangHoas.OrderByDescending(h => h.TenHh),
                "gia_tang" => hangHoas.OrderBy(h => h.DonGia),
                "gia_giam" => hangHoas.OrderByDescending(h => h.DonGia),
                _ => hangHoas.OrderByDescending(h => h.NgaySx)
            };

            int totalItems = hangHoas.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var results = hangHoas
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new HangHoaVM
                {
                    MaHh = p.MaHh,
                    TenHh = p.TenHh,
                    Hinh = p.Hinh ?? "",
                    DonGia = p.DonGia,
                    MoTaNgan = p.MoTaDonVi ?? "",
                    TenLoai = p.MaLoaiNavigation.TenLoai ?? ""
                }).ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.Loai = loai;
            ViewBag.MinGia = minGia;
            ViewBag.MaxGia = maxGia;
            ViewBag.Size = size;
            ViewBag.MauSac = mauSac;
            ViewBag.SapXep = sapXep;

            return View(results);
        }

        // ===== TÌM KIẾM =====
        public IActionResult Search(string query)
        {
            var hangHoas = db.HangHoas
                             .Include(p => p.MaLoaiNavigation)
                             .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                hangHoas = hangHoas.Where(p => p.TenHh.Contains(query));
            }

            var results = hangHoas
                .Select(p => new HangHoaVM
                {
                    MaHh = p.MaHh,
                    TenHh = p.TenHh,
                    Hinh = p.Hinh ?? "",
                    DonGia = p.DonGia,
                    MoTaNgan = p.MoTaDonVi ?? "",
                    TenLoai = p.MaLoaiNavigation.TenLoai ?? ""
                })
                .ToList();

            // Thêm ViewBag để _DanhSachHangHoa.cshtml không bị null
            ViewBag.Loai = null;
            ViewBag.MinGia = 0;
            ViewBag.MaxGia = 0;
            ViewBag.SapXep = null;
            ViewBag.CurrentPage = 1;
            ViewBag.TotalPages = 1;

            return View("Index", results); // Dùng chung view Index
        }

        // ===== CHI TIẾT =====
        public IActionResult Detail(int id)
        {
            var data = db.HangHoas
                         .Include(p => p.MaLoaiNavigation)
                         .SingleOrDefault(p => p.MaHh == id);

            if (data == null)
            {
                TempData["Message"] = "Sản phẩm không tồn tại hoặc đã bị xóa";
                return RedirectToAction("Index");
            }

            var result = new ChiTietHangHoaVM
            {
                MaHh = data.MaHh,
                TenHh = data.TenHh,
                Hinh = data.Hinh ?? "",
                DonGia = data.DonGia,
                MoTaNgan = data.MoTaDonVi ?? "",
                TenLoai = data.MaLoaiNavigation.TenLoai ?? "",
                ChiTiet = data.MoTa ?? "",
                DiemDanhGia = 5,
                SoLuongTon = 10
            };

            return View(result);
        }
    }
}