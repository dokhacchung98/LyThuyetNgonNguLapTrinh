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
        private int _lastIndexOfState = 1;
        private State _sourceState = null;
        private State _destinationState = null;
        public Form1()
        {
            Application.Restart();
            Environment.Exit(0);
        }

        private void BtnAddState_Click(object sender, EventArgs e)
        {
            AddNewState();
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
                itemRemoveSelector.Checked = false;
                if (_selectable is StateConnector)
                {
                    itemFinalState.Enabled = false;
                    itemStartState.Enabled = false;
                    selectableContextMenu.Show(automataView, e.Location);
                }
                else if (_selectable is State)
                {
                    itemCurve.Checked = _selectable is State;
                    itemFinalState.Enabled = true;
                    itemStartState.Enabled = true;

                    if (automataView.IsStartState((State)_selectable))
                    {
                        itemStartState.Checked = true;
                    }
                    else
                    {
                        itemStartState.Checked = false;
                    }

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
                case 0:
                    if (_selectable != null && _selectable is State)
                    {
                        State currentState = (State)_selectable;
                        automataView.SetStartState(currentState);
                    }
                    automataView.Invalidate();
                    break;
                case 1:
                    if (_selectable != null && _selectable is State)
                    {
                        var state = (State)_selectable;
                        automataView.SetFinalState((State)_selectable, automataView.IsFinalState(state) ? false : true);
                    }
                    automataView.Invalidate();
                    break;
                //xóa selectable
                case 2:
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



        private void BtnRemove_Click(object sender, EventArgs e)
        {
            InitProject();
        }

        //
        //Xử lý chuyển đổi
        //

        private int _handlerStep = 0;

        private IList<ItemTableConnector> _listItemConnector = new List<ItemTableConnector>();
        private IList<string> _listNameState = new List<string>();
        private string _nameStartState = "";
        private string _nameFinalState = "";

        //chuot click
        private void BtnConvert_Click(object sender, EventArgs e)
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
            else if (automataView.GetStartState() == null)
            {
                MessageBox.Show("Lỗi: Không có trạng thái bắt đầu");
                return;
            }


            switch (_handlerStep)
            {
                case 0:
                    HandlerStartState();
                    btnConvert.Text = "Bước tiếp";
                    btnAddState.Visible = false;
                    btnAddArrow.Visible = false;
                    break;
                case 1:
                    DetectFinalState();
                    break;
                case 2:
                    HandlerFinalState();
                    break;
                case 3:
                    CreateTransitionAllState();
                    break;
                case 4:
                    GroupStateConnector();
                    break;
                case 5:
                    EliminationState();
                    btnConvert.Text = "Hiển thị kết quả";
                    break;
                case 6:
                    ShowResultConvertSuccess();
                    break;
            }
            _handlerStep++;
            if (_handlerStep > 6)
            {
                _handlerStep = 6;
            }
        }
        public static string ResultReAfterConvert = "";

        private void ShowResultConvertSuccess()
        {
            ResultReAfterConvert = Extention.getExpressionFromGTG(_listItemConnector, _nameStartState, _nameFinalState);
            ShowResult showResult = new ShowResult();
            showResult.Show();
        }

        //Xác định trạng thái cuối cùng hay không
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
            if (listFinalState.Count > 1)
            {
                CreateSigleFinalState(listFinalState);
            }
            else if (listFinalState.Count == 1)
            {
                MessageBox.Show("Automata không có nhiều hơn 1 trạng thái kết thúc");
            }
        }


        //thay đổi trạng thái cuối cùng nếu có connector quay ngược lại start state
        private void HandlerStartState()
        {
            var startState = automataView.GetStartState();
            var connectors = automataView.GetListStateConnectorBySigleState(startState, false);
            bool check = true;
            if (connectors.Count > 0)
            {
                foreach (var item in connectors)
                {
                    if (item.DestinationState != item.SourceState)
                    {
                        check = false;
                        break;
                    }
                }
            }
            if (!check)
            {
                var state = new State("q0");
                _State_arr.Add(state);
                automataView.States.Add(state);
                automataView.SetStartState(state);
                state.AddTransition(FaToReConverter.VALUE_E, startState);
                automataView.BuildAutomata();
                automataView.Refresh();
            }
            else
            {
                MessageBox.Show("Không có đường nối nào quay lại trạng thái kết thúc");
            }
        }

        //thay đổi trạng thái cuối cùng nếu có connector đi từ state cuối đến state khác
        private void HandlerFinalState()
        {
            var listFinalState = automataView.GetListFinalState();
            if (listFinalState.Count == 1)
            {
                var state = listFinalState.ElementAt(0);

                var connectors = automataView.GetListStateConnectorBySigleState(state, true);
                bool check = true;
                foreach (var item in connectors)
                {
                    if (item.SourceState != item.DestinationState)
                    {
                        check = false;
                        break;
                    }
                }
                if (!check)
                {
                    AddNewState();
                    var stateFinal = _State_arr.ElementAt(_State_arr.Count - 1);

                    foreach (var item in listFinalState)
                    {
                        automataView.SetFinalState(item, false);
                        break;
                    }
                    automataView.SetFinalState(stateFinal, true);
                    state.AddTransition(FaToReConverter.VALUE_E, stateFinal);

                    automataView.BuildAutomata();
                    automataView.Refresh();
                }
                else
                {
                    MessageBox.Show("Không có đường nối từ trạng thái kết thúc sang trạng thái khác");
                }
            }
        }

        //Nhóm trạng thái khi chuyển từ state1 -> state1
        private void GroupStateConnector()
        {
            foreach (var item in automataView.GetListConnector())
            {
                if (item.Label.Text.Contains(','))
                {
                    automataView.SetStateConnectorInList(FaToReConverter.GrossConnector(item));
                }
            }

            CreateDataTable();
            automataView.Refresh();
        }

        //Tạo tất cả các đường nối giữa các trạng thái
        private void CreateTransitionAllState()
        {

            var listStateConnector = automataView.GetListConnector();

            foreach (var item1 in _State_arr)
            {
                _listNameState.Add(item1.Label);
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
                        value = tmp.Label.Text.Replace(" ", FaToReConverter.LAMBDA).Replace(",", FaToReConverter.OR);
                    }
                    else
                    {
                        value = FaToReConverter.VALUE_NULL.ToString();
                        item1.AddTransition(FaToReConverter.VALUE_NULL, item2);
                    }
                    _listItemConnector.Add(new ItemTableConnector()
                    {
                        SourceState = item1.Label,
                        DestinationState = item2.Label,
                        Value = value
                    });
                }
            }

            automataView.BuildAutomata();
            automataView.Refresh();
        }

        //Tạo bảng giá trị
        private void CreateDataTable()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("Trạng thái bắt đầu");
            dt.Columns.Add("Trạng thái kết thúc");
            dt.Columns.Add("Giá trị");

            foreach (var item in _listItemConnector)
            {
                dt.Rows.Add(item.SourceState, item.DestinationState, item.Value);
            }

            gridView.DataSource = dt;
        }

        //Tạo duy nhất 1 trạng thái kết thúc
        private void CreateSigleFinalState(IList<State> listFinalState)
        {
            AddNewState();
            var stateFinal = _State_arr.ElementAt(_State_arr.Count - 1);
            automataView.SetFinalState(stateFinal);

            foreach (var item in listFinalState)
            {
                automataView.SetFinalState(item, false);
                item.AddTransition(FaToReConverter.VALUE_E, stateFinal);
            }
            automataView.BuildAutomata();
            automataView.Refresh();
        }

        //Xử lý chuyển đổi
        private void EliminationState()
        {
            _nameFinalState = automataView.GetListFinalState()[0].Label;
            _nameStartState = automataView.GetStartState().Label;
            foreach (var item in _listNameState.ToList())
            {
                if (item != _nameFinalState && item != _nameStartState)
                {
                    GetAllConnectorForRemoveState(item);
                    RemoveState(item);
                }
            }

            RenderUi();
        }

        private void RenderUi()
        {
            CreateDataTable();
            foreach (var item in _State_arr)
            {
                automataView.DeleteSelectsable(item);
            }
            _State_arr.Clear();
            _lastIndexOfState = 1;


            State startState = new State(_nameStartState);
            State finalState = new State(_nameFinalState);
            _State_arr.Add(startState);
            _State_arr.Add(finalState);
            automataView.States.Add(startState);
            automataView.States.Add(finalState);
            automataView.SetStartState(startState);
            automataView.SetFinalState(finalState);
            
            foreach(var connector in _listItemConnector)
            {
                if (connector.SourceState.Equals(_nameStartState) && connector.DestinationState.Equals(_nameFinalState))
                {
                    startState.AddTransition(connector.Value.ToCharArray(), finalState);
                } else if (connector.SourceState.Equals(_nameStartState) && connector.DestinationState.Equals(_nameStartState))
                {
                    startState.AddTransition(connector.Value.ToCharArray(), startState);
                } else if (connector.SourceState.Equals(_nameFinalState) && connector.DestinationState.Equals(_nameFinalState))
                {
                    finalState.AddTransition(connector.Value.ToCharArray(), finalState);
                } else if (connector.SourceState.Equals(_nameFinalState) && connector.DestinationState.Equals(_nameStartState))
                {
                    finalState.AddTransition(connector.Value.ToCharArray(), startState);
                }
            }
            automataView.BuildAutomata();
            automataView.Invalidate();
            automataView.Refresh();
        }


        //Xóa state đã xét
        private void RemoveState(string stateRemove)
        {
            _listNameState.Remove(stateRemove);
            foreach (var item in _listItemConnector.ToList())
            {
                if (item.SourceState == stateRemove || item.DestinationState == stateRemove)
                {
                    _listItemConnector.Remove(item);
                }
            }
        }

        //Lấy tất cả connector giữa state cần xét
        private void GetAllConnectorForRemoveState(string state)
        {
            if (!IsCanRemoveState(state))
                return;
            foreach (var stateFrom in _listNameState.ToList())
            {
                if (stateFrom != state)
                {
                    foreach (var stateTo in _listNameState.ToList())
                    {
                        if (stateTo != state)
                        {
                            string exp = getExpression(stateFrom, stateTo, state);
                            ChangeLabel2StateFromAndTo(stateFrom, stateTo, exp);
                        }
                    }
                }
            }
        }

        //Kiểm tra state chỉ xóa được khi không phải start state và final state
        private bool IsCanRemoveState(string state)
        {
            _nameFinalState = automataView.GetListFinalState()[0].Label;
            _nameStartState = automataView.GetStartState().Label;
            if (state == _nameFinalState || state == _nameStartState)
            {
                return false;
            }
            return true;
        }

        //Lấy chuỗi label giữa 2 state qua remove state với công thức : r(pq) = r(pq) + r(pk)r(kk)*r(kq)
        //p: from state
        //q: to state
        //k: state cần loại bỏ
        public string getExpression(string p, string q, string k)
        {
            string pq = getExpressionBetweenStates(p, q);
            string pk = getExpressionBetweenStates(p, k);
            string kk = getExpressionBetweenStates(k, k);
            string kq = getExpressionBetweenStates(k, q);

            string temp1 = Extention.star(kk);
            string temp2 = Extention.concatenate(pk, temp1);
            string temp3 = Extention.concatenate(temp2, kq);
            string label = Extention.or(pq, temp3);
            return label;
        }

        //Lấy label giữa 2 trạng thái
        private string getExpressionBetweenStates(string fromState, string toState)
        {
            ItemTableConnector con = GetItemTableBySourceAndDes(fromState, toState);
            if (con != null)
            {
                return con.Value;
            }
            return FaToReConverter.VALUE_NULL.ToString();
        }

        public void ChangeLabel2StateFromAndTo(string p, string q, string expression)
        {
            ItemTableConnector itemOld = GetItemTableBySourceAndDes(p, q);
            ItemTableConnector itemNew = new ItemTableConnector()
            {
                SourceState = p,
                DestinationState = q,
                Value = expression
            };
            _listItemConnector.Remove(itemOld);
            _listItemConnector.Add(itemNew);
        }

        private ItemTableConnector GetItemTableBySourceAndDes(string source, string des)
        {
            return _listItemConnector.Where(t => t.SourceState == source && t.DestinationState == des).FirstOrDefault();
        }
    }
}
