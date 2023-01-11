using Abp.Domain.Repositories;
using DingDingSync.Application.AppService.Dtos;
using DingDingSync.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace DingDingSync.Application.AppService
{
    public class DepartmentAppService : IDepartmentAppService
    {
        public IRepository<DepartmentEntity, long> DeptRepository { get; set; }


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
    }
}