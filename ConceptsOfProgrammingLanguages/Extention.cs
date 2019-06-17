using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConceptsOfProgrammingLanguages
{
    public class Extention
    {
        public static string Concatenate(string r1, string r2)
        {
            if (r1 == FaToReConverter.VALUE_NULL.ToString() || r2 == FaToReConverter.VALUE_NULL.ToString())
                return FaToReConverter.VALUE_NULL.ToString();
            else if (r1.Equals(FaToReConverter.VALUE_E.ToString()))
                return r2;
            else if (r2.Equals(FaToReConverter.VALUE_E.ToString()))
                return r1;
            if (Or(r1).Length > 1)
                r1 = AddParen(r1);
            if (Or(r2).Length > 1)
                r2 = AddParen(r2);
            return r1 + r2;
        }

        public static string AddParen(string word)
        {
            return FaToReConverter.LEFT_PAREN + word + FaToReConverter.RIGHT_PAREN;
        }

        public static string Star(string r1)
        {
            if (r1 == FaToReConverter.VALUE_NULL.ToString() || r1 == FaToReConverter.LAMBDA.ToString() || r1 == FaToReConverter.VALUE_E.ToString())
                return FaToReConverter.LAMBDA;
            if (Or(r1).Length > 1 || cat(r1).Length > 1)
            {
                r1 = AddParen(r1);
            }
            else
            {
                if (r1.EndsWith(FaToReConverter.KLEENE_STAR))
                    return r1;
            }
            return r1 + FaToReConverter.KLEENE_STAR;
        }

        public static string Or(string r1, string r2)
        {
            if (r1 == FaToReConverter.VALUE_NULL.ToString())
                return r2;
            if (r2 == FaToReConverter.VALUE_NULL.ToString())
                return r1;
            if (r1 == FaToReConverter.VALUE_E.ToString() || r1 == FaToReConverter.LAMBDA)
            {
                if (r2 == FaToReConverter.VALUE_NULL.ToString() || r2 == FaToReConverter.VALUE_E.ToString() || r2 == FaToReConverter.LAMBDA)
                {
                    return FaToReConverter.VALUE_E.ToString();
                }
                else
                {
                    return r2;
                }
            }
            else if (r2 == FaToReConverter.VALUE_E.ToString() || r2 == FaToReConverter.LAMBDA)
            {
                if (r1 == FaToReConverter.VALUE_NULL.ToString() || r1 == FaToReConverter.VALUE_E.ToString() || r1 == FaToReConverter.LAMBDA)
                {
                    return FaToReConverter.VALUE_E.ToString();
                }
                else
                {
                    return r1;
                }
            }
            return r1 + FaToReConverter.OR + r2;
        }

        public static string[] Or(string expression)
        {
            List<string> se = new List<string>();
            int start = 0;
            int level = 0;
            for (int i = 0; i < expression.Length; i++)
            {
                if (expression.ElementAt(i) == '(')
                    level++;
                if (expression.ElementAt(i) == ')')
                    level--;
                if (expression.ElementAt(i) != '+')
                    continue;
                if (level != 0)
                    continue;
                // First level or!
                se.Add(Delambda(expression.Substring(start, i)));
                start = i + 1;
            }
            se.Add(Delambda(expression.Substring(start)));
            return se.ToArray();
        }
        public static string Delambda(string s)
        {
            return s.Equals(FaToReConverter.VALUE_E.ToString()) ? FaToReConverter.LAMBDA : s;
        }
        public static string[] cat(string expression)
        {
            IList<string> se = new List<string>(); // Subexpressions.
            int start = 0;
            int level = 0;
            for (int i = 0; i < expression.Length; i++)
            {
                char c = expression.ElementAt(i);
                if (c == ')')
                {
                    level--;
                    continue;
                }
                if (c == '(')
                    level++;
                if (!(c == '(' && level == 1) && level != 0)
                    continue;
                if (c == '+')
                {
                    throw new Exception();
                }
                if (c == '*')
                    continue;
                // Not an operator, and on the first level!
                if (i == 0)
                    continue;
                se.Add(Delambda(expression.Substring(start, i)));
                start = i;
            }
            se.Add(Delambda(expression.Substring(start)));
            return se.ToArray();
        }

        public static string GetExpressionFromGTG(IList<ItemTableConnector> list, string start, string end)
        {
            string ii = list.Where(t => t.SourceState == start && t.DestinationState == start).FirstOrDefault().Value;
            string ij = list.Where(t => t.SourceState == start && t.DestinationState == end).FirstOrDefault().Value;
            string jj = list.Where(t => t.SourceState == end && t.DestinationState == end).FirstOrDefault().Value;
            string ji = list.Where(t => t.SourceState == end && t.DestinationState == start).FirstOrDefault().Value;

            return GetFinalExpression(ii, ij, jj, ji);
        }

        public static string GetFinalExpression(string ii, string ij, string jj, string ji)
        {
            string temp = Concatenate(Star(ii), Concatenate(ij, Concatenate(
                    Star(jj), ji)));
            string temp2 = Concatenate(Star(ii), Concatenate(ij, Star(jj)));

            string expression = Concatenate(Star(temp), temp2);
            return expression;
        }
    }
}
