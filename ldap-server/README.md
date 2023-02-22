## ldap服务端程序



### create docker container in Ubuntu with root 

``` bash
docker run -itd --name ldap-server --restart always -p 389:389 -v /root/ldap/config.js:/app/config.js ldap-server
```