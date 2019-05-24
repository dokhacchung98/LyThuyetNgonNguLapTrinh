using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Automata
{
    public class State : BaseMouseHandler, Selectable
    {
        int _X, _Y;                                                 // vị trí của trạng thái
        int _mouse_dx, _mouse_dy;                                   // độ dịch chuyển khi có sự thay đổi chuột
        List<Transition> _transitionList;                           // các bước chuyển từ trạng thái hiện tại
        Hashtable _transitionHash;                                  // bảng quản lý các bước chuyển từ trạng thái hiện tại
        static List<State> stateCollection = new List<State>();     // danh sách các trạng thái của automata

        /// <summary>
        /// Initializes a new instance of the State class.
        /// </summary>
        /// <param name="name"></param>
        public State(String name)
        {
            _transitionList = new List<Transition>();
            _transitionHash = new Hashtable();
            _Label = name;
            stateCollection.Add(this);
            OnPositionChanged();
        }

        static State()
        {
            DisplaySize = Config.STATE_DISPLAY_SIZE;
        }

        public bool IsSelected { get; set; }
        private Rectangle _BoundingRect;
        public Rectangle BoundingRect
        {
            get { return _BoundingRect; }
        }

        public static State GetStateFromName(string name)
        {
            foreach (State state in stateCollection)
            {
                if (state.Label == name)
                    return state;
            }
            return null;
        }

        public Hashtable Transitions
        {
            get
            {
                return _transitionHash;
            }
        }

        public override bool TrackMouse(object sender, List<BaseMouseHandler> sourceChain,
            System.Windows.Forms.MouseEventArgs e)
        {
            var enumerator = sourceChain.GetEnumerator();
            enumerator.MoveNext();
            /*chỉ cập nhật trạng thái và quảng bá nếu thỏa mãn 1 trong 2 điều sau
             * - tôi nhận được sự kiện này đầu tiên (sourceChain = 0)
             * - đối tượng gửi đến là StateConnector (người dùng nhấp vào cung nối giữa 2 hình tròn
             * và di chuyển cung nối dẫn đến 2 hình tròn cũng bị di chuyển theo)
             */
            if (sourceChain.Count == 0 ||
               (sender is StateConnector && sender == enumerator.Current))
            {
                /* base.TrackMouse(sender, sourceChain, e);*/
                _mouse_dx = e.X - _X;
                _mouse_dy = e.Y - _Y;
                return true;
            }
            return false;
        }

        public override bool HandleMouseEvent(object sender, List<BaseMouseHandler> sourceChain, System.Windows.Forms.MouseEventArgs e)
        {

            /* chỉ quảng bá nếu tôi được nhận sự kiện này đầu tiên hoặc nhận được từ phía connector kết nối trực tiếp với mình
             và connector đó nhận được sự kiện đầu tiên */
            var enumerator = sourceChain.GetEnumerator();
            enumerator.MoveNext();
            if (sourceChain.Count == 0 || (sender is StateConnector && sender == enumerator.Current))
            {
                _X = e.X - _mouse_dx;
                _Y = e.Y - _mouse_dy;
                OnPositionChanged();
                /*base.HandleMouseEvent(sender, sourceChain, e);*/
                return true;
            }
            return false;
        }

        public void AddTransition(char TransitChar, State sTo)
        {
            if (!_transitionHash.ContainsKey(TransitChar))
            {
                List<State> newList = new List<State>();
                _transitionHash.Add(TransitChar, newList);
            }
            var list = _transitionHash[TransitChar] as List<State>;
            list.Add(sTo);

        }

        public void AddTransition(char[] transitChars, State sTo)
        {
            List<State> newList = new List<State>();
            _transitionHash.Add(transitChars, newList);

            var list = _transitionHash[transitChars] as List<State>;
            list.Add(sTo);
        }

        private string _Label;
        public string Label
        {
            get
            {
                return _Label;
            }
        }

        public int X
        {
            get
            {
                return _X;
            }
            set
            {
                _X = value;
                OnPositionChanged();
            }
        }

        public int Y
        {
            get
            {
                return _Y;
            }
            set
            {
                _Y = value;
                OnPositionChanged();
            }
        }

        private Point _Position;
        public Point Position
        {
            get { return _Position; }
        }

        private void OnPositionChanged()
        {
            int r = DisplaySize / 2;
            _BoundingRect = new Rectangle(_X - r, _Y - r, DisplaySize, DisplaySize);
            _Position = new Point(_X, _Y);
        }

        public static int DisplaySize { get; set; }
        public bool IsStartState { get; set; }
        public bool IsFinalState { get; set; }

        public bool HitTest(Point pt, Graphics g)
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddEllipse(BoundingRect);
            if (gp.IsVisible(pt, g))
            {
                gp.Dispose();
                return true;
            }
            gp.Dispose();
            return false;
        }

    }

    struct Transition
    {
        public string TransitChars;
        public State DestinedState;
    }
}
