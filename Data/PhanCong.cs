using _3TLPROJECTck.Data;
using System;
using System.Collections.Generic;

namespace _3TLPROJECTck.Data;

public partial class PhanCong
{
    public int MaPc { get; set; }

    public string MaNv { get; set; } = null!;

    public string MaPb { get; set; } = null!;

    public DateTime? NgayPc { get; set; }

    public bool? HieuLuc { get; set; }

    public virtual NhanVien MaNvNavigation { get; set; } = null!;

    public virtual PhongBan MaPbNavigation { get; set; } = null!;
}
