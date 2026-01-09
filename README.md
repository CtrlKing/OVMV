# 🌟 OpenVINO Model Information Viewer （OpenVINO 模型信息查看器）

[![License](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

---

## 📝 中文介绍

OpenVINO Model Information Viewer 是一款专为 OpenVINO IR 模型打造的 **智能模型解析工具**。 只需提供模型目录，工具即可自动完成以下任务：
* 加载分词器（Tokenizer）
* 解析模型配置（Model Config）
* 展示模型输入输出结构（IO Tensors）
* 显示特殊 Token 及其用途
* 识别动态维度、KV Cache、Beam Search 等关键字段
* 编译模型并检测可用设备

所有信息都会以 **清晰、结构化、可读性极高** 的方式呈现。 更重要的是，本工具会对 **每一个参数、每一个输入输出张量、每一个特殊 Token 给出 详细的用途解释**，让你无需查文档即可理解模型内部结构。(事实上很多模型卡基本上找不到基础说明)

### 📌 它能帮助你：
* 快速验证模型文件是否完整
* 检查 Tokenizer 是否与模型匹配
* 理解模型的输入输出格式
* 排查部署与推理过程中的常见错误
* 分析模型是否支持 stateful、beam search、动态 shape 等特性
* 帮助确认模型是否可以在 CPU 上成功编译

### 👤 适用人群
* OpenVINO 初学者
* LLM 模型部署工程师
* 想快速检查 IR 模型结构的开发者
* 需要调试 tokenizer、KV cache、beam search 的用户

### ✨ 功能亮点
* ✔ 自动加载 Tokenizer
* ✔ 自动解析模型配置（架构、层数、隐藏维度、KV heads 等）
* ✔ 自动读取 tokenizer config（chat template、special tokens）
* ✔ 自动展示模型输入输出张量
* ✔ 支持动态维度识别
* ✔ 支持 stateful 模型检测
* ✔ 支持 beam search 控制输入识别
* ✔ 自动编译模型并输出信息

### 🔍 核心特色
**不仅告诉你“参数是什么”，还告诉你“它有什么用”！** 这是本工具区别于普通模型信息打印脚本的最大亮点。

📄 输出示例（见文档末尾）
（作者备注：部分模型可能包含历史遗留的特殊标记，这些标记来自模型系列传承，并不一定实际使用。）

---

## 🌍 English Introduction

OpenVINO Model Information Viewer is an intelligent inspection tool designed specifically for OpenVINO IR models. Simply point it to your model directory, and the tool will automatically:
* Load the tokenizer
* Parse the model configuration
* Display input/output tensor structures
* Show special tokens and their purposes
* Detect dynamic shapes, KV cache, beam search fields
* Compile the model and list available devices

All information is presented in a clean, structured, and highly readable format. More importantly, the tool provides **detailed explanations for every parameter, every IO tensor, and every special token**, allowing you to understand the model without digging through documentation.(which, in practice, often doesn’t even contain these basic explanations in the model card). 

### 📌 This tool helps you:
* Verify model completeness
* Ensure tokenizer–model compatibility
* Understand model IO formats
* Troubleshoot deployment and inference issues
* Analyze support for stateful mode, beam search, and dynamic shapes
* Help confirm whether the model can be successfully compiled on CPU

### 👤 Ideal for:
* OpenVINO beginners
* LLM deployment engineers
* Developers inspecting IR model structures
* Users debugging tokenizer, KV cache, or beam search behavior

### ✨ Key Features
* ✔ Automatic tokenizer loading
* ✔ Model config parsing (architecture, layers, hidden size, KV heads, etc.)
* ✔ Tokenizer config parsing (chat template, special tokens)
* ✔ IO tensor inspection
* ✔ Dynamic shape detection
* ✔ Stateful model detection
* ✔ Beam search control input detection
* ✔ Automatic model compilation and info output

### 🔍 Core Highlight
**It doesn’t just show “what the parameters are” — it explains “what they are used for”.** This is the key feature that sets it apart from simple model info dumpers.

📄 Output Example
(Author note: Some models may contain unused or legacy special tokens inherited from earlier model families.)

---

## ⚖️ License

Licensed under the **MIT License**.  
