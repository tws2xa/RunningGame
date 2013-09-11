namespace RunningGame
{
    partial class FormSpring
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
            this.btnBegin = new System.Windows.Forms.Button();
            this.lblLoading = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnBegin
            // 
            this.btnBegin.Location = new System.Drawing.Point(502, 427);
            this.btnBegin.Name = "btnBegin";
            this.btnBegin.Size = new System.Drawing.Size(126, 41);
            this.btnBegin.TabIndex = 0;
            this.btnBegin.Text = "Begin!";
            this.btnBegin.UseVisualStyleBackColor = true;
            this.btnBegin.Click += new System.EventHandler(this.btnBegin_Click);
            // 
            // lblLoading
            // 
            this.lblLoading.AutoSize = true;
            this.lblLoading.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLoading.Location = new System.Drawing.Point(228, 202);
            this.lblLoading.Name = "lblLoading";
            this.lblLoading.Size = new System.Drawing.Size(0, 39);
            this.lblLoading.TabIndex = 1;
            this.lblLoading.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FormSpring
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(640, 480);
            this.Controls.Add(this.lblLoading);
            this.Controls.Add(this.btnBegin);
            this.Name = "FormSpring";
            this.Text = "Project: Spring";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormRunningGame_FormClosing);
            this.Load += new System.EventHandler(this.FormRunningGame_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormRunningGame_KeyDown);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.FormRunningGame_KeyPress);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.FormRunningGame_KeyUp);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnBegin;
        private System.Windows.Forms.Label lblLoading;
    }
}

