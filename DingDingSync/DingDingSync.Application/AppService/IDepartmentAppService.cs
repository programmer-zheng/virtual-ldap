using Abp.Application.Services;
using DingDingSync.Application.AppService.Dtos;
using DingDingSync.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DingDingSync.Application.AppService
{
    public interface IDepartmentAppService : IApplicationService
    {
        Task<List<DepartmentDto>> GetAllDepartments();

        Task AddDepartment(DepartmentEntity dto);
    }
}