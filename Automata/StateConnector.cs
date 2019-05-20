using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;


namespace Automata
{
    public class StateConnector : BaseMouseHandler, Selectable
    {
        protected State[] ConnectedStates;          // mảng chứa 2 phần tử là điểm đầu và điểm cuối của connector
        private Point[] _PtArrow;                   // mảng chứa các điểm của mũi tên chỉ hướng gồm 3 điểm: đầu và hai điểm mút cạnh
        const int TOLERANCE = 60;                   // ?
        protected bool _bArrowInit;                 // ?
        protected ConnectorLabel _connectorLabel;   // nhãn trên connector
        public StateConnector(State s1, State s2)
        {
            ConnectedStates = new State[2];
            ConnectedStates[0] = s1;
            ConnectedStates[1] = s2;
            AddHandler(s1);
            AddHandler(s2);
            s1.AddHandler(this);
            s2.AddHandler(this);
            _ArrowSize = Config.ARROW_SIZE;
            _bArrowInit = false;
            _PtArrow = new Point[3];
            for (int i = 0; i < _PtArrow.Length; i++)
            {
                _PtArrow[i] = new Point();
            }
            _connectorLabel = new ConnectorLabel(this);
        }
        public bool IsSelected { get; set; }

        public ConnectorLabel Label
        {
            get
            {
                return _connectorLabel;
            }
        }

        public virtual bool HitTest(Point pt, Graphics g)
        {
            //nếu 2 điểm của đoạn thẳng có cùng hoành độ, tạo một hình chữ nhật để
            //bọc 2 điểm này và việc kiểm tra điểm truyền vào pt trở thành việc kiểm
            //tra pt có nằm trong hcn ko
            if (ConnectedStates[0].X == ConnectedStates[1].X)
            {
                int yMin = Math.Min(ConnectedStates[0].Y, ConnectedStates[1].Y);
                int yMax = Math.Max(ConnectedStates[0].Y, ConnectedStates[1].Y);
                Rectangle rect = Rectangle.FromLTRB(ConnectedStates[0].X - TOLERANCE,
                                                    yMin - TOLERANCE,
                                                     ConnectedStates[0].X + TOLERANCE,
                                                     yMax + TOLERANCE);
                GraphicsPath gp = new GraphicsPath();
                gp.AddRectangle(rect);
                bool bReturn = false;
                if (gp.IsVisible(pt, g))
                {
                    bReturn = true;
                }
                gp.Dispose();
                return bReturn;
            }
            /*Đoạn thẳng tạo thành góc chéo
             * vậy kiểm tra xem pt có thuộc đoạn thẳng ko
             * Có cách làm đơn giản hơn, đó là dùng GraphicsPath.AddLine sau đó
             * gọi GraphicsPath.IsVisible
             */
            Point ptMin = ConnectedStates[0].X < ConnectedStates[1].X ?
                ConnectedStates[0].Position : ConnectedStates[1].Position;
            Point ptMax = ConnectedStates[0].X < ConnectedStates[1].X ?
                ConnectedStates[1].Position : ConnectedStates[0].Position;
            //int cxLine = Math.Abs(ConnectedStates[0].X - ConnectedStates[1].X);
            //int cyLine = Math.Abs(ConnectedStates[1].Y - ConnectedStates[1].Y);
            //int cxMouse = Math.Abs(pt.X - ConnectedStates[0].X);
            //int cyMouse = Math.Abs(pt.Y - ConnectedStates[0].Y);
            int cxLine = ptMax.X - ptMin.X;
            int cyLine = ptMax.Y - ptMin.Y;
            int cxMouse = pt.X - ptMin.X;
            int cyMouse = pt.Y - ptMin.Y;
            if (Math.Abs(cyMouse - (float)cyLine / cxLine * cxMouse) < TOLERANCE)
                return true;
            return false;
        }

        public override bool HandleMouseEvent(object sender, List<BaseMouseHandler> sourceChain,
            System.Windows.Forms.MouseEventArgs e)
        {
            /*base.HandleMouseEvent(sender, sourceChain, e);*/
            ExecuteAfter(CalcArrow);
            /*CalcArrow();*/
            return true;
        }

        public override bool TrackMouse(object sender, List<BaseMouseHandler> sourceChain, System.Windows.Forms.MouseEventArgs e)
        {
            //người dùng kéo thả cung nối thì luôn luôn phải quảng bá tới 2 hình tròn nối tới tôi
            return true;
        }

        private int _ArrowSize;
        public int ArrowSize
        {
            get
            {
                return _ArrowSize;
            }
            set
            {
                _ArrowSize = value;
                CalcArrow();

            }
        }

        protected void CalcTails(Point ptStart, Point ptEnd)
        {
            /* calculate slope of line */
            double slope = Math.Atan2(ptEnd.Y - ptStart.Y, ptEnd.X - ptStart.X);        // tính góc giữa 2 điểm so với đường thẳng ngang
            //ptArrow[0] is arrow head
            _PtArrow[0] = ptEnd;
            double dbl1 = slope + 5 * Math.PI / 6;                                      // tính các góc tạo bởi các cạnh của mũi tên với mặt phẳng ngang
            double dbl2 = slope + 7 * Math.PI / 6;
            _PtArrow[1] = new Point()                                                   // xác định điểm đầu cạnh thứ 1
            {
                X = _PtArrow[0].X + (int)(_ArrowSize * Math.Cos(dbl1)),
                Y = _PtArrow[0].Y + (int)(_ArrowSize * Math.Sin(dbl1))
            };
            _PtArrow[2] = new Point()                                                   // xác định điểm đầu cạnh thứ 2
            {
                X = _PtArrow[0].X + (int)(_ArrowSize * Math.Cos(dbl2)),
                Y = _PtArrow[0].Y + (int)(_ArrowSize * Math.Sin(dbl2))
            };

        }

        public virtual void CalcArrow()
        {
            /* calculate slope of line */
            double slope = Math.Atan2(ConnectedStates[1].Y - ConnectedStates[0].Y,
                                    ConnectedStates[1].X - ConnectedStates[0].X);
            /*tìm điểm giao giữa cạnh nối và hình tròn biểu diễn trạng thái */
            int hypotenus = State.DisplaySize / 2;
            int dx = (int)(hypotenus * Math.Cos(slope));
            int dy = (int)(hypotenus * Math.Sin(slope));
            Point ptHead = new Point(ConnectedStates[1].X - dx,
                ConnectedStates[1].Y - dy);
            _bArrowInit = true;
            CalcTails(ConnectedStates[0].Position, ptHead);

        }

        public Point[] ArrowPoints
        {
            get
            {
                if (!_bArrowInit) return null;
                return _PtArrow;
            }
        }

        public State SourceState
        {
            get { return ConnectedStates[0]; }
        }

        public State DestinationState
        {
            get { return ConnectedStates[1]; }
        }

        // trả về điểm đầu mút kia của cung nối
        public State GetOtherState(State state)
        {
            if (state == ConnectedStates[0])
                return ConnectedStates[1];
            else if (state == ConnectedStates[1])
                return ConnectedStates[0];
            return null;
        }

        public void AddState(State state)
        {
            if (ConnectedStates[0] == null)
            {
                ConnectedStates[0] = state;
                return;
            }
            if (ConnectedStates[1] == null)
                ConnectedStates[1] = state;
        }

        /// <summary>
        /// tính toán vị trí của nhãn
        /// </summary>
        public virtual void CalcLabelPosition()
        {
            if (ConnectedStates[0] == null || ConnectedStates[1] == null)
            {
                return;
            }
            var initPt = new Point((ConnectedStates[0].X + ConnectedStates[1].X) / 2,
                (ConnectedStates[0].Y + ConnectedStates[1].Y) / 2);
            double dx = ConnectedStates[0].X - ConnectedStates[1].X;
            double dy = ConnectedStates[0].Y - ConnectedStates[1].Y;
            if (dx == 0 || dy == 0)
            {
                if (dy == 0)
                    initPt.Offset(0, -ConnectorLabel.EstimatedHeight);
                _connectorLabel.Position = initPt;
                return;
            }
            var slope = Math.Atan(dy / dx);
            if (slope >= 0)
                initPt.Offset(0, -ConnectorLabel.EstimatedHeight);
            _connectorLabel.Position = initPt;

        }
    }
}
