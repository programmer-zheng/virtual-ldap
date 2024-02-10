using ContactsSync.Application.Contracts.SyncConfig;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace ContactsSync.Web.Controllers;

public class TestsController : AbpController
{
    private readonly ISyncConfigAppService _configAppService;


    public TestsController(ISyncConfigAppService configAppService)
    {
        _configAppService = configAppService;
    }

    [Route("/Fuck")]
    [HttpGet]
    public async Task<IActionResult> Fuck()
    {
        var service = await _configAppService.GetServiceByConfigKey();
        var departmentList = await service.GetDepartmentListAsync();
        return Json(departmentList);
    }
}