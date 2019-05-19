using Automata;
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
    public partial class Form1 : Form
    {
        private string[] _characters = { "a", "b" };
        private IList<State> _State_arr = new List<State>();
        Selectable _selectable;
        private int _lastIndexOfState = 0;
        private State _sourceState = null;
        private State _destinationState = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void BtnAddState_Click(object sender, EventArgs e)
        {
            var state = new State("q" + _lastIndexOfState);
            _State_arr.Add(state);
            _lastIndexOfState++;
            automataView.States.Add(state);

            //var state1 = new State("q" + 4);
            //var state2 = new State("q" + 5);

            //automataView.States.Add(state1);
            //automataView.States.Add(state2);

            //state1.AddTransition('a', state2);
            //state2.AddTransition('b', state2);
            //state2.AddTransition('c', state2);
            //state2.AddTransition('e', state2);

            //automataView.SetStartState(state1);
            //automataView.SetFinalState(state2);

            automataView.BuildAutomata();
            automataView.Refresh();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            automataView.ItemSelected += automataView_ItemSelected;
            KeyPreview = true;
        }

        private void automataView_ItemSelected(object sender, ItemSelectInfo info)
        {
            _selectable = info.selected;
        }
        private void automataView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && _selectable != null)
            {
                if (_selectable is StateConnector)
                {
                    itemFinalState.Enabled = false;
                    selectableContextMenu.Show(automataView, e.Location);
                }
                else if (_selectable is State)
                {
                    itemCurve.Checked = _selectable is State;
                    itemFinalState.Enabled = true;
                    if (automataView.IsFinalState((State)_selectable))
                    {
                        itemFinalState.Checked = true;
                    }
                    else
                    {
                        itemFinalState.Checked = false;
                    }
                    selectableContextMenu.Show(automataView, e.Location);
                }
            }
        }

        //khi click vao menu trip
        private void connectorContextMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            int selItemIndex = selectableContextMenu.Items.IndexOf(e.ClickedItem);
            switch (selItemIndex)
            {
                //xác định trạng thái kết thúc của state
                case 0:
                    if (_selectable != null && _selectable is State)
                    {
                        var state = (State)_selectable;
                        automataView.SetFinalState((State)_selectable, automataView.IsFinalState(state) ? false : true);
                    }
                    automataView.Invalidate();
                    break;
                //xóa selectable
                case 1:
                    if (_selectable != null && ((_selectable is State) || (_selectable is StateConnector)))
                    {
                        automataView.DeleteSelectsable(_selectable);
                    }
                    break;
            }
            automataView.Refresh();
        }


        private bool _isMouseDown = false;
        private void AutomataView_MouseDown(object sender, MouseEventArgs e)
        {
            if (!automataView.isEnableMouseHere)
            {
                automataView.startPoint = e.Location;
                _isMouseDown = true;

                foreach (var item in _State_arr)
                {
                    if (item.HitTest(e.Location, automataView.Graphics))
                    {
                        _sourceState = item;
                        break;
                    }
                }
            }
        }

        private void BtnAddArrow_Click(object sender, EventArgs e)
        {
            automataView.isEnableMouseHere = !automataView.isEnableMouseHere;
            if (!automataView.isEnableMouseHere)
            {
                btnAddArrow.BackColor = Color.BlueViolet;
            }
            else
            {
                btnAddArrow.BackColor = Color.Transparent;
            }
        }

        private void AutomataView_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isMouseDown)
            {
                automataView.endPoint = e.Location;
                Refresh();
            }
        }

        private void AutomataView_MouseUp(object sender, MouseEventArgs e)
        {
            if (_isMouseDown && !automataView.isEnableMouseHere)
            {
                automataView.endPoint = e.Location;
                _isMouseDown = false;

                if (_sourceState != null)
                {
                    foreach (var item in _State_arr)
                    {
                        if (item.HitTest(e.Location, automataView.Graphics))
                        {
                            _destinationState = item;
                            break;
                        }
                    }
                    if (_destinationState != null)
                    {
                        char resultForm = new char();
                        using (FormInputValue form = new FormInputValue())
                        {
                            if (form.ShowDialog() == DialogResult.OK)
                            {
                                resultForm = form.Result;
                            }
                        }
                        if (!string.IsNullOrEmpty(resultForm.ToString()) && !resultForm.Equals('\0'))
                        {
                            _sourceState.AddTransition(resultForm, _destinationState);
                            automataView.BuildAutomata();
                        }
                    }
                }
            }

            automataView.startPoint = automataView.endPoint = AutomataView.InitPoint;
            automataView.Refresh();
        }
    }
}
