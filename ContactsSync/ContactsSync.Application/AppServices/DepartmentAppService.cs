using ContactsSync.Application.Contracts;
using ContactsSync.Application.Contracts.Dtos;
using ContactsSync.Domain.Contacts;
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

    public async Task BatchAddDepartmentAsync(params CreateDepartmentDto[] depts)
    {
        var entities = ObjectMapper.Map<CreateDepartmentDto[], DepartmentEntity[]>(depts);
        await _deptRepository.InsertManyAsync(entities);
    }

    public async Task AddDepartmentAsync(CreateDepartmentDto dto)
    {
        var entity = ObjectMapper.Map<CreateDepartmentDto, DepartmentEntity>(dto);
        await _deptRepository.InsertAsync(entity);
    }

    public async Task RemoveDepartmentAsync(long id)
    {
        await _deptRepository.DeleteDirectAsync(t => t.OriginId == id);
    }

    public async Task UpdateDepartmentAsync(UpdateDepartmentDto dto)
    {
        var entity = ObjectMapper.Map<UpdateDepartmentDto, DepartmentEntity>(dto);
        await _deptRepository.UpdateAsync(entity);
    }
}