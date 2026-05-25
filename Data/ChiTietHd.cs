using _3TLPROJECTck.Data;
using System;
using System.Collections.Generic;

namespace _3TLPROJECTck.Data;

public partial class ChiTietHd
{
    public int MaCt { get; set; }

    public int MaHd { get; set; }

    public int MaHh { get; set; }

    public decimal DonGia { get; set; }

    public int SoLuong { get; set; }

    public double GiamGia { get; set; }

    public DateTime? Ngay { get; set; }

    public virtual HoaDon MaHdNavigation { get; set; } = null!;

    public virtual HangHoa MaHhNavigation { get; set; } = null!;

}
