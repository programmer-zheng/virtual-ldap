## ldap服务端程序

**重命名 `config.example.js` 文件 ，改为 `config.js` ，根据实际情况修改配置**


### npm

本人使用的是 `npm 18.12.1`，其他版本未尝试

### 本地调试ldap

``` bash
npm install
node index.js
```

![image-20230222103031873](https://img.zwsdk.cn/image-20230222103031873.png)







### docker运行

``` bash
# 在当前目录下编译docker镜像
docker build -t ldap-server .

# 使用已经编译的docker镜像运行容器，映射端口 389
# 将本地配置文件(/root/ldap/config.js)映射到容器相关文件（/app/config.js)
docker run -itd --name ldap-server --restart always -p 389:389 -v /root/ldap/config.js:/app/config.js ldap-server
```

### 测试连接

可下载  [Apache Directory Studio](https://directory.apache.org/studio/) 进行测试

![image-20230222104454366](https://img.zwsdk.cn/image-20230222104454366.png)