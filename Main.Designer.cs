namespace ExamplePacketPlugin
{
    partial class Main
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.trgtUsrnmLabel = new System.Windows.Forms.Label();
            this.tbGotoUsername = new System.Windows.Forms.TextBox();
            this.cbEnablePlugin = new System.Windows.Forms.CheckBox();
            this.tbSkillList = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbStopIf = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.numRelogDelay = new System.Windows.Forms.NumericUpDown();
            this.cbLockCell = new System.Windows.Forms.CheckBox();
            this.cbStopAttack = new System.Windows.Forms.CheckBox();
            this.cbEnableGlobalHotkey = new System.Windows.Forms.CheckBox();
            this.gbAdvancedOptions = new System.Windows.Forms.GroupBox();
            this.tbAttPriority = new System.Windows.Forms.TextBox();
            this.cbAttackPriority = new System.Windows.Forms.CheckBox();
            this.tbBuffSkill = new System.Windows.Forms.TextBox();
            this.cbBuffIfStop = new System.Windows.Forms.CheckBox();
            this.cbChangeColor = new System.Windows.Forms.CheckBox();
            this.timerStopAttack = new System.Windows.Forms.Timer(this.components);
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numRelogDelay)).BeginInit();
            this.gbAdvancedOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // trgtUsrnmLabel
            // 
            this.trgtUsrnmLabel.AutoSize = true;
            this.trgtUsrnmLabel.Location = new System.Drawing.Point(16, 14);
            this.trgtUsrnmLabel.Name = "trgtUsrnmLabel";
            this.trgtUsrnmLabel.Size = new System.Drawing.Size(84, 13);
            this.trgtUsrnmLabel.TabIndex = 4;
            this.trgtUsrnmLabel.Text = "Goto Username:";
            // 
            // tbGotoUsername
            // 
            this.tbGotoUsername.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbGotoUsername.Location = new System.Drawing.Point(19, 30);
            this.tbGotoUsername.Name = "tbGotoUsername";
            this.tbGotoUsername.Size = new System.Drawing.Size(135, 20);
            this.tbGotoUsername.TabIndex = 5;
            this.tbGotoUsername.Text = "username";
            // 
            // cbEnablePlugin
            // 
            this.cbEnablePlugin.AutoSize = true;
            this.cbEnablePlugin.Location = new System.Drawing.Point(19, 96);
            this.cbEnablePlugin.Name = "cbEnablePlugin";
            this.cbEnablePlugin.Size = new System.Drawing.Size(91, 17);
            this.cbEnablePlugin.TabIndex = 7;
            this.cbEnablePlugin.Text = "Enable Plugin";
            this.cbEnablePlugin.UseVisualStyleBackColor = true;
            this.cbEnablePlugin.CheckedChanged += new System.EventHandler(this.cbEnablePlugin_CheckedChanged);
            // 
            // tbSkillList
            // 
            this.tbSkillList.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbSkillList.Location = new System.Drawing.Point(19, 70);
            this.tbSkillList.Name = "tbSkillList";
            this.tbSkillList.Size = new System.Drawing.Size(135, 20);
            this.tbSkillList.TabIndex = 12;
            this.tbSkillList.Text = "1,2,3,4";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 54);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "Skill List:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbStopIf);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.numRelogDelay);
            this.groupBox1.Location = new System.Drawing.Point(166, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(164, 109);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Options";
            // 
            // cbStopIf
            // 
            this.cbStopIf.AutoSize = true;
            this.cbStopIf.Location = new System.Drawing.Point(14, 24);
            this.cbStopIf.Name = "cbStopIf";
            this.cbStopIf.Size = new System.Drawing.Size(144, 17);
            this.cbStopIf.TabIndex = 19;
            this.cbStopIf.Text = "Stop if failed goto 5 times";
            this.cbStopIf.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(122, 53);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(20, 13);
            this.label3.TabIndex = 18;
            this.label3.Text = "ms";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(48, 74);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 13);
            this.label2.TabIndex = 17;
            this.label2.Text = "(after relogin)";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 53);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 16;
            this.label1.Text = "Delay:";
            // 
            // numRelogDelay
            // 
            this.numRelogDelay.Location = new System.Drawing.Point(51, 51);
            this.numRelogDelay.Maximum = new decimal(new int[] {
            60000,
            0,
            0,
            0});
            this.numRelogDelay.Name = "numRelogDelay";
            this.numRelogDelay.Size = new System.Drawing.Size(65, 20);
            this.numRelogDelay.TabIndex = 15;
            this.numRelogDelay.Value = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            // 
            // cbLockCell
            // 
            this.cbLockCell.AutoSize = true;
            this.cbLockCell.Location = new System.Drawing.Point(107, 19);
            this.cbLockCell.Name = "cbLockCell";
            this.cbLockCell.Size = new System.Drawing.Size(84, 17);
            this.cbLockCell.TabIndex = 16;
            this.cbLockCell.Text = "LockCell (R)";
            this.cbLockCell.UseVisualStyleBackColor = true;
            this.cbLockCell.CheckedChanged += new System.EventHandler(this.cbLockCell_CheckedChanged);
            // 
            // cbStopAttack
            // 
            this.cbStopAttack.AutoSize = true;
            this.cbStopAttack.Location = new System.Drawing.Point(197, 19);
            this.cbStopAttack.Name = "cbStopAttack";
            this.cbStopAttack.Size = new System.Drawing.Size(95, 17);
            this.cbStopAttack.TabIndex = 17;
            this.cbStopAttack.Text = "StopAttack (T)";
            this.cbStopAttack.UseVisualStyleBackColor = true;
            this.cbStopAttack.CheckedChanged += new System.EventHandler(this.cbStopAttack_CheckedChanged);
            // 
            // cbEnableGlobalHotkey
            // 
            this.cbEnableGlobalHotkey.AutoSize = true;
            this.cbEnableGlobalHotkey.Location = new System.Drawing.Point(7, 19);
            this.cbEnableGlobalHotkey.Name = "cbEnableGlobalHotkey";
            this.cbEnableGlobalHotkey.Size = new System.Drawing.Size(93, 17);
            this.cbEnableGlobalHotkey.TabIndex = 18;
            this.cbEnableGlobalHotkey.Text = "Global Hotkey";
            this.cbEnableGlobalHotkey.UseVisualStyleBackColor = true;
            this.cbEnableGlobalHotkey.CheckedChanged += new System.EventHandler(this.cbEnableGlobalHotkey_CheckedChanged);
            // 
            // gbAdvancedOptions
            // 
            this.gbAdvancedOptions.Controls.Add(this.tbAttPriority);
            this.gbAdvancedOptions.Controls.Add(this.cbAttackPriority);
            this.gbAdvancedOptions.Controls.Add(this.tbBuffSkill);
            this.gbAdvancedOptions.Controls.Add(this.cbBuffIfStop);
            this.gbAdvancedOptions.Controls.Add(this.cbChangeColor);
            this.gbAdvancedOptions.Controls.Add(this.cbEnableGlobalHotkey);
            this.gbAdvancedOptions.Controls.Add(this.cbStopAttack);
            this.gbAdvancedOptions.Controls.Add(this.cbLockCell);
            this.gbAdvancedOptions.Location = new System.Drawing.Point(12, 130);
            this.gbAdvancedOptions.Name = "gbAdvancedOptions";
            this.gbAdvancedOptions.Size = new System.Drawing.Size(318, 92);
            this.gbAdvancedOptions.TabIndex = 19;
            this.gbAdvancedOptions.TabStop = false;
            this.gbAdvancedOptions.Text = "Advanced Options";
            // 
            // tbAttPriority
            // 
            this.tbAttPriority.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbAttPriority.Location = new System.Drawing.Point(94, 64);
            this.tbAttPriority.Name = "tbAttPriority";
            this.tbAttPriority.Size = new System.Drawing.Size(214, 20);
            this.tbAttPriority.TabIndex = 23;
            this.tbAttPriority.Text = "Defense Drone,Attack Drone";
            // 
            // cbAttackPriority
            // 
            this.cbAttackPriority.AutoSize = true;
            this.cbAttackPriority.Location = new System.Drawing.Point(7, 66);
            this.cbAttackPriority.Name = "cbAttackPriority";
            this.cbAttackPriority.Size = new System.Drawing.Size(91, 17);
            this.cbAttackPriority.TabIndex = 22;
            this.cbAttackPriority.Text = "AttackPriority:";
            this.cbAttackPriority.UseVisualStyleBackColor = true;
            this.cbAttackPriority.CheckedChanged += new System.EventHandler(this.cbAttackPriority_CheckedChanged);
            // 
            // tbBuffSkill
            // 
            this.tbBuffSkill.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbBuffSkill.Location = new System.Drawing.Point(268, 40);
            this.tbBuffSkill.Name = "tbBuffSkill";
            this.tbBuffSkill.Size = new System.Drawing.Size(40, 20);
            this.tbBuffSkill.TabIndex = 21;
            this.tbBuffSkill.Text = "3,4";
            // 
            // cbBuffIfStop
            // 
            this.cbBuffIfStop.AutoSize = true;
            this.cbBuffIfStop.Location = new System.Drawing.Point(161, 42);
            this.cbBuffIfStop.Name = "cbBuffIfStop";
            this.cbBuffIfStop.Size = new System.Drawing.Size(113, 17);
            this.cbBuffIfStop.TabIndex = 20;
            this.cbBuffIfStop.Text = "Buff If StopAttack:";
            this.cbBuffIfStop.UseVisualStyleBackColor = true;
            this.cbBuffIfStop.CheckedChanged += new System.EventHandler(this.cbBuffIfStop_CheckedChanged);
            // 
            // cbChangeColor
            // 
            this.cbChangeColor.AutoSize = true;
            this.cbChangeColor.Checked = true;
            this.cbChangeColor.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbChangeColor.Location = new System.Drawing.Point(7, 42);
            this.cbChangeColor.Name = "cbChangeColor";
            this.cbChangeColor.Size = new System.Drawing.Size(154, 17);
            this.cbChangeColor.TabIndex = 19;
            this.cbChangeColor.Text = "Change Color if StopAttack";
            this.cbChangeColor.UseVisualStyleBackColor = true;
            // 
            // timerStopAttack
            // 
            this.timerStopAttack.Tick += new System.EventHandler(this.timerStopAttack_Tick);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(342, 234);
            this.Controls.Add(this.gbAdvancedOptions);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tbSkillList);
            this.Controls.Add(this.cbEnablePlugin);
            this.Controls.Add(this.tbGotoUsername);
            this.Controls.Add(this.trgtUsrnmLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Maid Remake";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numRelogDelay)).EndInit();
            this.gbAdvancedOptions.ResumeLayout(false);
            this.gbAdvancedOptions.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label trgtUsrnmLabel;
        private System.Windows.Forms.TextBox tbGotoUsername;
        private System.Windows.Forms.CheckBox cbEnablePlugin;
        private System.Windows.Forms.TextBox tbSkillList;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox cbStopIf;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numRelogDelay;
        private System.Windows.Forms.CheckBox cbLockCell;
        private System.Windows.Forms.CheckBox cbStopAttack;
        private System.Windows.Forms.CheckBox cbEnableGlobalHotkey;
        private System.Windows.Forms.GroupBox gbAdvancedOptions;
        private System.Windows.Forms.CheckBox cbChangeColor;
        private System.Windows.Forms.TextBox tbBuffSkill;
        private System.Windows.Forms.CheckBox cbBuffIfStop;
        private System.Windows.Forms.TextBox tbAttPriority;
        private System.Windows.Forms.CheckBox cbAttackPriority;
        private System.Windows.Forms.Timer timerStopAttack;
    }
}