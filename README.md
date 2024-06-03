# Unity Log System

Unity Log System是一个用于Unity游戏引擎的 ~~高性能、可配置（高不高性能再说吧还没测）~~ 的日志系统。提供了类似于Unreal Engine的日志功能 ~~（其实是想照抄的）~~ ，支持不同的日志等级、不同构建版本的日志输出控制、日志写入文件以及日志着色等功能。

## 特点

以下目前一个都没有：

- 支持Log、Warning、Error三个基本的日志等级，并提供了着色功能以便于区分。
- 根据游戏的构建版本（Editor、Development、Release）控制日志的输出。
- 将日志写入文件，包含时间戳和格式化的日志条目。
- 高性能的日志写入，避免对游戏性能产生影响。
- 易于集成和使用，只需简单的API调用即可记录日志。

<!-- ## 安装

1. 将`Logger`文件夹复制到你的Unity项目的`Assets`目录中。
2. 在游戏启动时调用`Logger.Initialize()`方法初始化日志系统。 -->

## 使用

```csharp
// 记录一般日志
Logger.Log(Logger.LogLevel.Log, "This is a log message.");

// 记录警告日志
Logger.Log(Logger.LogLevel.Warning, "This is a warning message.");

// 记录错误日志
Logger.Log(Logger.LogLevel.Error, "This is an error message.");
```

--- 

本项目使用JetBrains Rider进行开发，IDE License由JetBrains 开源项目申请提供

<p>
<a href="https://www.jetbrains.com/"/>
<img src ="jb_beam.png" align="middle" width=25%/>
</a>
<a href="https://www.jetbrains.com/rider/"/>
<img src ="Rider.png" align="middle" width=25%/>
</a>
<a href="https://www.jetbrains.com/lp/rider-unreal/"/>
<img src ="Rider_icon.png" align="middle" width=25%/>
</a>
</p>

<font size = 5> [**JetBrains 开源项目支持申请链接**](https://www.jetbrains.com/lp/rider-unreal/)</font>

推荐使用Rider进行Unity开发。