using Abp.Application.Services;
using DingDingSync.Application.AppService.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DingDingSync.Application.AppService
{
    public interface IDepartmentAppService : IApplicationService
    {
        Task<List<DepartmentDto>> GetAllDepartments();
    }
}