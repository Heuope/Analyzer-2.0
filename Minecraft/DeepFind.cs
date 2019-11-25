using System;
using System.Collections.Generic;
using System.Text;

namespace Minecraft
{
    static class DeepFind
    {
        static private HashSet<string> _loops = new HashSet<string> { "for", "while",
                                                               "do", "if",
                                                               "else", "case",
                                                               "break", "default",
                                                               "foreach", "switch"};

        static private List<string> InitializeCode(string code)
        {            
            var result = new List<string>();
            
            code = code.Replace(';', ' ');
            code = code.Replace(':', ' ');
            code = code.Replace('(', ' ');

            foreach (var item in code.Split(" "))
                if (_loops.Contains(item) || item == "}")
                    result.Add(item);

            if (result.Count == 0)
                return result;

            while (!_loops.Contains(result[0]) && result.Count > 0)
            {
                result.RemoveAt(0);
                if (result.Count == 0)
                    break;
            }

            return result;
        }

        static public int FindDeep(string code)
        {
            var loops = InitializeCode(code);

            int doWhile = 0;
            int maxDeep = 0;
            int predict = -1;

            foreach (var item in loops)
            {
                if (doWhile > 0 && item == "while")
                {
                    predict--;
                    doWhile--;
                }

                if (item == "do")
                    doWhile++;

                if (item == "}" || item == "switch" || item == "break")
                    if (item == "}")
                        predict -= 2;
                    else
                        predict--;

                predict++;

                if (maxDeep < predict)
                    maxDeep = predict;
            }
            
            return maxDeep; // return CLI
        }        
    }
}
