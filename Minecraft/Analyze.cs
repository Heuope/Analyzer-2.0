using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minecraft
{
    static class Analyze
    {
        static public string[] _symbols = { "==", "||", "&&", "&", "|", "<", ">", "!=", "++", "--",
                                            "*", "/", "%", "!", "=", "+", "-", "{", "(", ".", ";",
                                            ":", "[", ",", "??" };

        static public string[] _functions = { "random", "print", "println", "toString", "abs" };

        static private string[] _banWords = { "Math", "System", "", "out", "IO", "int", "float",
                                              "double", "public", "static", "void", "main", "String",
                                              "args", "Random", "Color", "Font", "File", "Timer" };

        static public string[] Operators = { "for", "if", "switch", "case", "do", "while", "break",
                                              "continue", "goto", "new", "random", "print", "println",
                                              "==", "||", "&&", "&", "|", "<", ">", "!=", "++", "--",
                                              "*", "/", "%", "!", "=", "+", "-", "{", "(", ".", ";",
                                              ":", "[", ",", "else", "toString", "??", "try", "catch",
                                              "finaly", "abs", "default"};

        static private int FindAmountSymbols(string str, string sym)
        {
            if (sym.Length > str.Length)
                return 0;

            int counter = 0;

            for (var i = 0; i < str.Length - sym.Length; i++)
                if (str.Substring(i, sym.Length) == sym)
                    counter++;

            return counter;
        }

        static private List<string> GetFunctions(string code)
        {
            List<string> arrayFunc = new List<string>();

            string[] array = { "public", "static", "void", "private", "protected" };
            char[] list = { ' ', '\t', '\n', '=', '<', '>', '+', '-', '*', '/', '%', '(', ')', '!', ';', ':', '{', '}' };

            string word = "";
            string text = "";

            int scobka = 0;
            for (int i = 0; i < code.Length; i++)
            {
                if (!list.Contains(code[i]))  // собирает слова из символов
                    word += code[i];
                else if (word != "")
                {
                    if (array.Contains(word)) // находит начало функции
                    {
                        while (code[i] != '{')
                        {
                            text += code[i];
                            i++;
                        }
                        text += code[i];
                        if (text.IndexOf("class") != -1) // если это класс То все хуйня, давай по новой
                        {
                            text = "";
                            word = "";
                            continue;
                        }
                        scobka = 1;
                        i++;
                        while ((scobka > 0))
                        {
                            if (code[i] == '{')
                                scobka++;
                            if (code[i] == '}')
                                scobka--;
                            text += code[i];
                            i++;
                        }
                        arrayFunc.Add(text);
                        text = "";
                    }
                    text += word;
                    word = "";
                }
            }

            for (int i = 0; i < arrayFunc.Count; i++)
                arrayFunc[i] = Refactoring(arrayFunc[i]);

            var functions = FunctionCounter.FindFunctions(arrayFunc);
            var usedFunctions = new List<string>();

            foreach (var item in functions.Keys)
            {
                for (var i = 0; i < arrayFunc.Count; i++)
                    if (FindMethodName(arrayFunc[i]) == item)
                        for (int j = 0; j < functions[item]; j++)
                            if (j == 0)
                                usedFunctions.Add(arrayFunc[i]);
                            else
                                usedFunctions[usedFunctions.Count - 1] += FindCode(arrayFunc[i]);
            }

            return usedFunctions;
        }

        static private string Refactoring(string code)
        {
            code = code.Replace("\r", " ");
            code = code.Replace("\n", " ");
            code = code.Replace("\t", " ");

            return code;
        }

        static public string FindMethodName(string code)
        {
            string name = "";
            for (int i = code.IndexOf('('); ; i--)
                if (code[i] == ' ' && name != "")
                    break;
                else
                    name = code[i] + name;

            return (name.Trim() + ")");
        }

        static public string FindCode(string code)
        {
            return Refactoring(code.Substring(code.IndexOf(')') + 1));
        }

        static public List<string> FindMethodNames(string filePath)
        {
            var names = new List<string>();

            var code = GetFunctions(System.IO.File.ReadAllText(filePath));

            foreach (var item in code)
                names.Add(FindMethodName(item));

            return names;
        }

        static public List<Dictionary<string, int>> analyze(string filePath)
        {
            var code = GetFunctions(System.IO.File.ReadAllText(filePath));

            var listDictionary = new List<Dictionary<string, int>>();

            foreach (var itemG in code)
            {
                string tempCode = FindCode(itemG);

                var dictionary = new Dictionary<string, int>();

                dictionary.Clear();

                dictionary.Add(FindMethodName(itemG), -1);

                //
                // find all _symbols and replace them on space
                //

                foreach (var symbol in _symbols)
                    if (FindAmountSymbols(tempCode, symbol) != 0)
                    {
                        dictionary.Add(symbol, FindAmountSymbols(tempCode, symbol));
                        tempCode = tempCode.Replace(symbol, " ");
                    }

                //
                // find strings and chars
                //           

                int flag = 0, tmp = 0;
                string word;

                for (int i = 0; i < tempCode.Length; i++)
                    if (tempCode[i] == '"' || tempCode[i] == '\'')
                    {
                        flag++;
                        if (flag == 2)
                        {
                            word = tempCode.Substring(tmp, i - tmp + 1);
                            if (!dictionary.ContainsKey(word))
                                dictionary.Add(word, 1);
                            else
                                dictionary[word]++;
                            tempCode = tempCode.Remove(tmp, i - tmp + 1);
                            flag = 0;
                        }
                        tmp = i;
                    }

                // Create List with last words.

                tempCode = tempCode.Replace("}", " ");
                tempCode = tempCode.Replace("]", " ");
                tempCode = tempCode.Replace(")", " ");

                var lastWords = new List<string>();

                foreach (var item in tempCode.Split(' '))
                    if (item != "")
                        lastWords.Add(item);

                // Add "()" to functions and remove from global ().
                
                for (int i = 0; i < lastWords.Count; i++)
                    if (_functions.Contains(lastWords[i]))
                        dictionary["("]--;

                //  Add last world to dictionary.

                foreach (var item in lastWords)
                    if (!dictionary.ContainsKey(item))
                        dictionary.Add(item, 1);
                    else
                        dictionary[item]++;

                //  do while.

                if (dictionary.ContainsKey("while") && dictionary.ContainsKey("do"))
                    dictionary["while"] -= dictionary["do"];

                if (dictionary.ContainsKey("while"))
                    dictionary["("] -= dictionary["while"];

                if (dictionary.ContainsKey("do"))
                    dictionary["("] -= dictionary["do"];

                // if else.

                if (dictionary.ContainsKey("if") && dictionary.ContainsKey("else"))
                    dictionary["if"] -= dictionary["else"];

                // try catch finaly.

                if (dictionary.ContainsKey("try") && dictionary.ContainsKey("catch"))
                    dictionary["try"] -= dictionary["catch"];

                if (dictionary.ContainsKey("try") && dictionary.ContainsKey("finaly"))
                    dictionary["try"] -= dictionary["finaly"];

                //  Ban words from _banWords and remove all keys with value 0.

                foreach (var item in _banWords)
                    if (dictionary.ContainsKey(item))
                        dictionary.Remove(item);

                foreach (var item in Operators)
                    if (dictionary.ContainsKey(item))
                        if (dictionary[item] == 0)
                            dictionary.Remove(item);


                listDictionary.Add(dictionary);
            }

            // Make Global Dictionary with all Functions.

            var global = new Dictionary<string, int>();
            global.Add("GLOBAL", 0);
            foreach (var dictionary in listDictionary)
                foreach (var key in dictionary.Keys)
                    if (!global.ContainsKey(key))
                        global.Add(key, dictionary[key]);
                    else
                        global[key] += dictionary[key];

            foreach (var methodName in FindMethodNames(filePath))
                global.Remove(methodName);

            listDictionary.Add(global);

            return listDictionary;
        }
    }
}
