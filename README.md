# ProcessDaemon

一款轻量、无依赖的 Windows 原生常驻进程守护与自动重启工具，基于 **.NET 8.0 Windows Forms** 开发。

## 特性亮点

- **日常自动调度**：秒级精度（500ms 轮询）按每日设定时间自动关停与拉起目标程序，内置日期锁防止同秒多次触发。
- **全进程树强杀**：支持 `Kill(entireProcessTree: true)`，彻底清空多进程架构（如 Electron、Python 等）留下的孤儿后台进程。
- **手动突发重启**：一键强杀目标程序并进入 5~10 秒安全缓冲倒计时，缓冲结束后自动重新拉起。
- **异常崩溃自愈**：目标程序非计划意外退出时，可配置自动缓冲并重新拉起。
- **系统托盘常驻**：关闭主界面自动隐藏至 Windows 右下角通知区，托盘右键支持快捷控制。
- **零依赖单文件**：支持自包含打包输出独立单个 `.exe`，目标机器无需安装任何 .NET 运行时。
- **管理员特权**：内置 `requireAdministrator` 清单，具备跨会话与高权限进程接管能力。

## 快速开始

### 1. 独立单文件发布 (无需预装 .NET 运行时)
```powershell
.\publish.ps1 -SelfContained
```
产物将输出在 `publish/standalone/ProcessDaemon.exe`。

### 2. 轻量版发布 (依赖 .NET 8 运行时)
```powershell
.\publish.ps1
```

## 配置文件 (config.json)

```json
{
  "targetPath": "C:\\Windows\\notepad.exe",
  "stopTime": "04:00:00",
  "startTime": "04:05:00",
  "manualRestartBufferSeconds": 5,
  "killEntireProcessTree": true,
  "autoRecoverOnCrash": true,
  "autoStartDaemon": false
}
```

## 许可证

MIT License
