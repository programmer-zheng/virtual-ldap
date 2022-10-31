using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Controllers;
using DingDingSync.Application.AppService;
using Microsoft.AspNetCore.Mvc;

namespace DingDingSync.Web.Controllers;

public class LdapController : AbpController
{
    public IUserAppService _userAppService { get; set; }

    public IDepartmentAppService _departmentAppService { get; set; }

    //Ldap服务使用
    [Route("/departments")]
    public async Task<IActionResult> Departments()
    {
        var depts = await _departmentAppService.GetAllDepartments();

        return Json(depts);
    }

    //Ldap服务使用
    [Route("/deptusers")]
    public async Task<IActionResult> DeptUsers(long deptid)
    {
        var users = await _userAppService.DeptUsers(deptid);
        return Json(users);
    }
}