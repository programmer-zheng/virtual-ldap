using AutoMapper;
using ContactsSync.Domain.Shared;

namespace ContactsSync.Application.OpenPlatformProvider;

public class PlatformDtoMapperProfile : Profile
{
    public PlatformDtoMapperProfile()
    {
        CreateMap<PlatformDepartmentDto, DepartmentEntity>()
            .ForMember(dst => dst.OriginId, opt => opt.MapFrom(src => src.DepartmentId))
            .ForMember(dst => dst.DeptName, opt => opt.MapFrom(src => src.DeptName))
            .ForMember(dst => dst.ParentId, opt => opt.MapFrom(src => src.ParentId))
            .ReverseMap()
            ;

        CreateMap<PlatformDeptUserDto, UserEntity>()
            .ForMember(dst => dst.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dst => dst.Avatar, opt => opt.MapFrom(src => src.Avatar))
            .ForMember(dst => dst.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dst => dst.BizEmail, opt => opt.MapFrom(src => src.BizEmail))
            .ForMember(dst => dst.Mobile, opt => opt.MapFrom(src => src.Mobile))
            .ForMember(dst => dst.IsEnabled, opt => opt.MapFrom(src => src.IsDeptLeader))
            .ReverseMap()
            ;
    }
}