namespace RunningGame
{
    partial class FormEditor
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
            this.lstEntities = new System.Windows.Forms.ListBox();
            this.pnlMain = new System.Windows.Forms.Panel();
            this.lstSelectedEntProperties = new System.Windows.Forms.ListBox();
            this.txtVar = new System.Windows.Forms.TextBox();
            this.lblVar = new System.Windows.Forms.Label();
            this.pnlMainContainer = new System.Windows.Forms.Panel();
            this.btnLoadFromPaint = new System.Windows.Forms.Button();
            this.btnCreate = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.chkLockToGrid = new System.Windows.Forms.CheckBox();
            this.lblId = new System.Windows.Forms.Label();
            this.txtId = new System.Windows.Forms.TextBox();
            this.txtFileName = new System.Windows.Forms.TextBox();
            this.pnlMainContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // lstEntities
            // 
            this.lstEntities.FormattingEnabled = true;
            this.lstEntities.Location = new System.Drawing.Point(12, 12);
            this.lstEntities.Name = "lstEntities";
            this.lstEntities.Size = new System.Drawing.Size(155, 238);
            this.lstEntities.TabIndex = 0;
            this.lstEntities.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // pnlMain
            // 
            this.pnlMain.Location = new System.Drawing.Point(1, 1);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(632, 497);
            this.pnlMain.TabIndex = 2;
            this.pnlMain.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pnlMain_MouseClick);
            // 
            // lstSelectedEntProperties
            // 
            this.lstSelectedEntProperties.FormattingEnabled = true;
            this.lstSelectedEntProperties.Location = new System.Drawing.Point(12, 256);
            this.lstSelectedEntProperties.Name = "lstSelectedEntProperties";
            this.lstSelectedEntProperties.Size = new System.Drawing.Size(155, 173);
            this.lstSelectedEntProperties.TabIndex = 3;
            this.lstSelectedEntProperties.SelectedIndexChanged += new System.EventHandler(this.lstSelectedEntProperties_SelectedIndexChanged);
            // 
            // txtVar
            // 
            this.txtVar.Location = new System.Drawing.Point(127, 435);
            this.txtVar.Name = "txtVar";
            this.txtVar.Size = new System.Drawing.Size(40, 20);
            this.txtVar.TabIndex = 4;
            this.txtVar.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtChangeAccept);
            // 
            // lblVar
            // 
            this.lblVar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lblVar.AutoSize = true;
            this.lblVar.Location = new System.Drawing.Point(12, 438);
            this.lblVar.Name = "lblVar";
            this.lblVar.Size = new System.Drawing.Size(26, 13);
            this.lblVar.TabIndex = 5;
            this.lblVar.Text = "Var:";
            this.lblVar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // pnlMainContainer
            // 
            this.pnlMainContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlMainContainer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlMainContainer.Controls.Add(this.pnlMain);
            this.pnlMainContainer.Location = new System.Drawing.Point(173, 12);
            this.pnlMainContainer.Name = "pnlMainContainer";
            this.pnlMainContainer.Size = new System.Drawing.Size(684, 575);
            this.pnlMainContainer.TabIndex = 6;
            // 
            // btnLoadFromPaint
            // 
            this.btnLoadFromPaint.Location = new System.Drawing.Point(12, 511);
            this.btnLoadFromPaint.Name = "btnLoadFromPaint";
            this.btnLoadFromPaint.Size = new System.Drawing.Size(155, 22);
            this.btnLoadFromPaint.TabIndex = 7;
            this.btnLoadFromPaint.Text = "Load From Paint";
            this.btnLoadFromPaint.UseVisualStyleBackColor = true;
            this.btnLoadFromPaint.Click += new System.EventHandler(this.btnLoadFromPaint_Click);
            // 
            // btnCreate
            // 
            this.btnCreate.Location = new System.Drawing.Point(102, 539);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(65, 21);
            this.btnCreate.TabIndex = 8;
            this.btnCreate.Text = "Create!";
            this.btnCreate.UseVisualStyleBackColor = true;
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog1_FileOk);
            // 
            // chkLockToGrid
            // 
            this.chkLockToGrid.AutoSize = true;
            this.chkLockToGrid.Checked = true;
            this.chkLockToGrid.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkLockToGrid.Location = new System.Drawing.Point(12, 482);
            this.chkLockToGrid.Name = "chkLockToGrid";
            this.chkLockToGrid.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.chkLockToGrid.Size = new System.Drawing.Size(84, 17);
            this.chkLockToGrid.TabIndex = 10;
            this.chkLockToGrid.Text = "Lock to Grid";
            this.chkLockToGrid.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkLockToGrid.UseVisualStyleBackColor = true;
            this.chkLockToGrid.CheckedChanged += new System.EventHandler(this.chkLockToGrid_CheckedChanged);
            // 
            // lblId
            // 
            this.lblId.AutoSize = true;
            this.lblId.Location = new System.Drawing.Point(12, 460);
            this.lblId.Name = "lblId";
            this.lblId.Size = new System.Drawing.Size(53, 13);
            this.lblId.TabIndex = 11;
            this.lblId.Text = "Entity ID: ";
            // 
            // txtId
            // 
            this.txtId.BackColor = System.Drawing.SystemColors.Window;
            this.txtId.Location = new System.Drawing.Point(67, 457);
            this.txtId.Name = "txtId";
            this.txtId.ReadOnly = true;
            this.txtId.Size = new System.Drawing.Size(100, 20);
            this.txtId.TabIndex = 12;
            // 
            // txtFileName
            // 
            this.txtFileName.Location = new System.Drawing.Point(15, 540);
            this.txtFileName.Name = "txtFileName";
            this.txtFileName.Size = new System.Drawing.Size(84, 20);
            this.txtFileName.TabIndex = 13;
            this.txtFileName.TextChanged += new System.EventHandler(this.txtFileName_TextChanged);
            // 
            // FormEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(869, 599);
            this.Controls.Add(this.txtFileName);
            this.Controls.Add(this.txtId);
            this.Controls.Add(this.lblId);
            this.Controls.Add(this.chkLockToGrid);
            this.Controls.Add(this.btnCreate);
            this.Controls.Add(this.btnLoadFromPaint);
            this.Controls.Add(this.pnlMainContainer);
            this.Controls.Add(this.lblVar);
            this.Controls.Add(this.txtVar);
            this.Controls.Add(this.lstSelectedEntProperties);
            this.Controls.Add(this.lstEntities);
            this.Name = "FormEditor";
            this.Text = "FormEditor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormEditor_FormClosing);
            this.Load += new System.EventHandler(this.FormEditor_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.panelKeyDownEventHandler);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.panelKeyUpEventHandler);
            this.pnlMainContainer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lstEntities;
        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.ListBox lstSelectedEntProperties;
        private System.Windows.Forms.TextBox txtVar;
        private System.Windows.Forms.Label lblVar;
        private System.Windows.Forms.Panel pnlMainContainer;
        private System.Windows.Forms.Button btnLoadFromPaint;
        private System.Windows.Forms.Button btnCreate;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.CheckBox chkLockToGrid;
        private System.Windows.Forms.Label lblId;
        private System.Windows.Forms.TextBox txtId;
        private System.Windows.Forms.TextBox txtFileName;
    }
}