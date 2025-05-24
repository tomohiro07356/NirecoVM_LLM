namespace NirecoVM_LLM
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        private void InitializeComponent()
        {
            pictureBox = new PictureBox();
            btnSelectImage = new Button();
            btnAnalyze = new Button();
            txtResult = new TextBox();
            label1 = new Label();
            label2 = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox).BeginInit();
            SuspendLayout();
            // 
            // pictureBox
            // 
            pictureBox.BorderStyle = BorderStyle.FixedSingle;
            pictureBox.Location = new Point(21, 50);
            pictureBox.Name = "pictureBox";
            pictureBox.Size = new Size(656, 422);
            pictureBox.TabIndex = 0;
            pictureBox.TabStop = false;
            // 
            // btnSelectImage
            // 
            btnSelectImage.Font = new Font("Yu Gothic UI", 12F);
            btnSelectImage.Location = new Point(21, 490);
            btnSelectImage.Name = "btnSelectImage";
            btnSelectImage.Size = new Size(262, 50);
            btnSelectImage.TabIndex = 1;
            btnSelectImage.Text = "画像を選択";
            btnSelectImage.UseVisualStyleBackColor = true;
            btnSelectImage.Click += btnSelectImage_Click;
            // 
            // btnAnalyze
            // 
            btnAnalyze.Font = new Font("Yu Gothic UI", 12F);
            btnAnalyze.Location = new Point(415, 490);
            btnAnalyze.Name = "btnAnalyze";
            btnAnalyze.Size = new Size(262, 50);
            btnAnalyze.TabIndex = 2;
            btnAnalyze.Text = "ナンバープレート認識";
            btnAnalyze.UseVisualStyleBackColor = true;
            btnAnalyze.Click += btnAnalyze_Click;
            // 
            // txtResult
            // 
            txtResult.Font = new Font("Yu Gothic UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 128);
            txtResult.Location = new Point(21, 590);
            txtResult.Multiline = true;
            txtResult.Name = "txtResult";
            txtResult.ReadOnly = true;
            txtResult.ScrollBars = ScrollBars.Vertical;
            txtResult.Size = new Size(657, 135);
            txtResult.TabIndex = 3;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Yu Gothic UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 128);
            label1.Location = new Point(21, 24);
            label1.Name = "label1";
            label1.Size = new Size(90, 21);
            label1.TabIndex = 4;
            label1.Text = "車両画像：";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Yu Gothic UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 128);
            label2.Location = new Point(21, 560);
            label2.Name = "label2";
            label2.Size = new Size(90, 21);
            label2.TabIndex = 5;
            label2.Text = "認識結果：";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(700, 749);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(txtResult);
            Controls.Add(btnAnalyze);
            Controls.Add(btnSelectImage);
            Controls.Add(pictureBox);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "NirecoVM_LLM - ナンバープレート認識";
            ((System.ComponentModel.ISupportInitialize)pictureBox).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Button btnSelectImage;
        private System.Windows.Forms.Button btnAnalyze;
        private System.Windows.Forms.TextBox txtResult;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}
