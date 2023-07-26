# -ddns
定时更新本机的ip到动态域名服务商<br>
支持所有提供 update api 的动态域名服务商 如dynu丶3322丶dyndns丶no-ip 等(只要能用url更新的都支持)<br>


dynu.com写法:<br>
https://api.dynu.com/nic/update?hostname=(你的hostname)&myip={ip4}&myipv6={ip6}&username=(你的用户名)&password=(你的用户密码)<br>

3322.com写法:<br>
http://(你的用户名):(你的用户密码)@members.3322.org/dyndns/update?system=dyndns&hostname=(你的hostname)&myip={ip4}<br>

简单来说<br>
就是用{ip4}替换到你的更新路径中当前要更新的ip地址<br>

可以添加多个 用换行分割<br>

![image](https://raw.githubusercontent.com/lqs1848/-ddns/master/info/layout.png)

支持开机启动（需右键以管理员权限运行）<br>
支持最小化到托盘<br>



阿里云解析

```
<configuration>
  <appSettings>
    <add key="aliyunAppId" value="阿里云给的appid" />
    <add key="aliyunKeySecret" value="阿里云给的KeySecret" />
    <!-- 要更新的域名 多个用<,>逗号隔开 @ 就是 baidu.com 不带二级域名的地址-->
    <add key="aliyunDomain" value="@.baidu.com,www.baidu.com,ejym.baidu.com" />
  </appSettings>
  <startup>
    <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.5" />
  </startup>
</configuration>
```

阿里云 @.baidu.com -> Error 如果ip无变化是正常的 ip无变化 阿里云默认给 Code 400 



下载地址:<br>
https://github.com/lqs1848/-ddns/files/2078857/ddns.zip<br>
解压后启动即可<br>
