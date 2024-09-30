using Abp.AutoMapper;
using Newtonsoft.Json;
using VirtualLdap.Core.Entities;

namespace VirtualLdap.Application.WorkWeixinUtils;

[AutoMapTo(typeof(DepartmentEntity))]
public class WorkWeixinDeptListDto
{
    [JsonProperty("id")]
    public long Id {get;set;}

    [JsonProperty("name")]

    public string Name {get;set;}

    [JsonProperty("parentid")]
    public long Parentid {get;set;}

}

public class WorkWeixinDeptUserListDto
{
    
    [JsonProperty("userid")]
    public string Userid {get;set;}

    [JsonProperty("name")]
    public string Name {get;set;}

    [JsonProperty("position")]
    public string Position {get;set;}

    /// <summary>
    /// 激活状态: 1=已激活，2=已禁用，4=未激活，5=退出企业。
    /// </summary>
    [JsonProperty("status")]
    public int Status {get;set;}

    [JsonProperty("enable")]
    public bool Enable {get;set;}

    [JsonProperty("isleader")]
    public bool Isleader {get;set;}
}