using Abp.Application.Services;
using VirtualLdap.Application.AppService.Dtos;
using VirtualLdap.Core.Entities;

namespace VirtualLdap.Application.AppService
{
    public interface IDepartmentAppService : IApplicationService
    {
        List<DepartmentDto> GetAllDepartments();

        Task AddDepartmentAsync(DepartmentEntity dto);

        Task RemoveDepartmentAsync(long id);

        Task UpdateDepartmentAsync(DepartmentEntity dto);
    }
}