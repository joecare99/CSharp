namespace TestConsoleDemo.View
{
    /// <summary>
    /// Class TextConsoleDemoForm.
    /// Implements the <see cref="System.Windows.Forms.Form" />
    /// </summary>
    /// <seealso cref="System.Windows.Forms.Form" />
    partial class TextConsoleDemoForm
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
            this.components = new System.ComponentModel.Container();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.btnHello = new System.Windows.Forms.Button();
            this.btnLongText = new System.Windows.Forms.Button();
            this.btnColText = new System.Windows.Forms.Button();
            this.btnDisplayTest = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(25, 398);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(748, 31);
            this.textBox1.TabIndex = 0;
            // 
            // btnHello
            // 
            this.btnHello.Location = new System.Drawing.Point(15, 12);
            this.btnHello.Name = "btnHello";
            this.btnHello.Size = new System.Drawing.Size(85, 48);
            this.btnHello.TabIndex = 1;
            this.btnHello.Text = "Hello..";
            this.btnHello.UseVisualStyleBackColor = true;
            this.btnHello.Click += new System.EventHandler(this.btnHello_Click);
            // 
            // btnLongText
            // 
            this.btnLongText.Location = new System.Drawing.Point(106, 12);
            this.btnLongText.Name = "btnLongText";
            this.btnLongText.Size = new System.Drawing.Size(95, 48);
            this.btnLongText.TabIndex = 2;
            this.btnLongText.Text = "Long Text";
            this.btnLongText.UseVisualStyleBackColor = true;
            this.btnLongText.Click += new System.EventHandler(this.btnLongText_Click);
            // 
            // btnColText
            // 
            this.btnColText.Location = new System.Drawing.Point(207, 12);
            this.btnColText.Name = "btnColText";
            this.btnColText.Size = new System.Drawing.Size(95, 48);
            this.btnColText.TabIndex = 3;
            this.btnColText.Text = "Col. Text";
            this.btnColText.UseVisualStyleBackColor = true;
            this.btnColText.Click += new System.EventHandler(this.btnColText_Click);
            // 
            // btnDisplayTest
            // 
            this.btnDisplayTest.Location = new System.Drawing.Point(308, 12);
            this.btnDisplayTest.Name = "btnDisplayTest";
            this.btnDisplayTest.Size = new System.Drawing.Size(95, 48);
            this.btnDisplayTest.TabIndex = 4;
            this.btnDisplayTest.Text = "Displayt.";
            this.btnDisplayTest.UseVisualStyleBackColor = true;
            this.btnDisplayTest.Click += new System.EventHandler(this.btnDisplayTest_Click);
            // 
            // TextConsoleDemoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnDisplayTest);
            this.Controls.Add(this.btnColText);
            this.Controls.Add(this.btnLongText);
            this.Controls.Add(this.btnHello);
            this.Controls.Add(this.textBox1);
            this.Name = "TextConsoleDemoForm";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private TextBox textBox1;
        private Button btnHello;
        private Button btnLongText;
        private Button btnColText;
        private Button btnDisplayTest;
    }
}