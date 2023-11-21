using System.Collections;
using ContactsSync.Application.AppServices.Dtos;
using ContactsSync.Domain.Shared;
using Volo.Abp.Application.Services;

namespace ContactsSync.Application.AppServices;

public interface IDepartmentAppService : IApplicationService
{
    Task<List<DepartmentDto>> GetAllDepartments();

    Task BatchAddDepartmentAsync(params DepartmentEntity[] depts);
    
    Task AddDepartmentAsync(DepartmentEntity dto);

    Task RemoveDepartmentAsync(long id);

    Task UpdateDepartmentAsync(DepartmentEntity dto);
}