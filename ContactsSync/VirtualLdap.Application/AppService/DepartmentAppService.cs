using Abp.Domain.Repositories;
using VirtualLdap.Application.AppService.Dtos;
using VirtualLdap.Core.Entities;

namespace VirtualLdap.Application.AppService
{
    public class DepartmentAppService : IDepartmentAppService
    {
        private readonly IRepository<DepartmentEntity, long> DeptRepository;

        public DepartmentAppService(IRepository<DepartmentEntity, long> deptRepository)
        {
            DeptRepository = deptRepository;
        }

        public List<DepartmentDto> GetAllDepartments()
        {
            var list = DeptRepository.GetAll()
                .Select(t => new DepartmentDto
                {
                    Id = t.Id,
                    Name = t.DeptName,
                    Parentid = t.ParentId
                }).ToList();
            return list;
        }

        public async Task AddDepartmentAsync(DepartmentEntity dto)
        {
            await DeptRepository.InsertAsync(dto);
        }

        public async Task RemoveDepartmentAsync(long id)
        {
            await DeptRepository.HardDeleteAsync(t => t.Id == id);
        }

        public async Task UpdateDepartmentAsync(DepartmentEntity dto)
        {
            await DeptRepository.UpdateAsync(dto.Id, t =>
            {
                t.DeptName = dto.DeptName;
                t.ParentId = dto.ParentId;
                return Task.CompletedTask;
            });
        }
    }
}