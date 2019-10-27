using System;
using System.Collections.Generic;
using System.Text;

namespace Minecraft
{
    static class FunctionCounter
    {
        static private List<string> _functionNames = new List<string>();
        static private Dictionary<string, string> _functions = new Dictionary<string, string>();
        static private Dictionary<string, int> _usedFunctions = new Dictionary<string, int>();

        static private void RecFind(string funName)
        {
            foreach (var item in _functionNames)
            {
                if (_functions[funName].Contains(item) && item != funName)
                {
                    if (_usedFunctions.ContainsKey(item))
                        _usedFunctions[item]++;
                    else
                        _usedFunctions.Add(item , 1);

                    RecFind(item);

                    _functions[funName] = _functions[funName].Replace(item, " ");
                }
            }
        }

        static public Dictionary<string, int> FindFunctions(List<string> code)
        {
            foreach (var item in code)
            {
                string temp = Analyze.FindMethodName(item);
                _functionNames.Add(temp);
                _functions.Add(temp, Analyze.FindCode(item));
            }

            _usedFunctions.Add("main()", 1);
            RecFind("main()");

            return _usedFunctions;
        }

    }
}
