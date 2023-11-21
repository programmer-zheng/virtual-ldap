using AutoMapper;
using ContactsSync.Application.OpenPlatformProvider;
using Senparc.Weixin.Work.AdvancedAPIs.MailList;

namespace ContactsSync.Application.WeWork;

public class WeWorkMapperProfile : Profile
{
    public WeWorkMapperProfile()
    {
        CreateMap<DepartmentList, PlatformDepartmentDto>()
            .ForMember(dst => dst.DepartmentId, opt => opt.MapFrom(src => src.id))
            .ForMember(dst => dst.DeptName, opt => opt.MapFrom(src => src.name))
            .ForMember(dst => dst.ParentId, opt => opt.MapFrom(src => src.parentid))
            ;

        CreateMap<GetMemberResult, PlatformDeptUserDto>()
            .ForMember(dst => dst.UserId, opt => opt.MapFrom(src => src.userid))
            .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.name))
            .ForMember(dst => dst.Avatar, opt => opt.MapFrom(src => src.avatar))
            .ForMember(dst => dst.Email, opt => opt.MapFrom(src => src.email))
            .ForMember(dst => dst.BizEmail, opt => opt.MapFrom(src => src.biz_mail))
            .ForMember(dst => dst.IsActivated, opt => opt.MapFrom(src => src.status == 1))
            ;
    }
}