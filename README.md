# 后端部署说明

部署本项目较为简单，但是请配置相关的环境变量：

在Windows（Powershell）中：

```shell
[Environment]::SetEnvironmentVariable('CINEMA_DATABASE', "Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = <你的IP地址>)(PORT = 1521))(CONNECT_DATA =(SERVER = DEDICATED)(SERVICE_NAME = XE)));User Id=<用户名>;Password=<密码>;", 'User')
[Environment]::SetEnvironmentVariable('TCCLOUD_SECRETKEY', "<腾讯云Key>", 'User')
[Environment]::SetEnvironmentVariable('CINEMA_REDIS', "<你的IP地址>:6379,password=<密码>,ssl=False,abortConnect=False", 'User')
```

在Unix中：

```shell
export CINEMA_DATABASE="Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = <你的IP地址>)(PORT = 1521))(CONNECT_DATA =(SERVER = DEDICATED)(SERVICE_NAME = XE)));User Id=<用户名>;Password=<密码>;"
export TCCLOUD_SECRETKEY="<腾讯云Key>"
export CINEMA_REDIS="<你的IP地址>:6379,password=<密码>,ssl=False,abortConnect=False"
```

原始（包含密码/密钥/Key的）环境变量请见《附加到Oracle文档》。

接下来，进入`Cinema`文件夹，输入：

```shell
dotnet run -lp pro
```

即可。
