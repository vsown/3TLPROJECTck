
using _3TLPROJECTck.Data;
using _3TLPROJECTck.ViewModels;
using Microsoft.AspNetCore.Mvc;


namespace _3TLPROJECTck.ViewComponents
{
    public class MenuLoaiViewComponent : ViewComponent
    {
        private readonly Hshop2023Context db;
        public MenuLoaiViewComponent(Hshop2023Context context) => db = context;
        public IViewComponentResult Invoke()
        {
            var data = db.Loais.Select(lo => new MenuLoaiVM
            {
                MaLoai = lo.MaLoai,
                TenLoai = lo.TenLoai,
                Soluong = lo.HangHoas.Count
            });
            return View(data);

        }
    }
}
