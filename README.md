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

```text
[INFO] 开始加载(Start Load): openvino_model.xml

## [STEP 1/5] 尝试加载分词器...
------------------------------------------
[INFO] 分词器加载成功(Tokenizer LOAD SUCCESS)! ✅
------------------------------------------

## [STEP 2/5] 尝试加载并显示模型配置信息 (Loading Model Config Info)...
------------------------------------------

[CONFIG] 开始读取模型配置(Start reading model config):
  - **模型架构(Architecture):** qwen3 (Qwen3ForCausalLM)
  - **词汇表大小(Vocab Size):** 151936 (模型可识别的Token总数)
  - **隐藏层大小(Hidden Size):** 5120 (模型内部向量的维度)
  - **层数(Num Hidden Layers):** 40 (Transformer块的数量)
  - **注意力头数(Num Attention Heads):** 40 (每个Transformer块中注意力头的数量)
  - **KV头数(Num Key Value Heads):** 8 (多头注意力中键值矩阵的头数，用于优化性能)
  - **最大位置编码(Max Pos Embeddings):** 40960 (模型能处理的最大序列长度)
  - **激活函数(Hidden Act):** silu (模型内部使用的激活函数，如 silu)
  - **BOS ID:** 151643 (Beginning-of-Sentence Token ID)
  - **EOS ID:** 151645 (End-of-Sentence Token ID)
  - **Torch DType:** bfloat16 (模型训练时使用的数据精度，如 bfloat16)
[CONFIG] 配置信息读取完毕。✅
------------------------------------------

## [STEP 3/5] 尝试加载并显示分词器配置信息 (Loading Tokenizer Config Info)...
------------------------------------------

[TOKENIZER CONFIG] 开始读取分词器配置(Start reading tokenizer config):
  - **分词器类别(Class):** Qwen2Tokenizer
    (Tokenizer Class: Qwen2Tokenizer)
  - **最大长度(Max Length):** 131072 (分词器能处理的最大Token数)
    (Max Length: 131072 tokens max processed)

  --- 对话模板 (Chat Template) ---
    (Used for multi-turn dialogue formatting)
    模板内容(Template Content):

```jinja
  {%- if tools %}
      {{- '<|im_start|>system\n' }}
      {%- if messages[0].role == 'system' %}
          {{- messages[0].content + '\n\n' }}
      {%- endif %}
      {{- "# Tools\n\nYou may call one or more functions to assist with the user query.\n\nYou are provided with function signatures within <tools></tools> XML tags:\n<tools>" }}
      {%- for tool in tools %}
          {{- "\n" }}
          {{- tool | tojson }}
      {%- endfor %}
      {{- "\n</tools>\n\nFor each function call, return a json object with function name and arguments within <tool_call></tool_call> XML tags:\n<tool_call>\n{\"name\": <function-name>, \"arguments\": <args-json-object>}\n</tool_call><|im_end|>\n" }}
  {%- else %}
      {%- if messages[0].role == 'system' %}
          {{- '<|im_start|>system\n' + messages[0].content + '<|im_end|>\n' }}
      {%- endif %}
  {%- endif %}
  {%- set ns = namespace(multi_step_tool=true, last_query_index=messages|length - 1) %}
  {%- for message in messages[::-1] %}
      {%- set index = (messages|length - 1) - loop.index0 %}
      {%- if ns.multi_step_tool and message.role == "user" and not(message.content.startswith('<tool_response>') and message.content.endswith('</tool_response>')) %}
          {%- set ns.multi_step_tool = false %}
          {%- set ns.last_query_index = index %}
      {%- endif %}
  {%- endfor %}
  {%- for message in messages %}
      {%- if (message.role == "user") or (message.role == "system" and not loop.first) %}
          {{- '<|im_start|>' + message.role + '\n' + message.content + '<|im_end|>' + '\n' }}
      {%- elif message.role == "assistant" %}
          {%- set content = message.content %}
          {%- set reasoning_content = '' %}
          {%- if message.reasoning_content is defined and message.reasoning_content is not none %}
              {%- set reasoning_content = message.reasoning_content %}
          {%- else %}
              {%- if '</think>' in message.content %}
                  {%- set content = message.content.split('</think>')[-1].lstrip('\n') %}
                  {%- set reasoning_content = message.content.split('</think>')[0].rstrip('\n').split('<think>')[-1].lstrip('\n') %}
              {%- endif %}
          {%- endif %}
          {%- if loop.index0 > ns.last_query_index %}
              {%- if loop.last or (not loop.last and reasoning_content) %}
                  {{- '<|im_start|>' + message.role + '\n<think>\n' + reasoning_content.strip('\n') + '\n</think>\n\n' + content.lstrip('\n') }}
              {%- else %}
                  {{- '<|im_start|>' + message.role + '\n' + content }}
              {%- endif %}
          {%- else %}
              {{- '<|im_start|>' + message.role + '\n' + content }}
          {%- endif %}
          {%- if message.tool_calls %}
              {%- for tool_call in message.tool_calls %}
                  {%- if (loop.first and content) or (not loop.first) %}
                      {{- '\n' }}
                  {%- endif %}
                  {%- if tool_call.function %}
                      {%- set tool_call = tool_call.function %}
                  {%- endif %}
                  {{- '<tool_call>\n{"name": "' }}
                  {{- tool_call.name }}
                  {{- '", "arguments": ' }}
                  {%- if tool_call.arguments is string %}
                      {{- tool_call.arguments }}
                  {%- else %}
                      {{- tool_call.arguments | tojson }}
                  {%- endif %}
                  {{- '}\n</tool_call>' }}
              {%- endfor %}
          {%- endif %}
          {{- '<|im_end|>\n' }}
      {%- elif message.role == "tool" %}
          {%- if loop.first or (messages[loop.index0 - 1].role != "tool") %}
              {{- '<|im_start|>user' }}
          {%- endif %}
          {{- '\n<tool_response>\n' }}
          {{- message.content }}
          {{- '\n</tool_response>' }}
          {%- if loop.last or (messages[loop.index0 + 1].role != "tool") %}
              {{- '<|im_end|>\n' }}
          {%- endif %}
      {%- endif %}
  {%- endfor %}
  {%- if add_generation_prompt %}
      {{- '<|im_start|>assistant\n' }}
      {%- if enable_thinking is defined and enable_thinking is false %}
          {{- '<think>\n\n</think>\n\n' }}
      {%- endif %}
  {%- endif %}

  --- 扩展特殊 Token (Extended Special Tokens) 及其用途 ---
    (Tokens added outside of the base vocabulary)
    - ID: 151643  | Token: "<|endoftext|>" | 用途: 
            句尾/填充：常用于表示序列结束或作为 Padding Token。
            (Often used to denote the end of a sequence or as a Padding Token.)
    - ID: 151644  | Token: "<|im_start|>" | 用途: 
            对话开始：Qwen 系列模型中用于标记新的对话轮次开始（如 user, system, assistant）。
            (Conversation Start: Used in Qwen models to mark the beginning of a new conversation turn.)
    - ID: 151645  | Token: "<|im_end|>" | 用途: 
            对话结束：Qwen 系列模型中用于标记对话轮次的结束。
            (Conversation End: Used in Qwen models to mark the end of a conversation turn.)
    - ID: 151646  | Token: "<|object_ref_start|>" | 用途: 
            对象引用开始：用于标记模型输入中对象引用的起始位置。
            (Object Reference Start: Marks the beginning of an object reference in the model input.)
    - ID: 151647  | Token: "<|object_ref_end|>" | 用途: 
            对象引用结束：用于标记模型输入中对象引用的结束位置。
            (Object Reference End: Marks the end of an object reference in the model input.)
    - ID: 151648  | Token: "<|box_start|>" | 用途: 
            边界框开始：用于标记图像中特定边界框区域的起始（常用于定位）。
            (Bounding Box Start: Marks the start of a specific bounding box region within an image, often for localization.)
    - ID: 151649  | Token: "<|box_end|>" | 用途: 
            控制Token：特定模型(如多模态、工具调用)使用的特殊控制标记。
            (Control Token: Special control marker used by specific models (e.g., multimodal, tool calling).)
    - ID: 151650  | Token: "<|quad_start|>" | 用途: 
            四边形开始：用于标记图像中四边形区域的起始（比边界框更灵活）。
            (Quadrilateral Start: Marks the start of a quadrilateral region in an image, more flexible than a bounding box.)
    - ID: 151651  | Token: "<|quad_end|>" | 用途: 
            四边形结束：用于标记图像中四边形区域的结束。
            (Quadrilateral End: Marks the end of a quadrilateral region in an image.)
    - ID: 151652  | Token: "<|vision_start|>" | 用途: 
            视觉数据开始：用于标记多模态模型中视觉数据的起始（如图像特征序列）。
            (Vision Data Start: Marks the beginning of visual data (e.g., image feature sequence) in multimodal models.)
    - ID: 151653  | Token: "<|vision_end|>" | 用途: 
            视觉数据结束：用于标记多模态模型中视觉数据的结束。
            (Vision Data End: Marks the end of visual data in multimodal models.)
    - ID: 151654  | Token: "<|vision_pad|>" | 用途: 
            视觉填充：用于填充视觉特征序列，以达到统一的输入长度。
            (Vision Padding: Used to pad visual feature sequences to a uniform input length.)
    - ID: 151655  | Token: "<|image_pad|>" | 用途: 
            图像填充：用于填充图像特征序列。
            (Image Padding: Used to pad image feature sequences.)
    - ID: 151656  | Token: "<|video_pad|>" | 用途: 
            视频填充：用于填充视频特征序列。
            (Video Padding: Used to pad video feature sequences.)
    - ID: 151657  | Token: "<tool_call>" | 用途: 
            工具调用开始：用于 RAG 或 Agent 任务中。
            (Tool Call Start: Used for RAG or Agent tasks.)
    - ID: 151658  | Token: "</tool_call>" | 用途: 
            工具调用结束。
            (Tool Call End.)
    - ID: 151659  | Token: "<|fim_prefix|>" | 用途: 
            FIM 前缀：用于代码补全任务，标记需要被补全的代码的前缀部分。
            (FIM Prefix: Used in code completion tasks, marking the prefix part of the code to be completed.)
    - ID: 151660  | Token: "<|fim_middle|>" | 用途: 
            FIM 中间：用于代码补全任务，标记被补全代码的中间部分（或缺失部分）。
            (FIM Middle: Used in code completion tasks, marking the middle part (or missing part) of the code to be completed.)
    - ID: 151661  | Token: "<|fim_suffix|>" | 用途: 
            FIM 后缀：用于代码补全任务，标记需要被补全代码的后缀部分。
            (FIM Suffix: Used in code completion tasks, marking the suffix part of the code to be completed.)
    - ID: 151662  | Token: "<|fim_pad|>" | 用途: 
            FIM 填充：用于填充代码补全序列，保持长度一致。
            (FIM Padding: Used to pad code completion sequences to maintain consistent length.)
    - ID: 151663  | Token: "<|repo_name|>" | 用途: 
            代码仓库名：用于代码模型中，标记代码所属的仓库名称。
            (Repository Name: Used in code models to mark the name of the repository the code belongs to.)
    - ID: 151664  | Token: "<|file_sep|>" | 用途: 
            文件分隔符：用于分隔模型输入中不同的代码文件内容。
            (File Separator: Used to separate the content of different code files in the model input.)
    - ID: 151665  | Token: "<tool_response>" | 用途: 
            工具响应开始：用于 RAG 或 Agent 任务中。
            (Tool Response Start: Used for RAG or Agent tasks.)
    - ID: 151666  | Token: "</tool_response>" | 用途: 
            工具响应结束。
            (Tool Response End.)
    - ID: 151667  | Token: "<think>" | 用途: 
            思考开始：用于标记模型的内部推理或思考过程的开始 (Chain-of-Thought, CoT)。
            (Think Start: Marks the beginning of the model's internal reasoning or Chain-of-Thought process.)
    - ID: 151668  | Token: "</think>" | 用途: 
            思考结束：用于标记模型的内部推理或思考过程的结束。
            (Think End: Marks the end of the model's internal reasoning or thinking process.)

  --- 核心控制 Token (Core Control Tokens) ---
    (Directly read from top-level config fields)
    - Pad Token (填充):   "<|endoftext|>"
      Pad Token:   "<|endoftext|>" (Used to pad the sequence to a uniform length)
    - BOS Token (句首):   "N/A"
      BOS Token (Beginning of Sequence):   "N/A" (Marks the start of an input sequence)
    - EOS Token (句尾):   "<|im_end|>"
      EOS Token (End of Sequence):   "<|im_end|>" (Marks the end of a sequence or stops generation)
    - UNK Token (未知):   "N/A"
      UNK Token (Unknown):   "N/A" (Represents words not found in the vocabulary)
[TOKENIZER CONFIG] 配置信息读取完毕。✅
------------------------------------------

## [STEP 4/5] 正在加载并显示模型结构 (Loading and Displaying Model Architecture)...
------------------------------------------

[INPUTS] 模型输入张量(Tensor)信息 - 总计 4 个:
(Model Input Tensor Information - Total 4 inputs)
  Input[0] 名称(Name): "input_ids"
  Input[0] Name: "input_ids"
    - 形状/维度(Shape): Shape : {?,?} (表示张量的尺寸。其中 [如果出现**?**] 表示这是一个**动态维度**，如批次大小或序列长度。)
    - Shape/Dimensions: Shape : {?,?} (Represents the size of the tensor. [If **?** appears] indicates a **dynamic dimension**, such as batch size or sequence length.)
    - 数据类型(Type): i64 (如 i64: 64位整数，f32: 32位浮点数。)
    - Data Type: i64 (E.g., i64: 64-bit integer, f32: 32-bit floating point.)
    - 用途说明(Usage): 核心输入：分词器输出的**Token ID 序列**，是模型推理的主要数据。
(Core Input: The sequence of **Token IDs** output by the tokenizer, which is the main data for model inference.)

  Input[1] 名称(Name): "attention_mask"
  Input[1] Name: "attention_mask"
    - 形状/维度(Shape): Shape : {?,?} (表示张量的尺寸。其中 [如果出现**?**] 表示这是一个**动态维度**，如批次大小或序列长度。)
    - Shape/Dimensions: Shape : {?,?} (Represents the size of the tensor. [If **?** appears] indicates a **dynamic dimension**, such as batch size or sequence length.)
    - 数据类型(Type): i64 (如 i64: 64位整数，f32: 32位浮点数。)
    - Data Type: i64 (E.g., i64: 64-bit integer, f32: 32-bit floating point.)
    - 用途说明(Usage): 关键辅助：用于**掩盖填充部分 (Padding)**，确保模型忽略无效 Token。
(Key Auxiliary: Used to **mask padding parts**, ensuring the model ignores invalid tokens.)

  Input[2] 名称(Name): "position_ids"
  Input[2] Name: "position_ids"
    - 形状/维度(Shape): Shape : {?,?} (表示张量的尺寸。其中 [如果出现**?**] 表示这是一个**动态维度**，如批次大小或序列长度。)
    - Shape/Dimensions: Shape : {?,?} (Represents the size of the tensor. [If **?** appears] indicates a **dynamic dimension**, such as batch size or sequence length.)
    - 数据类型(Type): i64 (如 i64: 64位整数，f32: 32位浮点数。)
    - Data Type: i64 (E.g., i64: 64-bit integer, f32: 32-bit floating point.)
    - 用途说明(Usage): 位置信息：提供每个 Token 在序列中的**顺序索引**，用于 Transformer 结构。
(Positional Information: Provides the **sequential index** of each token in the sequence, used for the Transformer architecture.)

  Input[3] 名称(Name): "beam_idx"
  Input[3] Name: "beam_idx"
    - 形状/维度(Shape): Shape : {?} (表示张量的尺寸。其中 [如果出现**?**] 表示这是一个**动态维度**，如批次大小或序列长度。)
    - Shape/Dimensions: Shape : {?} (Represents the size of the tensor. [If **?** appears] indicates a **dynamic dimension**, such as batch size or sequence length.)
    - 数据类型(Type): i32 (如 i64: 64位整数，f32: 32位浮点数。)
    - Data Type: i32 (E.g., i64: 64-bit integer, f32: 32-bit floating point.)
    - 用途说明(Usage): 解码控制：用于 **Beam Search** 等复杂生成策略，标识当前 Token 所属的候选序列。
(Decoding Control: Used for complex generation strategies like **Beam Search**, identifying the candidate sequence the current token belongs to.)


[OUTPUTS] 模型输出张量(Tensor)信息 - 总计 1 个:
(Model Output Tensor Information - Total 1 outputs)
  Output[0] 名称(Name): "logits"
  Output[0] Name: "logits"
    - 形状/维度(Shape): Shape : {?,?,151936} (表示张量的尺寸。其中 [如果出现**?**] 表示这是一个**动态维度** (如批次大小或序列长度)。最后维度通常是词汇表大小，决定了模型能生成多少种不同的 Token。)
    - Shape/Dimensions: Shape : {?,?,151936} (Represents the tensor's dimensions. [If **?** appears] indicates a **dynamic dimension** (such as batch size or sequence length).The last dimension is usually the vocabulary size, determining the number of unique Tokens the model can generate.)
    - 数据类型(Type): f32 (如 f32: 32位浮点数，常用于存储概率分数。)
    - Data Type: f32 (E.g., f32: 32-bit floating point, often used to store probability scores.)
    - 用途说明(Usage): 核心输出：模型的**原始预测分数**。最后一个维度 (151936) 是**词汇表大小**，需 Softmax 或采样处理。
    - Usage Description: Core Output: The model's **raw prediction scores**. The last dimension (151936) is the **vocabulary size** and requires Softmax or sampling.

[SUCCESS] 模型读取并编译完成 (Model Read and Compile Finish)! ✅
(Model successfully read, compiled, and ready for inference!)
(编译成功了！没报错！如果你的项目出了问题，建议从自身找问题，OpenVINO表示模型没问题！ 😉)
(Compilation succeeded! No errors reported! If your project encounters issues, please look inward. OpenVINO states the model is fine! 😉)

------------------------------------------

## [STEP 5/5] 所有信息加载完毕 (All Information Loaded)! ✅
------------------------------------------
```
---

## ⚖️ License

Licensed under the **MIT License**.  
