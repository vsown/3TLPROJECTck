using _3TLPROJECTck.Helper;
using _3TLPROJECTck.Models;   // ✅ PHẢI LÀ MODELS

using Microsoft.AspNetCore.Mvc;

namespace _3TLPROJECTck.ViewModels
{
    public class CartViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var cart = HttpContext.Session.Get<List<CartItem>>(MySetting.CART_KEY)
                       ?? new List<CartItem>();

            return View("CartPanel", new CartModel
            {
                Quantity = cart.Sum(p => p.SoLuong),
                Total = cart.Sum(p => p.ThanhTien)
            });
        }
    }
}