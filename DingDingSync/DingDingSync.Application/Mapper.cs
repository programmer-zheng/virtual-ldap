using AutoMapper;
using DingDingSync.Application.WorkWeixinUtils;
using DingDingSync.Core.Entities;
using DingTalk.Api.Response;

namespace DingDingSync.Application
{
    public class WorkWeixinMapper : Profile
    {
        public WorkWeixinMapper()
        {
            CreateMap<WorkWeixinDeptListDto, DepartmentEntity>()
                .ForMember(t => t.DeptName, opt => opt.MapFrom(d => d.Name))
                .ForMember(t => t.Id, opt => opt.MapFrom(d => d.Id))
                .ForMember(t => t.ParentId, opt => opt.MapFrom(d => d.Parentid))
                ;

            CreateMap<WorkWeixinDeptUserListDto, UserEntity>()
                .ForMember(t => t.Id, opt => opt.MapFrom(d => d.Userid))
                .ForMember(t => t.UserName, opt => opt.MapFrom(d => d.Userid))
                .ForMember(t => t.Active, opt => opt.MapFrom(d => d.Status == 1))
                ;
        }
    }

    public class Mapper : Profile
    {
        public Mapper()
        {
            //部门列表
            CreateMap<OapiDepartmentListResponse.DepartmentDomain, DepartmentEntity>()
                .ForMember(t => t.Id, opt => opt.MapFrom(d => d.Id))
                .ForMember(t => t.DeptName, opt => opt.MapFrom(d => d.Name))
                ;

            //部门详情
            CreateMap<OapiV2DepartmentGetResponse.DeptGetResponseDomain, DepartmentEntity>()
                .ForMember(t => t.Id, opt => opt.MapFrom(d => d.DeptId))
                .ForMember(t => t.DeptName, opt => opt.MapFrom(d => d.Name))
                ;


            CreateMap<long, DateTime?>().ConvertUsing<TicksToDateTimeConverter>();

            //人员列表
            CreateMap<OapiV2UserListResponse.ListUserResponseDomain, UserEntity>()
                .ForMember(t => t.HiredDate, opt => opt.MapFrom(d => d.HiredDate))
                .ForMember(t => t.Id, opt => opt.MapFrom(d => d.Userid))
                .ForMember(t => t.Tel, opt => opt.MapFrom(d => d.Telephone))
                .ForMember(t => t.Position, opt => opt.MapFrom(d => d.Title))
                ;

            //人员详情
            CreateMap<OapiV2UserGetResponse.UserGetResponseDomain, UserEntity>()
                .ForMember(t => t.HiredDate, opt => opt.MapFrom(d => d.HiredDate))
                .ForMember(t => t.Id, opt => opt.MapFrom(d => d.Userid))
                .ForMember(t => t.Tel, opt => opt.MapFrom(d => d.Telephone))
                .ForMember(t => t.Position, opt => opt.MapFrom(d => d.Title))
                ;
        }
    }

    public class TicksToDateTimeConverter : ITypeConverter<long, DateTime?>
    {
        public DateTime? Convert(long source, DateTime? destination, ResolutionContext context)
        {
            if (source > 0)
            {
                return TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)).AddMilliseconds(source);
            }

            return null;
        }
    }
}