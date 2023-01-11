using Abp.Application.Services;
using DingDingSync.Application.AppService.Dtos;

namespace DingDingSync.Application.AppService
{
    public interface IDepartmentAppService : IApplicationService
    {
        Task<List<DepartmentDto>> GetAllDepartments();
    }
}