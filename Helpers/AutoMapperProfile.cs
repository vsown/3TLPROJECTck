
using _3TLPROJECTck.Data;
using _3TLPROJECTck.ViewModels;
using AutoMapper;

namespace _3TLPROJECTck.Helper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<RegisterVM, KhachHang>()
                .ForMember(dest => dest.MatKhau, opt => opt.Ignore()) // hash sau
                .ForMember(dest => dest.Hinh, opt => opt.Ignore());   // upload sau
        }
    }

}