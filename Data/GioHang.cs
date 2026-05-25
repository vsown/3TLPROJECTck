using _3TLPROJECTck.Data;
using System;
using System.Collections.Generic;

namespace _3TLPROJECTck.Data;

public partial class GioHang
{
    public int Id { get; set; }

    public int MaHh { get; set; }

    public double DonGia { get; set; }

    public int SoLuong { get; set; }

    public double GiamGia { get; set; }

    public DateTime Ngay { get; set; }

    public string SessionId { get; set; } = null!;

    public string? MaKh { get; set; }
}
