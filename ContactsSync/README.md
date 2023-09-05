## 通讯录同步后端服务
                              
### 数据库迁移
#### 添加迁移记录
``` bash
dotnet ef migrations add comment -s .\VirtualLdap.Web\VirtualLdap.Web.csproj -p .\VirtualLdap.EntityFrameworkCore\VirtualLdap.EntityFrameworkCore.csproj
```

#### 执行迁移
``` bash
dotnet ef database update  -s .\VirtualLdap.Web\VirtualLdap.Web.csproj -p .\VirtualLdap.EntityFrameworkCore\VirtualLdap.EntityFrameworkCore.csproj
```

## 引用文档
### SDK
- [Senparc.Weixin.Work 企业微信模块](https://sdk.weixin.senparc.com/Docs/Work/)

### 审批
#### 企业微信
- [审批流程引擎](https://developer.work.weixin.qq.com/document/path/90269)
