using ContactsSync.Application.Contracts;
using ContactsSync.Domain.Shared;
using ContactsSync.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace ContactsSync.Web.Controllers;

public class LdapController : AbpController
{
    public IUserAppService UserAppService { get; set; }
    public IDepartmentAppService DepartmentAppService { get; set; }

    [HttpGet]
    [Route("/Departments")]
    public async Task<IActionResult> Departments()
    {
        var depts = await DepartmentAppService.GetAllDepartments();
        return Json(depts);
    }

    [HttpGet]
    [Route("/DeptUsers")]
    public async Task<IActionResult> DeptUsers(Guid deptId)
    {
        var deptUsers = await UserAppService.GetDeptUsersAsync(deptId);
        return Json(deptUsers);
    }

    [HttpPost]
    [Route("/ValidateUser")]
    public async Task<IActionResult> ValidateUser([FromBody] LdapRequestViewModel model)
    {
        var result = new LdapResponseViewModel();
        var user = await UserAppService.GetLdapUserAsync(model.UserName);
        if (user == null)
        {
            result.Msg = "用户不存在";
        }
        else
        {
            if (!user.IsEnabled)
            {
                result.Msg = "账号未开通，请联系管理员开通";
            }
            else if (!user.IsPasswordInited)
            {
                result.Msg = "初始密码未修改，请修改后再登录";
            }
            else if (user.Password.DesDecrypt().ToMd5()
                     .Equals(model.Password.ToMd5(), StringComparison.OrdinalIgnoreCase))
            {
                //nodejs所写的ldap服务中，密码传输使用md5
                result.Success = true;
                result.Msg = "验证成功";
            }
            else
            {
                result.Msg = "密码不正确";
            }
        }

        return Json(result);
    }
}