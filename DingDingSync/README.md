## 钉钉服务器端同步服务
                              
### 数据库迁移
#### 添加迁移记录
``` bash
dotnet ef migrations add comment -s .\DingDingSync\VirtualLdap.Web.csproj -p .\VirtualLdap.EntityFrameworkCore\VirtualLdap.EntityFrameworkCore.csproj
```

#### 执行迁移
``` bash
dotnet ef database update  -s .\DingDingSync\VirtualLdap.Web.csproj -p .\VirtualLdap.EntityFrameworkCore\VirtualLdap.EntityFrameworkCore.csproj
```