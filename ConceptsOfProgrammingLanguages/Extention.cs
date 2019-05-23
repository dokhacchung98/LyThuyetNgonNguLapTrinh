using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConceptsOfProgrammingLanguages
{
    public class Extention
    {
        public static char VALUE_E = 'ε';
        public static char VALUE_NULL = 'Ø';

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
    }
}
