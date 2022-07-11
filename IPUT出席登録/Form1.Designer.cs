namespace IPUT出席登録
{
    partial class アカウント情報入力
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.userid = new System.Windows.Forms.TextBox();
            this.password = new System.Windows.Forms.TextBox();
            this.ログインボタン = new System.Windows.Forms.Button();
            this.ユーザーID = new System.Windows.Forms.Label();
            this.パスワード = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // userid
            // 
            this.userid.Location = new System.Drawing.Point(94, 101);
            this.userid.Name = "userid";
            this.userid.Size = new System.Drawing.Size(574, 31);
            this.userid.TabIndex = 0;
            // 
            // password
            // 
            this.password.Location = new System.Drawing.Point(94, 301);
            this.password.Name = "password";
            this.password.Size = new System.Drawing.Size(574, 31);
            this.password.TabIndex = 1;
            // 
            // ログインボタン
            // 
            this.ログインボタン.Location = new System.Drawing.Point(455, 420);
            this.ログインボタン.Name = "ログインボタン";
            this.ログインボタン.Size = new System.Drawing.Size(213, 51);
            this.ログインボタン.TabIndex = 2;
            this.ログインボタン.Text = "ログイン";
            this.ログインボタン.UseVisualStyleBackColor = true;
            this.ログインボタン.Click += new System.EventHandler(this.ログインボタン_Click);
            // 
            // ユーザーID
            // 
            this.ユーザーID.AutoSize = true;
            this.ユーザーID.Location = new System.Drawing.Point(94, 71);
            this.ユーザーID.Name = "ユーザーID";
            this.ユーザーID.Size = new System.Drawing.Size(112, 24);
            this.ユーザーID.TabIndex = 4;
            this.ユーザーID.Text = "ユーザーID";
            // 
            // パスワード
            // 
            this.パスワード.AutoSize = true;
            this.パスワード.Location = new System.Drawing.Point(98, 271);
            this.パスワード.Name = "パスワード";
            this.パスワード.Size = new System.Drawing.Size(103, 24);
            this.パスワード.TabIndex = 5;
            this.パスワード.Text = "パスワード";
            // 
            // アカウント情報入力
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 511);
            this.Controls.Add(this.パスワード);
            this.Controls.Add(this.ユーザーID);
            this.Controls.Add(this.ログインボタン);
            this.Controls.Add(this.password);
            this.Controls.Add(this.userid);
            this.Name = "アカウント情報入力";
            this.Text = "アカウント情報入力";
            this.Shown += new System.EventHandler(this.アカウント情報入力_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox userid;
        private System.Windows.Forms.TextBox password;
        private System.Windows.Forms.Button ログインボタン;
        private System.Windows.Forms.Label ユーザーID;
        private System.Windows.Forms.Label パスワード;
    }
}

