using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConceptsOfProgrammingLanguages
{
    public class Extention
    {
        public static string JoinString(string source, string destination, string operation)
        {
            string result = "";
            string va1 = "";
            string va2 = "";
            if (operation == FaToReConverter.OR)
            {
                if (source != FaToReConverter.VALUE_NULL.ToString())
                {
                    if (source.Length > 1)
                    {
                        va1 += FaToReConverter.LEFT_PAREN + source + FaToReConverter.RIGHT_PAREN;
                    }
                    else
                    {
                        va1 += source;
                    }
                }

                if (destination != FaToReConverter.VALUE_NULL.ToString())
                    if (destination.Length > 1)
                    {
                        va2 += FaToReConverter.LEFT_PAREN + destination + FaToReConverter.RIGHT_PAREN;
                    }
                    else
                    {
                        va2 += destination;
                    }
            }
            else
            {
                if (source != FaToReConverter.VALUE_NULL.ToString())
                    if (source.Length > 1)
                    {
                        va1 += FaToReConverter.LEFT_PAREN + source + FaToReConverter.RIGHT_PAREN;
                    }
                    else
                    {
                        va1 += source;
                    }

                if (destination != FaToReConverter.VALUE_NULL.ToString())
                    if (destination.Length > 1)
                    {
                        va2 += FaToReConverter.LEFT_PAREN + destination + FaToReConverter.RIGHT_PAREN;
                    }
                    else
                    {
                        va2 += destination;
                    }
            }

            if (va1 != FaToReConverter.LAMBDA && va2 != FaToReConverter.LAMBDA)
            {
                result = va1 + operation + va2;
            }
            else if (va1 != FaToReConverter.LAMBDA && va2 == FaToReConverter.LAMBDA)
            {
                result = va1;
            }
            else if (va2 != FaToReConverter.LAMBDA && va1 == FaToReConverter.LAMBDA)
            {
                result = va2;
            }

            if (result == FaToReConverter.LAMBDA)
            {
                result = FaToReConverter.VALUE_NULL.ToString();
            }
            return result;
        }

        public static string GroupString(string str)
        {
            string result = "";
            if (str != FaToReConverter.VALUE_NULL.ToString())
            {
                if (str.Length > 1)
                {
                    result = FaToReConverter.LEFT_PAREN + str + FaToReConverter.RIGHT_PAREN;
                }
                else
                {
                    result = str;
                }
                result += FaToReConverter.KLEENE_STAR;
            }
            else
            {
                result = FaToReConverter.VALUE_NULL.ToString();
            }
            return result;
        }

        public static string concatenate(string r1, string r2)
        {
            if (r1 == FaToReConverter.VALUE_NULL.ToString() || r2 == FaToReConverter.VALUE_NULL.ToString())
                return FaToReConverter.VALUE_NULL.ToString();
            else if (r1.Equals(FaToReConverter.LAMBDA))
                return r2;
            else if (r2.Equals(FaToReConverter.LAMBDA))
                return r1;
            if (or(r1).Length > 1)
                r1 = addParen(r1);
            if (or(r2).Length > 1)
                r2 = addParen(r2);
            return r1 + r2;
        }

        public static string addParen(string word)
        {
            return FaToReConverter.LEFT_PAREN + word + FaToReConverter.RIGHT_PAREN;
        }

        public static string star(string r1)
        {
            if (r1 == FaToReConverter.VALUE_NULL.ToString() || r1 == FaToReConverter.LAMBDA.ToString())
                return FaToReConverter.LAMBDA;
            if (or(r1).Length > 1 || cat(r1).Length > 1)
            {
                r1 = addParen(r1);
            }
            else
            {
                if (r1.EndsWith(FaToReConverter.KLEENE_STAR))
                    return r1;
            }
            return r1 + FaToReConverter.KLEENE_STAR;
        }

        public static string or(string r1, string r2)
        {
            if (r1 == FaToReConverter.VALUE_NULL.ToString())
                return r2;
            if (r2 == FaToReConverter.VALUE_NULL.ToString())
                return r1;
            if (r1 == FaToReConverter.LAMBDA.ToString() && r2 == FaToReConverter.LAMBDA.ToString())
                return FaToReConverter.LAMBDA;
            if (r1 == FaToReConverter.LAMBDA)
                r1 = FaToReConverter.VALUE_E.ToString();
            if (r2 == FaToReConverter.LAMBDA)
                r2 = FaToReConverter.VALUE_E.ToString();
            return r1 + FaToReConverter.OR + r2;
        }

        public static string[] or(string expression)
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
                se.Add(delambda(expression.Substring(start, i)));
                start = i + 1;
            }
            se.Add(delambda(expression.Substring(start)));
            return se.ToArray();
        }
        public static string delambda(string s)
        {
            return s.Equals(FaToReConverter.VALUE_E.ToString()) ? "" : s;
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
                se.Add(delambda(expression.Substring(start, i)));
                start = i;
            }
            se.Add(delambda(expression.Substring(start)));
            return se.ToArray();
        }

        public static string getExpressionFromGTG(IList<ItemTableConnector> list, string start, string end)
        {
            string ii = list.Where(t => t.SourceState == start && t.DestinationState == start).FirstOrDefault().Value;
            string ij = list.Where(t => t.SourceState == start && t.DestinationState == end).FirstOrDefault().Value;
            string jj = list.Where(t => t.SourceState == end && t.DestinationState == end).FirstOrDefault().Value;
            string ji = list.Where(t => t.SourceState == end && t.DestinationState == start).FirstOrDefault().Value;

            return getFinalExpression(ii, ij, jj, ji);
        }

        public static string getFinalExpression(string ii, string ij, string jj, string ji)
        {
            string temp = concatenate(star(ii), concatenate(ij, concatenate(
                    star(jj), ji)));
            string temp2 = concatenate(star(ii), concatenate(ij, star(jj)));

            string expression = concatenate(star(temp), temp2);
            return expression;
        }
    }
}
