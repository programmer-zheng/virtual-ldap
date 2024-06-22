using ContactsSync.Application.Contracts.Dtos;
using Volo.Abp.Application.Services;

namespace ContactsSync.Application.Contracts;

public interface IDepartmentAppService : IApplicationService
{
    Task<List<DepartmentDto>> GetAllDepartments();

    Task BatchAddDepartmentAsync(params CreateDepartmentDto[] depts);

    Task AddDepartmentAsync(CreateDepartmentDto dto);

    Task RemoveDepartmentAsync(long id);

    Task UpdateDepartmentAsync(UpdateDepartmentDto dto);
}