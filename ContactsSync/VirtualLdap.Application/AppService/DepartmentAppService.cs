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
            var list =  DeptRepository.GetAll()
                .Select(t => new DepartmentDto
                {
                    Id = t.Id,
                    Name = t.DeptName,
                    Parentid = t.ParentId
                }).ToList();
            return list;
        }

        public async Task AddDepartment(DepartmentEntity dto)
        {
            await DeptRepository.InsertAsync(dto);
        }
    }
}