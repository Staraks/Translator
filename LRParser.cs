using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Translator;

namespace Lexer
{
    internal class LRParser
    {
        private List<TokenType> LexemeType;
        private List<string> LexResult;
        private Stack<string> LexemeStack;
        private Stack<int> StateStack;
        private int LexIndex;
        private int State;

        public LRParser(List<TokenType> lexemeType, List<string> lexResult)
        {
            LexemeType = lexemeType;
            LexResult = lexResult;
            LexemeStack = new Stack<string>();
            StateStack = new Stack<int>();
            LexIndex = 0;
            GoToState(0);
        }

        private bool IsEnd()
        {
            return LexIndex > LexemeType.Count ? true : false;
        }

        private void Shift()
        {
            LexemeStack.Push(LexemeType[LexIndex].ToString());
            LexIndex++;
        }

        private void GoToState(int state)
        {
            StateStack.Push(state);
            State = state;
        }

        private void Reduce(int num, string neterminal)
        {
            for ( ; num > 0; num--)
            {
                LexemeStack.Pop();
                StateStack.Pop();
            }
            LexemeStack.Push(neterminal);
            State = StateStack.Peek();
        }

        private int CountOfElement(List<TokenType> elems, TokenType elem)
        {
            int count = 0;
            foreach (TokenType t in elems)
            {
                if (t == elem)
                {
                    count++;
                }
            }
            return count;
        }

        #region Expression

        public List<string> ReversePolishNotation = new List<string>();
        private Dictionary<TokenType, int> OpPriority;

        private string ToValue(int index)
        {
            string element = LexResult[index];
            string result = string.Empty;

            for (int i = 0; element[i] != ' '; i++)
            {
                result += element[i];
            }
            return result;
        }

        private string OperationToString(TokenType operation)
        {
            switch (operation)
            {
                case TokenType.OR:
                    return "or";
                case TokenType.AND:
                    return "and";
                case TokenType.NOT:
                    return "not";
                case TokenType.LESS_THAN:
                    return "<";
                case TokenType.GREATER_THAN:
                    return ">";
                case TokenType.ASSIGN:
                    return "=";
                case TokenType.NOT_EQUAL:
                    return "<>";
                case TokenType.PLUS:
                    return "+";
                case TokenType.MINUS:
                    return "-";
                case TokenType.STAR:
                    return "*";
                case TokenType.SLASH:
                    return "/";
                default:
                    return string.Empty;
            }
        }

        private int GetOpPriority(Dictionary<TokenType, int> opPriority, TokenType operation)
        {
            int priority;
            if (opPriority.ContainsKey(operation))
            {
                opPriority.TryGetValue(operation, out priority);
            }
            else
            {
                throw new Exception("Недопустимый символ в выражении!");
            }
            return priority;
        }

        private void Expression()
        {
            LexIndex--;
            if (LexemeType[LexIndex] != TokenType.ID && LexemeType[LexIndex] != TokenType.LITERAL && LexemeType[LexIndex] != TokenType.LEFT_PAREN)
            {
                throw new Exception("Ошибка в выражении");
            }
            string buffer = string.Empty;
            Stack<TokenType> operations = new Stack<TokenType>();
            OpPriority = new Dictionary<TokenType, int>()
            {
                { TokenType.LEFT_PAREN, 0 },
                { TokenType.RIGHT_PAREN, 1 },
                { TokenType.OR, 2 },
                { TokenType.AND, 3 },
                { TokenType.NOT, 4 },
                { TokenType.LESS_THAN, 5 },
                { TokenType.GREATER_THAN, 5 },
                { TokenType.ASSIGN, 5 },
                { TokenType.NOT_EQUAL, 5 },
                { TokenType.PLUS, 6 },
                { TokenType.MINUS, 6 },
                { TokenType.STAR, 7 },
                { TokenType.SLASH, 7 },
            };
            
            int lparCount = 0, rparCount = 0;

            for ( ; LexemeType[LexIndex] != TokenType.SEMICOLON && LexemeType[LexIndex] != TokenType.E; LexIndex++)
            {
                if (LexemeType[LexIndex] == TokenType.END || LexemeType[LexIndex] == TokenType.REPEAT || LexemeType[LexIndex] == TokenType.UNTIL
                    || LexemeType[LexIndex] == TokenType.DOT || (LexemeType[LexIndex] == TokenType.ID && LexemeType[LexIndex + 1] == TokenType.COLON_EQUALS))
                {
                    throw new Exception($"Ожидалась ';', а встретился {LexemeType[LexIndex]}");
                }

                if (LexemeType[LexIndex] == TokenType.ID || LexemeType[LexIndex] == TokenType.LITERAL)
                {
                    if (LexemeType[LexIndex + 1] == TokenType.ID || LexemeType[LexIndex + 1] == TokenType.LITERAL
                        || LexemeType[LexIndex + 1] == TokenType.LEFT_PAREN)
                    {
                        throw new Exception("Ошибка в выражении");
                    }
                    buffer += ToValue(LexIndex) + " ";
                }
                else
                {
                    #region Errors
                    if (LexemeType[LexIndex] == TokenType.LEFT_PAREN || LexemeType[LexIndex] == TokenType.RIGHT_PAREN)
                    {
                        if (LexemeType[LexIndex] == TokenType.LEFT_PAREN && LexemeType[LexIndex + 1] != TokenType.ID && LexemeType[LexIndex + 1] != TokenType.LITERAL &&
                            LexemeType[LexIndex + 1] != TokenType.LEFT_PAREN)
                        {
                            throw new Exception("Ошибка в выражении");
                        }
                        if (LexemeType[LexIndex] == TokenType.RIGHT_PAREN && (LexemeType[LexIndex + 1] == TokenType.ID || LexemeType[LexIndex + 1] == TokenType.LITERAL || LexemeType[LexIndex + 1] == TokenType.LEFT_PAREN))
                        {
                            throw new Exception("Ошибка в выражении");
                        }
                        if (LexemeType[LexIndex] == TokenType.LEFT_PAREN)
                        {
                            lparCount++;
                        }
                        else if (LexemeType[LexIndex] == TokenType.RIGHT_PAREN)
                        {
                            rparCount++;
                        }
                    }
                    else
                    {
                        if (LexemeType[LexIndex + 1] != TokenType.ID && LexemeType[LexIndex + 1] != TokenType.LITERAL &&
                        LexemeType[LexIndex + 1] != TokenType.LEFT_PAREN)
                        {
                            throw new Exception("Ошибка в выражении");
                        }
                    }
                    #endregion

                    if (operations.Count == 0 || LexemeType[LexIndex] == TokenType.LEFT_PAREN)
                    {
                        operations.Push(LexemeType[LexIndex]);
                        continue;
                    }
                    if (LexemeType[LexIndex] == TokenType.RIGHT_PAREN)
                    {
                        if (!operations.Contains(TokenType.LEFT_PAREN))
                        {
                            throw new Exception("Ошибка в выражении");
                        }
                        while (operations.Count() > 0 && operations.Peek() != TokenType.LEFT_PAREN)
                        {
                            buffer += OperationToString(operations.Pop()) + " ";
                        }
                        if (operations.Count() > 0)
                        {
                            operations.Pop();
                        }
                        continue;
                    }
                    int priority = GetOpPriority(OpPriority, LexemeType[LexIndex]);
                    if (priority == 0 || priority > GetOpPriority(OpPriority, operations.Peek()))
                    {
                        operations.Push(LexemeType[LexIndex]);
                    }
                    else
                    {
                        buffer += OperationToString(operations.Pop()) + " ";
                        while (operations.Count > 0)
                        {
                            int currPriority = GetOpPriority(OpPriority, operations.Peek());
                            if (currPriority >= priority)
                            {
                                buffer += OperationToString(operations.Pop()) + " ";
                                continue;
                            }
                            break;
                        }
                        operations.Push(LexemeType[LexIndex]);
                    }
                }
            }
            if (lparCount != rparCount)
            {
                throw new Exception("Ошибка в выражении");
            }
            while (operations.Count() > 0)
            {
                buffer += OperationToString(operations.Pop()) + " ";
            }
            ReversePolishNotation.Add(buffer);
        }

        #endregion

        private void State0()
        {
            if (LexemeStack.Count == 0)
            {
                Shift();
            }
            switch (LexemeStack.Peek())
            {
                case "<program>":
                    MessageBox.Show("Синтаксический разбор успешно завершён");
                    LexIndex = LexemeType.Count + 1;
                    break;
                case "VAR":
                    GoToState(1);
                    break;
                default:
                    throw new Exception($"Ожидался VAR, а встретился {LexemeStack.Peek()}");
            }
        }

        private void State1()
        {
            switch (LexemeStack.Peek())
            {
                case "VAR":
                    Shift();
                    break;
                case "<opis. perem.>":
                    GoToState(5);
                    break;
                case "<opis.>":
                    GoToState(36);
                    break;
                case "<spis. perem.>":
                    GoToState(3);
                    break;
                case "ID":
                    GoToState(4);
                    break;
                default:
                    throw new Exception($"Ожидался оператор ID, а встретился {LexemeStack.Peek()}");
            }
        }

        private void State3()
        {
            switch (LexemeStack.Peek())
            {
                case "<spis. perem.>":
                    Shift();
                    break;
                case "COLON":
                    GoToState(6);
                    break;
                case "COMMA":
                    GoToState(12);
                    break;
                default:
                    throw new Exception($"Ожидался : или ',', а встретился {LexemeStack.Peek()}");
            }

        }

        private void State4()
        {
            Reduce(1, "<spis. perem.>");
        }

        private void State5()
        {
            switch (LexemeStack.Peek())
            {
                case "<opis. perem.>":
                    Shift();
                    break;
                case "BEGIN":
                    GoToState(17);
                    break;
                case "<opis.>":
                    GoToState(14);
                    break;
                case "ID":
                    GoToState(1);
                    break;
                default:
                    throw new Exception($"Ожидался BEGIN, а встретился {LexemeStack.Peek()}");
            }
        }

        private void State6()
        {
            switch (LexemeStack.Peek())
            {
                case "COLON":
                    Shift();
                    break;
                case "<type>":
                    GoToState(7);
                    break;
                case "INTEGER":
                    GoToState(8);
                    break;
                case "BOOLEAN":
                    GoToState(9);
                    break;
                case "REAL":
                    GoToState(10);
                    break;
                default:
                    throw new Exception($"Ожидался тип, а встретился {LexemeStack.Peek()}");
            }

        }

        private void State7()
        {
            switch (LexemeStack.Peek())
            {
                case "<type>":
                    Shift();
                    break;
                case "SEMICOLON":
                    GoToState(11);
                    break;
                default:
                    throw new Exception($"Ожидался ;, а встретился {LexemeStack.Peek()}");
            }
        }

        private void State8()
        {
            if (string.Equals(LexemeStack.Peek(), "INTEGER"))
            {
                Reduce(1, "<type>");
            }
        }

        private void State9()
        {
            if (string.Equals(LexemeStack.Peek(), "BOOLEAN"))
            {
                Reduce(1, "<type>");
            }
        }

        private void State10()
        {
            if (string.Equals(LexemeStack.Peek(), "REAL"))
            {
                Reduce(1, "<type>");
            }
        }

        private void State11()
        {
            Reduce(4, "<opis.>");
        }

        private void State12()
        {
            switch (LexemeStack.Peek())
            {
                case "COMMA":
                    Shift();
                    break;
                case "ID":
                    GoToState(13);
                    break;
                default:
                    throw new Exception($"Ожидался ID, а встретился {LexemeStack.Peek()}");
            }
        }

        private void State13()
        {
            Reduce(3, "<spis. perem.>");
        }

        private void State14()
        {
            Reduce(2, "<opis. perem.>");
        }

        private void State17()
        {
            switch (LexemeStack.Peek())
            {
                case "BEGIN":
                    Shift();
                    break;
                case "<spis. oper.>":
                    GoToState(21);
                    break;
                case "<operator>":
                    GoToState(18);
                    break;
                case "<prisv>":
                    GoToState(19);
                    break;
                case "REPEAT":
                    GoToState(20);
                    break;
                case "ID":
                    GoToState(26);
                    break;
                default:
                    throw new Exception($"Ожидался ID или REPEAT, а встретился {LexemeStack.Peek()}");
            }
        }

        private void State18()
        {
            if (string.Equals(LexemeStack.Peek(), "<operator>"))
            {
                Reduce(1, "<spis. oper.>");
            }
        }

        private void State19()
        {
            if (string.Equals(LexemeStack.Peek(), "<prisv>"))
            {
                Reduce(1, "<operator>");
            }
        }

        private void State20()
        {
            switch (LexemeStack.Peek())
            {
                case "REPEAT":
                    Shift();
                    break;
                case "<spis. oper.>":
                    GoToState(23);
                    break;
                case "ID":
                    GoToState(17);
                    break;
                default:
                    throw new Exception($"Ожидался ID или REPEAT, а встретился {LexemeStack.Peek()}");
            }
        }

        private void State21()
        {
            switch (LexemeStack.Peek())
            {
                case "<spis. oper.>":
                    Shift();
                    break;
                case "END":
                    GoToState(34);
                    break;
                case "<operator>":
                    GoToState(22);
                    break;
                case "ID":
                    GoToState(17);
                    break;
                case "REPEAT":
                    GoToState(20);
                    break;
                case "UNTIL":
                    if (!LexemeStack.Contains("REPEAT"))
                    {
                        throw new Exception($"until используется только с циклом repeat!");
                    }
                    GoToState(24);
                    break;
                default:
                    throw new Exception($"Ожидался END или REPEAT или UNTIL или ID, а встретился {LexemeStack.Peek()}");
            }
        }

        private void State22()
        {
            Reduce(2, "<spis. oper.>");
        }

        private void State23()
        {
            switch (LexemeStack.Peek())
            {
                case "<spis. oper.>":
                    Shift();
                    break;
                case "UNTIL":
                    GoToState(24);
                    break;
                default:
                    throw new Exception($"Ожидался UNTIL, а встретился {LexemeStack.Peek()}");
            }
        }

        private void State24()
        {
            switch (LexemeStack.Peek())
            {
                case "UNTIL":
                    Shift();
                    Expression();
                    Shift();
                    GoToState(25);
                    break;
                default:
                    throw new Exception($"Ожидался expr, а встретился {LexemeStack.Peek()}");
            }
        }

        private void State25()
        {
            Reduce(5, "<operator>");
        }

        private void State26()
        {
            switch (LexemeStack.Peek())
            {
                case "ID":
                    Shift();
                    if (string.Equals(LexemeStack.Peek(), "ID"))
                    {
                        throw new Exception($"Ожидался :=, а встретился {LexemeStack.Peek()}");
                    }
                    break;
                case "COLON_EQUALS":
                    GoToState(27);
                    break;
                default:
                    throw new Exception($"Ожидался :=, а встретился {LexemeStack.Peek()}");
            }
        }

        private void State27()
        {
            switch (LexemeStack.Peek())
            {
                case "COLON_EQUALS":
                    Shift();
                    Expression();
                    Shift();
                    GoToState(30);
                    break;
                default:
                    throw new Exception($"Ожидался ID или LIT, а встретился {LexemeStack.Peek()}");
            }
        }

        private void State30()
        {
            switch (LexemeStack.Peek())
            {
                case "SEMICOLON":
                    GoToState(31);
                    break;
                default:
                    throw new Exception($"Ожидался ;, а встретился {LexemeStack.Peek()}");
            }
        }

        private void State31()
        {
            Reduce(4, "<prisv>");
        }

        private void State32()
        {
            switch (LexemeStack.Peek())
            {
                case "SEMICOLON":
                    GoToState(33);
                    break;
                default:
                    throw new Exception($"Ожидался ;, а встретился {LexemeStack.Peek()}");
            }
        }

        private void State33()
        {
            Reduce(4, "<prisv>");
        }

        private void State34()
        {
            if (CountOfElement(LexemeType, TokenType.REPEAT) != CountOfElement(LexemeType, TokenType.UNTIL))
            {
                throw new Exception($"Ожидался UNTIL, а встретился {LexemeStack.Peek()}");
            }
            switch (LexemeStack.Peek())
            {
                case "END":
                    Shift();
                    break;
                case "DOT":
                    GoToState(35);
                    break;
                default:
                    throw new Exception($"Ожидалась ., а встретился {LexemeStack.Peek()}");
            }
        }

        private void State35()
        {
            Reduce(6, "<program>");
            GoToState(0);
        }

        private void State36()
        {
            if (string.Equals(LexemeStack.Peek(), "<opis.>"))
            {
                Reduce(1, "<opis. perem.>");
            }
        }

        public void Analyze()
        {
            while (!IsEnd())
            {
                switch (State)
                {
                    case 0:
                        State0();
                        break;
                    case 1:
                        State1();
                        break;
                    case 3:
                        State3();
                        break;
                    case 4:
                        State4();
                        break;
                    case 5:
                        State5();
                        break;
                    case 6:
                        State6();
                        break;
                    case 7:
                        State7();
                        break;
                    case 8:
                        State8();
                        break;
                    case 9:
                        State9();
                        break;
                    case 10:
                        State10();
                        break;
                    case 11:
                        State11();
                        break;
                    case 12:
                        State12();
                        break;
                    case 13:
                        State13();
                        break;
                    case 14:
                        State14();
                        break;
                    case 17:
                        State17();
                        break;
                    case 18:
                        State18();
                        break;
                    case 19:
                        State19();
                        break;
                    case 20:
                        State20();
                        break;
                    case 21:
                        State21();
                        break;
                    case 22:
                        State22();
                        break;
                    case 23:
                        State23();
                        break;
                    case 24:
                        State24();
                        break;
                    case 25:
                        State25();
                        break;
                    case 26:
                        State26();
                        break;
                    case 27:
                        State27();
                        break;
                    case 30:
                        State30();
                        break;
                    case 31:
                        State31();
                        break;
                    case 32:
                        State32();
                        break;
                    case 33:
                        State33();
                        break;
                    case 34:
                        State34();
                        break;
                    case 35:
                        State35();
                        break;
                    case 36:
                        State36();
                        break;
                }
            }
        }
    }
}