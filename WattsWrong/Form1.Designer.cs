namespace WattsWrong
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.phase1_voltage = new System.Windows.Forms.TextBox();
            this.phase1_current = new System.Windows.Forms.TextBox();
            this.phase2_voltage = new System.Windows.Forms.TextBox();
            this.phase2_current = new System.Windows.Forms.TextBox();
            this.phase3_voltage = new System.Windows.Forms.TextBox();
            this.phase3_current = new System.Windows.Forms.TextBox();
            this.total_power = new System.Windows.Forms.TextBox();
            this.getReport_button = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reportUsersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(95, 70);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "PHASE 1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(299, 70);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 20);
            this.label2.TabIndex = 1;
            this.label2.Text = "Phase 2";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(527, 70);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 20);
            this.label3.TabIndex = 2;
            this.label3.Text = "Phase 3";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(12, 118);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "VOLTAGE";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(11, 171);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(67, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "CURRENT";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(281, 221);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(97, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "TOTAL POWER";
            // 
            // phase1_voltage
            // 
            this.phase1_voltage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.phase1_voltage.Location = new System.Drawing.Point(83, 115);
            this.phase1_voltage.Name = "phase1_voltage";
            this.phase1_voltage.Size = new System.Drawing.Size(102, 20);
            this.phase1_voltage.TabIndex = 6;
            // 
            // phase1_current
            // 
            this.phase1_current.Location = new System.Drawing.Point(83, 167);
            this.phase1_current.Name = "phase1_current";
            this.phase1_current.Size = new System.Drawing.Size(102, 20);
            this.phase1_current.TabIndex = 7;
            // 
            // phase2_voltage
            // 
            this.phase2_voltage.Location = new System.Drawing.Point(270, 108);
            this.phase2_voltage.Name = "phase2_voltage";
            this.phase2_voltage.Size = new System.Drawing.Size(116, 20);
            this.phase2_voltage.TabIndex = 8;
            // 
            // phase2_current
            // 
            this.phase2_current.Location = new System.Drawing.Point(270, 163);
            this.phase2_current.Name = "phase2_current";
            this.phase2_current.Size = new System.Drawing.Size(116, 20);
            this.phase2_current.TabIndex = 9;
            // 
            // phase3_voltage
            // 
            this.phase3_voltage.Location = new System.Drawing.Point(520, 115);
            this.phase3_voltage.Name = "phase3_voltage";
            this.phase3_voltage.Size = new System.Drawing.Size(94, 20);
            this.phase3_voltage.TabIndex = 10;
            // 
            // phase3_current
            // 
            this.phase3_current.Location = new System.Drawing.Point(520, 170);
            this.phase3_current.Name = "phase3_current";
            this.phase3_current.Size = new System.Drawing.Size(94, 20);
            this.phase3_current.TabIndex = 11;
            // 
            // total_power
            // 
            this.total_power.Location = new System.Drawing.Point(270, 254);
            this.total_power.Name = "total_power";
            this.total_power.Size = new System.Drawing.Size(116, 20);
            this.total_power.TabIndex = 12;
            // 
            // getReport_button
            // 
            this.getReport_button.Location = new System.Drawing.Point(520, 250);
            this.getReport_button.Name = "getReport_button";
            this.getReport_button.Size = new System.Drawing.Size(110, 45);
            this.getReport_button.TabIndex = 13;
            this.getReport_button.Text = "view";
            this.getReport_button.UseVisualStyleBackColor = true;
            this.getReport_button.Visible = false;
            this.getReport_button.Click += new System.EventHandler(this.button1_Click);
            // 
            // button1
            // 
            this.button1.AutoSize = true;
            this.button1.Location = new System.Drawing.Point(83, 250);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(102, 38);
            this.button1.TabIndex = 14;
            this.button1.Text = "Get Report";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 24);
            this.menuStrip1.TabIndex = 16;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.reportUsersToolStripMenuItem});
            this.settingsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("settingsToolStripMenuItem.Image")));
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(77, 20);
            this.settingsToolStripMenuItem.Text = "Settings";
            // 
            // reportUsersToolStripMenuItem
            // 
            this.reportUsersToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("reportUsersToolStripMenuItem.Image")));
            this.reportUsersToolStripMenuItem.Name = "reportUsersToolStripMenuItem";
            this.reportUsersToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.reportUsersToolStripMenuItem.Text = "Reporting Users";
            this.reportUsersToolStripMenuItem.Click += new System.EventHandler(this.reportUsersToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.getReport_button);
            this.Controls.Add(this.total_power);
            this.Controls.Add(this.phase3_current);
            this.Controls.Add(this.phase3_voltage);
            this.Controls.Add(this.phase2_current);
            this.Controls.Add(this.phase2_voltage);
            this.Controls.Add(this.phase1_current);
            this.Controls.Add(this.phase1_voltage);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Watts Wrong Equipment Monitor";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox phase1_voltage;
        private System.Windows.Forms.TextBox phase1_current;
        private System.Windows.Forms.TextBox phase2_voltage;
        private System.Windows.Forms.TextBox phase2_current;
        private System.Windows.Forms.TextBox phase3_voltage;
        private System.Windows.Forms.TextBox phase3_current;
        private System.Windows.Forms.TextBox total_power;
        private System.Windows.Forms.Button getReport_button;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reportUsersToolStripMenuItem;
    }
}

