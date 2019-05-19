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
            AddNewState();
        }

        private void InitProject()
        {
            foreach (var item in _State_arr)
            {
                automataView.DeleteSelectsable(item);
            }
            _State_arr.Clear();
            _lastIndexOfState = 0;

            automataView.BuildAutomata();
            automataView.Refresh();
        }

        private void AddNewState()
        {
            var state = new State("q" + _lastIndexOfState);
            _State_arr.Add(state);
            _lastIndexOfState++;
            automataView.States.Add(state);

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

        private void BtnConvert_Click(object sender, EventArgs e)
        {
            DetectFinalState();
        }

        private void DetectFinalState()
        {
            IList<State> listFinalState = new List<State>();
            foreach (var item in _State_arr)
            {
                if (automataView.IsFinalState(item))
                {
                    listFinalState.Add(item);
                }
            }
            if (listFinalState.Count == 0)
            {
                MessageBox.Show("Lỗi: Không có trạng thái kết thúc");
                return;
            }
            else if (listFinalState.Count > 1)
            {
                CreateSigleFinalState(listFinalState);
            }
            CreateTransitionAllState();
        }

        private void CreateTransitionAllState()
        {
            IList<ItemTableConnector> listItemConnector = new List<ItemTableConnector>();
            var listStateConnector = automataView.GetListConnector();

            foreach (var item1 in _State_arr)
            {
                foreach (var item2 in _State_arr)
                {
                    string value;
                    StateConnector tmp = null;
                    foreach (var item in listStateConnector)
                    {
                        if (item.SourceState == item1 && item.DestinationState == item2)
                        {
                            tmp = item;
                            break;
                        }
                    }
                    if (tmp != null)
                    {
                        value = tmp.Label.Text;
                    }
                    else
                    {
                        value = Extention.VALUE_NULL.ToString();
                        item1.AddTransition(Extention.VALUE_NULL, item2);
                    }

                    listItemConnector.Add(new ItemTableConnector()
                    {
                        SourceState = item1,
                        DestinationState = item2,
                        Value = value
                    });
                }
            }
            automataView.BuildAutomata();
            automataView.Refresh();
        }

        private void CreateSigleFinalState(IList<State> listFinalState)
        {
            AddNewState();
            var stateFinal = _State_arr.ElementAt(_State_arr.Count - 1);
            automataView.SetFinalState(stateFinal);

            foreach (var item in listFinalState)
            {
                automataView.SetFinalState(item, false);
                item.AddTransition(Extention.VALUE_E, stateFinal);
            }
            automataView.BuildAutomata();
            automataView.Refresh();
        }

        private void BtnRemove_Click(object sender, EventArgs e)
        {
            InitProject();
        }
    }
}
