using _3TLPROJECTck.Data;
using System;
using System.Collections.Generic;

namespace _3TLPROJECTck.Data;

public partial class HoiDap
{
    public int MaHd { get; set; }

    public string CauHoi { get; set; } = null!;

    public string TraLoi { get; set; } = null!;

    public DateOnly NgayDua { get; set; }

    public string MaNv { get; set; } = null!;

    public virtual NhanVien MaNvNavigation { get; set; } = null!;
}
