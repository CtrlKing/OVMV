using OpenVinoSharp;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;
using Tokenizers.DotNet;

namespace openvinoMXCKQ
{
    public partial class Form1 : Form
    {
        // 简化变量名
        private Tokenizer? tokenizer;             // 分词器对象
        private string? modelDirectory;          // 模型文件所在目录
        private CompiledModel? compiledModel;     // 编译后的模型实例

        public Form1()
        {
            InitializeComponent();
            // 确保主窗体允许拖放操作
            this.AllowDrop = true;
            // 1. 设置默认提示文本和颜色
            txtBox_select.Text = "请拖入或选择OpenVINO模型文件 (.xml)";
            txtBox_select.ForeColor = System.Drawing.Color.Gray;

            // 2. 绑定事件
            txtBox_select.GotFocus += RemoveWatermark; // 获取焦点时移除水印
            txtBox_select.LostFocus += ShowWatermark;  // 失去焦点时显示水印
        }
        // === 移除水印的事件处理程序 (用户点击文本框时) ===
        private void RemoveWatermark(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            // 仅当文本是默认提示时才移除
            if (tb.Text == "请拖入或选择OpenVINO模型文件 (.xml)")
            {
                tb.Text = "";
                tb.ForeColor = System.Drawing.Color.Black; // 恢复正常输入颜色
            }
        }

        // === 显示水印的事件处理程序 (用户离开文本框时) ===
        private void ShowWatermark(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            // 仅当文本框为空时才显示水印
            if (string.IsNullOrWhiteSpace(tb.Text))
            {
                tb.Text = "请拖入或选择OpenVINO模型文件 (.xml)";
                tb.ForeColor = System.Drawing.Color.Gray;
            }
        }
        // === 拖拽事件（保持原样）===
        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length > 0 && files[0].EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                {
                    e.Effect = DragDropEffects.Copy;
                    return;
                }
            }
            e.Effect = DragDropEffects.None;
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length > 0)
            {
                string xmlPath = files[0];
                if (xmlPath.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                {
                    LoadModelProcedure(xmlPath);
                }
            }
        }

        private void btn_select_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Select OpenVINO Model File (.xml)";
                openFileDialog.Filter = "OpenVINO Model Files (*.xml)|*.xml|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedFile = openFileDialog.FileName;
                    LoadModelProcedure(selectedFile);
                }
            }
        }

        // === 核心逻辑（保持原样）===
        private void LoadModelProcedure(string xmlPath)
        {
            compiledModel?.Dispose();
            compiledModel = null;
            tokenizer = null;

            txtBox_select.Text = xmlPath;
            modelDirectory = Path.GetDirectoryName(xmlPath);
            string binFile = Path.ChangeExtension(xmlPath, ".bin");

            textBox_info.Clear();
            textBox_info.AppendText($"[INFO] 开始加载(Start Load): {Path.GetFileName(xmlPath)}\r\n");

            if (!File.Exists(binFile))
            {
                textBox_info.AppendText($"[WARN] 权重文件未找到(Weight file NOT found): 请检查同目录下是否存在相应的 {Path.GetFileName(binFile)} 文件。\r\n");
            }

            // --- 定义分隔符和块说明 ---
            string separator = "------------------------------------------"; // 约 42 个字符长

            // ------------------------------------------------------------
            // 步骤 2: 尝试加载分词器
            // ------------------------------------------------------------

            // 块说明
            textBox_info.AppendText("\n## [STEP 1/5] 尝试加载分词器...\r\n");
            textBox_info.AppendText(separator + "\r\n");

            // 核心调用
            TryLoadTokenizer();

            // 块结束分隔
            textBox_info.AppendText(separator + "\r\n");


            // 假设 separator 已经定义为你的分隔符字符串

            // ------------------------------------------------------------
            // 步骤 3: 尝试加载并显示模型配置信息 (Config Info)
            // ------------------------------------------------------------

            // 块说明
            textBox_info.AppendText("\n## [STEP 2/5] 尝试加载并显示模型配置信息 (Loading Model Config Info)...\r\n");
            textBox_info.AppendText(separator + "\r\n");

            // 核心调用
            LoadAndDisplayConfigInfo();

            // 块结束分隔
            textBox_info.AppendText(separator + "\r\n");


            // ------------------------------------------------------------
            // 步骤 4: 尝试加载并显示分词器配置信息 (Tokenizer Config)
            // ------------------------------------------------------------

            // 块说明
            textBox_info.AppendText("\n## [STEP 3/5] 尝试加载并显示分词器配置信息 (Loading Tokenizer Config Info)...\r\n");
            textBox_info.AppendText(separator + "\r\n");

            // 核心调用
            LoadAndDisplayTokenizerConfig();

            // 块结束分隔
            textBox_info.AppendText(separator + "\r\n");


            // ------------------------------------------------------------
            // 步骤 5: 加载并显示模型结构 (Model Info)
            // ------------------------------------------------------------

            // 块说明
            textBox_info.AppendText("\n## [STEP 4/5] 正在加载并显示模型结构 (Loading and Displaying Model Architecture)...\r\n");
            textBox_info.AppendText(separator + "\r\n");

            // 核心调用
            LoadAndDisplayModelInfo(xmlPath);

            // 块结束分隔 (模型的详细信息将在 LoadAndDisplayModelInfo 内部输出)
            textBox_info.AppendText(separator + "\r\n");

            // ------------------------------------------------------------
            // 步骤 6: 完成并滚动
            // ------------------------------------------------------------

            // 最终完成提示
            textBox_info.AppendText("\n## [STEP 5/5] 所有信息加载完毕 (All Information Loaded)! ✅\r\n");
            textBox_info.AppendText(separator + "\r\n");

            // 光标返回顶部
            ScrollLogToTop();
        }

        private void TryLoadTokenizer()
        {
            if (string.IsNullOrEmpty(modelDirectory)) return;
            string path = Path.Combine(modelDirectory, "tokenizer.json");

            if (File.Exists(path))
            {
                try
                {
                    tokenizer = new Tokenizer(path);
                    textBox_info.AppendText("[INFO] 分词器加载成功(Tokenizer LOAD SUCCESS)! ✅\r\n");
                }
                catch (Exception ex)
                {
                    textBox_info.AppendText($"[ERROR] 分词器加载失败(Tokenizer load FAIL): {ex.Message}\r\n");
                }
            }
        }

        // === 关键修改：增强模型信息说明和错误提示 ===
        private void LoadAndDisplayModelInfo(string xmlPath)
        {
            try
            {
                var log = new StringBuilder();
                using var core = new Core();
                using var model = core.read_model(xmlPath);

                // --- Input Info ---
                var inputs = model.inputs();
                log.AppendLine($"\n[INPUTS] 模型输入张量(Tensor)信息 - 总计 {inputs.Count} 个:");
                log.AppendLine($"(Model Input Tensor Information - Total {inputs.Count} inputs)");
                for (int i = 0; i < inputs.Count; i++)
                {
                    var input = inputs[i];
                    string name = input.get_any_name();
                    string shapeStr = input.get_partial_shape().to_string();
                    string typeStr = input.get_element_type().get_type_name();

                    // 根据张量名称提供详细说明
                    string usageHint = "";
                    switch (name.ToLower())
                    {
                        case "input_ids":
                            usageHint = "核心输入：分词器输出的**Token ID 序列**，是模型推理的主要数据。\r\n(Core Input: The sequence of **Token IDs** output by the tokenizer, which is the main data for model inference.)";
                            break;
                        case "attention_mask":
                            usageHint = "关键辅助：用于**掩盖填充部分 (Padding)**，确保模型忽略无效 Token。\r\n(Key Auxiliary: Used to **mask padding parts**, ensuring the model ignores invalid tokens.)";
                            break;
                        case "position_ids":
                            usageHint = "位置信息：提供每个 Token 在序列中的**顺序索引**，用于 Transformer 结构。\r\n(Positional Information: Provides the **sequential index** of each token in the sequence, used for the Transformer architecture.)";
                            break;
                        case "beam_idx":
                            usageHint = "解码控制：用于 **Beam Search** 等复杂生成策略，标识当前 Token 所属的候选序列。\r\n(Decoding Control: Used for complex generation strategies like **Beam Search**, identifying the candidate sequence the current token belongs to.)";
                            break;
                        default:
                            usageHint = "通用输入张量。\r\n(General Input Tensor.)";
                            break;
                    }

                    log.AppendLine($"  Input[{i}] 名称(Name): \"{name}\"");
                    log.AppendLine($"  Input[{i}] Name: \"{name}\"");
                    log.AppendLine($"    - 形状/维度(Shape): {shapeStr} (表示张量的尺寸。其中 [如果出现**?**] 表示这是一个**动态维度**，如批次大小或序列长度。)");
                    log.AppendLine($"    - Shape/Dimensions: {shapeStr} (Represents the size of the tensor. [If **?** appears] indicates a **dynamic dimension**, such as batch size or sequence length.)");
                    log.AppendLine($"    - 数据类型(Type): {typeStr} (如 i64: 64位整数，f32: 32位浮点数。)");
                    log.AppendLine($"    - Data Type: {typeStr} (E.g., i64: 64-bit integer, f32: 32-bit floating point.)");
                    log.AppendLine($"    - 用途说明(Usage): {usageHint}\r\n"); // 增加用途说明
                }

                // --- Output Info ---
                var outputs = model.outputs();
                log.AppendLine($"\n[OUTPUTS] 模型输出张量(Tensor)信息 - 总计 {outputs.Count} 个:");
                log.AppendLine($"(Model Output Tensor Information - Total {outputs.Count} outputs)"); // 英文对照

                for (int i = 0; i < outputs.Count; i++)
                {
                    var output = outputs[i];
                    string name = output.get_any_name();
                    string shapeStr = output.get_partial_shape().to_string();
                    string typeStr = output.get_element_type().get_type_name();

                    // 获取形状数组，尝试提取词汇表大小
                    // 假设形状字符串格式为 {dim0,dim1,dim2}
                    string[] dims = shapeStr.Trim('{', '}').Split(',');
                    string lastDimStr = dims.Length > 0 ? dims[dims.Length - 1].Trim() : "N/A";

                    string outputUsageHint = "";
                    string outputUsageHintEn = "";

                    // --- 核心 Logits 识别 ---
                    // 识别依据：名称为 "logits" 且具有至少三个维度 (批次, 序列, 词汇表)
                    if (name.ToLower() == "logits" && dims.Length >= 3)
                    {
                        outputUsageHint = $"核心输出：模型的**原始预测分数**。最后一个维度 ({lastDimStr}) 是**词汇表大小**，需 Softmax 或采样处理。";
                        outputUsageHintEn = $"Core Output: The model's **raw prediction scores**. The last dimension ({lastDimStr}) is the **vocabulary size** and requires Softmax or sampling.";
                    }

                    // --- 动态维度解释 ---
                    string shapeExplanationCn = "表示张量的尺寸。其中 [如果出现**?**] 表示这是一个**动态维度** (如批次大小或序列长度)。";
                    string shapeExplanationEn = "Represents the tensor's dimensions. [If **?** appears] indicates a **dynamic dimension** (such as batch size or sequence length).";
                    if (name.ToLower() == "logits" && dims.Length >= 3)
                    {
                        shapeExplanationCn += "最后维度通常是词汇表大小，决定了模型能生成多少种不同的 Token。";
                        shapeExplanationEn += "The last dimension is usually the vocabulary size, determining the number of unique Tokens the model can generate.";
                    }


                    log.AppendLine($"  Output[{i}] 名称(Name): \"{name}\"");
                    log.AppendLine($"  Output[{i}] Name: \"{name}\"");

                    // 输出形状和解释
                    log.AppendLine($"    - 形状/维度(Shape): {shapeStr} ({shapeExplanationCn})");
                    log.AppendLine($"    - Shape/Dimensions: {shapeStr} ({shapeExplanationEn})");

                    log.AppendLine($"    - 数据类型(Type): {typeStr} (如 f32: 32位浮点数，常用于存储概率分数。)");
                    log.AppendLine($"    - Data Type: {typeStr} (E.g., f32: 32-bit floating point, often used to store probability scores.)");

                    if (!string.IsNullOrEmpty(outputUsageHint))
                    {
                        log.AppendLine($"    - 用途说明(Usage): {outputUsageHint}");
                        log.AppendLine($"    - Usage Description: {outputUsageHintEn}\r\n");
                    }
                    else
                    {
                        log.AppendLine($"\r\n");
                    }
                }


                // --- Compile Model ---
                compiledModel = core.compile_model(model, "CPU");

                // 1. 输出第一部分（标题/状态）
                log.AppendLine("[SUCCESS] 模型读取并编译完成 (Model Read and Compile Finish)! ✅");

                // 英文对照
                log.AppendLine("(Model successfully read, compiled, and ready for inference!)");

                // 2. 输出第二部分（幽默提示/注释）
                log.AppendLine("(编译成功了！没报错！如果你的项目出了问题，建议从自身找问题，OpenVINO表示模型没问题！ 😉)");
                // 英文幽默对照
                log.AppendLine("(Compilation succeeded! No errors reported! If your project encounters issues, please look inward. OpenVINO states the model is fine! 😉)");

                textBox_info.AppendText(log.ToString() + "\r\n");
            }
            catch (Exception ex)
            {
                // 增强的错误提示 (Enhanced Error Message)
                textBox_info.AppendText($"\n[FATAL ERROR] 无法加载或编译模型 (Cannot load or compile model): {ex.Message}\r\n");

                // 英文对照
                textBox_info.AppendText($"\n[FATAL ERROR] Cannot load or compile model: {ex.Message}\r\n");

                textBox_info.AppendText($"[GUIDANCE] 请选择正确的 OpenVINO 模型 XML 文件 (一般是openvino_model.xml)。如果文件路径选择无误，**模型文件 (.xml)** 或其对应的**权重文件 (.bin)** 可能已损坏。\r\n");

                // 英文对照
                textBox_info.AppendText($"[GUIDANCE] Please select the correct OpenVINO Model XML file (usually openvino_model.xml). If the file path is correct, the **Model file (.xml)** or its corresponding **Weight file (.bin)** might be corrupted.\r\n\r\n");
            }
        }
        // === 新增方法：加载并显示 config.json 信息 ===
        private void LoadAndDisplayConfigInfo()
        {
            if (string.IsNullOrEmpty(modelDirectory)) return;

            string configPath = Path.Combine(modelDirectory, "config.json");

            if (!File.Exists(configPath))
            {
                textBox_info.AppendText($"[WARN] 配置文件未找到(Config file NOT found): 缺少 {Path.GetFileName(configPath)} 文件，无法获取模型详细超参数。\r\n");
                return;
            }

            textBox_info.AppendText("\n[CONFIG] 开始读取模型配置(Start reading model config):\r\n");

            try
            {
                string jsonString = File.ReadAllText(configPath);
                using var document = JsonDocument.Parse(jsonString);
                var root = document.RootElement;
                var log = new StringBuilder();

                log.AppendLine($"  - **模型架构(Architecture):** {GetJsonString(root, "model_type")} ({GetJsonArrayValue(root, "architectures")})");
                log.AppendLine($"  - **词汇表大小(Vocab Size):** {GetJsonIntValue(root, "vocab_size")} (模型可识别的Token总数)");
                log.AppendLine($"  - **隐藏层大小(Hidden Size):** {GetJsonIntValue(root, "hidden_size")} (模型内部向量的维度)");
                log.AppendLine($"  - **层数(Num Hidden Layers):** {GetJsonIntValue(root, "num_hidden_layers")} (Transformer块的数量)");
                log.AppendLine($"  - **注意力头数(Num Attention Heads):** {GetJsonIntValue(root, "num_attention_heads")} (每个Transformer块中注意力头的数量)");
                log.AppendLine($"  - **KV头数(Num Key Value Heads):** {GetJsonIntValue(root, "num_key_value_heads")} (多头注意力中键值矩阵的头数，用于优化性能)");
                log.AppendLine($"  - **最大位置编码(Max Pos Embeddings):** {GetJsonIntValue(root, "max_position_embeddings")} (模型能处理的最大序列长度)");
                log.AppendLine($"  - **激活函数(Hidden Act):** {GetJsonString(root, "hidden_act")} (模型内部使用的激活函数，如 silu)");
                log.AppendLine($"  - **BOS ID:** {GetJsonIntValue(root, "bos_token_id")} (Beginning-of-Sentence Token ID)");
                log.AppendLine($"  - **EOS ID:** {GetJsonIntValue(root, "eos_token_id")} (End-of-Sentence Token ID)");
                log.AppendLine($"  - **Torch DType:** {GetJsonString(root, "torch_dtype")} (模型训练时使用的数据精度，如 bfloat16)");

                textBox_info.AppendText(log.ToString());
                textBox_info.AppendText("[CONFIG] 配置信息读取完毕。✅\r\n");

            }
            catch (Exception ex)
            {
                textBox_info.AppendText($"[ERROR] 配置解析失败(Config parse FAIL): {ex.Message}\r\n");
                textBox_info.AppendText($"[GUIDANCE] 请检查 config.json 文件格式是否正确。\r\n");
            }
        }
        // === 新增方法：加载并显示 tokenizer_config.json 信息 ===
        private void LoadAndDisplayTokenizerConfig()
        {
            if (string.IsNullOrEmpty(modelDirectory)) return;

            string configPath = Path.Combine(modelDirectory, "tokenizer_config.json");

            if (!File.Exists(configPath))
            {
                textBox_info.AppendText($"[WARN] Tokenizer配置未找到(Tokenizer Config NOT found): 缺少 {Path.GetFileName(configPath)} 文件，无法获取分词器详细信息。\r\n");
                return;
            }

            textBox_info.AppendText("\n[TOKENIZER CONFIG] 开始读取分词器配置(Start reading tokenizer config):\r\n");

            try
            {
                string jsonString = File.ReadAllText(configPath);
                using var document = JsonDocument.Parse(jsonString);
                var root = document.RootElement;
                var log = new StringBuilder();

                // --- 基础信息 ---
                log.AppendLine($"  - **分词器类别(Class):** {GetJsonString(root, "tokenizer_class")}");
                log.AppendLine($"    (Tokenizer Class: {GetJsonString(root, "tokenizer_class")})");
                log.AppendLine($"  - **最大长度(Max Length):** {GetJsonIntValue(root, "model_max_length")} (分词器能处理的最大Token数)");
                log.AppendLine($"    (Max Length: {GetJsonIntValue(root, "model_max_length")} tokens max processed)");

                // --- Chat Template (对话模板) ---
                if (root.TryGetProperty("chat_template", out JsonElement chatTemplateElement))
                {
                    string chatTemplate = chatTemplateElement.GetString() ?? "N/A";

                    log.AppendLine("\n  --- 对话模板 (Chat Template) ---");
                    log.AppendLine("    (Used for multi-turn dialogue formatting)");
                    log.AppendLine("    模板内容(Template Content):\r\n");

                    // 格式化输出模板内容
                    log.AppendLine("```jinja");
                    // 将模板内容每行前添加缩进，以保持格式清晰
                    foreach (var line in chatTemplate.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        log.AppendLine($"  {line}");
                    }
                    log.AppendLine("```");
                }
                else
                {
                    log.AppendLine("\n  --- 对话模板 (Chat Template) ---");
                    log.AppendLine("    (Used for multi-turn dialogue formatting)");
                    log.AppendLine("    **状态(Status):** 不存在(Not Exist)");
                }

                // --- 扩展特殊 Token 列表 (Added Tokens) ---
                log.AppendLine("\n  --- 扩展特殊 Token (Extended Special Tokens) 及其用途 ---");
                log.AppendLine("    (Tokens added outside of the base vocabulary)");

                if (root.TryGetProperty("added_tokens_decoder", out JsonElement addedTokensDecoder) && addedTokensDecoder.ValueKind == JsonValueKind.Object)
                {
                    foreach (var property in addedTokensDecoder.EnumerateObject())
                    {
                        string id = property.Name;
                        if (property.Value.TryGetProperty("content", out JsonElement contentElement))
                        {
                            string token = contentElement.GetString() ?? "N/A";
                            string explanation = GetSpecialTokenUsage(token);

                            // explanation 内部包含 \r\n，用于实现三行对齐 (Token信息 -> 中文说明 -> 英文说明)
                            log.AppendLine($"    - ID: {id,-7} | Token: \"{token}\" | 用途: {explanation}");
                        }
                    }
                }
                else
                {
                    log.AppendLine("    **状态(Status):** 未找到 added_tokens_decoder，无扩展 Token。(Added_tokens_decoder not found, no extended tokens.)");
                }


                // --- 核心控制 Token (Top-Level Config Tokens) ---
                // 这是对文件中顶级字段的直接读取，防止这些关键信息被遗漏。
                log.AppendLine("\n  --- 核心控制 Token (Core Control Tokens) ---");
                log.AppendLine("    (Directly read from top-level config fields)");

                // --- 核心 Token 列表 ---
                // Note: 使用 GetJsonString() 读取 JSON 根元素中的对应字段值。
                log.AppendLine($"    - Pad Token (填充):   \"{GetJsonString(root, "pad_token")}\"");
                log.AppendLine($"      Pad Token:   \"{GetJsonString(root, "pad_token")}\" (Used to pad the sequence to a uniform length)");

                log.AppendLine($"    - BOS Token (句首):   \"{GetJsonString(root, "bos_token")}\"");
                log.AppendLine($"      BOS Token (Beginning of Sequence):   \"{GetJsonString(root, "bos_token")}\" (Marks the start of an input sequence)");

                log.AppendLine($"    - EOS Token (句尾):   \"{GetJsonString(root, "eos_token")}\"");
                log.AppendLine($"      EOS Token (End of Sequence):   \"{GetJsonString(root, "eos_token")}\" (Marks the end of a sequence or stops generation)");

                log.AppendLine($"    - UNK Token (未知):   \"{GetJsonString(root, "unk_token")}\"");
                log.AppendLine($"      UNK Token (Unknown):   \"{GetJsonString(root, "unk_token")}\" (Represents words not found in the vocabulary)");

                // --- 输出到文本框 ---
                textBox_info.AppendText(log.ToString());
                textBox_info.AppendText("[TOKENIZER CONFIG] 配置信息读取完毕。✅\r\n");

            }
            catch (Exception ex)
            {
                textBox_info.AppendText($"[ERROR] Tokenizer配置解析失败(Config parse FAIL): {ex.Message}\r\n");
                textBox_info.AppendText($"[GUIDANCE] 请检查 {Path.GetFileName(configPath)} 文件格式是否正确。\r\n");
            }
        }

        // === 辅助方法：判断并说明常见特殊 Token 的用途 ===
        private string GetSpecialTokenUsage(string token)
        {
            // 常见特殊 Token 及其说明的字典
            var commonTokens = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                // --- 常见 BERT & 通用标记 ---
                {"[PAD]", "\r\n            填充(Padding)：用于将序列长度补齐到批次中的最大长度。\r\n            (Used to pad the sequence length to the maximum length in the batch.)"},
                {"<pad>", "\r\n            填充(Padding)：用于将序列长度补齐到批次中的最大长度。\r\n            (Used to pad the sequence length to the maximum length in the batch.)"},
                {"[CLS]", "\r\n            分类(Classification)：BERT 等模型中用于表示序列开始或进行分类任务。\r\n            (Used in BERT-like models to mark the start of a sequence or for classification tasks.)"},
                {"[SEP]", "\r\n            分隔(Separation)：用于分隔两个不同的句子或文本段落。\r\n            (Used to separate two different sentences or text segments.)"},
                {"[MASK]", "\r\n            掩码(Masking)：用于掩盖输入中的某些词，进行自监督学习。\r\n            (Used to mask certain words in the input for self-supervised learning.)"},
                {"<unk>", "\r\n            未知词(Unknown)：表示不在词汇表中的词汇。\r\n            (Unknown Word: Represents vocabulary not present in the tokenizer's vocabulary.)"},
                {"[UNK]", "\r\n            未知词(Unknown)：与 <unk> 功能类似，是另一种表示方法。\r\n            (Unknown Word: Another way to represent an unknown word, similar to <unk>.)"},
    
                // --- LLM 通用 BOS/EOS ---
                {"<bos>", "\r\n            序列开始 (BOS)：通用 Transformer 模型常用的序列起始标记。\r\n            (Beginning of Sequence: Commonly used Sequence Start marker in general Transformer models.)"},
                {"<eos>", "\r\n            序列结束 (EOS)：通用 Transformer 模型常用的序列结束标记。\r\n            (End of Sequence: Commonly used Sequence End marker in general Transformer models.)"},
                {"[BOS]", "\r\n            序列开始 (BOS)：与 <bos> 功能类似，是另一种表示方法，常见于基于 BERT 家族的模型。\r\n            (Sequence Start: Another way to represent the start of a sequence, similar to <bos>, often seen in BERT-family models.)"},
                {"[EOS]", "\r\n            序列结束 (EOS)：与 <eos> 功能类似，是另一种表示方法。\r\n            (Sequence End: Similar to EOS, marks the end of an input sequence.)"},
                {"[SOS]", "\r\n            序列开始 (SOS)：与 BOS 功能类似，表示输入序列的开始。\r\n            (Start of Sequence: Similar to BOS, marks the start of an input sequence.)"},
    
                // --- Llama/GPT-2/Qwen 风格 ---
                {"<s>", "\r\n            通用序列开始 (BOS)：广泛用于 RoBERTa, GPT-2, 现代 Llama/Mistral 等模型。\r\n            (Sequence Start: Start marker for models like RoBERTa, GPT-2, modern Llama/Mistral.)"},
                {"</s>", "\r\n            通用序列结束 (EOS)：广泛用于 RoBERTa, GPT-2, 现代 Llama/Mistral 等模型。\r\n            (Sequence End: End marker for models like RoBERTa, GPT-2, modern Llama/Mistral.)"},
                {"<s_bos>", "\r\n            Llama 专用序列开始 (BOS)：早期 Llama 模型中用于标记序列的起始。\r\n            (Llama-specific Beginning of Sequence: Used to mark the start of a sequence in older Llama models.)"},
                {"<|endoftext|>", "\r\n            句尾/填充：常用于表示序列结束或作为 Padding Token。\r\n            (Often used to denote the end of a sequence or as a Padding Token.)"},
    
                // --- 对话/Agent 角色和控制 ---
                {"<|im_start|>", "\r\n            对话开始：Qwen 系列模型中用于标记新的对话轮次开始（如 user, system, assistant）。\r\n            (Conversation Start: Used in Qwen models to mark the beginning of a new conversation turn.)"},
                {"<|im_end|>", "\r\n            对话结束：Qwen 系列模型中用于标记对话轮次的结束。\r\n            (Conversation End: Used in Qwen models to mark the end of a conversation turn.)"},
                {"<|im_sep|>", "\r\n            分隔符：用于分隔对话中的不同部分。\r\n            (Separator: Used to separate different parts within a dialogue.)"},
                {"<|user|>", "\r\n            对话角色：标记用户输入内容的开始。\r\n            (Conversation Role: Marks the beginning of user input content.)"},
                {"<|assistant|>", "\r\n            对话角色：标记模型生成内容的开始。\r\n            (Conversation Role: Marks the beginning of model generated content.)"},
                {"<|system|>", "\r\n            系统角色：用于标记系统或预设指令的输入。\r\n            (System Role: Used to mark input from the system or predefined instructions.)"},
                {"<|end_of_turn|>", "\r\n            回合结束：用于标记对话中一方言论的结束。\r\n            (End of Turn: Used to mark the end of one party's utterance in a dialogue.)"},
                {"<think>", "\r\n            思考开始：用于标记模型的内部推理或思考过程的开始 (Chain-of-Thought, CoT)。\r\n            (Think Start: Marks the beginning of the model's internal reasoning or Chain-of-Thought process.)"},
                {"</think>", "\r\n            思考结束：用于标记模型的内部推理或思考过程的结束。\r\n            (Think End: Marks the end of the model's internal reasoning or thinking process.)"},
                {"<tool_call>", "\r\n            工具调用开始：用于 RAG 或 Agent 任务中。\r\n            (Tool Call Start: Used for RAG or Agent tasks.)"},
                {"</tool_call>", "\r\n            工具调用结束。\r\n            (Tool Call End.)"},
                {"<tool_response>", "\r\n            工具响应开始：用于 RAG 或 Agent 任务中。\r\n            (Tool Response Start: Used for RAG or Agent tasks.)"},
                {"</tool_response>", "\r\n            工具响应结束。\r\n            (Tool Response End.)"},
    
                // --- 多模态与视觉 (已修正 `<|box_end|>`) ---
                {"<|object_ref_start|>", "\r\n            对象引用开始：用于标记模型输入中对象引用的起始位置。\r\n            (Object Reference Start: Marks the beginning of an object reference in the model input.)"},
                {"<|object_ref_end|>", "\r\n            对象引用结束：用于标记模型输入中对象引用的结束位置。\r\n            (Object Reference End: Marks the end of an object reference in the model input.)"},
                {"<|box_start|>", "\r\n            边界框开始：用于标记图像中特定边界框区域的起始（常用于定位）。\r\n            (Bounding Box Start: Marks the start of a specific bounding box region within an image, often for localization.)"},
                {"**<|box_end|>**", "\r\n            边界框结束：用于标记图像中特定边界框区域的结束。\r\n            (Bounding Box End: Marks the end of a specific bounding box region within an image.)"}, // **修正了键名**
                {"<|quad_start|>", "\r\n            四边形开始：用于标记图像中四边形区域的起始（比边界框更灵活）。\r\n            (Quadrilateral Start: Marks the start of a quadrilateral region in an image, more flexible than a bounding box.)"},
                {"<|quad_end|>", "\r\n            四边形结束：用于标记图像中四边形区域的结束。\r\n            (Quadrilateral End: Marks the end of a quadrilateral region in an image.)"},
                {"<|vision_start|>", "\r\n            视觉数据开始：用于标记多模态模型中视觉数据的起始（如图像特征序列）。\r\n            (Vision Data Start: Marks the beginning of visual data (e.g., image feature sequence) in multimodal models.)"},
                {"<|vision_end|>", "\r\n            视觉数据结束：用于标记多模态模型中视觉数据的结束。\r\n            (Vision Data End: Marks the end of visual data in multimodal models.)"},
                {"<|vision_pad|>", "\r\n            视觉填充：用于填充视觉特征序列，以达到统一的输入长度。\r\n            (Vision Padding: Used to pad visual feature sequences to a uniform input length.)"},
                {"<|image_pad|>", "\r\n            图像填充：用于填充图像特征序列。\r\n            (Image Padding: Used to pad image feature sequences.)"},
                {"<|video_pad|>", "\r\n            视频填充：用于填充视频特征序列。\r\n            (Video Padding: Used to pad video feature sequences.)"},
                {"<image>", "\r\n            图像占位符：多模态模型中用于插入图像。\r\n            (Image Placeholder: Used in multimodal models to insert an image.)"},
                {"<audio>", "\r\n            音频占位符：多模态模型中用于插入音频。\r\n            (Audio Placeholder: Used in multimodal models to insert audio.)"},
                {"<video>", "\r\n            视频占位符：多模态模型中用于插入视频。\r\n            (Video Placeholder: Used in multimodal models to insert video.)"},
    
                // === T5/占位符 & 代码/文本控制 ===
                {"<extra_id_0>", "\r\n            占位符：T5 模型中用于填空或生成的多功能特殊 Token。\r\n            (Placeholder: Multi-purpose special Token used for span filling or generation in T5 models.)"},
                {"<extra_id_1>", "\r\n            占位符：T5 模型中用于填空或生成的多功能特殊 Token。\r\n            (Placeholder: Multi-purpose special Token used for span filling or generation in T5 models.)"},
                {"[unused0]", "\r\n            未使用占位符：BERT 等模型预留给用户添加新特殊 Token 的位置。\r\n            (Unused Placeholder: A slot reserved for users to add new special Tokens in models like BERT.)"},
                {"[unused1]", "\r\n            未使用占位符：BERT 等模型预留给用户添加新特殊 Token 的位置。\r\n            (Unused Placeholder: A slot reserved for users to add new special Tokens in models like BERT.)"},
                {"<|padding|>", "\r\n            填充：另一种形式的 Padding Token。\r\n            (Padding: Another form of Padding Token.)"},
                {"<|eot_id|>", "\r\n            序列结束ID：用于标记序列结束的特定 ID。\r\n            (End of Sequence ID: A specific ID used to mark the end of a sequence.)"},
                {"<|endofprompt|>", "\r\n            提示符结束：一些模型用于在提示 (Prompt) 和生成的回复之间进行明确分隔。\r\n            (End of Prompt: Used by some models for clear separation between the prompt and the generated response.)"},
    
                // === 代码和文本填充相关的控制 Token (FIM) ===
                {"<|fim_prefix|>", "\r\n            FIM 前缀：用于代码补全任务，标记需要被补全的代码的前缀部分。\r\n            (FIM Prefix: Used in code completion tasks, marking the prefix part of the code to be completed.)"},
                {"<|fim_middle|>", "\r\n            FIM 中间：用于代码补全任务，标记被补全代码的中间部分（或缺失部分）。\r\n            (FIM Middle: Used in code completion tasks, marking the middle part (or missing part) of the code to be completed.)"},
                {"<|fim_suffix|>", "\r\n            FIM 后缀：用于代码补全任务，标记需要被补全代码的后缀部分。\r\n            (FIM Suffix: Used in code completion tasks, marking the suffix part of the code to be completed.)"},
                {"<|fim_pad|>", "\r\n            FIM 填充：用于填充代码补全序列，保持长度一致。\r\n            (FIM Padding: Used to pad code completion sequences to maintain consistent length.)"},
                {"<|repo_name|>", "\r\n            代码仓库名：用于代码模型中，标记代码所属的仓库名称。\r\n            (Repository Name: Used in code models to mark the name of the repository the code belongs to.)"},
                {"<|file_sep|>", "\r\n            文件分隔符：用于分隔模型输入中不同的代码文件内容。\r\n            (File Separator: Used to separate the content of different code files in the model input.)"},
                {"<newline>", "\r\n            换行符：代码生成模型中常见，用于表示换行。\r\n            (Newline Character: Common in code generation models to denote a line break.)"},
                {"<tab>", "\r\n            制表符：代码生成模型中常见，用于表示制表符。\r\n            (Tabulator Character: Common in code generation models to denote a tab stop.)"},
                {"<code>", "\r\n            代码块标记：代码生成模型中用于标记代码段。\r\n            (Code Block Marker: Used in code generation models to delimit a code segment.)"},
    
                // --- 文本/文档/特殊用途 ---
                {"<html>", "\r\n            HTML标记：专用模型中用于生成 HTML 内容。\r\n            (HTML Tag: Used in specialized models for generating HTML content.)"},
                {"<sql>", "\r\n            SQL标记：专用模型中用于生成 SQL 查询。\r\n            (SQL Tag: Used in specialized models for generating SQL queries.)"},
                {"<doc>", "\r\n            文档开始：RAG 或文档检索模型中用于标记文档起始。\r\n            (Document Start: Used in RAG or document retrieval models to mark the beginning of a document.)"},
                {"</doc>", "\r\n            文档结束：RAG 或文档检索模型中用于标记文档结束。\r\n            (Document End: Used in RAG or document retrieval models to mark the end of a document.)"},
                {"<context>", "\r\n            上下文开始：Agent 或 RAG 模型中用于标记上下文。\r\n            (Context Start: Used in Agent or RAG models to mark the beginning of context.)"},
                {"</context>", "\r\n            上下文结束：Agent 或 RAG 模型中用于标记上下文。\r\n            (Context End: Used in Agent or RAG models to mark the end of context.)"},
                {"<|begin_of_document|>", "\r\n            文档开始：用于标记文档级输入内容的起始。\r\n            (Document Start: Used to mark the beginning of document-level input content.)"},
                {"<|end_of_document|>", "\r\n            文档结束：用于标记文档级输入内容的结束。\r\n            (Document End: Used to mark the end of document-level input content.)"},
                {"<sot>", "\r\n            文本开始(Start of Text)：常用于一些编码器/解码器结构。\r\n            (Start of Text: Commonly used in some encoder/decoder architectures.)"},
                {"<eot>", "\r\n            文本结束(End of Text)：常用于一些编码器/解码器结构。\r\n            (End of Text: Commonly used in some encoder/decoder architectures.)"},
                {"<|begin_of_text|>", "\r\n            文本开始：与 BOS 类似，用于标记文本内容的起始。\r\n            (Beginning of Text: Similar to BOS, used to mark the start of the text content.)"},
                {"<p>", "\r\n            段落分隔：特定模型用于区分不同段落的标记。\r\n            (Paragraph Separator: A marker used by specific models to distinguish different paragraphs.)"},
                {"<br>", "\r\n            软换行：在代码或文本生成中表示软换行。\r\n            (Soft Newline: Represents a soft line break in code or text generation.)"},
                {"<null>", "\r\n            空值：表示缺失或空输入。\r\n            (Null Value: Indicates missing or empty input.)"},
                {"<reserved>", "\r\n            保留 Token：用于未来扩展或特殊用途。\r\n            (Reserved Token: For future expansion or special uses.)"},
                {"<mask_1>", "\r\n            编号掩码：部分模型支持多掩码任务时的编号掩码。\r\n            (Numbered Mask: Numbered mask used when models support multiple masking tasks.)"},
                {"<copy>", "\r\n            复制机制：部分 Seq2Seq 模型中用于指示复制输入内容。\r\n            (Copy Mechanism: Used in some Seq2Seq models to indicate copying input content.)"},
                {"<|user_0|>", "\r\n            多用户角色：用于区分多个不同用户或角色的对话输入。\r\n            (Multi-user Role: Used to distinguish dialogue input from multiple different users or roles.)"},
            };

            if (commonTokens.TryGetValue(token, out string? usage))
            {
                return usage;
            }

            // 对于文件中出现的但未在常见列表中定义的 token，给出通用说明
            if (token.StartsWith("<|") && token.EndsWith("|>"))
            {
                // 使用与字典项相同的格式进行返回
                return "\r\n            控制Token：特定模型(如多模态、工具调用)使用的特殊控制标记。\r\n            (Control Token: Special control marker used by specific models (e.g., multimodal, tool calling).)";
            }

            // 对于既不是字典项，也不符合 <|> 格式的 token，给出最终的未收录说明
            return "\r\n            未收录/特定模型控制标记。\r\n            (Unlisted/Model Specific Control Token.)";
        }
        // 辅助方法：安全读取 JSON 字符串
        private string GetJsonString(JsonElement root, string propertyName)
        {
            if (root.TryGetProperty(propertyName, out var element) && element.ValueKind == JsonValueKind.String)
            {
                return element.GetString() ?? "N/A";
            }
            return "N/A";
        }

        // 辅助方法：安全读取 JSON 整数
        private string GetJsonIntValue(JsonElement root, string propertyName)
        {
            if (root.TryGetProperty(propertyName, out var element) && (element.ValueKind == JsonValueKind.Number))
            {
                return element.GetInt32().ToString();
            }
            return "N/A";
        }

        // 辅助方法：安全读取 JSON 数组中的第一个值 (用于 architectures)
        private string GetJsonArrayValue(JsonElement root, string propertyName)
        {
            if (root.TryGetProperty(propertyName, out var element) && element.ValueKind == JsonValueKind.Array)
            {
                if (element.GetArrayLength() > 0 && element[0].ValueKind == JsonValueKind.String)
                {
                    return element[0].GetString() ?? "N/A";
                }
            }
            return "N/A";
        }
        private void ScrollLogToTop()
        {
            // 1. 设置 SelectionStart 到文本的起始位置 (索引 0)
            // 这一步是将光标逻辑上定位到文本框的开头
            textBox_info.SelectionStart = 0;

            // 2. 调用 ScrollToCaret() 方法
            // 这个方法会强制文本框滚动，将 SelectionStart (即光标位置) 移动到可见区域
            textBox_info.ScrollToCaret();
        }
    }
}