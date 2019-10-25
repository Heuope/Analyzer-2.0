using System;
using System.Collections.Generic;
using System.Text;

namespace Minecraft
{
    static class FunctionCounter
    {

        static private List<string> _functions;
        static private List<string> _functionNames = new List<string>();
        static private Dictionary<string, int> _usedFunctions = new Dictionary<string, int>();

        static private bool IsItMe(string function, string functionName)
        {
            string name = Analyze.FindMethodName(function);
            name = " " + name.Substring(0, name.Length - 1);
            if (name == functionName)
                return true;
            return false;
        }

        static private int WhatIsFunction(string name)
        {
            
            for (int i = 0; i < _functions.Count; i++)
            {
                if (name == _functions[i])
                    return i;
            }

            return 0;
        }

        static private void RecFind(string fun)
        {
            foreach (var item in _functionNames)
            {
                if (fun.Contains(item) && !IsItMe(fun, item))
                {
                    for (int i = 0; i < _functions.Count; i++)
                        if (_functions[i].Contains(item))
                        {
                            string temp = item.Substring(0, item.Length - 1).Trim();
                            if (_usedFunctions.ContainsKey(temp))
                                _usedFunctions[temp]++;
                            else
                                _usedFunctions.Add(temp , 1);

                            RecFind(_functions[i]);

                            int index = WhatIsFunction(fun);
                            _functions[index] = _functions[index].Replace(item, " ");
                            break;
                        }
                }
            }
        }

        static public Dictionary<string, int> FindFunctions(List<string> code)
        {
            _functions = code;

            string mainFuction = "";
            
            for (int i = 0; i < _functions.Count; i++)
            {
                if (_functions[i].Contains("main"))
                {
                    mainFuction = _functions[i];
                    _functions.RemoveAt(i);
                    _usedFunctions.Add("main", 1);
                }
            }

            foreach (var item in code)
            {
                string temp = Analyze.FindMethodName(item);
                _functionNames.Add(" " + temp.Substring(0, temp.Length - 1));
            }

            RecFind(mainFuction);            

            return _usedFunctions;
        }

    }
}
