namespace _01_윷놀이
{
    partial class Form1
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnRed = new System.Windows.Forms.Button();
            this.btnBlue = new System.Windows.Forms.Button();
            this.btnFlip = new System.Windows.Forms.Button();
            this.btnRed2 = new System.Windows.Forms.Button();
            this.btnBlue2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnRed
            // 
            this.btnRed.BackColor = System.Drawing.Color.Red;
            this.btnRed.Font = new System.Drawing.Font("Segoe Script", 15F);
            this.btnRed.ForeColor = System.Drawing.Color.White;
            this.btnRed.Location = new System.Drawing.Point(882, 379);
            this.btnRed.Name = "btnRed";
            this.btnRed.Size = new System.Drawing.Size(120, 80);
            this.btnRed.TabIndex = 0;
            this.btnRed.Text = "RED1";
            this.btnRed.UseVisualStyleBackColor = false;
            this.btnRed.Click += new System.EventHandler(this.BtnRed_Click);
            // 
            // btnBlue
            // 
            this.btnBlue.BackColor = System.Drawing.Color.Blue;
            this.btnBlue.Font = new System.Drawing.Font("Segoe Script", 15F);
            this.btnBlue.ForeColor = System.Drawing.Color.White;
            this.btnBlue.Location = new System.Drawing.Point(882, 490);
            this.btnBlue.Name = "btnBlue";
            this.btnBlue.Size = new System.Drawing.Size(120, 80);
            this.btnBlue.TabIndex = 0;
            this.btnBlue.Text = "BLUE1";
            this.btnBlue.UseVisualStyleBackColor = false;
            this.btnBlue.Click += new System.EventHandler(this.BtnBlue_Click);
            // 
            // btnFlip
            // 
            this.btnFlip.Location = new System.Drawing.Point(954, 282);
            this.btnFlip.Name = "btnFlip";
            this.btnFlip.Size = new System.Drawing.Size(123, 77);
            this.btnFlip.TabIndex = 1;
            this.btnFlip.Text = "윷 던지기";
            this.btnFlip.UseVisualStyleBackColor = true;
            this.btnFlip.Click += new System.EventHandler(this.BtnFlip_Click);
            // 
            // btnRed2
            // 
            this.btnRed2.BackColor = System.Drawing.Color.Red;
            this.btnRed2.Font = new System.Drawing.Font("Segoe Script", 15F);
            this.btnRed2.ForeColor = System.Drawing.Color.White;
            this.btnRed2.Location = new System.Drawing.Point(1029, 379);
            this.btnRed2.Name = "btnRed2";
            this.btnRed2.Size = new System.Drawing.Size(120, 80);
            this.btnRed2.TabIndex = 2;
            this.btnRed2.Text = "RED2";
            this.btnRed2.UseVisualStyleBackColor = false;
            this.btnRed2.Click += new System.EventHandler(this.BtnRed2_Click);
            // 
            // btnBlue2
            // 
            this.btnBlue2.BackColor = System.Drawing.Color.Blue;
            this.btnBlue2.Font = new System.Drawing.Font("Segoe Script", 15F);
            this.btnBlue2.ForeColor = System.Drawing.Color.White;
            this.btnBlue2.Location = new System.Drawing.Point(1029, 490);
            this.btnBlue2.Name = "btnBlue2";
            this.btnBlue2.Size = new System.Drawing.Size(120, 80);
            this.btnBlue2.TabIndex = 3;
            this.btnBlue2.Text = "BLUE2";
            this.btnBlue2.UseVisualStyleBackColor = false;
            this.btnBlue2.Click += new System.EventHandler(this.BtnBlue2_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Goldenrod;
            this.ClientSize = new System.Drawing.Size(1184, 961);
            this.Controls.Add(this.btnBlue2);
            this.Controls.Add(this.btnRed2);
            this.Controls.Add(this.btnFlip);
            this.Controls.Add(this.btnBlue);
            this.Controls.Add(this.btnRed);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnRed;
        private System.Windows.Forms.Button btnBlue;
        private System.Windows.Forms.Button btnFlip;
        private System.Windows.Forms.Button btnRed2;
        private System.Windows.Forms.Button btnBlue2;
    }
}

