using System;
using System.ComponentModel;

namespace Add_Application
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
            this.components = new System.ComponentModel.Container();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.runButton = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.Cancel = new System.Windows.Forms.Button();
            this.Remove = new System.Windows.Forms.CheckBox();
            this.List = new System.Windows.Forms.Button();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader0 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.RemoveItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.clearToolStripMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.useSccm = new System.Windows.Forms.CheckBox();
            this.availableRadio = new System.Windows.Forms.RadioButton();
            this.requiredRadio = new System.Windows.Forms.RadioButton();
            this.contextMenuStrip1.SuspendLayout();
            this.contextMenuStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(12, 423);
            this.listBox1.Name = "listBox1";
            this.listBox1.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.listBox1.Size = new System.Drawing.Size(493, 238);
            this.listBox1.TabIndex = 0;
            this.listBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ListBox1_MouseUp);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(460, 18);
            this.label1.TabIndex = 1;
            this.label1.Text = "Add printers with a CSV, Printer Server, or type in the printer";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.MenuText;
            this.label2.Location = new System.Drawing.Point(12, 307);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(258, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "Example: \\\\PrinterServer\\Printer or Browse";
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.button1.Location = new System.Drawing.Point(452, 307);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(53, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "Help";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 326);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(294, 20);
            this.textBox1.TabIndex = 4;
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.ForeColor = System.Drawing.SystemColors.MenuText;
            this.button2.Location = new System.Drawing.Point(312, 322);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(77, 28);
            this.button2.TabIndex = 5;
            this.button2.Text = "Browse...";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // runButton
            // 
            this.runButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.runButton.ForeColor = System.Drawing.SystemColors.WindowText;
            this.runButton.Location = new System.Drawing.Point(408, 356);
            this.runButton.Name = "runButton";
            this.runButton.Size = new System.Drawing.Size(97, 23);
            this.runButton.TabIndex = 6;
            this.runButton.Text = "Run";
            this.runButton.UseVisualStyleBackColor = true;
            this.runButton.Click += new System.EventHandler(this.runButton_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(84, 356);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(112, 17);
            this.checkBox1.TabIndex = 7;
            this.checkBox1.Text = "Use Printer Server";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.CheckBox1_CheckedChanged);
            // 
            // Cancel
            // 
            this.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancel.Enabled = false;
            this.Cancel.Location = new System.Drawing.Point(408, 356);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(97, 23);
            this.Cancel.TabIndex = 8;
            this.Cancel.Text = "Cancel";
            this.Cancel.UseVisualStyleBackColor = true;
            this.Cancel.Visible = false;
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // Remove
            // 
            this.Remove.AutoSize = true;
            this.Remove.Location = new System.Drawing.Point(12, 356);
            this.Remove.Name = "Remove";
            this.Remove.Size = new System.Drawing.Size(66, 17);
            this.Remove.TabIndex = 9;
            this.Remove.Text = "Remove";
            this.Remove.UseVisualStyleBackColor = true;
            this.Remove.CheckedChanged += new System.EventHandler(this.Remove_CheckedChanged);
            // 
            // List
            // 
            this.List.Location = new System.Drawing.Point(312, 356);
            this.List.Name = "List";
            this.List.Size = new System.Drawing.Size(73, 27);
            this.List.TabIndex = 10;
            this.List.Text = "Add to List";
            this.List.UseVisualStyleBackColor = true;
            this.List.Click += new System.EventHandler(this.List_Click);
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader0,
            this.columnHeader1,
            this.columnHeader2});
            this.listView1.FullRowSelect = true;
            this.listView1.LabelEdit = true;
            this.listView1.Location = new System.Drawing.Point(12, 39);
            this.listView1.Name = "listView1";
            this.listView1.ShowGroups = false;
            this.listView1.Size = new System.Drawing.Size(493, 255);
            this.listView1.TabIndex = 11;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ListView1_MouseClick);
            // 
            // columnHeader0
            // 
            this.columnHeader0.Text = "Printer";
            this.columnHeader0.Width = 163;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Printer Server";
            this.columnHeader1.Width = 163;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Model";
            this.columnHeader2.Width = 163;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.RemoveItem,
            this.removeAllToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(135, 48);
            // 
            // RemoveItem
            // 
            this.RemoveItem.Name = "RemoveItem";
            this.RemoveItem.Size = new System.Drawing.Size(134, 22);
            this.RemoveItem.Text = "Remove";
            this.RemoveItem.Click += new System.EventHandler(this.RemoveItem_Click);
            // 
            // removeAllToolStripMenuItem
            // 
            this.removeAllToolStripMenuItem.Name = "removeAllToolStripMenuItem";
            this.removeAllToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.removeAllToolStripMenuItem.Text = "Remove All";
            this.removeAllToolStripMenuItem.Click += new System.EventHandler(this.RemoveAllToolStripMenuItem_Click);
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearToolStripMenu});
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new System.Drawing.Size(102, 26);
            // 
            // clearToolStripMenu
            // 
            this.clearToolStripMenu.Name = "clearToolStripMenu";
            this.clearToolStripMenu.Size = new System.Drawing.Size(101, 22);
            this.clearToolStripMenu.Text = "Clear";
            this.clearToolStripMenu.Click += new System.EventHandler(this.ClearToolStripMenu_Click);
            // 
            // useSccm
            // 
            this.useSccm.AutoSize = true;
            this.useSccm.Enabled = false;
            this.useSccm.Location = new System.Drawing.Point(202, 356);
            this.useSccm.Name = "useSccm";
            this.useSccm.Size = new System.Drawing.Size(93, 17);
            this.useSccm.TabIndex = 12;
            this.useSccm.Text = "Search SCCM";
            this.useSccm.UseVisualStyleBackColor = true;
            // 
            // availableRadio
            // 
            this.availableRadio.AutoSize = true;
            this.availableRadio.Location = new System.Drawing.Point(15, 391);
            this.availableRadio.Name = "availableRadio";
            this.availableRadio.Size = new System.Drawing.Size(68, 17);
            this.availableRadio.TabIndex = 13;
            this.availableRadio.TabStop = true;
            this.availableRadio.Text = "Available";
            this.availableRadio.UseVisualStyleBackColor = true;
            this.availableRadio.Checked = true;
            this.availableRadio.CheckedChanged += new System.EventHandler(this.RadioButtons_CheckedChanged);
            // 
            // requiredRadio
            // 
            this.requiredRadio.AutoSize = true;
            this.requiredRadio.Location = new System.Drawing.Point(111, 391);
            this.requiredRadio.Name = "requiredRadio";
            this.requiredRadio.Size = new System.Drawing.Size(68, 17);
            this.requiredRadio.TabIndex = 14;
            this.requiredRadio.TabStop = true;
            this.requiredRadio.Text = "Required";
            this.requiredRadio.UseVisualStyleBackColor = true;
            this.requiredRadio.CheckedChanged += new System.EventHandler(this.RadioButtons_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.Cancel;
            this.ClientSize = new System.Drawing.Size(519, 673);
            this.Controls.Add(this.requiredRadio);
            this.Controls.Add(this.availableRadio);
            this.Controls.Add(this.useSccm);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.List);
            this.Controls.Add(this.Remove);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.runButton);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Add Printers";
            this.contextMenuStrip1.ResumeLayout(false);
            this.contextMenuStrip2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button runButton;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.CheckBox Remove;
        private System.Windows.Forms.Button List;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader0;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem RemoveItem;
        private System.Windows.Forms.ToolStripMenuItem removeAllToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private System.Windows.Forms.ToolStripMenuItem clearToolStripMenu;
        private System.Windows.Forms.CheckBox useSccm;
        private System.Windows.Forms.RadioButton availableRadio;
        private System.Windows.Forms.RadioButton requiredRadio;
    }
}

