namespace RunningGame {
    partial class FormSpring {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing ) {
            if ( disposing && ( components != null ) ) {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.btnBegin = new System.Windows.Forms.Button();
            this.lblLoading = new System.Windows.Forms.Label();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnWorld1 = new System.Windows.Forms.Button();
            this.btnWorld2 = new System.Windows.Forms.Button();
            this.btnWorld3 = new System.Windows.Forms.Button();
            this.btnWorld4 = new System.Windows.Forms.Button();
            this.btnWorld5 = new System.Windows.Forms.Button();
            this.btnLvl1 = new System.Windows.Forms.Button();
            this.btnLvl2 = new System.Windows.Forms.Button();
            this.btnLvl3 = new System.Windows.Forms.Button();
            this.btnLvlReturn = new System.Windows.Forms.Button();
            this.btnPlay = new System.Windows.Forms.Button();
            this.btnWorld6 = new System.Windows.Forms.Button();
            this.displayFontLbl = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnBegin
            // 
            this.btnBegin.Location = new System.Drawing.Point(502, 432);
            this.btnBegin.Name = "btnBegin";
            this.btnBegin.Size = new System.Drawing.Size(126, 41);
            this.btnBegin.TabIndex = 0;
            this.btnBegin.Text = "Debug Begin!";
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
            // btnEdit
            // 
            this.btnEdit.Enabled = false;
            this.btnEdit.Location = new System.Drawing.Point(502, 386);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(126, 41);
            this.btnEdit.TabIndex = 2;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Visible = false;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnWorld1
            // 
            this.btnWorld1.Location = new System.Drawing.Point(12, 292);
            this.btnWorld1.Name = "btnWorld1";
            this.btnWorld1.Size = new System.Drawing.Size(117, 25);
            this.btnWorld1.TabIndex = 3;
            this.btnWorld1.Text = "World 1";
            this.btnWorld1.UseVisualStyleBackColor = true;
            this.btnWorld1.Click += new System.EventHandler(this.btnWorld1_Click);
            // 
            // btnWorld2
            // 
            this.btnWorld2.Location = new System.Drawing.Point(12, 323);
            this.btnWorld2.Name = "btnWorld2";
            this.btnWorld2.Size = new System.Drawing.Size(117, 25);
            this.btnWorld2.TabIndex = 4;
            this.btnWorld2.Text = "World 2";
            this.btnWorld2.UseVisualStyleBackColor = true;
            this.btnWorld2.Click += new System.EventHandler(this.btnWorld2_Click);
            // 
            // btnWorld3
            // 
            this.btnWorld3.Location = new System.Drawing.Point(12, 354);
            this.btnWorld3.Name = "btnWorld3";
            this.btnWorld3.Size = new System.Drawing.Size(117, 25);
            this.btnWorld3.TabIndex = 5;
            this.btnWorld3.Text = "World 3";
            this.btnWorld3.UseVisualStyleBackColor = true;
            this.btnWorld3.Click += new System.EventHandler(this.btnWorld3_Click);
            // 
            // btnWorld4
            // 
            this.btnWorld4.Location = new System.Drawing.Point(12, 385);
            this.btnWorld4.Name = "btnWorld4";
            this.btnWorld4.Size = new System.Drawing.Size(117, 25);
            this.btnWorld4.TabIndex = 6;
            this.btnWorld4.Text = "World 4";
            this.btnWorld4.UseVisualStyleBackColor = true;
            this.btnWorld4.Click += new System.EventHandler(this.btnWorld4_Click);
            // 
            // btnWorld5
            // 
            this.btnWorld5.Location = new System.Drawing.Point(12, 416);
            this.btnWorld5.Name = "btnWorld5";
            this.btnWorld5.Size = new System.Drawing.Size(117, 25);
            this.btnWorld5.TabIndex = 7;
            this.btnWorld5.Text = "World 5";
            this.btnWorld5.UseVisualStyleBackColor = true;
            this.btnWorld5.Click += new System.EventHandler(this.btnWorld5_Click);
            // 
            // btnLvl1
            // 
            this.btnLvl1.Location = new System.Drawing.Point(12, 354);
            this.btnLvl1.Name = "btnLvl1";
            this.btnLvl1.Size = new System.Drawing.Size(117, 25);
            this.btnLvl1.TabIndex = 8;
            this.btnLvl1.Text = "Level 1";
            this.btnLvl1.UseVisualStyleBackColor = true;
            this.btnLvl1.Click += new System.EventHandler(this.btnLevel1_Click);
            // 
            // btnLvl2
            // 
            this.btnLvl2.Location = new System.Drawing.Point(12, 385);
            this.btnLvl2.Name = "btnLvl2";
            this.btnLvl2.Size = new System.Drawing.Size(117, 25);
            this.btnLvl2.TabIndex = 9;
            this.btnLvl2.Text = "Level 2";
            this.btnLvl2.UseVisualStyleBackColor = true;
            this.btnLvl2.Click += new System.EventHandler(this.btnLvl2_Click);
            // 
            // btnLvl3
            // 
            this.btnLvl3.Location = new System.Drawing.Point(12, 416);
            this.btnLvl3.Name = "btnLvl3";
            this.btnLvl3.Size = new System.Drawing.Size(117, 25);
            this.btnLvl3.TabIndex = 10;
            this.btnLvl3.Text = "Level 3";
            this.btnLvl3.UseVisualStyleBackColor = true;
            this.btnLvl3.Click += new System.EventHandler(this.btnLvl3_Click);
            // 
            // btnLvlReturn
            // 
            this.btnLvlReturn.Location = new System.Drawing.Point(12, 448);
            this.btnLvlReturn.Name = "btnLvlReturn";
            this.btnLvlReturn.Size = new System.Drawing.Size(117, 25);
            this.btnLvlReturn.TabIndex = 11;
            this.btnLvlReturn.Text = "Return";
            this.btnLvlReturn.UseVisualStyleBackColor = true;
            this.btnLvlReturn.Click += new System.EventHandler(this.btnLvlReturn_Click);
            // 
            // btnPlay
            // 
            this.btnPlay.Location = new System.Drawing.Point(12, 432);
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(117, 41);
            this.btnPlay.TabIndex = 12;
            this.btnPlay.Text = "Play";
            this.btnPlay.UseVisualStyleBackColor = true;
            this.btnPlay.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnWorld6
            // 
            this.btnWorld6.Location = new System.Drawing.Point(12, 447);
            this.btnWorld6.Name = "btnWorld6";
            this.btnWorld6.Size = new System.Drawing.Size(117, 25);
            this.btnWorld6.TabIndex = 13;
            this.btnWorld6.Text = "World 6";
            this.btnWorld6.UseVisualStyleBackColor = true;
            this.btnWorld6.Click += new System.EventHandler(this.btnWorld6_Click);
            // 
            // displayFontLbl
            // 
            this.displayFontLbl.AutoSize = true;
            this.displayFontLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.displayFontLbl.Location = new System.Drawing.Point(467, 248);
            this.displayFontLbl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.displayFontLbl.Name = "displayFontLbl";
            this.displayFontLbl.Size = new System.Drawing.Size(152, 55);
            this.displayFontLbl.TabIndex = 14;
            this.displayFontLbl.Tag = "displayFontLbl";
            this.displayFontLbl.Text = "label1";
            this.displayFontLbl.Visible = false;
            // 
            // FormSpring
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImage = global::RunningGame.Properties.Resources.SpringBlurSmall;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(640, 480);
            this.Controls.Add(this.displayFontLbl);
            this.Controls.Add(this.btnWorld6);
            this.Controls.Add(this.btnPlay);
            this.Controls.Add(this.btnLvlReturn);
            this.Controls.Add(this.btnLvl3);
            this.Controls.Add(this.btnLvl2);
            this.Controls.Add(this.btnLvl1);
            this.Controls.Add(this.btnWorld5);
            this.Controls.Add(this.btnWorld4);
            this.Controls.Add(this.btnWorld3);
            this.Controls.Add(this.btnWorld2);
            this.Controls.Add(this.btnWorld1);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.lblLoading);
            this.Controls.Add(this.btnBegin);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Name = "FormSpring";
            this.Text = "Project: Spring";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormRunningGame_FormClosing);
            this.Load += new System.EventHandler(this.FormRunningGame_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormRunningGame_KeyDown);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.FormRunningGame_KeyPress);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.FormRunningGame_KeyUp);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.FormSpring_MouseClick);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnBegin;
        private System.Windows.Forms.Label lblLoading;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnWorld1;
        private System.Windows.Forms.Button btnWorld2;
        private System.Windows.Forms.Button btnWorld3;
        private System.Windows.Forms.Button btnWorld4;
        private System.Windows.Forms.Button btnWorld5;
        private System.Windows.Forms.Button btnLvl1;
        private System.Windows.Forms.Button btnLvl2;
        private System.Windows.Forms.Button btnLvl3;
        private System.Windows.Forms.Button btnLvlReturn;
        private System.Windows.Forms.Button btnPlay;
        private System.Windows.Forms.Button btnWorld6;
        private System.Windows.Forms.Label displayFontLbl;
    }
}

