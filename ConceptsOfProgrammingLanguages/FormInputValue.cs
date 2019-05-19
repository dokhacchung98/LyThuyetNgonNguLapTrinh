using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConceptsOfProgrammingLanguages
{
    public partial class FormInputValue : Form
    {
        public char Result { get; set; }

        public FormInputValue()
        {
            InitializeComponent();
        }

        private void BtnSend_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtValue.Text))
            {
                Result = char.Parse(txtValue.Text);
            }
            else
            {
                Result = Extention.VALUE_E;
            }
            this.DialogResult = DialogResult.OK;
                this.Close();
        }
    }
}
