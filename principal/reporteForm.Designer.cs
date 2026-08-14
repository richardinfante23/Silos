namespace principal
{
    partial class reporteForm
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
            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(reporteForm));
            this.rvReporte = new Microsoft.Reporting.WinForms.ReportViewer();
            this.recepcionBOBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.analisisBindingSource = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.recepcionBOBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.analisisBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // rvReporte
            // 
            this.rvReporte.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            reportDataSource1.Name = "boleta";
            reportDataSource1.Value = this.recepcionBOBindingSource;
            this.rvReporte.LocalReport.DataSources.Add(reportDataSource1);
            this.rvReporte.LocalReport.ReportEmbeddedResource = "principal.boletaRecepcion.rdlc";
            this.rvReporte.Location = new System.Drawing.Point(0, 0);
            this.rvReporte.Margin = new System.Windows.Forms.Padding(0);
            this.rvReporte.Name = "rvReporte";
            this.rvReporte.Size = new System.Drawing.Size(1213, 564);
            this.rvReporte.TabIndex = 0;
            // 
            // recepcionBOBindingSource
            // 
            this.recepcionBOBindingSource.DataSource = typeof(principal.recepcionBO);
            // 
            // analisisBindingSource
            // 
            this.analisisBindingSource.DataSource = typeof(principal.analisis);
            // 
            // reporteForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1213, 564);
            this.Controls.Add(this.rvReporte);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "reporteForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Boleta";
            this.Load += new System.EventHandler(this.reporteForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.recepcionBOBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.analisisBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public Microsoft.Reporting.WinForms.ReportViewer rvReporte;
        private System.Windows.Forms.BindingSource analisisBindingSource;
        private System.Windows.Forms.BindingSource recepcionBOBindingSource;

    }
}