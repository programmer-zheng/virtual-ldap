using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.ObjectMapping;
using Abp.Runtime.Caching;
using Castle.Core.Logging;
using Microsoft.Extensions.Configuration;
using VirtualLdap.Core.Entities;

namespace VirtualLdap.Application.AppService;

public interface ISyncContacts
{
    Task SyncContactsAsync();
}

public class SyncContactsBase 
{
    public IRepository<UserEntity, string> UserRepository { get; set; }

    public IRepository<DepartmentEntity, long> DeptRepository { get; set; }

    public IRepository<UserDepartmentsRelationEntity, string> UserDeptRelaRepository { get; set; }

    public ILogger Logger { get; set; }

    public IConfiguration Configuration { get; set; }

    public IObjectMapper ObjectMapper { get; set; }

    public ICacheManager CacheManager { get; set; }

    /// <summary>
    /// 处理新数据
    /// </summary>
    /// <param name="newUserList"></param>
    /// <param name="newDeptList"></param>
    /// <param name="newRelaList"></param>
    [UnitOfWork]
    protected void ProcessNewData(List<UserEntity> newUserList, List<DepartmentEntity> newDeptList,
        List<UserDepartmentsRelationEntity> newRelaList)
    {
        foreach (var item in newDeptList)
        {
            DeptRepository.Insert(item);
        }

        foreach (var item in newUserList)
        {
            UserRepository.Insert(item);
        }

        foreach (var item in newRelaList)
        {
            UserDeptRelaRepository.Insert(item);
        }
    }
}