namespace helloworld
{
    partial class ViewsCaiDat
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ViewsCaiDat));
            tableLayoutPanel1 = new TableLayoutPanel();
            tableLayoutPanel2 = new TableLayoutPanel();
            label1 = new Label();
            tableLayoutPanel3 = new TableLayoutPanel();
            textBoxSMTPPASSWORD = new TextBox();
            label5 = new Label();
            textBoxSMTPEMAIL = new TextBox();
            label4 = new Label();
            textBoxSMTPPOST = new TextBox();
            label3 = new Label();
            label2 = new Label();
            textBoxSMTPHOST = new TextBox();
            buttonLuuCaiDat = new Button();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 0, 0);
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Size = new Size(800, 384);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 1;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.Controls.Add(label1, 0, 0);
            tableLayoutPanel2.Controls.Add(tableLayoutPanel3, 0, 1);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(3, 3);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 2;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.Size = new Size(394, 186);
            tableLayoutPanel2.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(3, 0);
            label1.Name = "label1";
            label1.Size = new Size(76, 31);
            label1.TabIndex = 0;
            label1.Text = "SMTP";
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.ColumnCount = 2;
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel3.Controls.Add(textBoxSMTPPASSWORD, 1, 3);
            tableLayoutPanel3.Controls.Add(label5, 0, 3);
            tableLayoutPanel3.Controls.Add(textBoxSMTPEMAIL, 1, 2);
            tableLayoutPanel3.Controls.Add(label4, 0, 2);
            tableLayoutPanel3.Controls.Add(textBoxSMTPPOST, 1, 1);
            tableLayoutPanel3.Controls.Add(label3, 0, 1);
            tableLayoutPanel3.Controls.Add(label2, 0, 0);
            tableLayoutPanel3.Controls.Add(textBoxSMTPHOST, 1, 0);
            tableLayoutPanel3.Dock = DockStyle.Fill;
            tableLayoutPanel3.Location = new Point(3, 43);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 4;
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel3.Size = new Size(388, 140);
            tableLayoutPanel3.TabIndex = 1;
            // 
            // textBoxSMTPPASSWORD
            // 
            textBoxSMTPPASSWORD.Dock = DockStyle.Fill;
            textBoxSMTPPASSWORD.Location = new Point(153, 108);
            textBoxSMTPPASSWORD.Name = "textBoxSMTPPASSWORD";
            textBoxSMTPPASSWORD.PasswordChar = '*';
            textBoxSMTPPASSWORD.Size = new Size(232, 27);
            textBoxSMTPPASSWORD.TabIndex = 7;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Dock = DockStyle.Fill;
            label5.Location = new Point(3, 105);
            label5.Name = "label5";
            label5.Size = new Size(144, 35);
            label5.TabIndex = 6;
            label5.Text = "smtp Password";
            // 
            // textBoxSMTPEMAIL
            // 
            textBoxSMTPEMAIL.Dock = DockStyle.Fill;
            textBoxSMTPEMAIL.Location = new Point(153, 73);
            textBoxSMTPEMAIL.Name = "textBoxSMTPEMAIL";
            textBoxSMTPEMAIL.Size = new Size(232, 27);
            textBoxSMTPEMAIL.TabIndex = 5;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Dock = DockStyle.Fill;
            label4.Location = new Point(3, 70);
            label4.Name = "label4";
            label4.Size = new Size(144, 35);
            label4.TabIndex = 4;
            label4.Text = "smtp Email";
            // 
            // textBoxSMTPPOST
            // 
            textBoxSMTPPOST.Dock = DockStyle.Fill;
            textBoxSMTPPOST.Location = new Point(153, 38);
            textBoxSMTPPOST.Name = "textBoxSMTPPOST";
            textBoxSMTPPOST.Size = new Size(232, 27);
            textBoxSMTPPOST.TabIndex = 3;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Dock = DockStyle.Fill;
            label3.Location = new Point(3, 35);
            label3.Name = "label3";
            label3.Size = new Size(144, 35);
            label3.TabIndex = 2;
            label3.Text = "smtp Port";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Dock = DockStyle.Fill;
            label2.Location = new Point(3, 0);
            label2.Name = "label2";
            label2.Size = new Size(144, 35);
            label2.TabIndex = 0;
            label2.Text = "smtp Host";
            // 
            // textBoxSMTPHOST
            // 
            textBoxSMTPHOST.Dock = DockStyle.Fill;
            textBoxSMTPHOST.Location = new Point(153, 3);
            textBoxSMTPHOST.Name = "textBoxSMTPHOST";
            textBoxSMTPHOST.Size = new Size(232, 27);
            textBoxSMTPHOST.TabIndex = 1;
            // 
            // buttonLuuCaiDat
            // 
            buttonLuuCaiDat.BackColor = Color.SkyBlue;
            buttonLuuCaiDat.Location = new Point(6, 390);
            buttonLuuCaiDat.Name = "buttonLuuCaiDat";
            buttonLuuCaiDat.Size = new Size(117, 39);
            buttonLuuCaiDat.TabIndex = 1;
            buttonLuuCaiDat.Text = "Lưu cài đặt";
            buttonLuuCaiDat.UseVisualStyleBackColor = false;
            buttonLuuCaiDat.Click += buttonLuuCaiDat_Click;
            // 
            // ViewsCaiDat
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(buttonLuuCaiDat);
            Controls.Add(tableLayoutPanel1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "ViewsCaiDat";
            Text = "Cài đặt";
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            tableLayoutPanel3.ResumeLayout(false);
            tableLayoutPanel3.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private TableLayoutPanel tableLayoutPanel2;
        private Label label1;
        private TableLayoutPanel tableLayoutPanel3;
        private Label label2;
        private TextBox textBoxSMTPHOST;
        private Label label3;
        private TextBox textBoxSMTPPASSWORD;
        private Label label5;
        private TextBox textBoxSMTPEMAIL;
        private Label label4;
        private TextBox textBoxSMTPPOST;
        private Button buttonLuuCaiDat;
    }
}