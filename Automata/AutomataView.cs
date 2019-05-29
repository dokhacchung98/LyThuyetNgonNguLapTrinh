using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Threading;
using System.Linq;

namespace Automata
{
    public class AutomataView : PictureBox
    {
        System.Windows.Forms.Timer _demoTimer;
        private const string CONVERSION_ERROR = "Selected object cannot be converted to curve";
        private const string OBJECT_NOT_TRACKED_ERROR = "Selected object isn't in collection";
        private static Point _InvalidPoint = new Point(-1, -1);
        private const int HANDLE_SIZE = 6;
        private Bitmap _automatBitmap;
        private Graphics _automatGraphics;
        private List<State> _drawnStateList;                // danh sách các trạng thái của automata
        State _startState;                                  // trạng thái bắt đầu
        List<State> _finalStates;                           // danh sách trang thái kết thúc
        private List<StateConnector> _drawnConnectors;      // danh sách các cung nối
        private Selectable[] _selectables;                  // các đối tượng được lựa chọn
        private bool _bShow;                                // thực hiện mô phỏng
        private double _angleStep;                          // góc đểt tính toán sự phân bố các trạng thái trên vòng tròn
        private bool _bCurve;                               // cờ báo chế độ vẽ cong
        Selectable _selectedObj;                            // đối tượng được lựa chọn trong vùng vẽ

        // cần cài đặt thêm điều khiển chuột phải khi có nhiều đối tượng được lựa chọn
        // khi đó có một danh sách các đối tượng được lựa chọn, 
        // với các phần tử ở trong cập nhật lại vị trí, chỉ quảng bá các hành động 
        // ra các đối tượng ngoài danh sách (các cung nối) mà có một đầu nằm bên ngoài
        //List<Selectable> _selectedObjects;
        // các điểm điều khiển hiện thời cho đường cong đang được lựa chọn
        Point[] _curveControlPoints;                        // điểm điều khiển cung nối vẽ cong???
        private int _SelCtrlPointIndex;
        private Size _distance_CtrlPoint_Mouse;
        private Point[] _endPoints;
        private Point _paramPoint;
        private Point[] _bPoints;
        private float? _t0, _t1;
        private Color[] _ColorGradients;
        Color _paramColor;
        AutoResetEvent _event;
        State _hilightState;                                // trạng thái đượclựa chọn sẽ được bôi đậm
        CurvedStateConnector _animateCurve;                 // cung nối cần demo bước chuyển
        Random _rnd;

        /// <summary>
        /// Initializes a new instance of the AutomataView class.
        /// </summary>
        ///
        public AutomataView()
        {
            _drawnStateList = new List<State>();
            _finalStates = new List<State>();
            _drawnConnectors = new List<StateConnector>();
            _curveControlPoints = new Point[2];
            _automatBitmap = new Bitmap(SystemInformation.VirtualScreen.Width,
                SystemInformation.VirtualScreen.Height);
            _automatGraphics = Graphics.FromImage(_automatBitmap);
            _automatGraphics.SmoothingMode = SmoothingMode.AntiAlias;
            _demoTimer = new System.Windows.Forms.Timer();
            _demoTimer.Tick += _demoTimer_Tick;
            _demoTimer.Interval = Config.DELAY_TIME;
            _event = new AutoResetEvent(false);
            _endPoints = new Point[2];
            _ColorGradients = new Color[2];
            _rnd = new Random();
        }
        #region Animate

        /// <summary>
        /// tinh toan va tr? v? cac di?m c?a cung n?i trong qua trinh demo
        /// khong c?n thi?t l?m, c?n hilight toan b? cung
        /// </summary>
        /// <param name="arrPoints">cac tham s? c?a cung n?i</param>
        /// <param name="t">h? s? bi?n d?i?</param>
        /// <param name="bBezier"> n?u la du?ng cong Bezier</param>
        /// <returns></returns>
        private static Point[] CalcParamPoint(Point[] arrPoints, float t, bool bBezier)
        {
            Point point = new Point(0, 0);
            if (bBezier)
            {
                Point[] ptBeziers = new Point[4];
                Point pt12, pt23, pt34, pt1223, pt2334, ptAll;
                pt12 = pt23 = pt34 = pt1223 = pt2334 = ptAll = new Point();
                for (int i = 0; i < 4; i++)
                {
                    ptBeziers[i] = new Point();
                }
                ptBeziers[0] = arrPoints[0];
                pt12.X = (int)((1 - t) * arrPoints[0].X + t * arrPoints[1].X);
                pt12.Y = (int)((1 - t) * arrPoints[0].Y + t * arrPoints[1].Y);
                pt23.X = (int)((1 - t) * arrPoints[1].X + t * arrPoints[2].X);
                pt23.Y = (int)((1 - t) * arrPoints[1].Y + t * arrPoints[2].Y);
                pt34.X = (int)((1 - t) * arrPoints[2].X + t * arrPoints[3].X);
                pt34.Y = (int)((1 - t) * arrPoints[2].Y + t * arrPoints[3].Y);
                pt1223.X = (int)((1 - t) * pt12.X + t * pt23.X);
                pt1223.Y = (int)((1 - t) * pt12.Y + t * pt23.Y);
                pt2334.X = (int)((1 - t) * pt23.X + t * pt34.X);
                pt2334.Y = (int)((1 - t) * pt23.Y + t * pt34.Y);
                ptAll.X = (int)((1 - t) * pt1223.X + t * pt2334.X);
                ptAll.Y = (int)((1 - t) * pt1223.Y + t * pt2334.Y);
                ptBeziers[1] = pt12;
                ptBeziers[2] = pt1223;
                ptBeziers[3] = ptAll;
                return ptBeziers;

            }
            else
            {
                point.X = (int)((1 - t) * arrPoints[0].X + t * arrPoints[1].X);
                point.Y = (int)((1 - t) * arrPoints[0].Y + t * arrPoints[1].Y);
            }
            return new Point[] { point };
        }

        /// <summary>
        /// animate cung n?i khi doan nh?n chu?i
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _demoTimer_Tick(object sender, EventArgs e)
        {
            if (_t0 != null)
            {
                if (_t0 <= 1)
                {
                    bool isBezier = _animateCurve != null;
                    Point[] arrPoints = isBezier ? new Point[]
                                        {
                                            _animateCurve.SourceState.Position,
                                            _animateCurve.ControlPoints[0],
                                            _animateCurve.ControlPoints[1],
                                            _animateCurve.DestinationState.Position
                                        } : _endPoints;
                    Point[] resultPoints = CalcParamPoint(arrPoints, (float)_t0, isBezier);
                    if (isBezier)
                        _bPoints = resultPoints;
                    else
                        _paramPoint = resultPoints[0];
                    _t0 += 0.05f;
                }
                else
                {
                    _t1 = 255;
                    _t0 = null;
                }
            }
            if (_t1 != null)
            {
                int r = (int)(_ColorGradients[1].R * (255 - _t1) / 255.0 +
                    _ColorGradients[0].R * _t1 / 255.0);
                int g = (int)(_ColorGradients[1].G * (255 - _t1) / 255.0 +
                    _ColorGradients[0].G * _t1 / 255.0);
                int b = (int)(_ColorGradients[1].B * (255 - _t1) / 255.0 +
                    _ColorGradients[0].B * _t1 / 255.0);
                _paramColor = Color.FromArgb(r, g, b);
                _t1 -= 12.75f;
                if (_t1 < 0)
                {
                    _demoTimer.Stop();
                    _event.Set();
                    _hilightState = null;
                    _t0 = _t1 = null;
                    _animateCurve = null;
                }
            }
            Invalidate();
        }

        private void startTimer()
        {
            if (InvokeRequired)
                Invoke(new timerDelegate(startTimer));
            else
                _demoTimer.Start();
        }

        private delegate void timerDelegate();

        /// <summary>
        ///  chu?n b? cho vi?c demo doan nh?n chu?i
        /// </summary>
        /// <param name="obj">chu?i c?n xem xet</param>
        public void doBackground(object obj)
        {
            var inputString = obj as string;
            var state = _startState;
            DemoFinishedEvent dfe = new DemoFinishedEvent();
            // l?n lu?t duy?t t?ng ky t? trong chu?i da cho
            for (int i = 0; i < inputString.Length; i++)
            {
                if (state.Transitions.ContainsKey(inputString[i]))
                {
                    _t0 = 0;
                    _t1 = null;
                    // tim cac tr?ng thai co th? d?t d?n t? tr?ng thai da cho;
                    // v?n d? c?n xem xet l?i d?i v?i automat co d?ch chuy?n epsilon
                    var destinedStates = state.Transitions[inputString[i]] as List<State>;
                    _endPoints[0] = state.Position;
                    int random = 0;
                    // d?i v?i NFA, m?t tr?ng thai t?i ng?u nhien s? du?c ch?n
                    // ch? nay c?n ph?i s?a, ph?i xet toan b? cac tr?ng thai k? c?n
                    // khong c?n thi?t ph?i demo du?ng ch?y
                    // ch? c?n hilight trang thai ti?p nh?n chu?i

                    if (destinedStates.Count > 1)
                    {
                        random = _rnd.Next(0, destinedStates.Count);
                    }
                    var destinedState = destinedStates[random];

                    // chua hi?u ro l?m, d? xac d?nh cung n?i c?n animate
                    // c?n hi?u ro v? predicate
                    _animateCurve = (CurvedStateConnector)state.Find(
                        o => o is CurvedStateConnector &&
                       ((CurvedStateConnector)(o)).DestinationState == destinedState);

                    _endPoints[1] = destinedState.Position;
                    _ColorGradients[0] = Color.FromArgb(0xCC, 0x66, 0xFF);
                    _ColorGradients[1] = Color.FromArgb(0x00, 0x66, 0xFF);
                    _hilightState = destinedState;
                    //ph?i du?c g?i trong ng? c?nh c?a UI thread
                    startTimer();
                    _event.WaitOne();
                    state = destinedState;              // chuy?n sang tr?ng thai k? ti?p sau khi x? ly 1 ky t?
                }
                else
                {
                    dfe.WordIsExcepted = false;
                    DemoFinished(this, dfe);
                    return;
                }
            }
            if (_finalStates.Contains(state))
            {
                dfe.WordIsExcepted = true;
                System.Console.WriteLine("Automat accepts this input string");
            }
            else
                dfe.WordIsExcepted = false;
            DemoFinished(this, dfe);
        }

        public void StartDemo(string inputString)
        {
            Thread bgThread = new Thread(doBackground);
            bgThread.Start(inputString);
        }
        #endregion

        public static Point InvalidPoint
        {
            get
            {
                return _InvalidPoint;
            }
        }
        /// <summary>
        /// chế độ vẽ cung nối cong
        /// </summary>
        public void EnterCurveDrawingMode()
        {
            _bCurve = true;
            CurvedStateConnector curvedConnector;
            if (!(_selectedObj is CurvedStateConnector))
            {
                // nếu cung cần animate là cung thẳng, chuyển đổi về cung cong???
                if (_selectedObj is StateConnector)
                {
                    var stateConnector = _selectedObj as StateConnector;
                    int index = Array.FindIndex(_selectables, obj => obj == _selectedObj);
                    if (index != -1)
                    {
                        int labelIndex = Array.FindIndex(_selectables,
                            obj => obj == stateConnector.Label);
                        curvedConnector = new CurvedStateConnector(
                                                stateConnector.SourceState,
                                                stateConnector.DestinationState);
                        _selectables[index] = curvedConnector;
                        _selectables[labelIndex] = curvedConnector.Label;
                        _selectedObj = curvedConnector;
                        curvedConnector.IsSelected = true;
                    }
                    else
                        throw new InvalidOperationException(OBJECT_NOT_TRACKED_ERROR);
                }
                else
                    throw new InvalidOperationException(CONVERSION_ERROR);
            }
            curvedConnector = _selectedObj as CurvedStateConnector;
            _curveControlPoints[0] = curvedConnector.ControlPoints[0];
            _curveControlPoints[1] = curvedConnector.ControlPoints[1];
        }

        /// <summary>
        /// thoát khỏi chế độ vẽ cong
        /// </summary>
        public void LeaveCurveDrawingMode()
        {
            _bCurve = false;
            _curveControlPoints[0] = _curveControlPoints[1] = _InvalidPoint;
            _SelCtrlPointIndex = -1;
        }

        public bool IsFinalState(State s)
        {
            return _finalStates.Contains(s);
        }

        public void SetFinalState(State s)
        {
            if (!_finalStates.Contains(s))
                _finalStates.Add(s);
        }

        public void SetFinalState(State s, bool enabled)
        {
            if (enabled) SetFinalState(s);
            else
            {
                _finalStates.Remove(s);
            }
        }

        public bool IsStartState(State state)
        {
            return _startState == state;
        }

        public void SetStartState(State state)
        {
            foreach (State s in _drawnStateList)
            {
                if (s.IsStartState)
                {
                    if (s == state) return;
                    else
                    {
                        s.IsStartState = false;
                        state.IsStartState = true;
                        _startState = state;
                        return;
                    }
                }
            }
            state.IsStartState = true;
            _startState = state;
        }

        public List<State> States
        {
            get
            {
                return _drawnStateList;
            }
        }

        /// <summary>
        /// thực thi việc thiết lập các tham số cho các đối tượng trong automata
        /// chuẩn bị cho việc thể hiện trên picturebox;
        /// </summary>
        public void BuildAutomata()
        {
            _bShow = true;
            _SelCtrlPointIndex = -1;
            // sửa đổi, cho phép một số thuật toán khác nhau thực thi
            // như GEM, Random, Tree, Spiral, Circle
            // tạm thời thực hiện theo giải thuật Circle
            // theo đó các trạng thái sẽ phân bổ đều trên vòng tròn trong vùng vẽ
            _angleStep = 2 * Math.PI / _drawnStateList.Count;
            // nếu số trạng thái nhiều, cần thiết phải tăng kích cỡ vòng tròn lên tỉ lệ với STANDARD_DISTANCE
            int radius = (_drawnStateList.Count / 4 + 1) * Config.STATE_DISPLAY_SIZE * 2;
            int halfWidth = DisplayRectangle.Width / 2;
            Point ptCenter = new Point(halfWidth, DisplayRectangle.Height / 2);
            int r = halfWidth * 2 / 3 - Config.STATE_DISPLAY_SIZE;
            r = radius < r ? radius : r;
            double a = Math.PI;
            List<Selectable> selectableList = new List<Selectable>();

            //pre calculate States's position
            foreach (State state in _drawnStateList)
            {
                if (state != _startState && state.IsStartState)
                {
                    state.IsStartState = false;
                }
                if (state == _startState)
                {
                    state.IsStartState = true;
                }
                selectableList.Add(state);
                state.X = (int)(r * Math.Cos(a)) + ptCenter.X;
                state.Y = (int)(r * Math.Sin(a)) + ptCenter.Y;
                a += _angleStep;
            }

            foreach (State state in _drawnStateList)
            {
                foreach (DictionaryEntry de in state.Transitions)
                {
                    var deValue = de.Value as List<State>;
                    foreach (var destinedState in deValue)
                    {
                        //try to compact State Connectors
                        StateConnector dupConnector = null;
                        if (state == destinedState)
                        {
                            //phải tìm đường nối cong sẵn có, tránh vẽ trùng 
                            dupConnector = (CurvedStateConnector)selectableList.Find
                                (obj => obj is CurvedStateConnector
                                && ((StateConnector)obj).DestinationState == destinedState
                                && ((StateConnector)obj).SourceState == state);
                        }
                        else
                            dupConnector = (StateConnector)selectableList.Find
                                (obj => obj is StateConnector &&
                                    ((StateConnector)obj).DestinationState == destinedState
                                    && ((StateConnector)obj).SourceState == state);
                        if (dupConnector != null)
                        {
                            continue;
                        }
                        else
                        {
                            StateConnector connector;
                            if (state == destinedState)
                            {
                                //nếu trạng thái đi và đến là cùng một trạng thái thì ta sẽ tạo đường cong
                                CurvedStateConnector newCurvedConnector = new CurvedStateConnector(state, destinedState);
                                var p = new Point(state.Position.X, state.Position.Y - Config.STATE_DISPLAY_SIZE * 3 / 2);
                                var controlPoints = newCurvedConnector.ControlPoints;
                                controlPoints[0].X = p.X - Config.STATE_DISPLAY_SIZE;
                                controlPoints[0].Y = p.Y;
                                controlPoints[1].Y = p.Y;
                                controlPoints[1].X = p.X + Config.STATE_DISPLAY_SIZE;
                                connector = newCurvedConnector;
                            }
                            else
                            {
                                // nếu giữa hai trạng thái có nhiều hơn 1 cung nối thì vẽ cong, không thì vẽ thẳng
                                bool curve = false;
                                foreach (DictionaryEntry t in destinedState.Transitions)
                                {
                                    var s = t.Value as List<State>;
                                    if (s.Contains(state))
                                    {
                                        curve = true;
                                        break;
                                    }
                                }
                                if (curve)
                                {
                                    CurvedStateConnector newCurvedConnector = new CurvedStateConnector(state, destinedState);
                                    var controlPoints = newCurvedConnector.ControlPoints;
                                    ///int sign = 1;                             
                                    //if (_drawnStateList.IndexOf(state) < _drawnStateList.IndexOf(destinedState)) sign = -1;
                                    /*
                                    double slope = Math.Atan2(destinedState.Y-state.Y, destinedState.X - state.X);
                                    int dx = (int)(Config.STATE_DISPLAY_SIZE * Math.Sin(slope));
                                    int dy = (int)(Config.STATE_DISPLAY_SIZE * Math.Cos(slope));

                                    var p = new Point((state.X + destinedState.X) / 2 + dx, 
                                                      (state.Y + destinedState.Y) / 2 + dy);
                                    controlPoints[0].X = p.X;
                                    controlPoints[0].Y = p.Y;
                                    controlPoints[1].Y = p.Y + dx;
                                    controlPoints[1].X = p.X + dy;
                                    */
                                    int y = destinedState.Y - state.Y;
                                    int x = destinedState.X - state.X;
                                    double slope = Math.Atan2(y, x);
                                    int dx = (int)(Config.STATE_DISPLAY_SIZE * Math.Sin(slope) / 2);
                                    int dy = (int)(Config.STATE_DISPLAY_SIZE * Math.Cos(slope) / 2);
                                    int z = (int)Math.Sqrt(x * x + y * y) / 4;
                                    int ddx = (int)(z * Math.Cos(slope));
                                    int ddy = (int)(z * Math.Sin(slope));
                                    var p = new Point((state.X + destinedState.X) / 2 - dx,
                                                      (state.Y + destinedState.Y) / 2 + dy);

                                    controlPoints[0].X = p.X;
                                    controlPoints[0].Y = p.Y;
                                    controlPoints[1].X = p.X;
                                    controlPoints[1].Y = p.Y;
                                    connector = newCurvedConnector;
                                }
                                else
                                    connector = new StateConnector(state, destinedState);
                            }

                            selectableList.Add(connector);
                        }
                    }
                }
            }

            List<Selectable> labelList = new List<Selectable>();
            foreach (Selectable _selectable in selectableList)
            {
                if (_selectable is StateConnector)
                {
                    var connector = _selectable as StateConnector;
                    connector.CalcArrow();
                    connector.CalcLabelPosition();
                    labelList.Add(connector.Label);
                }
            }
            selectableList.AddRange(labelList);
            selectableList.Sort(CompareSelectables);
            _selectables = selectableList.ToArray();
        }

        /// <summary>
        /// Kiểm tra người dùng có đang nhấp điểm điều khiển ko
        /// </summary>
        /// <param name="handlePoint">Điểm điều khiển</param>
        /// <param name="pt">Tọa độ chuột</param>
        /// <returns>true nếu nhấp phải và false nếu ko</returns>
        private static bool HitHandle(Point handlePoint, Point pt)
        {
            GraphicsPath gp = new GraphicsPath();
            var rect = new Rectangle(handlePoint, new Size(HANDLE_SIZE, HANDLE_SIZE));
            rect.Offset(-HANDLE_SIZE / 2, -HANDLE_SIZE / 2);
            gp.AddEllipse(rect);
            bool bReturn = false;
            if (gp.IsVisible(pt))
                bReturn = true;
            gp.Dispose();
            return bReturn;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (isEnableMouseHere)
            {
                /* Nếu ở chế độ vẽ đường cong, tìm 2 điểm người dùng nhấp chuột làm điểm điều khiển */
                if (_bCurve)
                {
                    //tìm điểm đầu tiên và vẽ
                    if (_curveControlPoints[0] == _InvalidPoint)
                    {
                        _curveControlPoints[0] = e.Location;
                        Invalidate();
                    }
                    else if (_curveControlPoints[1] == _InvalidPoint)
                    {
                        //người dùng đã xong việc nhập điểm điều khiển
                        //tìm được điểm thứ 2 và vẽ
                        _curveControlPoints[1] = e.Location;
                        var curveConnector = _selectedObj as CurvedStateConnector;
                        curveConnector.ControlPoints[0] = _curveControlPoints[0];
                        curveConnector.ControlPoints[1] = _curveControlPoints[1];
                        curveConnector.CalcArrow();
                        curveConnector.CalcLabelPosition();
                        Invalidate();
                    }
                    else
                    {
                        //đây là trường hợp người dùng thay đổi điểm điều khiển
                        _SelCtrlPointIndex = -1;
                        //kéo thả điểm đk đầu tiên?
                        if (HitHandle(_curveControlPoints[0], e.Location))
                            _SelCtrlPointIndex = 0;
                        //hay kéo thả điểm đk thứ 2?
                        if (HitHandle(_curveControlPoints[1], e.Location))
                            _SelCtrlPointIndex = 1;
                        //một trong 2 điểm đk được chọn
                        if (_SelCtrlPointIndex != -1)
                        {
                            var selCtrlPt = _curveControlPoints[_SelCtrlPointIndex];
                            /*tính toán độ lệch giữa tâm của điểm điều khiển với vị trí chuột được nhấp
                            *độ lệch này được sử dụng trong suốt quá trình kéo điểm đk để cập nhật tọa độ điểm đk
                            *và vẽ lại */
                            _distance_CtrlPoint_Mouse = new Size(selCtrlPt.X - e.Location.X,
                                selCtrlPt.Y - e.Location.Y);
                        }
                    }
                    return;
                }
                if (_selectables != null && _selectables.Length > 0)
                    for (int i = _selectables.Length - 1; i >= 0; i--)
                    {
                        var _selectable = _selectables[i];
                        if (_selectable.HitTest(e.Location, _automatGraphics))
                        {
                            _selectable.IsSelected = true;
                            ItemSelectInfo info = new ItemSelectInfo
                            {
                                deselected = _selectedObj,
                                selected = _selectable,
                                location = e.Location

                            };
                            ItemSelected(this, info);

                            if (_selectable is CurvedStateConnector)
                            {
                                var curvedConnector = _selectable as CurvedStateConnector;
                                _curveControlPoints[0] = curvedConnector.ControlPoints[0];
                                _curveControlPoints[1] = curvedConnector.ControlPoints[1];
                            }

                            if (_selectedObj != null && _selectedObj != _selectable)
                            {
                                _selectedObj.IsSelected = false;
                            }
                            _selectedObj = _selectable;
                            List<BaseMouseHandler> sourceChain = new List<BaseMouseHandler>();
                            var mouseHandler = _selectedObj as BaseMouseHandler;
                            /*mouseHandler.TrackMouse(this, sourceChain, e);*/
                            mouseHandler.InitiateTrackMouse(this, sourceChain, e);
                            Invalidate();
                            break;
                        }
                    }
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (isEnableMouseHere)
            {
                if (_bCurve)
                {
                    if (_SelCtrlPointIndex != -1)
                    {
                        _SelCtrlPointIndex = -1;
                    }
                    return;
                }
                if (_selectedObj != null)
                {
                    if (!_selectedObj.HitTest(e.Location, _automatGraphics))
                    {
                        _selectedObj.IsSelected = false;
                        ItemSelectInfo info = new ItemSelectInfo
                        {
                            deselected = _selectedObj,
                            selected = null,
                            location = e.Location

                        };
                        ItemSelected(this, info);
                        Invalidate();
                        _selectedObj = null;
                    }
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (isEnableMouseHere)
            {
                //nếu đang ở chế độ vẽ đường cong và người dùng đang rê chuột
                if (_bCurve)
                {
                    if (_SelCtrlPointIndex != -1 && _curveControlPoints[0] != _InvalidPoint
                        && _curveControlPoints[1] != _InvalidPoint)
                    {
                        //cập nhật lại vị trí của điểm đk dựa theo độ lệch được tính toán ở sự kiện OnMouseDown
                        _curveControlPoints[_SelCtrlPointIndex].X = e.X +
                            _distance_CtrlPoint_Mouse.Width;
                        _curveControlPoints[_SelCtrlPointIndex].Y = e.Y +
                            _distance_CtrlPoint_Mouse.Height;
                        if (_selectedObj is CurvedStateConnector)
                        {
                            /*cập nhật lại điểm đk và tính toán lại hình mũi tên và 
                             * đường cong Bezier dựa theo tọa độ mới của điểm đk */
                            var curvedConnector = _selectedObj as CurvedStateConnector;
                            curvedConnector.ControlPoints[0] = _curveControlPoints[0];
                            curvedConnector.ControlPoints[1] = _curveControlPoints[1];
                            curvedConnector.CalcArrow();
                            curvedConnector.CalcBezierPoint();
                            /*Refresh();*/
                            Invalidate();
                        }
                    }
                    return;
                }
                /* Không ở chế độ vẽ cong, quảng bá xử lý sự kiện chuột cho 
                 * đối tượng được nhấp vào và các đối tượng liên quan
                 */
                if (e.Button == MouseButtons.Left && _selectedObj != null)
                {
                    List<BaseMouseHandler> sourceChain = new List<BaseMouseHandler>();
                    var mouseHandler = _selectedObj as BaseMouseHandler;
                    /*mouseHandler.HandleMouseEvent(this, sourceChain, e);*/
                    mouseHandler.InitiateHandleMouse(this, sourceChain, e);
                    Refresh();
                }
            }
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);

            if (!_bShow) return;

            StringFormat strFormat = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            Pen borderPen = new Pen(Color.ForestGreen, 1);
            Pen redPen = new Pen(Color.Red, 2);
            Pen connectorPen = new Pen(Color.FromArgb(0x33, 0x00, 0x33), 1);
            Brush arrowBrush = new SolidBrush(Color.FromArgb(0x33, 0x00, 0x33));
            Font stateFont = new Font("Arial", Config.STATE_FONT_SIZE, FontStyle.Bold, GraphicsUnit.Point);

            foreach (Selectable _selectable in _selectables)
            {
                CurvedStateConnector curvedConnector = null;
                if (_selectable is StateConnector)
                {
                    var connector = _selectable as StateConnector;
                    if (connector.Label.IsSelected)
                    {
                        Pen labelBorderPen = new Pen(Color.DarkRed, 1.5f) { DashStyle = DashStyle.Dash };
                        var labelRect = connector.Label.GetRect(_automatGraphics);
                        _automatGraphics.DrawRectangle(labelBorderPen, labelRect.Left,
                            labelRect.Top, labelRect.Width, labelRect.Height);
                        labelBorderPen.Dispose();
                    }
                    Pen pen = connector.IsSelected ? redPen : connectorPen;
                    if (_selectable is CurvedStateConnector)
                    {
                        curvedConnector = _selectable as CurvedStateConnector;
                    }
                    if (curvedConnector != null &&
                        curvedConnector.ControlPoints[0] != _InvalidPoint &&
                        curvedConnector.ControlPoints[1] != _InvalidPoint)
                    {
                        _automatGraphics.DrawBezier(pen, curvedConnector.SourceState.Position,
                    curvedConnector.ControlPoints[0],
                    curvedConnector.ControlPoints[1],
                    curvedConnector.DestinationState.Position);
                        _automatGraphics.DrawString(curvedConnector.Label.Text, ConnectorLabel.LabelFont,
                     Brushes.Brown, connector.Label.Position);
                    }
                    else
                    {
                        _automatGraphics.DrawLine(pen, connector.SourceState.Position,
                            connector.DestinationState.Position);
                        _automatGraphics.DrawString(connector.Label.Text, ConnectorLabel.LabelFont,
                     Brushes.Brown, connector.Label.Position);
                    }
                    if (connector.ArrowPoints != null)
                    {
                        _automatGraphics.DrawLine(pen, connector.ArrowPoints[0],
                            connector.ArrowPoints[1]);
                        _automatGraphics.DrawLine(pen, connector.ArrowPoints[0],
                           connector.ArrowPoints[2]);
                    }


                }
                else if (_selectable is State)
                {

                    var state = _selectable as State;
                    GraphicsPath gp = new GraphicsPath();
                    gp.AddEllipse(state.BoundingRect);
                    var boundColor = (_selectable == _hilightState && _t1 >= 0
                                        && _t1 <= 255) ? _paramColor :
                                        Color.FromArgb(0xCC, 0x66, 0xFF);
                    Color[] colors = {
                                           boundColor,                          // dark green
                                           Color.FromArgb(0xCC, 0xCC, 0xFF),    // aqua
                                           Color.FromArgb(0xCC, 0xFF, 0xFF)     // blue
                                     };

                    float[] relativePositions = {
                                                   0f,      // Dark green is at the boundary of the triangle.
                                                   0.58f,   // Aqua is 40 percent of the way from the boundary to the center point.
                                                   1.0f     // Blue is at the center point.
                                                };

                    ColorBlend colorBlend = new ColorBlend();
                    colorBlend.Colors = colors;
                    colorBlend.Positions = relativePositions;

                    PathGradientBrush grd = new PathGradientBrush(gp);
                    //{
                    //    SurroundColors = new Color[] { Color.FromArgb(0xCC, 0x33, 0xCC) },
                    //    CenterColor = Color.FloralWhite
                    //};
                    grd.InterpolationColors = colorBlend;
                    _automatGraphics.FillEllipse(grd, state.BoundingRect);
                    _automatGraphics.DrawString(state.Label, stateFont, Brushes.DarkSlateBlue,
                        state.BoundingRect, strFormat);
                    Pen stateBorderPen = state.IsSelected ? redPen : borderPen;
                    _automatGraphics.DrawEllipse(stateBorderPen, state.BoundingRect);

                    if (IsFinalState(state))
                    {
                        Pen markPen = new Pen(Color.GreenYellow, 2);
                        var newRect = state.BoundingRect;
                        newRect.Inflate(-3, -3);
                        _automatGraphics.DrawEllipse(markPen, newRect);
                        markPen.Dispose();
                    }

                    if (IsStartState(state))
                    {
                        // tạm thời chấp nhận cái hình này cho trạng thái bắt đầu, sẽ tính tiếp sau
                        Point[] star = new Point[]{new Point(state.X-Config.STATE_DISPLAY_SIZE/2, state.Y),
                                                   new Point(state.X-Config.STATE_DISPLAY_SIZE, state.Y-Config.STATE_DISPLAY_SIZE/4),
                                                   new Point(state.X-Config.STATE_DISPLAY_SIZE, state.Y+Config.STATE_DISPLAY_SIZE/4)};
                        _automatGraphics.DrawPolygon(connectorPen, star);
                    }
                    grd.Dispose();
                    gp.Dispose();
                }
            }
            if (_bCurve)
            {
                if (_curveControlPoints[0] != InvalidPoint)
                    OnPaintHandle(_curveControlPoints[0]);
                if (_curveControlPoints[1] != InvalidPoint)
                    OnPaintHandle(_curveControlPoints[1]);
            }
            OnAnimatePaint(_automatGraphics);
            pe.Graphics.DrawImage(_automatBitmap, DisplayRectangle, DisplayRectangle,
                GraphicsUnit.Pixel);
            stateFont.Dispose();
            strFormat.Dispose();
            borderPen.Dispose();
            redPen.Dispose();
            connectorPen.Dispose();
            arrowBrush.Dispose();


            if (!isEnableMouseHere)
            {
                Pen p = new Pen(Color.Red, 2);
                pe.Graphics.DrawLine(p, startPoint.X, startPoint.Y, endPoint.X, endPoint.Y);
                p.Dispose();
            }
        }

        private void OnAnimatePaint(Graphics g)
        {
            if (!_demoTimer.Enabled) return;
            if (_t0 != null)
            {
                Pen pen = new Pen(Color.DarkRed, 2);
                if (_animateCurve != null)
                    g.DrawBezier(pen, _bPoints[0], _bPoints[1], _bPoints[2], _bPoints[3]);
                else
                    g.DrawLine(pen, _endPoints[0], _paramPoint);
                pen.Dispose();
            }
        }
        //vẽ điểm điều khiển, điểm điều khiển là một hình tròn nhỏ màu xanh có đường kính là HANDLE_SIZE
        private void OnPaintHandle(Point pt)
        {
            var rect = new Rectangle(pt, new Size(HANDLE_SIZE, HANDLE_SIZE));
            rect.Offset(-HANDLE_SIZE / 2, -HANDLE_SIZE / 2);
            _automatGraphics.FillEllipse(Brushes.BlueViolet, rect);
        }

        private static int CompareSelectables(Selectable s1, Selectable s2)
        {
            //các connector sẽ được sắp xếp trước
            int a = s1 is StateConnector ? 0 : (s1 is State ? 1 : 2);
            int b = s2 is StateConnector ? 0 : (s2 is State ? 1 : 2);
            return a - b;
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            base.OnPaintBackground(pevent);
            SolidBrush newSolidBrush = new SolidBrush(BackColor);
            _automatGraphics.FillRectangle(newSolidBrush,
                pevent.ClipRectangle);
            newSolidBrush.Dispose();
        }
        public delegate void DemoFinishedHandler(object sender, DemoFinishedEvent ea);
        public delegate void ItemSelectedHandler(object sender, ItemSelectInfo info);
        public event ItemSelectedHandler ItemSelected;
        public event DemoFinishedHandler DemoFinished;

        public void InitializeComponent()
        {
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        public void DeleteSelectsable(Selectable selectable)
        {


            if (selectable is State)
            {
                var state = (State)selectable;
                foreach (var con in _selectables)
                {
                    if (con is StateConnector)
                    {
                        StateConnector s = (StateConnector)con;
                        _selectables = _selectables.Where(t => t is StateConnector && (((StateConnector)t).SourceState.Label != s.SourceState.Label ||
                                           ((StateConnector)t).DestinationState.Label != s.DestinationState.Label)).ToArray();
                    }
                }
                _drawnStateList.Remove(state);
                Invalidate();
                BuildAutomata();
                Refresh();
            }
            else if (selectable is StateConnector)
            {
                var stateConnector = (StateConnector)selectable;
                _selectables = _selectables.Where(t => t != stateConnector).ToArray();
                Refresh();
            }
        }

        //Tự thêm các thuộc tính sau:
        public bool isEnableMouseHere { get; set; } = true;
        public Point startPoint { get; set; }
        public Point endPoint { get; set; }
        public static Point InitPoint = new Point(0, 0);

        public Graphics Graphics { get { return _automatGraphics; } }

        public IList<StateConnector> GetListConnector()
        {
            IList<StateConnector> list = new List<StateConnector>();
            foreach (var item in _selectables)
            {
                if (item is StateConnector)
                {
                    list.Add((StateConnector)item);
                }
            }
            return list;
        }

        public void SetStateConnectorInList(StateConnector stateConnector)
        {
            foreach (var item in _selectables)
            {
                if (item is StateConnector)
                {
                    if ((StateConnector)item == stateConnector)
                    {
                        _selectables[Array.IndexOf(_selectables, item)] = stateConnector;
                        return;
                    }
                }
            }
        }

        public StateConnector GetStateConnectorByState(State from, State to)
        {
            foreach (var item in _selectables)
            {
                if (item is StateConnector)
                {
                    if (((StateConnector)item).SourceState == from && ((StateConnector)item).DestinationState == to)
                    {
                        return (StateConnector)item;
                    }
                }
            }
            return null;
        }

        public IList<StateConnector> GetListStateConnectorBySigleState(State state, bool isSource)
        {
            IList<StateConnector> list = new List<StateConnector>();
            foreach (var item in _selectables)
            {
                if (item is StateConnector)
                {
                    if (isSource && ((StateConnector)item).SourceState == state)
                    {
                        list.Add((StateConnector)item);
                    }
                    else if (!isSource && ((StateConnector)item).DestinationState == state)
                    {
                        list.Add((StateConnector)item);
                    }
                }
            }
            return list;
        }

        public State GetStartState()
        {
            return _startState;
        }

        public IList<State> GetListFinalState()
        {
            return _finalStates;
        }

        public State GetStateByLabel(string label)
        {
            foreach (var item in _selectables)
            {
                if (item is State)
                {
                    var state = (State)item;
                    if (state.Label == label)
                    {
                        return state;
                    }
                }
            }
            return null;
        }

        public void SetAllSelectable(Selectable[] selects)
        {
            _selectables = selects;
            BuildAutomata();
            Refresh();
        }

        public Selectable[] GetAllSelectable()
        {
            return _selectables;
        }
    }
}
