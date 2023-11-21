## 通讯录同步后端服务
                              
### 数据库迁移
#### 添加迁移记录
``` bash
# add后面的为迁移名称
dotnet ef migrations add init -s .\VirtualLdap.Web\VirtualLdap.Web.csproj -p .\VirtualLdap.EntityFrameworkCore\VirtualLdap.EntityFrameworkCore.csproj


dotnet ef migrations add init -s .\ContactsSync.Web\ContactsSync.Web.csproj -p .\ContactsSync.EntityFrameworkCore\ContactsSync.EntityFrameworkCore.csproj

```

#### 执行迁移
``` bash
dotnet ef database update  -s .\VirtualLdap.Web\VirtualLdap.Web.csproj -p .\VirtualLdap.EntityFrameworkCore\VirtualLdap.EntityFrameworkCore.csproj


dotnet ef database update  -s .\ContactsSync.Web\ContactsSync.Web.csproj -p .\ContactsSync.EntityFrameworkCore\ContactsSync.EntityFrameworkCore.csproj
```

## 引用文档
### SDK
- [Senparc.Weixin.Work 企业微信模块](https://sdk.weixin.senparc.com/Docs/Work/)

