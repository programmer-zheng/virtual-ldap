﻿using Abp.AspNetCore.Mvc.Controllers;
using Abp.Extensions;
using Microsoft.AspNetCore.Mvc;
using VirtualLdap.Application;
using VirtualLdap.Application.AppService;
using VirtualLdap.Application.AppService.Dtos;
using VirtualLdap.Web.Startup;

namespace VirtualLdap.Web.Controllers;

/// <summary>
/// 为LDAP服务提供相关数据支持
/// </summary>
[ServiceFilter(typeof(CheckTokenFilterAttribute))]
public class LdapController : AbpController
{
    public IUserAppService _userAppService { get; set; }

    public IDepartmentAppService _departmentAppService { get; set; }

    /// <summary>
    /// 获取所有部门
    /// </summary>
    /// <returns></returns>
    [Route("/departments")]
    public IActionResult Departments()
    {
        var depts = _departmentAppService.GetAllDepartments();

        return Json(depts);
    }

    /// <summary>
    /// 获取指定部门下的人员信息
    /// </summary>
    /// <param name="deptid"></param>
    /// <returns></returns>
    [Route("/deptusers")]
    public IActionResult DeptUsers(long deptid)
    {
        var users = _userAppService.DeptUsers(deptid);
        return Json(users);
    }

    /// <summary>
    /// LDAP服务中验证用户密码
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("/validateuser")]
    public async Task<IActionResult> ValidateUser([FromBody] LdapRequestViewModel input)
    {
        var result = new LdapResponseViewModel();
        var user = await _userAppService.GetByUserNameAsync(input.username);
        if (user == null)
        {
            result.Msg = "用户不存在";
        }
        else
        {
            if (!user.AccountEnabled)
            {
                result.Msg = "账号未开通，请联系管理员开通";
            }
            else if (!user.PasswordInited)
            {
                result.Msg = "初始密码未修改，请修改后再登录";
            }
            else if (user.Password.DesDecrypt().ToMd5()
                     .Equals(input.password.ToMd5(), StringComparison.OrdinalIgnoreCase))
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