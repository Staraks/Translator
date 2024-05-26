using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Translator
{
    public class Lexer
    {
        private string Source;
        private List<string> Result;
        public List<TokenType> LexemeType;

        public Lexer(string source)
        {
            Source = source.ToLower() + '\n';
            Result = new List<string>();
            LexemeType = new List<TokenType>();
        }


        #region Tokens

        static Dictionary<string, TokenType> SpecialWords = new Dictionary<string, TokenType>()
        {
            { "var", TokenType.VAR },
            { "integer", TokenType.INTEGER },
            { "boolean", TokenType.BOOLEAN },
            { "real", TokenType.REAL },
            { "begin", TokenType.BEGIN },
            { "end", TokenType.END },
            { "repeat", TokenType.REPEAT },
            { "until", TokenType.UNTIL },
            { ":=", TokenType.COLON_EQUALS },
            { "or", TokenType.OR },
            { "and", TokenType.AND },
            { "not", TokenType.NOT },
            { "<>", TokenType.NOT_EQUAL },
        };

        static Dictionary<char, TokenType> SpecialSymbols = new Dictionary<char, TokenType>()
        {
            { ',', TokenType.COMMA },
            { ':', TokenType.COLON },
            { ';', TokenType.SEMICOLON },
            { '+', TokenType.PLUS },
            { '-', TokenType.MINUS },
            { '*', TokenType.STAR },
            { '/', TokenType.SLASH },
            { '=', TokenType.ASSIGN },
            { '>', TokenType.GREATER_THAN },
            { '<', TokenType.LESS_THAN },
            { '(', TokenType.LEFT_PAREN },
            { ')', TokenType.RIGHT_PAREN },
            { '.', TokenType.DOT }

        };

        public static bool IsSpecialWord(string word)
        {
            if (string.IsNullOrEmpty(word))
            {
                return false;
            }
            return SpecialWords.ContainsKey(word);
        }

        public static bool IsSpecialSymbol(char symbol)
        {
            return SpecialSymbols.ContainsKey(symbol);
        }

        #endregion


        private void InitTokenAndAddResult(out Token token, TokenType type, string value)
        {
            token = new Token(type);
            token.Value = value;
            Result.Add(token.ToString());
            LexemeType.Add(type);
        }

        private char Check(char c)
        {
            if (char.IsDigit(c))
            {
                return 'd';
            }
            else if (char.IsLetter(c))
            {
                return 'l';
            }
            return c;
        }

        private void AddToken(string source)
        {
            Token token;
            TokenType type;

            if (source.Length == 1)
            {
                if (IsSpecialSymbol(source[0]))
                {
                    SpecialSymbols.TryGetValue(source[0], out type);
                    InitTokenAndAddResult(out token, type, " ");
                }
                else
                {
                    InitTokenAndAddResult(out token, TokenType.ID, source);
                }
            }
            else
            {
                if (IsSpecialWord(source))
                {
                    SpecialWords.TryGetValue(source, out type);
                    InitTokenAndAddResult(out token, type, " ");
                }
                else
                {
                    InitTokenAndAddResult(out token, TokenType.ID, source);
                }
            }
        }

        /// <summary>
        /// Анализирует исходный текст программы в виде строки и возвращет список полученных токенов
        /// </summary>
        /// <returns></returns>
        public List<string> Analyze()
        {
            int i = -1;
            string buffer = string.Empty;
            char symbol;

            while (++i < Source.Length)
            {
                symbol = Check(Source[i]);

                switch (symbol)
                {
                    case 'd':
                        for (; char.IsLetterOrDigit(Source[i]); i++)
                        {
                            if (char.IsLetter(Source[i]))
                            {
                                throw new Exception("Некорректное название переменной. Имя не должно начинаться с цифр!");
                            }
                            buffer += Source[i];
                        }
                        Token token;
                        InitTokenAndAddResult(out token, TokenType.LITERAL, buffer);
                        buffer = string.Empty;
                        --i;
                        continue;
                    case 'l':
                        for (; char.IsLetterOrDigit(Source[i]); i++)
                        {
                            buffer += Source[i];
                        }
                        AddToken(buffer);
                        buffer = string.Empty;
                        --i;
                        continue;
                    case ':':
                        if (Source[++i] == '=')
                        {
                            AddToken(":=");
                        }
                        else
                        {
                            --i;
                            AddToken(":");
                        }
                        break;
                    case '<':
                        if (Source[++i] == '>')
                        {
                            AddToken("<>");
                        }
                        else
                        {
                            --i;
                            AddToken("<");
                        }
                        break;
                    case ' ':
                        continue;
                    case '\n':
                        continue;
                    default:
                        AddToken(symbol.ToString());
                        continue;
                }
            }
            return Result;
        }
    }
}