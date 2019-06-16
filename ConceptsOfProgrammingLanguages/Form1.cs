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
        private IList<State> _State_arr_Result = new List<State>();
        Selectable _selectable;
        Selectable _selectableResult;
        private int _lastIndexOfState = 1;
        private State _sourceState = null;
        private State _destinationState = null;
        private bool _isCreateState = true;
        private bool _isStepRemoveState = false;
        public Form1()
        {
            InitializeComponent();
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
            state.X = 50;
            state.Y = 50;
            automataView.States.Add(state);

            automataView.BuildAutomata();
            automataView.Refresh();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            automataView.ItemSelected += automataView_ItemSelected;
            automataViewResult.ItemSelected += automataViewResult_ItemSelected;
            KeyPreview = true;
        }

        private void automataView_ItemSelected(object sender, ItemSelectInfo info)
        {
            _selectable = info.selected;
        }

        private void automataViewResult_ItemSelected(object sender, ItemSelectInfo info)
        {
            _selectableResult = info.selected;
        }

        private void automataView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && _selectable != null && _isCreateState)
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
            if (!automataView.isEnableMouseHere && _isCreateState)
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
            else if (e.Button == MouseButtons.Left && _isStepRemoveState)
            {
                foreach (var item in _State_arr)
                {
                    if (item.HitTest(e.Location, automataView.Graphics))
                    {
                        EliminationStateByNameState(item.Label);
                        return;
                    }
                }
            }
        }

        private void BtnAddArrow_Click(object sender, EventArgs e)
        {
            automataView.isEnableMouseHere = !automataView.isEnableMouseHere;
            if (!automataView.isEnableMouseHere)
            {
                btnAddArrow.BackColor = Color.CornflowerBlue;
            }
            else
            {
                btnAddArrow.BackColor = Color.Transparent;
            }
        }

        private void AutomataView_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isMouseDown && _isCreateState)
            {
                automataView.endPoint = e.Location;
                Refresh();
            }
        }

        private void AutomataView_MouseUp(object sender, MouseEventArgs e)
        {
            if (_isMouseDown && !automataView.isEnableMouseHere && _isCreateState)
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
            Application.Restart();
            Environment.Exit(0);
        }

        //
        //Xử lý chuyển đổi
        //

        private int _handlerStep = 0;

        private IList<ItemTableConnector> _listItemConnector = new List<ItemTableConnector>();
        private IList<TempState> _listNameState = new List<TempState>();
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
                    _isCreateState = false;
                    automataView.isEnableMouseHere = !automataView.isEnableMouseHere;
                    HandleImage();
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
                    GroupStateConnector();
                    btnConvert.Visible = false;
                    VisiableBtnShowResult();
                    HandleCursor();
                    break;
            }
            _handlerStep++;
            if (_handlerStep > 4)
            {
                _handlerStep = 4;
            }
        }

        //tạo 1 automata mới lưu trữ trạng thái cũ
        private void HandleImage()
        {
            _State_arr_Result.Clear();
            foreach (var item in _State_arr)
            {
                _State_arr_Result.Add(item);
                automataViewResult.States.Add(item);
            }

            automataViewResult.BuildAutomata();
            var listfinalState = automataView.GetListFinalState();
            var startState = automataView.GetStartState();
            if (startState != null)
            {
                State s = automataViewResult.GetStateByLabel(startState.Label);
                if (s != null)
                    automataViewResult.SetStartState(s);
            }

            foreach (var item in listfinalState)
            {
                State t = automataViewResult.GetStateByLabel(item.Label);
                if (t != null)
                {
                    automataViewResult.SetFinalState(t);
                }
            }

            automataViewResult.BuildAutomata();
            automataViewResult.Refresh();

            var locationTmp = automataView.Location;
            automataView.Location = automataViewResult.Location;
            automataViewResult.Location = locationTmp;

            Refresh();
        }

        public static string ResultReAfterConvert = "";

        //mở form mới hiển thị kết quả cuối cùng
        private void ShowResultConvertSuccess()
        {
            string result = Extention.GetExpressionFromGTG(_listItemConnector, _nameStartState, _nameFinalState);
            ResultReAfterConvert = FormatResult(result);
            ShowResult showResult = new ShowResult();
            showResult.Show();
        }

        private string FormatResult(string result)
        {
            result = result.Replace(FaToReConverter.VALUE_E.ToString(), string.Empty);

            return result;
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

            //CreateDataTable();
            automataView.Refresh();
        }

        //Tạo tất cả các đường nối giữa các trạng thái
        private void CreateTransitionAllState()
        {

            var listStateConnector = automataView.GetListConnector();

            foreach (var item1 in _State_arr)
            {
                _listNameState.Add(new TempState()
                {
                    NameState = item1.Label,
                    Position = item1.Position
                });
                foreach (var item2 in _State_arr)
                {
                    string value = FaToReConverter.VALUE_NULL.ToString();
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
                        value = tmp.Label.Text.Replace(" ", FaToReConverter.LAMBDA).Replace(",", FaToReConverter.OR).Replace(FaToReConverter.VALUE_E.ToString(), FaToReConverter.LAMBDA).Trim(FaToReConverter.OR.ToCharArray());
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

        //Xử lý chuyển đổi tất cả các state
        private void EliminationState()
        {
            _nameFinalState = automataView.GetListFinalState()[0].Label;
            _nameStartState = automataView.GetStartState().Label;
            foreach (var item in _listNameState.ToList())
            {
                if (item.NameState != _nameFinalState && item.NameState != _nameStartState)
                {
                    GetAllConnectorForRemoveState(item);
                    RemoveState(item);
                }
            }
            RenderUi();
        }

        private void EliminationStateByNameState(string nameState)
        {
            _nameFinalState = automataView.GetListFinalState()[0].Label;
            _nameStartState = automataView.GetStartState().Label;
            if (_listNameState.Any(t => t.NameState == nameState) && nameState != _nameFinalState && nameState != _nameStartState)
            {
                TempState t = _listNameState.Where(item => item.NameState == nameState).FirstOrDefault();

                GetAllConnectorForRemoveState(t);
                RemoveState(t);
            }
            RenderAutomata();
        }

        //Render lại ra automata
        private void RenderUi()
        {
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

            foreach (var connector in _listItemConnector)
            {
                if (connector.SourceState.Equals(_nameStartState) && connector.DestinationState.Equals(_nameFinalState))
                {
                    startState.AddTransition(connector.Value.ToCharArray(), finalState);
                }
                else if (connector.SourceState.Equals(_nameStartState) && connector.DestinationState.Equals(_nameStartState))
                {
                    startState.AddTransition(connector.Value.ToCharArray(), startState);
                }
                else if (connector.SourceState.Equals(_nameFinalState) && connector.DestinationState.Equals(_nameFinalState))
                {
                    finalState.AddTransition(connector.Value.ToCharArray(), finalState);
                }
                else if (connector.SourceState.Equals(_nameFinalState) && connector.DestinationState.Equals(_nameStartState))
                {
                    finalState.AddTransition(connector.Value.ToCharArray(), startState);
                }
            }
            automataView.BuildAutomata();
            automataView.Invalidate();
            automataView.Refresh();
        }

        private void RenderAutomata()
        {
            foreach (var item in _State_arr)
            {
                automataView.DeleteSelectsable(item);
            }
            _State_arr.Clear();
            _lastIndexOfState = 1;

            foreach (var item in _listNameState)
            {
                State tmp = new State(item.NameState);
                tmp.X = item.Position.X;
                tmp.Y = item.Position.Y;
                _State_arr.Add(tmp);
                automataView.States.Add(tmp);
                if (item.NameState == _nameStartState)
                {
                    automataView.SetStartState(tmp);
                }
                if (item.NameState == _nameFinalState)
                {
                    automataView.SetFinalState(tmp);
                }
            }

            VisiableBtnShowResult();

            foreach (var stt1 in _State_arr)
            {
                foreach (var stt2 in _State_arr)
                {
                    var value = _listItemConnector.Where(t => t.SourceState == stt1.Label
                        && t.DestinationState == stt2.Label).FirstOrDefault().Value;
                    if (value != FaToReConverter.EMPTY
                        && value != FaToReConverter.VALUE_NULL.ToString()
                        && value != FaToReConverter.LAMBDA)
                    {
                        stt1.AddTransition(value.ToCharArray(), stt2);
                    }
                }
            }

            automataView.BuildAutomata();
            automataView.Invalidate();
            automataView.Refresh();
        }

        //Kiểm tra nếu có còn phải là 2 trạng thái cuối ko để hiển thị kết quả
        private void VisiableBtnShowResult()
        {
            if ((_listNameState.Count == 2 && _nameFinalState != _nameStartState)
                || _listNameState.Count == 1)
            {
                btnShowResult.Visible = true;
            }
        }

        //Xóa state đã xét
        private void RemoveState(TempState stateRemove)
        {
            _listNameState.Remove(stateRemove);
            foreach (var item in _listItemConnector.ToList())
            {
                if (item.SourceState == stateRemove.NameState || item.DestinationState == stateRemove.NameState)
                {
                    _listItemConnector.Remove(item);
                }
            }
        }

        //Lấy tất cả connector giữa state cần xét
        private void GetAllConnectorForRemoveState(TempState state)
        {
            if (!IsCanRemoveState(state.NameState))
                return;
            foreach (var stateFrom in _listNameState.ToList())
            {
                if (stateFrom != state)
                {
                    foreach (var stateTo in _listNameState.ToList())
                    {
                        if (stateTo != state)
                        {
                            string exp = getExpression(stateFrom.NameState, stateTo.NameState, state.NameState);
                            ChangeLabel2StateFromAndTo(stateFrom.NameState, stateTo.NameState, exp);
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

            string temp1 = Extention.Star(kk);
            string temp2 = Extention.Concatenate(pk, temp1);
            string temp3 = Extention.Concatenate(temp2, kq);
            string label = Extention.Or(pq, temp3);
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

        //thay đổi con trỏ chuột
        private void ChangeCursorImage()
        {
            _isStepRemoveState = true;
            automataView.Cursor = CreateCursor((Bitmap)imageList1.Images[0], new Size(50, 50));
        }

        private static Cursor CreateCursor(Bitmap bm, Size size)
        {
            bm = new Bitmap(bm, size);
            bm.MakeTransparent();
            return new Cursor(bm.GetHicon());
        }

        private void HandleCursor()
        {
            ChangeCursorImage();
        }

        private void BtnShowResult_Click(object sender, EventArgs e)
        {
            ShowResultConvertSuccess();
        }
    }
}
