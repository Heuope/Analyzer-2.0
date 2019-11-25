using System;
using System.Collections.Generic;
using System.Linq;

namespace Minecraft
{
    class Box
    {
        public Dictionary<string, Dictionary<string, int>> FirstMetric = new Dictionary<string, Dictionary<string, int>>();

        public Dictionary<string, Dictionary<string, Dictionary<string, int>>> SecondMetric =
            new Dictionary<string, Dictionary<string, Dictionary<string, int>>>();

        public Dictionary<string, Dictionary<string, Dictionary<string, int>>> ThirdMetric =
            new Dictionary<string, Dictionary<string, Dictionary<string, int>>>();
    }

    static class Chepin
    {
        static private List<char> _numbers = new List<char> { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '"', '\'' };

        static private string[] _allSigns = { "(", "<=", ">=", "==", "!=", "+=", "-=", "++", "--", "+", "-", "<", ">", "=", "*", "/", "%" };

        static private string[] _limitedList = { "+", "-", "!", "<", ">" };

        static private string[] _arifmeticOperation = { "+", "-", "*", "/", "=", "++", "--" };

        static private string[] _comparisonSigns = { "<", ">", "<=", ">=", "==", "!=" };

        static private string[] _assignmentSigns = { "=", "++", "--", "+=", "-=" };

        static private string[] _assignmentSignsBack = { "++", "--" };

        static private string[] _symbols = { "==", "||", "&&", "&", "|", "<", ">", "!=", "++", "--",
                                            "*", "/", "%", "!", "=", "+", "-", "{", "(", ";",
                                            ":", "[", ",", "??", "}", ")", "]" };

        static private string[] _functions = { "random", "println", "print", "toString", "abs" };

        static private string[] _banWords = { "nextInt", "Math", "System", "out", "IO", "int", "float",
                                              "double", "public", "static", "void", "main", "String",
                                              "args", "Random", "random", "Color", "Font", "File", "Timer", "return", "Arrays",".in", "."};

        static private string[] _banWordsChepin = { "nextInt", "Math", "System", "IO", "int(", "float(",
                                                    "double(", "int", "float",
                                                    "double", "public", "static", "void", "main", "String",
                                                    "args", "Random", "random", "Color", "Font", "File", "Timer", "return", "Arrays"};

        static public string[] _operatorsChepin = {   ".length", "random", "for", "if", "switch", "case", "do", "while", "break",
                                                      "continue", "goto", "new", "println", "print",
                                                      "{", "}" , ")", ".", ";",
                                                      ":", "[" , "]", ",", "else", "toString", "??", "try", "catch",
                                                      "finaly", "abs(", "default"};

        static public string[] _operators = { ".length", "random", "for", "if", "switch", "case", "do", "while", "break",
                                              "continue", "goto", "new", "println", "print",
                                              "==", "||", "&&", "&", "|", "<", ">", "!=", "++", "--",
                                              "*", "/", "%", "!", "=", "+", "-", "{", "(", ";",
                                              ":", "[", ",", "else", "toString", "??", "try", "catch",
                                              "finaly", "abs", "default"};

        static private string Refactoring(string _str)
        {
            _str = _str.Replace("\r", " ");
            _str = _str.Replace("\n", " ");
            _str = _str.Replace("\t", " ");

            return _str;
        }

        // Возврощает внутренности функции 
        static private List<string> GetFunctionContents(string code)
        {
            List<string> arrayFunc = new List<string>();
            string[] headingWords = { "public", "static", "void", "private", "protected" };
            char[] ignoredSigns = { ' ', '\t', '\n', '=', '<', '>', '+', '-', '*', '/', '%', '(', ')', '!', ';', ':', '{', '}' };
            string currentWord = "";
            string functionCode = "";
            int braceCount = 0;
            for (int i = 0; i < code.Length; i++)
            {
                if (!ignoredSigns.Contains(code[i]))
                    currentWord += code[i];
                else if (currentWord != "")
                {
                    if (headingWords.Contains(currentWord))
                    {
                        while (code[i] != '{')
                            functionCode += code[i++];
                        functionCode += code[i];
                        if (functionCode.IndexOf("class") != -1)
                        {
                            functionCode = "";
                            currentWord = "";
                            continue;
                        }
                        braceCount = 1;
                        i++;
                        while ((braceCount > 0))
                        {
                            if (code[i] == '{')
                                braceCount++;
                            if (code[i] == '}')
                                braceCount--;
                            functionCode += code[i++];
                        }
                        arrayFunc.Add(functionCode);
                        functionCode = "";
                    }
                    functionCode += currentWord;
                    currentWord = "";
                }
            }
            return arrayFunc;
        }

        // Удаляет из строки все лишнее
        static string RemoveComponents(string code, string[] deletedWords)
        {
            foreach (var word in deletedWords)
                code = code.Replace(word, " ");
            return code;
        }

        static string RemoveString(string code, char startOfRemoval, char endOfRemoval) // Удаляет Строки
        {
            int flag = 0, temp = 0;
            for (int i = 0; i < code.Length; i++)
                if (code[i] == startOfRemoval || code[i] == endOfRemoval)
                {
                    flag++;
                    if (flag == 2)
                    {
                        code = code.Remove(temp, i - temp + 1);
                        flag = 0;
                    }
                    temp = i;
                }
            return code;
        }

        static private List<string> GetVariables(string functionContent) // Возврощает список переменых
        {
            List<string> functionArray = new List<string>();
            List<string> variables = new List<string>();
            functionContent = functionContent.Substring(functionContent.IndexOf("{"));

            functionContent = RemoveComponents(functionContent, _operators);
            functionContent = RemoveComponents(functionContent, _symbols);
            functionContent = RemoveComponents(functionContent, _functions);
            functionContent = RemoveComponents(functionContent, _banWords);

            foreach (var item in functionContent.Split(" "))
                if (item != "")
                    functionArray.Add(item);

            foreach (var item in functionArray)
                if (!_numbers.Contains(item[0]))
                    variables.Add(item);

            return variables;
        }

        static private string PreparationToPreparationToSplit(string functionContent, string symbol)
        {
            string functionSplitingContent = "";
            for (int i = 0; i < functionContent.Length; i++)
            {
                if (functionContent[i].ToString() == symbol)
                {
                    if (i > 0)
                        if (_limitedList.Contains(functionContent[i - 1].ToString()) & symbol == "=")
                        {
                            functionSplitingContent += symbol + " ";
                            continue;
                        }
                    if (functionContent[i + 1].ToString() == symbol)
                    {
                        i++;
                        functionSplitingContent += " " + symbol + symbol + " ";
                        continue;
                    }
                    if ((functionContent[i + 1].ToString() == "=") & _limitedList.Contains(symbol))
                    {
                        i++;
                        functionSplitingContent += " " + symbol + "=" + " ";
                        continue;
                    }
                    functionSplitingContent += " " + symbol + " ";
                    continue;
                }
                if (i != functionContent.Length - 1)
                    if ((functionContent[i].ToString() + functionContent[i + 1].ToString()) == symbol)
                    {
                        functionSplitingContent += " " + symbol + " ";
                        i++;
                        continue;
                    }
                functionSplitingContent += functionContent[i];
            }
            return functionSplitingContent;
        }

        static private string PreparationToSplit(string functionContent)
        {
            foreach (var sign in _allSigns)
                functionContent = PreparationToPreparationToSplit(functionContent, sign);
            return functionContent;

        }

        static private List<string> GetCodeForAnalysis(string functionContent) // Формируется список для ИИ
        {
            List<string> functionArray = new List<string>();
            List<string> variables = new List<string>();

            functionContent = functionContent.Substring(functionContent.IndexOf("{"));

            functionContent = RemoveComponents(functionContent, _operatorsChepin);
            functionContent = RemoveComponents(functionContent, _banWordsChepin);

            functionContent = PreparationToSplit(functionContent);

            foreach (var item in functionContent.Split(" "))
                if (item != "")
                    functionArray.Add(item);

            foreach (var item in functionArray)
                if (!_numbers.Contains(item[0]))
                    variables.Add(item);
            return variables;

        }

        static private List<string> GetMethodName(List<string> arrayFunctions)
        {
            List<string> methodNames = new List<string>();
            foreach (var function in arrayFunctions)
                for (int i = function.IndexOf("("); i > 0; i--)
                    if (function[i] == ' ')
                    {
                        methodNames.Add(function.Substring(++i, function.IndexOf("(") - i));
                        break;
                    }
            return methodNames;

        }

        private static Dictionary<string, List<string>> GetVariablesGroup(IEnumerable<string> variableSet, List<string> analisisCode) // ИИ, там все сложно
        {
            List<string> P = new List<string>();
            List<string> M = new List<string>();
            List<string> C = new List<string>();
            List<string> T = new List<string>();

            Dictionary<string, List<string>> functionGroups = new Dictionary<string, List<string>>(4);
            bool modified, contoll, introduced, appropriation;
            foreach (var variable in variableSet)
            {
                modified = contoll = introduced = appropriation = false;
                for (int i = 0; i < analisisCode.Count; i++)
                {
                    if (analisisCode[i] == variable)
                    {
                        if (i < analisisCode.Count - 1)
                            if (_assignmentSigns.Contains(analisisCode[i + 1]))
                            {
                                if (analisisCode[i + 1] != "=")
                                    appropriation = true;
                                modified = true;
                            }
                        if (i > 1)
                        {
                            if (_assignmentSignsBack.Contains(analisisCode[i - 1]))
                                modified = true;
                            if (_arifmeticOperation.Contains(analisisCode[i - 1]))
                                appropriation = true;
                            if (_comparisonSigns.Contains(analisisCode[i - 1]))
                                contoll = true;
                        }
                        if (i < analisisCode.Count - 1)
                            if (_comparisonSigns.Contains(analisisCode[i + 1]))
                                contoll = true;
                        if (i >= 2)
                            if ((analisisCode[i - 1] == "(") & (analisisCode[i - 2] == "out"))
                            {
                                introduced = true;
                            }
                        if (i < analisisCode.Count - 2)
                            if ((analisisCode[i + 1] == "=") & (analisisCode[i + 2] == "in"))
                            {
                                modified = false;
                                introduced = true;
                            }
                    }

                }
                if (contoll)
                {
                    C.Add(variable);
                    continue;
                }
                if (modified & appropriation)
                {
                    M.Add(variable);
                    continue;
                }
                if (introduced) // & appropriation
                {
                    P.Add(variable);
                    //continue;
                }
                if (!appropriation)
                    T.Add(variable);
            }
            functionGroups.Add("P", P);
            functionGroups.Add("C", C);
            functionGroups.Add("M", M);
            functionGroups.Add("T", T);

            return functionGroups;
        }

        private static Dictionary<string, List<string>> GetVariablesGroupIO(IEnumerable<string> variableSet, List<string> analisisCode) // ИИ, там все сложно
        {
            List<string> P = new List<string>();
            List<string> M = new List<string>();
            List<string> C = new List<string>();
            List<string> T = new List<string>();

            Dictionary<string, List<string>> functionGroups = new Dictionary<string, List<string>>(4);
            bool modified, contoll, introduced, appropriation;
            foreach (var variable in variableSet)
            {
                modified = contoll = introduced = appropriation = false;
                for (int i = 0; i < analisisCode.Count; i++)
                {
                    if (analisisCode[i] == variable)
                    {
                        if (i < analisisCode.Count - 1)
                            if (_assignmentSigns.Contains(analisisCode[i + 1]))
                            {
                                if (analisisCode[i + 1] != "=")
                                    appropriation = true;
                                modified = true;
                            }
                        if (i > 1)
                        {
                            if (_assignmentSignsBack.Contains(analisisCode[i - 1]))
                                modified = true;
                            if (_arifmeticOperation.Contains(analisisCode[i - 1]))
                                appropriation = true;
                            if (_comparisonSigns.Contains(analisisCode[i - 1]))
                                contoll = true;
                        }
                        if (i < analisisCode.Count - 1)
                            if (_comparisonSigns.Contains(analisisCode[i + 1]))
                                contoll = true;
                        if (i >= 2)
                            if ((analisisCode[i - 1] == "(") & (analisisCode[i - 2] == "out"))
                            {
                                introduced = true;
                            }
                        if (i < analisisCode.Count - 2)
                            if ((analisisCode[i + 1] == "=") & (analisisCode[i + 2] == "in"))
                            {
                                modified = false;
                                introduced = true;
                            }
                    }

                }
                if (contoll & introduced)
                {
                    C.Add(variable);
                    continue;
                }
                if (modified & appropriation & introduced)
                {
                    M.Add(variable);
                    continue;
                }
                if (introduced) // & appropriation
                {
                    P.Add(variable);
                    //continue;
                }
                if (introduced & !appropriation)
                    T.Add(variable);
            }
            functionGroups.Add("P", P);
            functionGroups.Add("C", C);
            functionGroups.Add("M", M);
            functionGroups.Add("T", T);

            return functionGroups;
        }

        private static int GetCount(List<string> list, string value)
        {
            int count = 0;            
            foreach (var item in list)
                if (item == value)
                    count++;                            

            return count;
        }

        static public Box NotMain(string filePath)
        {
            List<string> functions;
            List<string> analisisCode;
            List<string> methodNames;

            var box = new Box();            

            string fileCode = System.IO.File.ReadAllText(filePath);

            fileCode = Refactoring(fileCode);
            fileCode = RemoveString(fileCode, '"', '\'');

            var _functions = Analyze.GetFunctions(fileCode);

            var _functionNames = new List<string>();

            foreach (var item in _functions)
                _functionNames.Add(Analyze.FindMethodName(item));

            var _uselesNames = new List<string>();

            methodNames = GetMethodName(GetFunctionContents(fileCode));

            foreach (var name in methodNames)
                if (!_functionNames.Contains(name + "()"))
                    _uselesNames.Add(name);

            foreach (var item in GetMethodName(GetFunctionContents(fileCode)))  // Подготавливаем код проги
                fileCode = fileCode.Replace(item + "(", " ");

            functions = GetFunctionContents(fileCode); // Список функций
            
            foreach (var functionCode in functions)
            {
                var tempDict = new Dictionary<string, int>();

                var alilya = new Dictionary<string, int>();

                var variableSet = GetVariables(functionCode); // Список уникальных переменных

                foreach (var item in variableSet)
                    if (!alilya.ContainsKey(item))
                        alilya.Add(item, GetCount(variableSet, item) * GetCount(methodNames, methodNames[functions.IndexOf(functionCode)]) - GetCount(methodNames, methodNames[functions.IndexOf(functionCode)]));

                box.FirstMetric.Add(methodNames[functions.IndexOf(functionCode)], alilya);                                
            }

            fileCode = RemoveString(fileCode, '[', ']');
            functions = GetFunctionContents(fileCode);                     

            foreach (var functionCode in functions)
            {
                analisisCode = GetCodeForAnalysis(functionCode); // Список для ИИ
                var variableSet = GetVariables(functionCode).Distinct();
                string functionName = methodNames[functions.IndexOf(functionCode)];
                var tempVariableGroup = GetVariablesGroup(variableSet, analisisCode);

                var secondDick = new Dictionary<string, Dictionary<string, int>>();

                foreach (var item in tempVariableGroup.Keys) // Получает Словарь переменных и в нем перебирает значения
                {                        
                    var firstDick = new Dictionary<string, int>();

                    var variables = tempVariableGroup[item];

                    foreach (var vari in variables)
                        firstDick.Add(vari, GetCount(variables, vari) * GetCount(methodNames, functionName) - GetCount(methodNames, functionName));

                    secondDick.Add(item, firstDick);                    
                }

                box.SecondMetric.Add(functionName, secondDick);
            }
                       
            foreach (var functionCode in functions)
            {
                analisisCode = GetCodeForAnalysis(functionCode);
                var variableSet = GetVariables(functionCode).Distinct();
                string functionName = methodNames[functions.IndexOf(functionCode)];
                var tempVariableGroup = GetVariablesGroupIO(variableSet, analisisCode);

                var secondDick = new Dictionary<string, Dictionary<string, int>>();

                foreach (var item in tempVariableGroup.Keys) // Получает Словарь переменных и в нем перебирает значения
                {
                    var firstDick = new Dictionary<string, int>();

                    var variables = tempVariableGroup[item];

                    foreach (var vari in variables)
                        firstDick.Add(vari, GetCount(variables, vari) * GetCount(methodNames, functionName) - GetCount(methodNames, functionName));

                    secondDick.Add(item, firstDick);
                }

                box.ThirdMetric.Add(functionName, secondDick);
            }


            foreach (var name in _uselesNames)
            {
                box.FirstMetric.Remove(name);
                box.SecondMetric.Remove(name);
                box.ThirdMetric.Remove(name);
            }
                                   
            return box;
        }
    }
}

