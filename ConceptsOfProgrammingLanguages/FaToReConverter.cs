using Automata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConceptsOfProgrammingLanguages
{
    public class FaToReConverter
    {
        public static String EMPTY = "Ø";
        public static String LAMBDA = "";
        public static String KLEENE_STAR = "*";
        public static String OR = "+";
        public static String RIGHT_PAREN = ")";
        public static String LEFT_PAREN = "(";

        public static StateConnector GrossConnector(StateConnector stateConnector)
        {
            string label = stateConnector.Label.Text.Replace(" ", LAMBDA);
            string[] characters = label.Split(',');
            label = "";
            label += characters[0];
            for (int i = 1; i < characters.Length; i++)
            {
                label += "+" + characters[i];
            }
            stateConnector.Label.Text = label;
            return stateConnector;
        }
    }
}
