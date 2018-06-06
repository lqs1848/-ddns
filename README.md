# -ddns
定时更新动态ip到动态域名服务商  支持所有提供 update api 的动态域名服务商 如dynu丶3322丶dyndns丶no-ip 等(只要能用url更新的都支持)


dynu.com写法
https://api.dynu.com/nic/update?hostname=(你的hostname)&myip={ip4}&myipv6={ip6}&username=(你的用户名)&password=(你的用户密码)

3322.com写法
http://(你的用户名):(你的用户密码)@members.3322.org/dyndns/update?system=dyndns&hostname=(你的hostname)&myip={ip4}

可以添加多个 用换行分割

![image](https://raw.githubusercontent.com/lqs1848/-ddns/master/info/layout.png)

支持开机启动（需右键以管理员权限运行）
支持最小化到托盘

