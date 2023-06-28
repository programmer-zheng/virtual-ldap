# 配置文件说明
### WorkEnv 工作环境
默认值：DingDing  
只支持钉钉和企业微信通讯录同步，运行项目前需要先指定通讯录数据来源

```bash
# 指定工作环境为钉钉
"WorkEnv": "DingDing"

# 指定工作环境为企业微信
"WorkEnv": "WorkWeiXin" 
```

### DefaultPassword 默认密码
默认值：123456  
同步通讯录时，为账号设置的默认密码；若不在配置中指定，则使用`123456`作为默认密码
```bash
# 指定默认密码为 Qwe123#
"DefaultPassword":"Qwe123#"
```
**注意：** 除管理员账号外，其他账号即使启用，也无法直接使用，需要由员工自行修改个人账号的默认密码后，方可正常使用

### LdapRequestToken LDAP服务同步Token
默认值：空  
LDAP服务请求当前WEB服务时验证使用  
需要注意的是，若指定，请保证LDAP服务配置中与当前WEB服务配置一致  
若不设置，在外部直接访问相关接口时，可直接获取数据，有可能会暴露敏感信息
``` bash
# 设置LDAP服务同步Token为 FV*X@2rl&y@k2BQ7 
"LdapRequestToken": "FV*X@2rl&y@k2BQ7"
```

### Email 邮箱后缀
默认值：空  
若通讯录中未设置邮箱，单位又未为员工开通企业邮箱，可设置虚拟邮箱地址，因部分第三方服务默认需要使用邮箱登录，如：`Gitlab`
``` bash
# 设置邮箱后缀为 baidu.com
"EmailSuffix": "baidu.com"
```

### DingDing 钉钉相关配置
登录 https://open.dingtalk.com/   
1、打开开发者后台，即可查看到 CorpId  
2、应用开发，点击“企业内部开发”，创建应用后，点击应用详情，即可查看应用信息，`AgentId、AppKey、AppSecret`复制过来即可  
3、应用配置中 “事件与回调”，配置时，可填写自定义“加密aes_key”与“签名token”  
```json
{
  "DingDing": {
    "CorpId": "dingxxxxxxxxxxxxxxxxxxxxxxx",
    "AgentId": 123456789,
    "AppKey": "dingdxxxxxxxxxxx",
    "AppSecret": "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
    "Aes_Key": "xxxxxxxxxxxxxxxxxxxxxxxxxxxx",
    "Token": "xxxxxxxxxxxxxxxxx"
  }
}  

```

### WorkWeiXin 企业微信相关设置
登录 https://work.weixin.qq.com/
- CorpId 企业ID 详见：https://work.weixin.qq.com/wework_admin/frame#profile
- AgentId 创建应用后，见应用首页
- AppSecret 应用首页，Secret字段 
- Token 企业微信回调处理Token，需要自行设置  
  详见：https://work.weixin.qq.com/wework_admin/frame#/apps/contactsApi 
- EncodingAesKey 企业微信回调处理Token，需要自行设置  
  详见：https://work.weixin.qq.com/wework_admin/frame#/apps/contactsApi
```json
{
  "WorkWeiXin": {
    "CorpId": "wwexxxxxxxxxxxxxxx",
    "AgentId": 1234567,
    "AppSecret": "xxxxxxxxxxxxxxxxxxxxxxxxxxxx",
    "Token": "xxxxxxxxxxxxxxxxxx",
    "EncodingAESKey": "xxxxxxxxxxxxxxxxxxxxxxxxxxxx"
  }
}
```

### IKuai 爱快路由器相关设置
注册并登录 http://open.ikuai8.com/ ，实名认证后，创建应用并关联相关设备
```json
{
  "IKuai": {
    "IsEnabled": false,
    "BasePath": "https://open.ikuai8.com/api/v3",
    "OpenId": "xxxxxxxxxxxxxxxxx",
    "Gwid": "xxxxxxxxxxxxxxxxx",
    "Open_Rsa_Pubkey": "-----BEGIN PUBLIC KEY-----\nxxxxxxxxxxxxxxxxxxn\nxxxxxxxxxxxxxxxxxxxx\nxxxxxxxxxxxxxxxxxxxxxx\n-----END PUBLIC KEY-----"
  } 
}
```

### 使用Docker镜像部署，相关配置修改
假设当前解决方案已经编译为Docker镜像，镜像名为 `ldap`  
以下以钉钉为示例，设置相关参数，向外暴露端口为 `8080`
```bash
docker run -itd --name ldap --restart always \
-e "WorkEnv"="DingDing" \
-e "DingDing:CorpId"="ding1234567"  \
-e "DingDing:AgentId"="1234567"  \
-e "DingDing:AppKey"="ding123456789"  \
-e "DingDing:AppSecret"="xxxxxx"  \
-e "DingDing:Aes_Key"="xxxxxxxxxxxx"  \
-e "DingDing:Token"="xxxxxxxxxxxxxxxxxxx"  \
-e "ConnectionStrings:Default"='Server=1.2.3.4; Database=VirtualLdap;User ID=root; Password=123qwe;port=3306;CharSet=utf8;SslMode=none;' \
-p 8080:80  \
ldap
```
以下为企业微信示例：
```bash
docker run -itd --name ldap --restart always \
-e "WorkEnv"="WorkWeiXin" \
-e "WorkWeiXin:CorpId"="wwexxxxxxxxxxxxxxx" \
-e "WorkWeiXin:AgentId"="1234567" \
-e "WorkWeiXin:AppSecret"="xxxxxxxxxxxxxxxxxxxxxxxxx" \
-e "WorkWeiXin:Token"="xxxxxxxxxxxxxxxxxx" \
-e "WorkWeiXin:EncodingAESKey"="xxxxxxxxxxxxxxxxxxxxxxxxxxxx"  \
-e "ConnectionStrings:Default"='Server=1.2.3.4; Database=VirtualLdap;User ID=root; Password=123qwe;port=3306;CharSet=utf8;SslMode=none;' \
-p 8080:80  \
ldap
```