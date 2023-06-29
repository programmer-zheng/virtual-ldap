﻿using Abp.Application.Services;
using VirtualLdap.Application.AppService.Dtos;
using VirtualLdap.Core.Entities;

namespace VirtualLdap.Application.AppService
{
    public interface IDepartmentAppService : IApplicationService
    {
        List<DepartmentDto> GetAllDepartments();

        Task AddDepartment(DepartmentEntity dto);
    }
}