namespace labirint
{
    partial class Form1
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.textresult = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonGo = new System.Windows.Forms.Button();
            this.trackspeed = new System.Windows.Forms.TrackBar();
            this.textspeed = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.textdensity = new System.Windows.Forms.TextBox();
            this.trackdensity = new System.Windows.Forms.TrackBar();
            this.label6 = new System.Windows.Forms.Label();
            this.textsize = new System.Windows.Forms.TextBox();
            this.buttonGenerat = new System.Windows.Forms.Button();
            this.tracksize = new System.Windows.Forms.TrackBar();
            this.buttonClearTrack = new System.Windows.Forms.Button();
            this.textmolot = new System.Windows.Forms.TextBox();
            this.opisanie = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textbomb = new System.Windows.Forms.TextBox();
            this.textSgore = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.molot1 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.trackspeed)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackdensity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tracksize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.molot1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // textresult
            // 
            this.textresult.Location = new System.Drawing.Point(44, 237);
            this.textresult.Name = "textresult";
            this.textresult.ReadOnly = true;
            this.textresult.Size = new System.Drawing.Size(150, 20);
            this.textresult.TabIndex = 0;
            this.textresult.TabStop = false;
            this.textresult.Text = "exit:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 13);
            this.label1.TabIndex = 1;
            // 
            // buttonGo
            // 
            this.buttonGo.Location = new System.Drawing.Point(780, 510);
            this.buttonGo.Name = "buttonGo";
            this.buttonGo.Size = new System.Drawing.Size(188, 23);
            this.buttonGo.TabIndex = 6;
            this.buttonGo.TabStop = false;
            this.buttonGo.Text = "go";
            this.buttonGo.UseVisualStyleBackColor = true;
            this.buttonGo.Visible = false;
            // 
            // trackspeed
            // 
            this.trackspeed.Enabled = false;
            this.trackspeed.Location = new System.Drawing.Point(94, 173);
            this.trackspeed.Maximum = 20;
            this.trackspeed.Minimum = 1;
            this.trackspeed.Name = "trackspeed";
            this.trackspeed.Size = new System.Drawing.Size(100, 45);
            this.trackspeed.TabIndex = 5;
            this.trackspeed.TabStop = false;
            this.trackspeed.Value = 20;
            this.trackspeed.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // textspeed
            // 
            this.textspeed.Enabled = false;
            this.textspeed.Location = new System.Drawing.Point(44, 173);
            this.textspeed.Name = "textspeed";
            this.textspeed.ReadOnly = true;
            this.textspeed.Size = new System.Drawing.Size(44, 20);
            this.textspeed.TabIndex = 5;
            this.textspeed.TabStop = false;
            this.textspeed.Text = "20";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 176);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "speed";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 240);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(32, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "result";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.textdensity);
            this.groupBox1.Controls.Add(this.trackdensity);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.textsize);
            this.groupBox1.Controls.Add(this.buttonGenerat);
            this.groupBox1.Controls.Add(this.tracksize);
            this.groupBox1.Controls.Add(this.textresult);
            this.groupBox1.Controls.Add(this.trackspeed);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.textspeed);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(788, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 270);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "management";
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // button1
            // 
            this.button1.Enabled = false;
            this.button1.Location = new System.Drawing.Point(6, 144);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(188, 23);
            this.button1.TabIndex = 22;
            this.button1.Text = "prison break";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 67);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(40, 13);
            this.label7.TabIndex = 16;
            this.label7.Text = "density";
            // 
            // textdensity
            // 
            this.textdensity.BackColor = System.Drawing.SystemColors.Menu;
            this.textdensity.Enabled = false;
            this.textdensity.Location = new System.Drawing.Point(44, 64);
            this.textdensity.Name = "textdensity";
            this.textdensity.Size = new System.Drawing.Size(44, 20);
            this.textdensity.TabIndex = 15;
            this.textdensity.TabStop = false;
            this.textdensity.Text = "33%";
            // 
            // trackdensity
            // 
            this.trackdensity.Enabled = false;
            this.trackdensity.Location = new System.Drawing.Point(94, 64);
            this.trackdensity.Minimum = 1;
            this.trackdensity.Name = "trackdensity";
            this.trackdensity.Size = new System.Drawing.Size(100, 45);
            this.trackdensity.TabIndex = 2;
            this.trackdensity.TabStop = false;
            this.trackdensity.Value = 3;
            this.trackdensity.Scroll += new System.EventHandler(this.trackBar3_Scroll);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 24);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(25, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "size";
            // 
            // textsize
            // 
            this.textsize.BackColor = System.Drawing.SystemColors.Menu;
            this.textsize.Enabled = false;
            this.textsize.Location = new System.Drawing.Point(44, 21);
            this.textsize.Name = "textsize";
            this.textsize.Size = new System.Drawing.Size(44, 20);
            this.textsize.TabIndex = 11;
            this.textsize.TabStop = false;
            this.textsize.Text = "19";
            // 
            // buttonGenerat
            // 
            this.buttonGenerat.Location = new System.Drawing.Point(6, 115);
            this.buttonGenerat.Name = "buttonGenerat";
            this.buttonGenerat.Size = new System.Drawing.Size(188, 23);
            this.buttonGenerat.TabIndex = 4;
            this.buttonGenerat.TabStop = false;
            this.buttonGenerat.Text = "generation";
            this.buttonGenerat.UseVisualStyleBackColor = true;
            this.buttonGenerat.Click += new System.EventHandler(this.button2_Click);
            // 
            // tracksize
            // 
            this.tracksize.Enabled = false;
            this.tracksize.Location = new System.Drawing.Point(94, 21);
            this.tracksize.Maximum = 1000;
            this.tracksize.Minimum = 1;
            this.tracksize.Name = "tracksize";
            this.tracksize.Size = new System.Drawing.Size(100, 45);
            this.tracksize.TabIndex = 1;
            this.tracksize.TabStop = false;
            this.tracksize.Value = 19;
            this.tracksize.Scroll += new System.EventHandler(this.trackBar2_Scroll);
            // 
            // buttonClearTrack
            // 
            this.buttonClearTrack.Location = new System.Drawing.Point(780, 481);
            this.buttonClearTrack.Name = "buttonClearTrack";
            this.buttonClearTrack.Size = new System.Drawing.Size(188, 23);
            this.buttonClearTrack.TabIndex = 3;
            this.buttonClearTrack.TabStop = false;
            this.buttonClearTrack.Text = "clear track";
            this.buttonClearTrack.UseVisualStyleBackColor = true;
            this.buttonClearTrack.Visible = false;
            this.buttonClearTrack.Click += new System.EventHandler(this.button3_Click);
            // 
            // textmolot
            // 
            this.textmolot.Enabled = false;
            this.textmolot.Location = new System.Drawing.Point(788, 344);
            this.textmolot.Name = "textmolot";
            this.textmolot.ReadOnly = true;
            this.textmolot.Size = new System.Drawing.Size(50, 20);
            this.textmolot.TabIndex = 12;
            this.textmolot.Text = "3";
            // 
            // opisanie
            // 
            this.opisanie.AutoEllipsis = true;
            this.opisanie.Location = new System.Drawing.Point(785, 367);
            this.opisanie.Name = "opisanie";
            this.opisanie.Size = new System.Drawing.Size(200, 100);
            this.opisanie.TabIndex = 14;
            this.opisanie.Text = "Control:\r\nleft;up;down; right;\r\nA - hammer (break the wall) (next key \"left\",\"rig" +
                "ht\",\"up\",\"down\");\r\nB - bomb (break all wall);\r\nPurpose of the mission:\r\ncome up " +
                "with it\r\n\r\n\r\n";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(953, 818);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 13);
            this.label4.TabIndex = 15;
            this.label4.Text = "606217";
            // 
            // textbomb
            // 
            this.textbomb.Enabled = false;
            this.textbomb.Location = new System.Drawing.Point(850, 344);
            this.textbomb.Name = "textbomb";
            this.textbomb.ReadOnly = true;
            this.textbomb.Size = new System.Drawing.Size(50, 20);
            this.textbomb.TabIndex = 17;
            this.textbomb.Text = "1";
            // 
            // textSgore
            // 
            this.textSgore.Enabled = false;
            this.textSgore.Location = new System.Drawing.Point(906, 305);
            this.textSgore.Name = "textSgore";
            this.textSgore.ReadOnly = true;
            this.textSgore.Size = new System.Drawing.Size(76, 20);
            this.textSgore.TabIndex = 18;
            this.textSgore.Text = "0";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(907, 289);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(38, 13);
            this.label5.TabIndex = 19;
            this.label5.Text = "Sgore:";
            // 
            // textBox1
            // 
            this.textBox1.Enabled = false;
            this.textBox1.Location = new System.Drawing.Point(906, 344);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(76, 20);
            this.textBox1.TabIndex = 20;
            this.textBox1.Text = "0";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(907, 328);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(61, 13);
            this.label8.TabIndex = 21;
            this.label8.Text = "High sgore:";
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = global::labirint.Properties.Resources.bfaadb09eb2d1eb21b7b5f1eac3ea902;
            this.pictureBox3.Location = new System.Drawing.Point(500, 237);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(100, 101);
            this.pictureBox3.TabIndex = 22;
            this.pictureBox3.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::labirint.Properties.Resources.images;
            this.pictureBox2.Location = new System.Drawing.Point(850, 288);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(50, 50);
            this.pictureBox2.TabIndex = 16;
            this.pictureBox2.TabStop = false;
            // 
            // molot1
            // 
            this.molot1.Image = global::labirint.Properties.Resources._02A057;
            this.molot1.Location = new System.Drawing.Point(788, 288);
            this.molot1.Name = "molot1";
            this.molot1.Size = new System.Drawing.Size(50, 50);
            this.molot1.TabIndex = 11;
            this.molot1.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.ErrorImage = null;
            this.pictureBox1.Image = global::labirint.Properties.Resources._124;
            this.pictureBox1.Location = new System.Drawing.Point(10, 10);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(20, 20);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Visible = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1000, 840);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textSgore);
            this.Controls.Add(this.buttonClearTrack);
            this.Controls.Add(this.textbomb);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.buttonGo);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.opisanie);
            this.Controls.Add(this.textmolot);
            this.Controls.Add(this.molot1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1016, 878);
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Text = "labirint 3d";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.trackspeed)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackdensity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tracksize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.molot1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textresult;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button buttonGo;
        private System.Windows.Forms.TrackBar trackspeed;
        private System.Windows.Forms.TextBox textspeed;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonGenerat;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textsize;
        private System.Windows.Forms.TrackBar tracksize;
        private System.Windows.Forms.Button buttonClearTrack;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textdensity;
        private System.Windows.Forms.TrackBar trackdensity;
        private System.Windows.Forms.PictureBox molot1;
        private System.Windows.Forms.TextBox textmolot;
        private System.Windows.Forms.Label opisanie;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.TextBox textbomb;
        private System.Windows.Forms.TextBox textSgore;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.PictureBox pictureBox3;


    }
}

