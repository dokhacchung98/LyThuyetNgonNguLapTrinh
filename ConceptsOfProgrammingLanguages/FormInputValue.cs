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
                if ((((int)char.Parse(txtValue.Text)) >= 97 && ((int)char.Parse(txtValue.Text)) <= 122) || (((int)char.Parse(txtValue.Text)) >= 48 && ((int)char.Parse(txtValue.Text)) <= 57))
                {
                    Result = char.Parse(txtValue.Text);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }

            }
            else
            {
                Result = Extention.VALUE_E;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}
