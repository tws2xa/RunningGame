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
            this.pnlMainContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // lstEntities
            // 
            this.lstEntities.FormattingEnabled = true;
            this.lstEntities.Location = new System.Drawing.Point(12, 12);
            this.lstEntities.Name = "lstEntities";
            this.lstEntities.Size = new System.Drawing.Size(155, 264);
            this.lstEntities.TabIndex = 0;
            this.lstEntities.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // pnlMain
            // 
            this.pnlMain.Location = new System.Drawing.Point(1, 1);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(685, 526);
            this.pnlMain.TabIndex = 2;
            this.pnlMain.Scroll += new System.Windows.Forms.ScrollEventHandler(this.pnlMain_Scroll);
            this.pnlMain.Paint += new System.Windows.Forms.PaintEventHandler(this.panel2_Paint);
            // 
            // lstSelectedEntProperties
            // 
            this.lstSelectedEntProperties.FormattingEnabled = true;
            this.lstSelectedEntProperties.Location = new System.Drawing.Point(12, 282);
            this.lstSelectedEntProperties.Name = "lstSelectedEntProperties";
            this.lstSelectedEntProperties.Size = new System.Drawing.Size(155, 173);
            this.lstSelectedEntProperties.TabIndex = 3;
            this.lstSelectedEntProperties.SelectedIndexChanged += new System.EventHandler(this.lstSelectedEntProperties_SelectedIndexChanged);
            // 
            // txtVar
            // 
            this.txtVar.Location = new System.Drawing.Point(127, 461);
            this.txtVar.Name = "txtVar";
            this.txtVar.Size = new System.Drawing.Size(40, 20);
            this.txtVar.TabIndex = 4;
            this.txtVar.TextChanged += new System.EventHandler(this.txtVar_TextChanged);
            // 
            // lblVar
            // 
            this.lblVar.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblVar.AutoSize = true;
            this.lblVar.Location = new System.Drawing.Point(12, 464);
            this.lblVar.Name = "lblVar";
            this.lblVar.Size = new System.Drawing.Size(26, 13);
            this.lblVar.TabIndex = 5;
            this.lblVar.Text = "Var:";
            this.lblVar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // pnlMainContainer
            // 
            this.pnlMainContainer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlMainContainer.Controls.Add(this.pnlMain);
            this.pnlMainContainer.Location = new System.Drawing.Point(173, 12);
            this.pnlMainContainer.Name = "pnlMainContainer";
            this.pnlMainContainer.Size = new System.Drawing.Size(684, 525);
            this.pnlMainContainer.TabIndex = 6;
            this.pnlMainContainer.Scroll += new System.Windows.Forms.ScrollEventHandler(this.pnlMainContainer_Scroll);
            // 
            // btnLoadFromPaint
            // 
            this.btnLoadFromPaint.Location = new System.Drawing.Point(12, 489);
            this.btnLoadFromPaint.Name = "btnLoadFromPaint";
            this.btnLoadFromPaint.Size = new System.Drawing.Size(155, 22);
            this.btnLoadFromPaint.TabIndex = 7;
            this.btnLoadFromPaint.Text = "Load From Paint";
            this.btnLoadFromPaint.UseVisualStyleBackColor = true;
            this.btnLoadFromPaint.Click += new System.EventHandler(this.btnLoadFromPaint_Click);
            // 
            // btnCreate
            // 
            this.btnCreate.Location = new System.Drawing.Point(12, 517);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(153, 20);
            this.btnCreate.TabIndex = 8;
            this.btnCreate.Text = "Create!";
            this.btnCreate.UseVisualStyleBackColor = true;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog1_FileOk);
            // 
            // FormEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(869, 549);
            this.Controls.Add(this.btnCreate);
            this.Controls.Add(this.btnLoadFromPaint);
            this.Controls.Add(this.pnlMainContainer);
            this.Controls.Add(this.lblVar);
            this.Controls.Add(this.txtVar);
            this.Controls.Add(this.lstSelectedEntProperties);
            this.Controls.Add(this.lstEntities);
            this.Name = "FormEditor";
            this.Text = "FormEditor";
            this.Load += new System.EventHandler(this.FormEditor_Load);
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
    }
}