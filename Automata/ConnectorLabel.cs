using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
namespace Automata
{
    public class ConnectorLabel : BaseMouseHandler, Selectable
    {
        private const string FATAL_EXCEPTION = "Exception is thrown in an unexpected way";
        int _mouse_dx, _mouse_dy;                               // độ dời chuyển chuột
        private StringBuilder _StringBuilder;                   // tên nhãn, có thể gồm nhiều chữ cái của bảng chữ cái cách nhau bởi dấu phảy
        private static Font _labelFont = new Font("Arial", 
                                                    Config.LABEL_FONT_SIZE,
                                                    FontStyle.Bold, 
                                                    GraphicsUnit.Point
                                                 );
        private StateConnector _connector;                      // connector mà nhãn này gắn với
        private static Graphics _ScreenGraphics = Graphics.FromHdc(GetDC(IntPtr.Zero));
        //private double _dAngle;                                 //
        //private double _hypotenus;
        //private Point _anchorPoint, _movePoint;                 //
        private Size _OffsetFromBezier;                         // khoảng cách đến đường cong Bezier

        public ConnectorLabel(StateConnector connector)
        {
            connector.AddHandler(this);
            AddHandler(connector);
            _connector = connector;
            _OffsetFromBezier = new Size();
            _StringBuilder = new StringBuilder(50);

            foreach (DictionaryEntry de in _connector.SourceState.Transitions)      // tạo tên nhãn (có thể gồm nhiều ký tự cách nhau bởi dấu phảy)
            {
                var deValue = de.Value as List<State>;
                foreach (var destinedState in deValue)
                {
                    if (destinedState == _connector.DestinationState)
                    {
                        if (_StringBuilder.ToString().Length > 0)
                            _StringBuilder.Append(", ");
                        _StringBuilder.Append((char)de.Key);
                        break;
                    }
                }
            }

        }

        public override bool TrackMouse(object sender, List<BaseMouseHandler> sourceChain, System.Windows.Forms.MouseEventArgs e)
        {
            var enumerator = sourceChain.GetEnumerator();
            double current;
            // nếu do chính nhãn bị dịch chuyển
            if (!enumerator.MoveNext())
            {
                _mouse_dx = e.X - _Position.X;
                _mouse_dy = e.Y - _Position.Y;
                return true;
            }

            // nếu đó là cung nối cong bị dịch chuyển
            if (_connector is CurvedStateConnector)
            {
                var curveConnector = _connector as CurvedStateConnector;
                curveConnector.CalcBezierPoint();
                _OffsetFromBezier = new Size(_Position.X - curveConnector.BezierPoint.X,
                                             _Position.Y - curveConnector.BezierPoint.Y);
            }
            else
            {
                // nếu do cung nối thẳng bị dịch chuyển
                if (enumerator.Current is StateConnector)
                {
                    var connector1 = enumerator.Current as StateConnector;
                    if (connector1 == _connector ||
                        (connector1.GetOtherState(_connector.SourceState) != null &&
                        connector1.GetOtherState(_connector.DestinationState) != null))
                    {
                        _mouse_dx = e.X - _Position.X;
                        _mouse_dy = e.Y - _Position.Y;                       
                        return true;
                    }
                }

                // do trạng thái bị dịch chuyển
                // không có cách nào biết vị trí cũ của trạng thái bị dịch chuyển?
                // không cần thiết phải tính toán những tham số này
                /*
                double slope;
                if (sourceChain.IndexOf(_connector.SourceState) <
                    sourceChain.IndexOf(_connector))
                {
                    _anchorPoint = _connector.DestinationState.Position;
                    _movePoint = _connector.SourceState.Position;

                }
                else if (sourceChain.IndexOf(_connector.DestinationState) <
                    sourceChain.IndexOf(_connector))
                {
                    _anchorPoint = _connector.SourceState.Position;
                    _movePoint = _connector.DestinationState.Position;

                }
                else
                    throw new InvalidOperationException(FATAL_EXCEPTION);

                slope = Math.Atan2(_movePoint.Y - _anchorPoint.Y, _movePoint.X - _anchorPoint.X);
                var dy = _Position.Y - _anchorPoint.Y;
                var dx = _Position.X - _anchorPoint.X;
                current = Math.Atan2(dy, dx);
                _hypotenus = Math.Sqrt(dx * dx + dy * dy);
                _dAngle = current - slope;
                 */
            }

            /* base.TrackMouse(sender, sourceChain, e);*/
            return true;

        }

        public override bool HandleMouseEvent(object sender, List<BaseMouseHandler> sourceChain, System.Windows.Forms.MouseEventArgs e)
        {
            var enumerator = sourceChain.GetEnumerator();
            //tôi có được nhận sự kiện này đầu tiên, như vậy trong sourceChain không có ai khác
            if (!enumerator.MoveNext())
            {
                _Position.X = e.X - _mouse_dx;
                _Position.Y = e.Y - _mouse_dy;
                //_OffsetFromBezier.Width = _mouse_dx;
                //_OffsetFromBezier.Height = _mouse_dy;
                /*
	                if (_connector is CurvedStateConnector)
	                {
	                    var curveConnector = _connector as CurvedStateConnector;
	                    _OffsetFromBezier = new Size(_Position.X - curveConnector.BezierPoint.X, _Position.Y - curveConnector.BezierPoint.Y);                    
	                }
                */
                return true;
            }
            // nếu đối tượng tác động lên tôi là cung nối cong
            if (_connector is CurvedStateConnector)
            {
                var curveConnector = _connector as CurvedStateConnector;
                curveConnector.CalcBezierPoint();
                _Position = curveConnector.BezierPoint +_OffsetFromBezier;
            }
            else
            {
                // nếu đối tượng tác động lên tôi là cung nối thẳng
                if (enumerator.Current is StateConnector)
                {
                    var connector1 = enumerator.Current as StateConnector;
                    if (connector1 == _connector ||
                        (connector1.GetOtherState(_connector.SourceState) != null &&
                        connector1.GetOtherState(_connector.DestinationState) != null))
                    {
                        _Position.X = e.X - _mouse_dx;
                        _Position.Y = e.Y - _mouse_dy;
                        return true;
                    }
                }

                // nếu đối tượng tác động lên tôi là trạng thái
                // cũng không cần thiết phải xác định
                /*
                if (sourceChain.IndexOf(_connector.SourceState) <
                    sourceChain.IndexOf(_connector))
                {
                    _movePoint = _connector.SourceState.Position;

                }
                else if (sourceChain.IndexOf(_connector.DestinationState) <
                    sourceChain.IndexOf(_connector))
                {
                    _movePoint = _connector.DestinationState.Position;
                }
                else
                    throw new InvalidOperationException(FATAL_EXCEPTION);
                var slope = Math.Atan2(_movePoint.Y - _anchorPoint.Y,
                    _movePoint.X - _anchorPoint.X);
                var current = slope + _dAngle;
                //_Position.X = (int)(_hypotenus * Math.Cos(current) + _anchorPoint.X);
                //_Position.Y = (int)(_hypotenus * Math.Sin(current) + _anchorPoint.Y);
                // dựa vào tam giac đồng dạng
                */
                // _Position.X = (int)((_movePoint.X - _anchorPoint.X)/2
               _Position.X = (_connector.DestinationState.Position.X + _connector.SourceState.Position.X) / 2 + _OffsetFromBezier.Height;
               _Position.Y = (_connector.DestinationState.Position.Y + _connector.SourceState.Position.Y) / 2 + _OffsetFromBezier.Width;
            }

            /*base.HandleMouseEvent(sender, sourceChain, e);*/
            return true;
        }

        public string Text
        {
            get
            {
                return _StringBuilder.ToString();
            }

        }
        private Point _Position;
        public Point Position
        {
            get
            {
                return _Position;
            }
            set
            {
                _Position = value;

            }
        }

        public static Font LabelFont
        {
            get
            {
                return _labelFont;
            }
        }

        public bool IsSelected { get; set; }
        public RectangleF GetRect(Graphics g)
        {
            var measureString = g.MeasureString(Text, _labelFont);
            var rectf = new RectangleF(Position, measureString);
            return rectf;
        }

        public bool HitTest(Point pt, Graphics g)
        {
            RectangleF rectf = GetRect(g);
            return rectf.Contains(pt);
        }

        public static int EstimatedHeight
        {
            get
            {
                int h = (int)(_ScreenGraphics.DpiX * _labelFont.SizeInPoints / (float)72 + 0.5);
                return h;
            }
        }

        public static Graphics ScreenGraphics
        {
            get
            {
                return _ScreenGraphics;
            }
        }
        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hwnd);
    }
}
