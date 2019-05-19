using Automata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConceptsOfProgrammingLanguages
{
    public class ItemTableConnector
    {
        public State SourceState { get; set; }
        public State DestinationState { get; set; }
        public string Value { get; set; }
    }
}
