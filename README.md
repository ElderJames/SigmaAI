<h1 align="center">🤖 Sigma AI Platform</h1>

<div align="center">

An out-of-box enterprise intelligence platform based on LLM and GenAI.

Our goal is to empower your business systems to become intelligent, thereby achieving greater business value.

![Build](https://img.shields.io/github/actions/workflow/status/ElderJames/Sigma/dotnet.yml?style=flat-square)

</div>

*Note that this is still in early development*

English | [简体中文](README-zh_CN.md)

## ✨ Implemented Features

- Very easy to integrate existing systems through WebAPI or Native Function.
- Support for all LLMs with OpenAI restful protocol.
- Support integration of local models using LLamaSharp or Ollama.
- Support for native Function Calling API like OpenAI/SparkDesk/DashScope.
- Support for implementing Function Calling without native API through intent recognition.
- Support knowledge-based RAG
- Based on an earlier version of AntSK

## 📦 Installation Guide

### Prerequirement

- Install [.NET Core SDK](https://dotnet.microsoft.com/download/dotnet-core/8.0?WT.mc_id=DT-MVP-5003987).

- Clone and run

```bash
$ git clone https://github.com/ElderJames/Sigma.git
$ cd Sigma
$ dotnet run --project src/sigma
```

- Create a account and engjoy!

## 🔨 Development

- Technology stack
  - .NET 8,EF Core and Blazor is the main tech.
  - Using Ant Design Blazar for the beautiful interactive WebUI.
  - Using Semantic Kernel is used to integrate Large Language Models.
  - Using Kernel Memory for searching and indexing of RAG.

## 🤝 Contributing

[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg?style=flat-square)](https://github.com/ElderJames/Sigma/pulls)

If you would like to contribute, feel free to create a [Pull Request](https://github.com/ElderJames/Sigma/pulls), or give us [Bug Report](https://github.com/ElderJames/Sigma/issues/new).


## 💕 Contributors

This project exists thanks to all the people who contribute.

<a href="https://github.com/ElderJames/Sigma/graphs/contributors">
  <img src="https://contrib.rocks/image?repo=ElderJames/Sigma&max=1000&columns=15&anon=1" />
</a>

## 🚨 Code of Conduct

This project has adopted the code of conduct defined by the Contributor Covenant to clarify expected behavior in our community.
For more information see the [.NET Foundation Code of Conduct](https://dotnetfoundation.org/code-of-conduct).

## ☀️ License

Licensed under the Apache 2.0 license.