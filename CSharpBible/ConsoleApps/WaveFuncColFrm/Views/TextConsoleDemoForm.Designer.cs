using TestConsoleDemo.ViewModels.Interfaces;
using Views;

namespace TestConsoleDemo.Views
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
            components = new System.ComponentModel.Container();
            timer1 = new System.Windows.Forms.Timer(components);
            textBox1 = new TextBox();
            btnHello = new Button();
            btnLongText = new Button();
            btnColText = new Button();
            btnDisplayTest = new Button();
            SuspendLayout();
            // 
            // timer1
            // 
            timer1.Enabled = true;
            timer1.Tick += timer1_Tick;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(25, 398);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(748, 31);
            textBox1.TabIndex = 0;
            // 
            // btnHello
            // 
            btnHello.Location = new Point(15, 12);
            btnHello.Name = "btnHello";
            btnHello.Size = new Size(130, 64);
            btnHello.TabIndex = 1;
            btnHello.Text = "Hello..";
            btnHello.UseVisualStyleBackColor = true;
            // 
            // btnLongText
            // 
            btnLongText.Location = new Point(145, 12);
            btnLongText.Name = "btnLongText";
            btnLongText.Size = new Size(140, 64);
            btnLongText.TabIndex = 2;
            btnLongText.Text = "Show Images";
            btnLongText.UseVisualStyleBackColor = true;
            // 
            // btnColText
            // 
            btnColText.Location = new Point(285, 12);
            btnColText.Name = "btnColText";
            btnColText.Size = new Size(140, 64);
            btnColText.TabIndex = 3;
            btnColText.Text = "Analyse Images";
            btnColText.UseVisualStyleBackColor = true;
            // 
            // btnDisplayTest
            // 
            btnDisplayTest.Location = new Point(425, 12);
            btnDisplayTest.Name = "btnDisplayTest";
            btnDisplayTest.Size = new Size(140, 64);
            btnDisplayTest.TabIndex = 4;
            btnDisplayTest.Text = "Displayt.";
            btnDisplayTest.UseVisualStyleBackColor = true;
            // 
            // TextConsoleDemoForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btnDisplayTest);
            Controls.Add(btnColText);
            Controls.Add(btnLongText);
            Controls.Add(btnHello);
            Controls.Add(textBox1);
            Name = "TextConsoleDemoForm";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private TextBox textBox1;
        [CommandBinding(nameof(ITextConsoleDemoViewModel.DoHelloCommand))]
        private Button btnHello;
        [CommandBinding(nameof(ITextConsoleDemoViewModel.DoShowImagesCommand))]
        private Button btnLongText;
        [CommandBinding(nameof(ITextConsoleDemoViewModel.DoAnalyseImageCommand))]
        private Button btnColText;
        [CommandBinding(nameof(ITextConsoleDemoViewModel.DoDisplayTestCommand))]
        private Button btnDisplayTest;
    }
}