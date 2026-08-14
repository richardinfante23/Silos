namespace principal
{
    partial class preciosForm
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
            this.cbPrecios = new System.Windows.Forms.ComboBox();
            this.Precios = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cbPrecios
            // 
            this.cbPrecios.FormattingEnabled = true;
            this.cbPrecios.Location = new System.Drawing.Point(44, 73);
            this.cbPrecios.Name = "cbPrecios";
            this.cbPrecios.Size = new System.Drawing.Size(258, 24);
            this.cbPrecios.TabIndex = 0;
            // 
            // Precios
            // 
            this.Precios.AutoSize = true;
            this.Precios.Location = new System.Drawing.Point(127, 39);
            this.Precios.Name = "Precios";
            this.Precios.Size = new System.Drawing.Size(55, 17);
            this.Precios.TabIndex = 1;
            this.Precios.Text = "Precios";
            // 
            // preciosForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(340, 143);
            this.ControlBox = false;
            this.Controls.Add(this.Precios);
            this.Controls.Add(this.cbPrecios);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "preciosForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label Precios;
        public System.Windows.Forms.ComboBox cbPrecios;
    }
}