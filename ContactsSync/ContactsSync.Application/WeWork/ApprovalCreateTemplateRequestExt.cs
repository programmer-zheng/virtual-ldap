using Senparc.Weixin.Work.AdvancedAPIs.OA.OAJson;

namespace ContactsSync.Application.WeWork;

public class ApprovalCreateTemplateRequestExt
{
    /// <summary>
    /// 
    /// </summary>
    public List<ApprovalCreateTemplateRequest_TextAndLang> template_name { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public ApprovalCreateTemplateRequest_TemplateContent template_content { get; set; }
}