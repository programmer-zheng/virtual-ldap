using ContactsSync.Application.AppServices.Dtos;
using ContactsSync.Domain.Shared;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace ContactsSync.Application.AppServices;

public class DepartmentAppService : ApplicationService, IDepartmentAppService
{
    private readonly IRepository<DepartmentEntity> _deptRepository;

    public DepartmentAppService(IRepository<DepartmentEntity> deptRepository)
    {
        _deptRepository = deptRepository;
    }

    public async Task<List<DepartmentDto>> GetAllDepartments()
    {
        var departments = await _deptRepository.GetListAsync();
        var result = ObjectMapper.Map<List<DepartmentEntity>, List<DepartmentDto>>(departments);
        return result;
    }

    public async Task BatchAddDepartmentAsync(params DepartmentEntity[] depts)
    {
        await _deptRepository.InsertManyAsync(depts);
    }

    public async Task AddDepartmentAsync(DepartmentEntity dto)
    {
        await _deptRepository.InsertAsync(dto);
    }

    public async Task RemoveDepartmentAsync(long id)
    {
        await _deptRepository.DeleteDirectAsync(t => t.OriginId == id);
    }

    public async Task UpdateDepartmentAsync(DepartmentEntity dto)
    {
        await _deptRepository.UpdateAsync(dto);
    }
}