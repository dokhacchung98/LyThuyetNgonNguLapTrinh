using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automata
{
    public class DemoFinishedEvent : EventArgs
    {
        public bool WordIsExcepted { get; set; }
    }
}
