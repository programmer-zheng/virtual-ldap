using Abp.Domain.Repositories;
using VirtualLdap.Application.AppService.Dtos;
using VirtualLdap.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace VirtualLdap.Application.AppService
{
    public class DepartmentAppService : IDepartmentAppService
    {
        private readonly IRepository<DepartmentEntity, long> DeptRepository;

        public DepartmentAppService(IRepository<DepartmentEntity, long> deptRepository)
        {
            DeptRepository = deptRepository;
        }

        public async Task<List<DepartmentDto>> GetAllDepartments()
        {
            var list = await DeptRepository.GetAll()
                .Select(t => new DepartmentDto
                {
                    Id = t.Id,
                    Name = t.DeptName,
                    Parentid = t.ParentId
                }).ToListAsync();
            return list;
        }

        public async Task AddDepartment(DepartmentEntity dto)
        {
            await DeptRepository.InsertAsync(dto);
        }
    }
}