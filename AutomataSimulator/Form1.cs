using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Automata;

namespace AutomataSimulator
{
    public partial class Form1 : Form
    {
        private string[] _characters = { "a", "b" };
        private State[] _State_arr = new State[3];
        Selectable _selectable;
        

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 3; i++)
            {
                string stateLabel = String.Format("q{0}", i);
                _State_arr[i] = new State(stateLabel);
                automataView1.States.Add(_State_arr[i]);
            }

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    String stateLabel = "q" + j.ToString();
                    String transitChar = _characters[i];
                    if (stateLabel != null && stateLabel != String.Empty)
                    {
                        string[] stateNames = stateLabel.Split(new char[] { ',' });
                        foreach (string stateName in stateNames)
                        {
                            _State_arr[i].AddTransition(transitChar[0],
                                State.GetStateFromName(stateName.Trim()));
                        }
                    }
                }
            }
            _State_arr[0].AddTransition('a', _State_arr[2]);
            _State_arr[2].AddTransition('a', _State_arr[0]);
            _State_arr[2].AddTransition('b', _State_arr[0]);
            _State_arr[2].AddTransition('b', _State_arr[1]);
            automataView1.SetFinalState(_State_arr[2]);

            automataView1.BuildAutomata();
            automataView1.Refresh();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            ConnectorLabel.ScreenGraphics.Dispose();
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                automataView1.LeaveCurveDrawingMode();
                automataView1.Invalidate();
            }
        }

        private void automataView1_ItemSelected(object sender, ItemSelectInfo info)
        {
            _selectable = info.selected;
        }

        private void automataView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && _selectable != null)
            {
                if (_selectable is StateConnector)
                {
                    MessageBox.Show("Connector!");
                }
                else if (_selectable is State)
                {
                    var state = _selectable as State;
                    MessageBox.Show("State!");
                }
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            automataView1.ItemSelected += automataView1_ItemSelected;
            KeyPreview = true;
        }

    }
}
