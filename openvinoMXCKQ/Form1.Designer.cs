namespace openvinoMXCKQ
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            btn_select = new Button();
            txtBox_select = new TextBox();
            textBox_info = new TextBox();
            SuspendLayout();
            // 
            // btn_select
            // 
            btn_select.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btn_select.Location = new Point(950, 12);
            btn_select.Name = "btn_select";
            btn_select.Size = new Size(141, 34);
            btn_select.TabIndex = 0;
            btn_select.Text = "选择模型";
            btn_select.UseVisualStyleBackColor = true;
            btn_select.Click += btn_select_Click;
            // 
            // txtBox_select
            // 
            txtBox_select.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtBox_select.Location = new Point(12, 12);
            txtBox_select.Name = "txtBox_select";
            txtBox_select.Size = new Size(932, 31);
            txtBox_select.TabIndex = 1;
            // 
            // textBox_info
            // 
            textBox_info.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            textBox_info.Location = new Point(12, 49);
            textBox_info.Multiline = true;
            textBox_info.Name = "textBox_info";
            textBox_info.ScrollBars = ScrollBars.Vertical;
            textBox_info.Size = new Size(1079, 1094);
            textBox_info.TabIndex = 2;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1103, 1155);
            Controls.Add(textBox_info);
            Controls.Add(txtBox_select);
            Controls.Add(btn_select);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form1";
            Text = "OpenVINO Model Information Viewer py 虞城县广厦互联软件开发服务中心(GoShine Tech)";
            DragDrop += Form1_DragDrop;
            DragEnter += Form1_DragEnter;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btn_select;
        private TextBox txtBox_select;
        private TextBox textBox_info;
    }
}
