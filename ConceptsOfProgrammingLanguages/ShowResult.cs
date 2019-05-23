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
    public partial class ShowResult : Form
    {
        public ShowResult()
        {
            InitializeComponent();
        }

        private void Label1_Click(object sender, EventArgs e)
        {

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ShowResult_Load(object sender, EventArgs e)
        {
            string result = Form1.ResultReAfterConvert;
            textBox1.Text = result;
        }
    }
}
