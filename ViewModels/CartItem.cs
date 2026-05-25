namespace _3TLPROJECTck.ViewModels
{
    public class CartItem
    {
        public int MaHh { get; set; }
        public string Hinh { get; set; } = null!;
        public string TenHh { get; set; } = null!;
        public decimal DonGia { get; set; }   // ✅ double → decimal
        public int SoLuong { get; set; }
        public decimal ThanhTien => DonGia * SoLuong;  // ✅ double → decimal
    }
}
