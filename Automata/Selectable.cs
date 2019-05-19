using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Automata
{
    public interface Selectable
    {
        bool IsSelected
        {
            get;
            set;
        }
        bool HitTest(Point pt, Graphics g);
    }
}
