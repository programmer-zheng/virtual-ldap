using AutoMapper;
using ContactsSync.Application.Contracts.Dtos;
using ContactsSync.Application.OpenPlatformProvider;
using ContactsSync.Domain.Shared;

namespace ContactsSync.Application;

public class ContactsSyncApplicationAutoMapperProfile : Profile
{
    public ContactsSyncApplicationAutoMapperProfile()
    {
        #region dto <<==>> entity

        CreateMap<CreateDepartmentDto, DepartmentEntity>(MemberList.Source);

        CreateMap<CreateUserDto, UserEntity>(MemberList.Source);

        CreateMap<CreateUserDeptRelaDto, UserDepartmentsRelationEntity>(MemberList.Source);

        CreateMap<DepartmentEntity, DepartmentDto>()
            .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.DeptName))
            // .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
            // .ForMember(dst => dst.OriginId, opt => opt.MapFrom(src => src.OriginId))
            // .ForMember(dst => dst.ParentId, opt => opt.MapFrom(src => src.ParentId))
            .ReverseMap()
            ;

        CreateMap<UserEntity, LdapDeptUserDto>(MemberList.Source)
            .ForMember(dst => dst.Email, opt => opt.MapFrom(src => src.BizEmail.IsNullOrWhiteSpace() ? src.Email : src.BizEmail))
            ;
        CreateMap<UserEntity, DeptUserDto>()
            .ForMember(dst => dst.Uid, opt => opt.MapFrom(src => src.Id.ToString()))
            // .ForMember(dst => dst.UserId, opt => opt.MapFrom(src => src.UserId))
            // .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.Name))
            // .ForMember(dst => dst.Avatar, opt => opt.MapFrom(src => src.Avatar))
            // .ForMember(dst => dst.Email, opt => opt.MapFrom(src => src.Email))
            // .ForMember(dst => dst.BizEmail, opt => opt.MapFrom(src => src.BizEmail))
            // .ForMember(dst => dst.Mobile, opt => opt.MapFrom(src => src.Mobile))
            .ReverseMap()
            ;

        CreateMap<UserDepartmentsRelationEntity, DeptUserRelaDto>()
            .ForMember(dst => dst.OriginDeptId, opt => opt.MapFrom(src => src.OriginDeptId))
            .ForMember(dst => dst.UserId, opt => opt.MapFrom(src => src.UserId))
            .ReverseMap()
            ;


        CreateMap<UserEntity, UserSimpleDto>()
            .ForMember(dst => dst.Uid, opt => opt.MapFrom(src => src.Id))
            .ForMember(dst => dst.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dst => dst.UserName, opt => opt.MapFrom(src => src.UserName))
            .ForMember(dst => dst.IsPasswordInited, opt => opt.MapFrom(src => src.IsPasswordInited))
            .ForMember(dst => dst.IsEnabled, opt => opt.MapFrom(src => src.IsEnabled))
            .ForMember(dst => dst.IsVpnEnabled, opt => opt.MapFrom(src => src.IsVpnEnabled))
            ;

        #endregion

        #region OpenPlatformDto <<==>> Dto

        CreateMap<PlatformDepartmentDto, CreateDepartmentDto>()
            .ForMember(dst => dst.OriginId, opt => opt.MapFrom(src => src.DepartmentId))
            .ForMember(dst => dst.DeptName, opt => opt.MapFrom(src => src.DeptName))
            .ForMember(dst => dst.ParentId, opt => opt.MapFrom(src => src.ParentId))
            .ReverseMap()
            ;

        CreateMap<PlatformDeptUserDto, CreateUserDto>()
            .ForMember(dst => dst.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dst => dst.Avatar, opt => opt.MapFrom(src => src.Avatar))
            .ForMember(dst => dst.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dst => dst.BizEmail, opt => opt.MapFrom(src => src.BizEmail))
            .ForMember(dst => dst.Mobile, opt => opt.MapFrom(src => src.Mobile))
            .ForMember(dst => dst.Position, opt => opt.MapFrom(src => src.Position))
            .ForMember(dst => dst.IsEnabled, opt => opt.MapFrom(src => src.IsDeptLeader))
            .ReverseMap()
            ;

        #endregion
    }
}