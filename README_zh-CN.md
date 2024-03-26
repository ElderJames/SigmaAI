<h1 align="center">🤖 Sigma AI Platform</h1>

<div align="center">

一个开箱即用的基于 LLM 和 GenAI 的企业级智能平台。

我们的目标是赋能您的业务系统实现智能化，从而实现更大的商业价值。

![Build](https://img.shields.io/github/actions/workflow/status/ElderJames/Sigma/dotnet.yml?style=flat-square)

</div>

*注意目前仍在积极的开发阶段，正式发布前请谨慎用于生产环境。*

## ✨ 已实现的功能

- 非常容易地通过 WebAPI 或原生函数与已有系统集成。
- 支持所有 OpenAI Restful 协议的大模型服务。
- 支持利用 LLamaSharp 或 Ollama 集成本地模型。
- 支持原生 Function Calling API, 如 OpenAI/SparkDesk/DashScope。
- 支持利用语义识别使没有原生API的模型实现 Function Calling。
- 支持基于知识的检索增强生成（RAG）
- 本项目基于早期版本的 AntSK

## 📦 安装


- 安装 [.NET Core SDK](https://dotnet.microsoft.com/download/dotnet-core/8.0?WT.mc_id=DT-MVP-5003987).

- 克隆并启动项目

  ```bash
  $ git clone https://github.com/ElderJames/Sigma.git
  $ cd Sigma
  $ dotnet run --project src/sigma
  ```

- 最后创建用户，尽情享受!

## 🔨 开发

- 技术栈
  - 主要技术是 .NET 8、EF Core 和 Blazor。
  - 使用 Ant Design Blazar 实现优美的交互界面。
  - 使用 Semantic Kernel 集成大语言模型。
  - 使用 Kernel Memory 实现 RAG 的拆分、索引和查询。

## 🤝 贡献

[![欢迎 PR](https://img.shields.io/badge/PRs-welcome-brightgreen.svg?style=flat-square)](https://github.com/ElderJames/Sigma/pulls)

如果你想来贡献, 尽管创建 [Pull Request](https://github.com/ElderJames/Sigma/pulls), 或者提交 [Bug Report](https://github.com/ElderJames/Sigma/issues/new).


## 💕 贡献者

感谢本项目的贡献者

<a href="https://github.com/ElderJames/Sigma/graphs/contributors">
  <img src="https://contrib.rocks/image?repo=ElderJames/Sigma&max=1000&columns=15&anon=1" />
</a>

## 🚨 Code of Conduct

本项目采用了《贡献者公约》所定义的行为准则，以明确我们社区的预期行为。
更多信息请见 [.NET Foundation Code of Conduct](https://dotnetfoundation.org/code-of-conduct).

## ☀️ 授权协议

Licensed under the Apache 2.0 license.