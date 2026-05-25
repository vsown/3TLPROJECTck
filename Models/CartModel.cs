namespace _3TLPROJECTck.Models
{
    public class CartModel
    {
        public int Quantity { get; set; }
        public decimal Total { get; set; }  // ✅ double → decimal
    }
}
