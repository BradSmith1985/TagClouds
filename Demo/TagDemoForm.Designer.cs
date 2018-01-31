namespace Demo {
	partial class TagDemoForm {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.label1 = new System.Windows.Forms.Label();
			this.nudVariation = new System.Windows.Forms.NumericUpDown();
			this.canvas = new System.Windows.Forms.Panel();
			this.label2 = new System.Windows.Forms.Label();
			this.txtCycles = new System.Windows.Forms.TextBox();
			this.nudTags = new System.Windows.Forms.NumericUpDown();
			this.label3 = new System.Windows.Forms.Label();
			this.btnStats = new System.Windows.Forms.Button();
			this.btnTest = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.nudVariation)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudTags)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(169, 14);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(97, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Font Size Gradient:";
			// 
			// nudVariation
			// 
			this.nudVariation.DecimalPlaces = 1;
			this.nudVariation.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
			this.nudVariation.Location = new System.Drawing.Point(272, 12);
			this.nudVariation.Name = "nudVariation";
			this.nudVariation.Size = new System.Drawing.Size(50, 20);
			this.nudVariation.TabIndex = 1;
			this.nudVariation.Value = new decimal(new int[] {
            25,
            0,
            0,
            65536});
			this.nudVariation.ValueChanged += new System.EventHandler(this.nudVariation_ValueChanged);
			// 
			// canvas
			// 
			this.canvas.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.canvas.Location = new System.Drawing.Point(15, 38);
			this.canvas.Name = "canvas";
			this.canvas.Size = new System.Drawing.Size(597, 551);
			this.canvas.TabIndex = 2;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(337, 14);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(41, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "Cycles:";
			// 
			// txtCycles
			// 
			this.txtCycles.Location = new System.Drawing.Point(384, 11);
			this.txtCycles.Name = "txtCycles";
			this.txtCycles.ReadOnly = true;
			this.txtCycles.Size = new System.Drawing.Size(75, 20);
			this.txtCycles.TabIndex = 4;
			// 
			// nudTags
			// 
			this.nudTags.Location = new System.Drawing.Point(104, 12);
			this.nudTags.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.nudTags.Name = "nudTags";
			this.nudTags.Size = new System.Drawing.Size(50, 20);
			this.nudTags.TabIndex = 6;
			this.nudTags.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.nudTags.ValueChanged += new System.EventHandler(this.nudTags_ValueChanged);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(12, 14);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(86, 13);
			this.label3.TabIndex = 5;
			this.label3.Text = "Number of Tags:";
			// 
			// btnStats
			// 
			this.btnStats.Location = new System.Drawing.Point(544, 10);
			this.btnStats.Name = "btnStats";
			this.btnStats.Size = new System.Drawing.Size(68, 21);
			this.btnStats.TabIndex = 7;
			this.btnStats.Text = "Copy stats";
			this.btnStats.UseVisualStyleBackColor = true;
			this.btnStats.Click += new System.EventHandler(this.btnStats_Click);
			// 
			// btnTest
			// 
			this.btnTest.Location = new System.Drawing.Point(465, 10);
			this.btnTest.Name = "btnTest";
			this.btnTest.Size = new System.Drawing.Size(76, 21);
			this.btnTest.TabIndex = 8;
			this.btnTest.Text = "O(n^2) test";
			this.btnTest.UseVisualStyleBackColor = true;
			this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
			// 
			// TagDemoForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(624, 601);
			this.Controls.Add(this.btnTest);
			this.Controls.Add(this.btnStats);
			this.Controls.Add(this.nudTags);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.txtCycles);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.canvas);
			this.Controls.Add(this.nudVariation);
			this.Controls.Add(this.label1);
			this.Name = "TagDemoForm";
			this.Text = "Tag Cloud Demo";
			((System.ComponentModel.ISupportInitialize)(this.nudVariation)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudTags)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.NumericUpDown nudVariation;
		private System.Windows.Forms.Panel canvas;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox txtCycles;
		private System.Windows.Forms.NumericUpDown nudTags;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button btnStats;
		private System.Windows.Forms.Button btnTest;
	}
}

