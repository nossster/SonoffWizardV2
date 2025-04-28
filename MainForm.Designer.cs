namespace SonoffWizardV2
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.DataGridView dataGridViewDevices;
        private System.Windows.Forms.Button buttonAdd;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonScan;
        private System.Windows.Forms.Button buttonGenerate;
        private System.Windows.Forms.Button buttonHelp;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.dataGridViewDevices = new System.Windows.Forms.DataGridView();
            this.buttonAdd      = new System.Windows.Forms.Button();
            this.buttonDelete   = new System.Windows.Forms.Button();
            this.buttonSave     = new System.Windows.Forms.Button();
            this.buttonScan     = new System.Windows.Forms.Button();
            this.buttonGenerate = new System.Windows.Forms.Button();
            this.buttonHelp     = new System.Windows.Forms.Button();
            this.statusStrip    = new System.Windows.Forms.StatusStrip();
            this.statusLabel    = new System.Windows.Forms.ToolStripStatusLabel();

            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDevices)).BeginInit();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridViewDevices
            // 
            this.dataGridViewDevices.Anchor =
                ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top
                   | System.Windows.Forms.AnchorStyles.Bottom)
                   | System.Windows.Forms.AnchorStyles.Left)
                   | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewDevices.ColumnHeadersHeightSizeMode =
                System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewDevices.Location = new System.Drawing.Point(12, 12);
            this.dataGridViewDevices.Name     = "dataGridViewDevices";
            this.dataGridViewDevices.Size     = new System.Drawing.Size(760, 310);
            // 
            // buttonAdd
            // 
            this.buttonAdd.Text   = "Add";
            this.buttonAdd.Location = new System.Drawing.Point(12, 335);
            this.buttonAdd.Size     = new System.Drawing.Size(100, 30);
            this.buttonAdd.Anchor   = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.buttonAdd.Click   += new System.EventHandler(this.buttonAdd_Click);
            // 
            // buttonDelete
            // 
            this.buttonDelete.Text   = "Delete";
            this.buttonDelete.Location = new System.Drawing.Point(118, 335);
            this.buttonDelete.Size     = new System.Drawing.Size(100, 30);
            this.buttonDelete.Anchor   = this.buttonAdd.Anchor;
            // 
            // buttonSave
            // 
            this.buttonSave.Text   = "Save";
            this.buttonSave.Location = new System.Drawing.Point(224, 335);
            this.buttonSave.Size     = new System.Drawing.Size(100, 30);
            this.buttonSave.Anchor   = this.buttonAdd.Anchor;
            this.buttonSave.Click   += new System.EventHandler(this.buttonSave_Click);
            // 
            // buttonScan
            // 
            this.buttonScan.Text   = "Scan";
            this.buttonScan.Location = new System.Drawing.Point(330, 335);
            this.buttonScan.Size     = new System.Drawing.Size(100, 30);
            this.buttonScan.Anchor   = this.buttonAdd.Anchor;
            this.buttonScan.Click   += new System.EventHandler(this.buttonScan_Click);
            // 
            // buttonGenerate
            // 
            this.buttonGenerate.Text   = "Generate BAT";
            this.buttonGenerate.Location = new System.Drawing.Point(436, 335);
            this.buttonGenerate.Size     = new System.Drawing.Size(130, 30);
            this.buttonGenerate.Anchor   = this.buttonAdd.Anchor;
            this.buttonGenerate.Click   += new System.EventHandler(this.buttonGenerate_Click);
            // 
            // buttonHelp
            // 
            this.buttonHelp.Text   = "?";
            this.buttonHelp.Location = new System.Drawing.Point(740, 335);
            this.buttonHelp.Size     = new System.Drawing.Size(32, 30);
            this.buttonHelp.Anchor   = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this.buttonHelp.Click   += new System.EventHandler(this.buttonHelp_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.statusLabel });
            this.statusStrip.Location = new System.Drawing.Point(0, 375);
            this.statusStrip.Size     = new System.Drawing.Size(784, 22);
            // 
            // statusLabel
            // 
            this.statusLabel.Text = "Ready.";
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(784, 397);
            this.Controls.AddRange(new System.Windows.Forms.Control[]
            {
                this.dataGridViewDevices, this.buttonAdd, this.buttonDelete,
                this.buttonSave, this.buttonScan, this.buttonGenerate, this.buttonHelp,
                this.statusStrip
            });
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text          = "Sonoff Wizard V2";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDevices)).EndInit();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
