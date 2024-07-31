using Abp.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc;
using VirtualLdap.Web.Models;

namespace VirtualLdap.Web.Controllers
{
    public class OutSideUserController : AbpController
    {
        public IActionResult Index()
        {
            return View();
        }

        [Route("/AddOutSideUser")]
        public IActionResult AddOutSideUser(AddOutSideUserDto dto)
        {
            return Json(dto);
        }

    }
}
