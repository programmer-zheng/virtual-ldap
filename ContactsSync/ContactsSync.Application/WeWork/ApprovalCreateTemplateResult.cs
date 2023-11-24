using Senparc.Weixin.Entities;

namespace ContactsSync.Application.WeWork;

public class ApprovalCreateTemplateResult:WxJsonResult
{
    public string template_id { get; set; }
}