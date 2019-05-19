namespace ConceptsOfProgrammingLanguages
{
    partial class Form1
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
            this.btnAddState = new System.Windows.Forms.Button();
            this.itemCurve = new System.Windows.Forms.ToolStripMenuItem();
            this.itemFinalState = new System.Windows.Forms.ToolStripMenuItem();
            this.itemRemoveSelector = new System.Windows.Forms.ToolStripMenuItem();
            this.selectableContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.btnAddArrow = new System.Windows.Forms.Button();
            this.automataView = new Automata.AutomataView();
            this.txtParametor = new System.Windows.Forms.TextBox();
            this.btnConvert = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.selectableContextMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.automataView)).BeginInit();
            this.SuspendLayout();
            // 
            // btnAddState
            // 
            this.btnAddState.Location = new System.Drawing.Point(12, 667);
            this.btnAddState.Name = "btnAddState";
            this.btnAddState.Size = new System.Drawing.Size(108, 43);
            this.btnAddState.TabIndex = 1;
            this.btnAddState.Text = "Thêm State";
            this.btnAddState.UseVisualStyleBackColor = true;
            this.btnAddState.Click += new System.EventHandler(this.BtnAddState_Click);
            // 
            // itemCurve
            // 
            this.itemCurve.CheckOnClick = true;
            this.itemCurve.Name = "itemCurve";
            this.itemCurve.Size = new System.Drawing.Size(174, 22);
            this.itemCurve.Text = "Vẽ cong";
            // 
            // itemFinalState
            // 
            this.itemFinalState.CheckOnClick = true;
            this.itemFinalState.Name = "itemFinalState";
            this.itemFinalState.Size = new System.Drawing.Size(173, 22);
            this.itemFinalState.Text = "Trạng thái kết thúc";
            // 
            // itemRemoveSelector
            // 
            this.itemRemoveSelector.CheckOnClick = true;
            this.itemRemoveSelector.Name = "itemRemoveSelector";
            this.itemRemoveSelector.Size = new System.Drawing.Size(173, 22);
            this.itemRemoveSelector.Text = "Xóa";
            // 
            // selectableContextMenu
            // 
            this.selectableContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.itemFinalState,
            this.itemRemoveSelector});
            this.selectableContextMenu.Name = "connectorContextMenu";
            this.selectableContextMenu.Size = new System.Drawing.Size(174, 48);
            this.selectableContextMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.connectorContextMenu_ItemClicked);
            // 
            // btnAddArrow
            // 
            this.btnAddArrow.Location = new System.Drawing.Point(179, 667);
            this.btnAddArrow.Name = "btnAddArrow";
            this.btnAddArrow.Size = new System.Drawing.Size(116, 43);
            this.btnAddArrow.TabIndex = 2;
            this.btnAddArrow.Text = "Thêm đường nối";
            this.btnAddArrow.UseVisualStyleBackColor = true;
            this.btnAddArrow.Click += new System.EventHandler(this.BtnAddArrow_Click);
            // 
            // automataView
            // 
            this.automataView.BackColor = System.Drawing.SystemColors.ControlLight;
            this.automataView.endPoint = new System.Drawing.Point(0, 0);
            this.automataView.isEnableMouseHere = true;
            this.automataView.Location = new System.Drawing.Point(0, 1);
            this.automataView.Name = "automataView";
            this.automataView.Size = new System.Drawing.Size(658, 658);
            this.automataView.startPoint = new System.Drawing.Point(0, 0);
            this.automataView.TabIndex = 0;
            this.automataView.TabStop = false;
            this.automataView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.automataView_MouseClick);
            this.automataView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.AutomataView_MouseDown);
            this.automataView.MouseMove += new System.Windows.Forms.MouseEventHandler(this.AutomataView_MouseMove);
            this.automataView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.AutomataView_MouseUp);
            // 
            // txtParametor
            // 
            this.txtParametor.Location = new System.Drawing.Point(0, 0);
            this.txtParametor.Name = "txtParametor";
            this.txtParametor.Size = new System.Drawing.Size(100, 20);
            this.txtParametor.TabIndex = 0;
            // 
            // btnConvert
            // 
            this.btnConvert.Location = new System.Drawing.Point(354, 667);
            this.btnConvert.Name = "btnConvert";
            this.btnConvert.Size = new System.Drawing.Size(116, 43);
            this.btnConvert.TabIndex = 3;
            this.btnConvert.Text = "Chuyển đổi";
            this.btnConvert.UseVisualStyleBackColor = true;
            this.btnConvert.Click += new System.EventHandler(this.BtnConvert_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(530, 665);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(116, 43);
            this.btnRemove.TabIndex = 4;
            this.btnRemove.Text = "Xóa dữ liệu";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.BtnRemove_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(658, 722);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnConvert);
            this.Controls.Add(this.btnAddArrow);
            this.Controls.Add(this.btnAddState);
            this.Controls.Add(this.automataView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Form1";
            this.RightToLeftLayout = true;
            this.Text = "Phạm Tiến Nam - Đỗ Khắc Chung : FA to RE";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.selectableContextMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.automataView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Automata.AutomataView automataView;
        private System.Windows.Forms.Button btnAddState;
        private System.Windows.Forms.ToolStripMenuItem itemCurve;
        private System.Windows.Forms.ToolStripMenuItem itemFinalState;
        private System.Windows.Forms.ToolStripMenuItem itemRemoveSelector;
        private System.Windows.Forms.ContextMenuStrip selectableContextMenu;
        private System.Windows.Forms.Button btnAddArrow;
        private System.Windows.Forms.TextBox txtParametor;
        private System.Windows.Forms.Button btnConvert;
        private System.Windows.Forms.Button btnRemove;
    }
}

