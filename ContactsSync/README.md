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
### Abp前端库
#### 有哪些包
[Abp client packages](https://www.npmjs.com/~volo)

#### 如何安装单个包
```bash
# 通过npm安装abp中的jquery
npm install @abp/jquery
```

## 引用文档
### SDK
- [Senparc.Weixin.Work 企业微信模块](https://sdk.weixin.senparc.com/Docs/Work/)

                                                                       
### 同一接口，多个实现，如何获取
https://www.google.com/search?q=asp.net+core+%E5%AE%B9%E5%99%A8%E4%B8%AD%E5%90%8C%E4%B8%80%E4%B8%AA%E6%8E%A5%E5%8F%A3%E5%A4%9A%E4%B8%AA%E5%AE%9E%E7%8E%B0&oq=asp.net+core+%E5%AE%B9%E5%99%A8%E4%B8%AD%E5%90%8C%E4%B8%80%E4%B8%AA%E6%8E%A5%E5%8F%A3%E5%A4%9A%E4%B8%AA%E5%AE%9E%E7%8E%B0&gs_lcrp=EgZjaHJvbWUyBggAEEUYOTIKCAEQABiABBiiBNIBCTExODk3ajBqMagCALACAA&sourceid=chrome&ie=UTF-8

https://blog.csdn.net/qq_27337291/article/details/110231635

https://blog.51cto.com/u_15060551/4376664

https://blog.csdn.net/WuLex/article/details/128709517

https://cloud.tencent.com/developer/article/1462933

#### 测试abp中使用.net8新特性 KeyService
``` bash
abp new Acme.BookStore -t app-nolayers -csf
cd Acme.BookStore\Acme.BookStore
```
```csharp
public interface IMessage
{
    string GetMessage();
}

public class MessageA : IMessage
{
    public string GetMessage()
    {
        return "aaaaa";
    }
}

public class MessageB : IMessage
{
    public string GetMessage()
    {
        return "bbbbb";
    }
}

// add in BookStoreModule ConfigureServices
context.Services.AddKeyedScoped<IMessage, MessageA>("A");
context.Services.AddKeyedScoped<IMessage, MessageB>("B");
```

