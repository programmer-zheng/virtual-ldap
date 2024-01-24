using AutoMapper;
using ContactsSync.Application.Contracts.OpenPlatformProvider;
using DingTalk.Api.Response;

namespace ContactsSync.Application.DingDing;

public class DingDingMapperProfile : Profile
{
    public DingDingMapperProfile()
    {
        CreateMap<OapiV2DepartmentListsubResponse.DeptBaseResponseDomain, PlatformDepartmentDto>()
            .ForMember(dst => dst.DepartmentId, opt => opt.MapFrom(src => src.DeptId))
            .ForMember(dst => dst.DeptName, opt => opt.MapFrom(src => src.Name))
            .ForMember(dst => dst.ParentId, opt => opt.MapFrom(src => src.ParentId))
            ;


        CreateMap<OapiV2UserListResponse.ListUserResponseDomain, PlatformDeptUserDto>()
            .ForMember(dst => dst.UserId, opt => opt.MapFrom(src => src.Userid))
            .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dst => dst.Avatar, opt => opt.MapFrom(src => src.Avatar))
            .ForMember(dst => dst.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dst => dst.BizEmail, opt => opt.MapFrom(src => src.OrgEmail))
            .ForMember(dst => dst.IsActivated, opt => opt.MapFrom(src => src.Active))
            .ForMember(dst => dst.Position, opt => opt.MapFrom(src => src.Title))
            
            ;
    }
}