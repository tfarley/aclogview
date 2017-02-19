﻿namespace aclogview
{
    partial class FindOpcodeInFilesForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            this.txtSearchPathRoot = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnStartSearch = new System.Windows.Forms.Button();
            this.btnStopSearch = new System.Windows.Forms.Button();
            this.btnChangeSearchPathRoot = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.label2 = new System.Windows.Forms.Label();
            this.txtOpcode = new System.Windows.Forms.TextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.columnHits = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnFileSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnFilePath = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtSearchPathRoot
            // 
            this.txtSearchPathRoot.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSearchPathRoot.Location = new System.Drawing.Point(15, 25);
            this.txtSearchPathRoot.Name = "txtSearchPathRoot";
            this.txtSearchPathRoot.Size = new System.Drawing.Size(723, 20);
            this.txtSearchPathRoot.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Search Path Root";
            // 
            // btnStartSearch
            // 
            this.btnStartSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStartSearch.Location = new System.Drawing.Point(524, 51);
            this.btnStartSearch.Name = "btnStartSearch";
            this.btnStartSearch.Size = new System.Drawing.Size(121, 23);
            this.btnStartSearch.TabIndex = 2;
            this.btnStartSearch.Text = "Start Search";
            this.btnStartSearch.UseVisualStyleBackColor = true;
            this.btnStartSearch.Click += new System.EventHandler(this.btnStartSearch_Click);
            // 
            // btnStopSearch
            // 
            this.btnStopSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStopSearch.Enabled = false;
            this.btnStopSearch.Location = new System.Drawing.Point(651, 51);
            this.btnStopSearch.Name = "btnStopSearch";
            this.btnStopSearch.Size = new System.Drawing.Size(121, 23);
            this.btnStopSearch.TabIndex = 3;
            this.btnStopSearch.Text = "Stop Search";
            this.btnStopSearch.UseVisualStyleBackColor = true;
            this.btnStopSearch.Click += new System.EventHandler(this.btnStopSearch_Click);
            // 
            // btnChangeSearchPathRoot
            // 
            this.btnChangeSearchPathRoot.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnChangeSearchPathRoot.Location = new System.Drawing.Point(744, 23);
            this.btnChangeSearchPathRoot.Name = "btnChangeSearchPathRoot";
            this.btnChangeSearchPathRoot.Size = new System.Drawing.Size(28, 23);
            this.btnChangeSearchPathRoot.TabIndex = 4;
            this.btnChangeSearchPathRoot.Text = "F";
            this.btnChangeSearchPathRoot.UseVisualStyleBackColor = true;
            this.btnChangeSearchPathRoot.Click += new System.EventHandler(this.btnChangeSearchPathRoot_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeColumns = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.columnHits,
            this.columnFileSize,
            this.columnFilePath});
            this.dataGridView1.Location = new System.Drawing.Point(15, 80);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.Size = new System.Drawing.Size(757, 456);
            this.dataGridView1.TabIndex = 5;
            this.dataGridView1.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentDoubleClick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Opcode 0x";
            // 
            // txtOpcode
            // 
            this.txtOpcode.Location = new System.Drawing.Point(77, 53);
            this.txtOpcode.Name = "txtOpcode";
            this.txtOpcode.Size = new System.Drawing.Size(64, 20);
            this.txtOpcode.TabIndex = 7;
            this.txtOpcode.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 539);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(784, 22);
            this.statusStrip1.TabIndex = 8;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(89, 17);
            this.toolStripStatusLabel1.Text = "Files Processed:";
            // 
            // timer1
            // 
            this.timer1.Interval = 200;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // columnHits
            // 
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle7.Format = "N0";
            this.columnHits.DefaultCellStyle = dataGridViewCellStyle7;
            this.columnHits.HeaderText = "Hits";
            this.columnHits.Name = "columnHits";
            this.columnHits.ReadOnly = true;
            this.columnHits.Width = 60;
            // 
            // columnFileSize
            // 
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle8.Format = "N0";
            this.columnFileSize.DefaultCellStyle = dataGridViewCellStyle8;
            this.columnFileSize.HeaderText = "File Size";
            this.columnFileSize.Name = "columnFileSize";
            this.columnFileSize.ReadOnly = true;
            this.columnFileSize.Width = 80;
            // 
            // columnFilePath
            // 
            this.columnFilePath.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.columnFilePath.HeaderText = "File Path";
            this.columnFilePath.Name = "columnFilePath";
            this.columnFilePath.ReadOnly = true;
            // 
            // FindOpcodeInFilesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.txtOpcode);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.btnChangeSearchPathRoot);
            this.Controls.Add(this.btnStopSearch);
            this.Controls.Add(this.btnStartSearch);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtSearchPathRoot);
            this.Name = "FindOpcodeInFilesForm";
            this.Text = "Find Opcode In Files";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtSearchPathRoot;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnStartSearch;
        private System.Windows.Forms.Button btnStopSearch;
        private System.Windows.Forms.Button btnChangeSearchPathRoot;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtOpcode;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnHits;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnFileSize;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnFilePath;
    }
}