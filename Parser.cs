using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Translator
{
    internal class Parser
    {
        private List<TokenType> LexemeType;
        private TokenType CurrentLexeme;
        private int Index;

        public Parser(List<TokenType> lexemeType)
        {
            LexemeType = lexemeType;
            Index = 0;
            CurrentLexeme = LexemeType[Index];
        }

        // Переход на следующий тип лексемы
        private void Next()
        {
            if (Index < LexemeType.Count - 1)
            {
                CurrentLexeme = LexemeType[++Index];
            }
        }

        // Программа
        public void Program()
        {
            if (CurrentLexeme != TokenType.VAR)
            {
                throw new Exception($"Ожидался var, а встретился {CurrentLexeme}");
            }
            Next();
            VariablesDescription();
            if (CurrentLexeme != TokenType.BEGIN)
            {
                throw new Exception($"Ожидался begin, а встретился {CurrentLexeme}");
            }
            Next();
            ListOper();
            if (CurrentLexeme != TokenType.END)
            {
                throw new Exception($"Ожидался end, а встретился {CurrentLexeme}");
            }
            Next();
            if (CurrentLexeme != TokenType.DOT)
            {
                throw new Exception($"Ожидалась '.', а встретился {CurrentLexeme}");
            }
            MessageBox.Show("Разбор успешно выполнен!");
        }

        // Описание переменных
        private void VariablesDescription()
        {
            Description();
            Next();
            X();
        }

        // Описание
        private void Description()
        {
            ListOfVars();
            if (CurrentLexeme != TokenType.COLON)
            {
                throw new Exception($"Ожидался ':', а встретился {CurrentLexeme}");
            }
            Next();
            Type();
            Next();
            if (CurrentLexeme != TokenType.SEMICOLON)
            {
                throw new Exception($"Ожидался ';', а встретился {CurrentLexeme}");
            }
        }

        // Список переменных
        private void ListOfVars()
        {
            if (CurrentLexeme != TokenType.ID)
            {
                throw new Exception($"Ожидался id, а встретился {CurrentLexeme}");
            }
            Next();
            Y();
        }

        private void Y()
        {
            if (CurrentLexeme == TokenType.COLON)
            {
                return;
            }
            else if (CurrentLexeme == TokenType.COMMA)
            {
                Z();
            }
            else
            {
                throw new Exception($"Ожидался ':' или ',', а встретился {CurrentLexeme}");
            }
        }

        private void Z()
        {
            if (CurrentLexeme != TokenType.COMMA)
            {
                throw new Exception($"Ожидалась ',', а встретился {CurrentLexeme}");
            }
            Next();
            if (CurrentLexeme != TokenType.ID)
            {
                throw new Exception($"Ожидался id, а встретился {CurrentLexeme}");
            }
            Next();
            Y();
        }

        // Тип
        private void Type()
        {
            if (CurrentLexeme != TokenType.INTEGER && CurrentLexeme != TokenType.BOOLEAN && CurrentLexeme != TokenType.REAL)
            {
                throw new Exception($"Ожидался тип даных, а встретился {CurrentLexeme}");
            }
        }

        private void X()
        {
            if (CurrentLexeme == TokenType.BEGIN)
            {
                return;
            }
            else if (CurrentLexeme == TokenType.ID)
            {
                U();
            }
            else
            {
                throw new Exception($"Ожидался begin или id, а встретился {CurrentLexeme}");
            }
        }

        private void U()
        {
            VariablesDescription();
        }

        // Спис. опер.
        private void ListOper()
        {
            Operator();
            Next();
            R();
        }

        // Оператор
        private void Operator()
        {
            if (CurrentLexeme == TokenType.ID)
            {
                Assigment();
            }
            else if (CurrentLexeme == TokenType.REPEAT)
            {
                Next();
                ListOper();
                if (CurrentLexeme != TokenType.UNTIL)
                {
                    throw new Exception($"Ожидался until, а встретился {CurrentLexeme}");
                }
                Next();
                Expression();
            }
            else
            {
                throw new Exception($"Ожидался id или repeat, а встретился {CurrentLexeme}");
            }
        }

        // Присваивание
        private void Assigment()
        {
            if (CurrentLexeme != TokenType.ID)
            {
                throw new Exception($"Ожидался id, а встретился {CurrentLexeme}");
            }
            Next();
            if (CurrentLexeme != TokenType.COLON_EQUALS)
            {
                throw new Exception($"Ожидался ':=', а встретился {CurrentLexeme}");
            }
            Next();
            Expression();
        }

        // Выражение
        private void Expression()
        {
            while (CurrentLexeme != TokenType.SEMICOLON)
            {
                Next();
            }
        }

        private void R()
        {
            if (CurrentLexeme == TokenType.END || CurrentLexeme == TokenType.UNTIL)
            {
                return;
            }
            else if (CurrentLexeme == TokenType.ID || CurrentLexeme == TokenType.REPEAT)
            {
                W();
            }
            else
            {
                throw new Exception($"Ожидался end, id, repeat, или until, а встретился {CurrentLexeme}");
            }
        }

        private void W()
        {
            ListOper();
        }
    }
}