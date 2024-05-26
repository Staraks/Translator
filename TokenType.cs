using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Translator
{
    public enum TokenType
    {
        VAR,
        INTEGER,
        BOOLEAN,
        REAL,
        BEGIN,
        END,
        LITERAL,
        ID,
        COMMA,
        COLON,
        SEMICOLON,
        PLUS,
        MINUS,
        STAR,
        SLASH,
        ASSIGN,
        GREATER_THAN,
        LESS_THAN,
        LEFT_PAREN,
        RIGHT_PAREN,
        REPEAT,
        UNTIL,
        DOT,
        COLON_EQUALS,
        OR,
        AND,
        NOT_EQUAL,
        NOT,
        E
    }
}