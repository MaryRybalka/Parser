using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static parser.LexerTypes;

namespace parser
{
    public class Lexer
    {
        private readonly string[] _statusNames =
        {
            "START",
            "IDENT",
            "NUMBER", //number literal
            "REALNUMBER_ONE",
            "REALNUMBER_DOT",
            "STRING", //string literal
            "STRING_END",
            "STRING_SLASH",
            "SLASH",
            "ONE_LC",
            "MULTI_LC",
            "MULTI_LC_PRE_END",
            "MULTI_LC_END",
            "STAR",
            "PLUS",
            "PLUSPLUS",
            "MINUS",
            "MINUSMINUS",
            "PERCENT",
            "NOT",
            "EQUAL",
            "TWO_EQUAL",
            "THREE_EQUAL",
            "SLASHEQUAL",
            "STAREQUAL",
            "PLUSEQUAL",
            "MINUSEQUAL",
            "PERCENTEQUAL",
            "NOTEQUAL",
            "NOT_TWO_EQUAL",
            "OPEN_ROUND_BRACKET",
            "CLOSE_ROUND_BRACKET",
            "OPEN_CURLY_BRACKET",
            "CLOSE_CURLY_BRACKET",
            "COMMA",
            "SEMICOLON",
            "AND",
            "ANDEQUAL",
            "ANDAND",
            "OR",
            "OREQUAL",
            "OROR",
            "MORE",
            "MOREOREQUAL",
            "LESS",
            "LESSOREQUAL"
        };

        private static readonly State[][] Table =
        {
            new[]
            {
                new State(Status.START, 1),
                new State(Status.START, 1),
                new State(Status.IDENT, 0),
                new State(Status.NUMBER, 0),
                new State(Status.STRING, 0),
                new State(Status.ERROR, 0),
                new State(Status.REALNUMBER_DOT, 0),
                new State(Status.SLASH, 0),
                new State(Status.STAR, 0),
                new State(Status.PLUS, 0),
                new State(Status.MINUS, 0),
                new State(Status.PERCENT, 0),
                new State(Status.NOT, 0),
                new State(Status.EQUAL, 0),
                new State(Status.OPEN_ROUND_BRACKET, 0),
                new State(Status.CLOSE_ROUND_BRACKET, 0),
                new State(Status.OPEN_CURLY_BRACKET, 0),
                new State(Status.CLOSE_CURLY_BRACKET, 0),
                new State(Status.COMMA, 0),
                new State(Status.SEMICOLON, 0),
                new State(Status.AND, 0),
                new State(Status.OR, 0),
                new State(Status.MORE, 0),
                new State(Status.LESS, 0),
                new State(Status.ERROR, 0)
            },
            new[]
            {
                new State(Status.START, 1), new State(Status.START, 1), new State(Status.IDENT, 0),
                new State(Status.IDENT, 0), new State(Status.STRING, 1), new State(Status.ERROR, 0),
                new State(Status.REALNUMBER_DOT, 1), new State(Status.START, 1), new State(Status.STAR, 1),
                new State(Status.PLUS, 1), new State(Status.MINUS, 1), new State(Status.PERCENT, 1),
                new State(Status.NOT, 1), new State(Status.EQUAL, 1), new State(Status.OPEN_ROUND_BRACKET, 1),
                new State(Status.CLOSE_ROUND_BRACKET, 1), new State(Status.OPEN_CURLY_BRACKET, 1),
                new State(Status.CLOSE_CURLY_BRACKET, 1), new State(Status.COMMA, 1), new State(Status.SEMICOLON, 1),
                new State(Status.AND, 1), new State(Status.OR, 1), new State(Status.MORE, 1), new State(Status.LESS, 1),
                new State(Status.ERROR, 0)
            },
            new[]
            {
                new State(Status.START, 1), new State(Status.START, 1), new State(Status.IDENT, 1),
                new State(Status.NUMBER, 0), new State(Status.STRING, 1), new State(Status.ERROR, 0),
                new State(Status.REALNUMBER_ONE, 0), new State(Status.SLASH, 1), new State(Status.STAR, 1),
                new State(Status.PLUS, 1), new State(Status.MINUS, 1), new State(Status.PERCENT, 1),
                new State(Status.NOT, 1), new State(Status.EQUAL, 1), new State(Status.OPEN_ROUND_BRACKET, 1),
                new State(Status.CLOSE_ROUND_BRACKET, 1), new State(Status.OPEN_CURLY_BRACKET, 1),
                new State(Status.CLOSE_CURLY_BRACKET, 1), new State(Status.COMMA, 1), new State(Status.SEMICOLON, 1),
                new State(Status.AND, 1), new State(Status.OR, 1), new State(Status.MORE, 1), new State(Status.LESS, 1),
                new State(Status.ERROR, 0)
            },
            new[]
            {
                new State(Status.START, 1), new State(Status.START, 1), new State(Status.IDENT, 1),
                new State(Status.REALNUMBER_ONE, 0), new State(Status.STRING, 1), new State(Status.ERROR, 0),
                new State(Status.REALNUMBER_DOT, 0), new State(Status.SLASH, 1), new State(Status.STAR, 1),
                new State(Status.PLUS, 1), new State(Status.MINUS, 1), new State(Status.PERCENT, 1),
                new State(Status.NOT, 1), new State(Status.EQUAL, 1), new State(Status.OPEN_ROUND_BRACKET, 1),
                new State(Status.CLOSE_ROUND_BRACKET, 1), new State(Status.OPEN_CURLY_BRACKET, 1),
                new State(Status.CLOSE_CURLY_BRACKET, 1), new State(Status.COMMA, 1), new State(Status.SEMICOLON, 1),
                new State(Status.AND, 1), new State(Status.OR, 1), new State(Status.MORE, 1), new State(Status.LESS, 1),
                new State(Status.ERROR, 0)
            },
            new[]
            {
                new State(Status.ERROR, 0), new State(Status.ERROR, 0), new State(Status.ERROR, 0),
                new State(Status.REALNUMBER_ONE, 0), new State(Status.ERROR, 0), new State(Status.ERROR, 0),
                new State(Status.ERROR, 0), new State(Status.ERROR, 0), new State(Status.ERROR, 0),
                new State(Status.ERROR, 0), new State(Status.ERROR, 0), new State(Status.ERROR, 0),
                new State(Status.ERROR, 0), new State(Status.ERROR, 0), new State(Status.ERROR, 0),
                new State(Status.ERROR, 0), new State(Status.ERROR, 0), new State(Status.ERROR, 0),
                new State(Status.ERROR, 0), new State(Status.ERROR, 0), new State(Status.ERROR, 0),
                new State(Status.ERROR, 0), new State(Status.ERROR, 0), new State(Status.ERROR, 0),
                new State(Status.ERROR, 0)
            },
            new[]
            {
                new State(Status.ERROR, 0), new State(Status.STRING, 0), new State(Status.STRING, 0),
                new State(Status.STRING, 0), new State(Status.STRING_END, 0), new State(Status.STRING_SLASH, 0),
                new State(Status.STRING, 0), new State(Status.STRING, 0), new State(Status.STRING, 0),
                new State(Status.STRING, 0), new State(Status.STRING, 0), new State(Status.STRING, 0),
                new State(Status.STRING, 0), new State(Status.STRING, 0), new State(Status.STRING, 0),
                new State(Status.STRING, 0), new State(Status.STRING, 0), new State(Status.STRING, 0),
                new State(Status.STRING, 0), new State(Status.STRING, 0), new State(Status.STRING, 0),
                new State(Status.STRING, 0), new State(Status.STRING, 0), new State(Status.STRING, 0),
                new State(Status.STRING, 0)
            },
            new[]
            {
                new State(Status.START, 1), new State(Status.START, 1), new State(Status.IDENT, 1),
                new State(Status.NUMBER, 1), new State(Status.STRING, 1), new State(Status.ERROR, 0),
                new State(Status.REALNUMBER_DOT, 1), new State(Status.SLASH, 1), new State(Status.STAR, 1),
                new State(Status.PLUS, 1), new State(Status.MINUS, 1), new State(Status.PERCENT, 1),
                new State(Status.NOT, 1), new State(Status.EQUAL, 1), new State(Status.OPEN_ROUND_BRACKET, 1),
                new State(Status.CLOSE_ROUND_BRACKET, 1), new State(Status.OPEN_CURLY_BRACKET, 1),
                new State(Status.CLOSE_CURLY_BRACKET, 1), new State(Status.COMMA, 1), new State(Status.SEMICOLON, 1),
                new State(Status.AND, 1), new State(Status.OR, 1), new State(Status.MORE, 1), new State(Status.LESS, 1),
                new State(Status.ERROR, 0)
            },
            new[]
            {
                new State(Status.STRING, 0), new State(Status.STRING, 0), new State(Status.STRING, 0),
                new State(Status.STRING, 0), new State(Status.STRING, 0), new State(Status.STRING_SLASH, 0),
                new State(Status.STRING, 0), new State(Status.STRING, 0), new State(Status.STRING, 0),
                new State(Status.STRING, 0), new State(Status.STRING, 0), new State(Status.STRING, 0),
                new State(Status.STRING, 0), new State(Status.STRING, 0), new State(Status.STRING, 0),
                new State(Status.STRING, 0), new State(Status.STRING, 0), new State(Status.STRING, 0),
                new State(Status.STRING, 0), new State(Status.STRING, 0), new State(Status.STRING, 0),
                new State(Status.STRING, 0), new State(Status.STRING, 0), new State(Status.STRING, 0),
                new State(Status.STRING, 0)
            },
            new[]
            {
                new State(Status.START, 1), new State(Status.START, 1), new State(Status.IDENT, 1),
                new State(Status.NUMBER, 1), new State(Status.STRING, 1), new State(Status.ERROR, 0),
                new State(Status.REALNUMBER_DOT, 1), new State(Status.ONE_LC, 0), new State(Status.MULTI_LC, 0),
                new State(Status.PLUS, 1), new State(Status.MINUS, 1), new State(Status.PERCENT, 1),
                new State(Status.NOT, 1), new State(Status.SLASHEQUAL, 0), new State(Status.OPEN_ROUND_BRACKET, 1),
                new State(Status.CLOSE_ROUND_BRACKET, 1), new State(Status.OPEN_CURLY_BRACKET, 1),
                new State(Status.CLOSE_CURLY_BRACKET, 1), new State(Status.COMMA, 1), new State(Status.SEMICOLON, 1),
                new State(Status.AND, 1), new State(Status.OR, 1), new State(Status.MORE, 1), new State(Status.LESS, 1),
                new State(Status.ERROR, 0)
            },
            new[]
            {
                new State(Status.START, 1), new State(Status.ONE_LC, 0), new State(Status.ONE_LC, 0),
                new State(Status.ONE_LC, 0), new State(Status.ONE_LC, 0), new State(Status.ONE_LC, 0),
                new State(Status.ONE_LC, 0), new State(Status.ONE_LC, 0), new State(Status.ONE_LC, 0),
                new State(Status.ONE_LC, 0), new State(Status.ONE_LC, 0), new State(Status.ONE_LC, 0),
                new State(Status.ONE_LC, 0), new State(Status.ONE_LC, 0), new State(Status.ONE_LC, 0),
                new State(Status.ONE_LC, 0), new State(Status.ONE_LC, 0), new State(Status.ONE_LC, 0),
                new State(Status.ONE_LC, 0), new State(Status.ONE_LC, 0), new State(Status.ONE_LC, 0),
                new State(Status.ONE_LC, 0), new State(Status.ONE_LC, 0), new State(Status.ONE_LC, 0),
                new State(Status.ONE_LC, 0)
            },
            new[]
            {
                new State(Status.MULTI_LC, 0), new State(Status.MULTI_LC, 0), new State(Status.MULTI_LC, 0),
                new State(Status.MULTI_LC, 0), new State(Status.MULTI_LC, 0), new State(Status.MULTI_LC, 0),
                new State(Status.MULTI_LC, 0), new State(Status.MULTI_LC, 0), new State(Status.MULTI_LC_PRE_END, 0),
                new State(Status.MULTI_LC, 0), new State(Status.MULTI_LC, 0), new State(Status.MULTI_LC, 0),
                new State(Status.MULTI_LC, 0), new State(Status.MULTI_LC, 0), new State(Status.MULTI_LC, 0),
                new State(Status.MULTI_LC, 0), new State(Status.MULTI_LC, 0), new State(Status.MULTI_LC, 0),
                new State(Status.MULTI_LC, 0), new State(Status.MULTI_LC, 0), new State(Status.MULTI_LC, 0),
                new State(Status.MULTI_LC, 0), new State(Status.MULTI_LC, 0), new State(Status.MULTI_LC, 0),
                new State(Status.MULTI_LC, 0)
            },
            new[]
            {
                new State(Status.MULTI_LC, 0), new State(Status.MULTI_LC, 0), new State(Status.MULTI_LC, 0),
                new State(Status.MULTI_LC, 0), new State(Status.MULTI_LC, 0), new State(Status.MULTI_LC, 0),
                new State(Status.MULTI_LC, 0), new State(Status.MULTI_LC_END, 0), new State(Status.MULTI_LC, 0),
                new State(Status.MULTI_LC, 0), new State(Status.MULTI_LC, 0), new State(Status.MULTI_LC, 0),
                new State(Status.MULTI_LC, 0), new State(Status.MULTI_LC, 0), new State(Status.MULTI_LC, 0),
                new State(Status.MULTI_LC, 0), new State(Status.MULTI_LC, 0), new State(Status.MULTI_LC, 0),
                new State(Status.MULTI_LC, 0), new State(Status.MULTI_LC, 0), new State(Status.MULTI_LC, 0),
                new State(Status.MULTI_LC, 0), new State(Status.MULTI_LC, 0), new State(Status.MULTI_LC, 0),
                new State(Status.MULTI_LC, 0)
            },
            new[]
            {
                new State(Status.START, 1), new State(Status.START, 1), new State(Status.IDENT, 1),
                new State(Status.NUMBER, 1), new State(Status.STRING, 1), new State(Status.ERROR, 0),
                new State(Status.REALNUMBER_DOT, 1), new State(Status.SLASH, 1), new State(Status.STAR, 1),
                new State(Status.PLUS, 1), new State(Status.MINUS, 1), new State(Status.PERCENT, 1),
                new State(Status.NOT, 1), new State(Status.EQUAL, 1), new State(Status.OPEN_ROUND_BRACKET, 1),
                new State(Status.CLOSE_ROUND_BRACKET, 1), new State(Status.OPEN_CURLY_BRACKET, 1),
                new State(Status.CLOSE_CURLY_BRACKET, 1), new State(Status.COMMA, 1), new State(Status.SEMICOLON, 1),
                new State(Status.AND, 1), new State(Status.OR, 1), new State(Status.MORE, 1), new State(Status.LESS, 1),
                new State(Status.ERROR, 0)
            },
            new[]
            {
                new State(Status.START, 1), new State(Status.START, 1), new State(Status.IDENT, 1),
                new State(Status.NUMBER, 1), new State(Status.STRING, 1), new State(Status.ERROR, 0),
                new State(Status.REALNUMBER_DOT, 1), new State(Status.SLASH, 1), new State(Status.STAR, 1),
                new State(Status.PLUS, 1), new State(Status.MINUS, 1), new State(Status.PERCENT, 1),
                new State(Status.NOT, 1), new State(Status.STAREQUAL, 0), new State(Status.OPEN_ROUND_BRACKET, 1),
                new State(Status.CLOSE_ROUND_BRACKET, 1), new State(Status.OPEN_CURLY_BRACKET, 1),
                new State(Status.CLOSE_CURLY_BRACKET, 1), new State(Status.COMMA, 1), new State(Status.SEMICOLON, 1),
                new State(Status.AND, 1), new State(Status.OR, 1), new State(Status.MORE, 1), new State(Status.LESS, 1),
                new State(Status.ERROR, 0)
            },
            new[]
            {
                new State(Status.START, 1), new State(Status.START, 1), new State(Status.IDENT, 1),
                new State(Status.NUMBER, 1), new State(Status.STRING, 1), new State(Status.ERROR, 0),
                new State(Status.REALNUMBER_DOT, 1), new State(Status.SLASH, 1), new State(Status.STAR, 1),
                new State(Status.PLUSPLUS, 0), new State(Status.MINUS, 1), new State(Status.PERCENT, 1),
                new State(Status.NOT, 1), new State(Status.PLUSEQUAL, 0), new State(Status.OPEN_ROUND_BRACKET, 1),
                new State(Status.CLOSE_ROUND_BRACKET, 1), new State(Status.OPEN_CURLY_BRACKET, 1),
                new State(Status.CLOSE_CURLY_BRACKET, 1), new State(Status.COMMA, 1), new State(Status.SEMICOLON, 1),
                new State(Status.AND, 1), new State(Status.OR, 1), new State(Status.MORE, 1), new State(Status.LESS, 1),
                new State(Status.ERROR, 0)
            },
            new[]
            {
                new State(Status.START, 1), new State(Status.START, 1), new State(Status.IDENT, 1),
                new State(Status.NUMBER, 1), new State(Status.STRING, 1), new State(Status.ERROR, 0),
                new State(Status.REALNUMBER_DOT, 1), new State(Status.SLASH, 1), new State(Status.STAR, 1),
                new State(Status.PLUS, 1), new State(Status.MINUS, 1), new State(Status.PERCENT, 1),
                new State(Status.NOT, 1), new State(Status.EQUAL, 1), new State(Status.OPEN_ROUND_BRACKET, 1),
                new State(Status.CLOSE_ROUND_BRACKET, 1), new State(Status.OPEN_CURLY_BRACKET, 1),
                new State(Status.CLOSE_CURLY_BRACKET, 1), new State(Status.COMMA, 1), new State(Status.SEMICOLON, 1),
                new State(Status.AND, 1), new State(Status.OR, 1), new State(Status.MORE, 1), new State(Status.LESS, 1),
                new State(Status.ERROR, 0)
            },
            new[]
            {
                new State(Status.START, 1), new State(Status.START, 1), new State(Status.IDENT, 1),
                new State(Status.NUMBER, 1), new State(Status.STRING, 1), new State(Status.ERROR, 0),
                new State(Status.REALNUMBER_DOT, 1), new State(Status.SLASH, 1), new State(Status.STAR, 1),
                new State(Status.PLUS, 1), new State(Status.MINUSMINUS, 0), new State(Status.PERCENT, 1),
                new State(Status.NOT, 1), new State(Status.MINUSEQUAL, 0), new State(Status.OPEN_ROUND_BRACKET, 1),
                new State(Status.CLOSE_ROUND_BRACKET, 1), new State(Status.OPEN_CURLY_BRACKET, 1),
                new State(Status.CLOSE_CURLY_BRACKET, 1), new State(Status.COMMA, 1), new State(Status.SEMICOLON, 1),
                new State(Status.AND, 1), new State(Status.OR, 1), new State(Status.MORE, 1), new State(Status.LESS, 1),
                new State(Status.ERROR, 0)
            },
            new[]
            {
                new State(Status.START, 1), new State(Status.START, 1), new State(Status.IDENT, 1),
                new State(Status.NUMBER, 1), new State(Status.STRING, 1), new State(Status.ERROR, 0),
                new State(Status.REALNUMBER_DOT, 1), new State(Status.SLASH, 1), new State(Status.STAR, 1),
                new State(Status.PLUS, 1), new State(Status.MINUS, 1), new State(Status.PERCENT, 1),
                new State(Status.NOT, 1), new State(Status.EQUAL, 1), new State(Status.OPEN_ROUND_BRACKET, 1),
                new State(Status.CLOSE_ROUND_BRACKET, 1), new State(Status.OPEN_CURLY_BRACKET, 1),
                new State(Status.CLOSE_CURLY_BRACKET, 1), new State(Status.COMMA, 1), new State(Status.SEMICOLON, 1),
                new State(Status.AND, 1), new State(Status.OR, 1), new State(Status.MORE, 1), new State(Status.LESS, 1),
                new State(Status.ERROR, 0)
            },
            new[]
            {
                new State(Status.START, 1), new State(Status.START, 1), new State(Status.IDENT, 1),
                new State(Status.NUMBER, 1), new State(Status.STRING, 1), new State(Status.ERROR, 0),
                new State(Status.REALNUMBER_DOT, 1), new State(Status.SLASH, 1), new State(Status.STAR, 1),
                new State(Status.PLUS, 1), new State(Status.MINUS, 1), new State(Status.PERCENT, 1),
                new State(Status.NOT, 1), new State(Status.PERCENTEQUAL, 0), new State(Status.OPEN_ROUND_BRACKET, 1),
                new State(Status.CLOSE_ROUND_BRACKET, 1), new State(Status.OPEN_CURLY_BRACKET, 1),
                new State(Status.CLOSE_CURLY_BRACKET, 1), new State(Status.COMMA, 1), new State(Status.SEMICOLON, 1),
                new State(Status.AND, 1), new State(Status.OR, 1), new State(Status.MORE, 1), new State(Status.LESS, 1),
                new State(Status.ERROR, 0)
            },
            new[]
            {
                new State(Status.START, 1), new State(Status.START, 1), new State(Status.IDENT, 1),
                new State(Status.NUMBER, 1), new State(Status.STRING, 1), new State(Status.ERROR, 0),
                new State(Status.REALNUMBER_DOT, 1), new State(Status.SLASH, 1), new State(Status.STAR, 1),
                new State(Status.PLUS, 1), new State(Status.MINUS, 1), new State(Status.PERCENT, 1),
                new State(Status.NOT, 1), new State(Status.NOTEQUAL, 0), new State(Status.OPEN_ROUND_BRACKET, 1),
                new State(Status.CLOSE_ROUND_BRACKET, 1), new State(Status.OPEN_CURLY_BRACKET, 1),
                new State(Status.CLOSE_CURLY_BRACKET, 1), new State(Status.COMMA, 1), new State(Status.SEMICOLON, 1),
                new State(Status.AND, 1), new State(Status.OR, 1), new State(Status.MORE, 1), new State(Status.LESS, 1),
                new State(Status.ERROR, 0)
            },
            new[]
            {
                new State(Status.START, 1), new State(Status.START, 1), new State(Status.IDENT, 1),
                new State(Status.NUMBER, 1), new State(Status.STRING, 1), new State(Status.ERROR, 0),
                new State(Status.REALNUMBER_DOT, 1), new State(Status.SLASH, 1), new State(Status.STAR, 1),
                new State(Status.PLUS, 1), new State(Status.MINUS, 1), new State(Status.PERCENT, 1),
                new State(Status.NOT, 1), new State(Status.TWO_EQUAL, 0), new State(Status.OPEN_ROUND_BRACKET, 1),
                new State(Status.CLOSE_ROUND_BRACKET, 1), new State(Status.OPEN_CURLY_BRACKET, 1),
                new State(Status.CLOSE_CURLY_BRACKET, 1), new State(Status.COMMA, 1), new State(Status.SEMICOLON, 1),
                new State(Status.AND, 1), new State(Status.OR, 1), new State(Status.MORE, 1), new State(Status.LESS, 1),
                new State(Status.ERROR, 0)
            },
            new[]
            {
                new State(Status.START, 1), new State(Status.START, 1), new State(Status.IDENT, 1),
                new State(Status.NUMBER, 1), new State(Status.STRING, 1), new State(Status.ERROR, 0),
                new State(Status.REALNUMBER_DOT, 1), new State(Status.SLASH, 1), new State(Status.STAR, 1),
                new State(Status.PLUS, 1), new State(Status.MINUS, 1), new State(Status.PERCENT, 1),
                new State(Status.NOT, 1), new State(Status.THREE_EQUAL, 0), new State(Status.OPEN_ROUND_BRACKET, 1),
                new State(Status.CLOSE_ROUND_BRACKET, 1), new State(Status.OPEN_CURLY_BRACKET, 1),
                new State(Status.CLOSE_CURLY_BRACKET, 1), new State(Status.COMMA, 1), new State(Status.SEMICOLON, 1),
                new State(Status.AND, 1), new State(Status.OR, 1), new State(Status.MORE, 1), new State(Status.LESS, 1),
                new State(Status.ERROR, 0)
            },
            new[]
            {
                new State(Status.START, 1), new State(Status.START, 1), new State(Status.IDENT, 1),
                new State(Status.NUMBER, 1), new State(Status.STRING, 1), new State(Status.ERROR, 0),
                new State(Status.REALNUMBER_DOT, 1), new State(Status.SLASH, 1), new State(Status.STAR, 1),
                new State(Status.PLUS, 1), new State(Status.MINUS, 1), new State(Status.PERCENT, 1),
                new State(Status.NOT, 1), new State(Status.EQUAL, 1), new State(Status.OPEN_ROUND_BRACKET, 1),
                new State(Status.CLOSE_ROUND_BRACKET, 1), new State(Status.OPEN_CURLY_BRACKET, 1),
                new State(Status.CLOSE_CURLY_BRACKET, 1), new State(Status.COMMA, 1), new State(Status.SEMICOLON, 1),
                new State(Status.AND, 1), new State(Status.OR, 1), new State(Status.MORE, 1), new State(Status.LESS, 1),
                new State(Status.ERROR, 0)
            },
            new[]
            {
                new State(Status.START, 1), new State(Status.START, 1), new State(Status.IDENT, 1),
                new State(Status.NUMBER, 1), new State(Status.STRING, 1), new State(Status.ERROR, 0),
                new State(Status.REALNUMBER_DOT, 1), new State(Status.SLASH, 1), new State(Status.STAR, 1),
                new State(Status.PLUS, 1), new State(Status.MINUS, 1), new State(Status.PERCENT, 1),
                new State(Status.NOT, 1), new State(Status.EQUAL, 1), new State(Status.OPEN_ROUND_BRACKET, 1),
                new State(Status.CLOSE_ROUND_BRACKET, 1), new State(Status.OPEN_CURLY_BRACKET, 1),
                new State(Status.CLOSE_CURLY_BRACKET, 1), new State(Status.COMMA, 1), new State(Status.SEMICOLON, 1),
                new State(Status.AND, 1), new State(Status.OR, 1), new State(Status.MORE, 1), new State(Status.LESS, 1),
                new State(Status.ERROR, 0)
            },
            new[]
            {
                new State(Status.START, 1), new State(Status.START, 1), new State(Status.IDENT, 1),
                new State(Status.NUMBER, 1), new State(Status.STRING, 1), new State(Status.ERROR, 0),
                new State(Status.REALNUMBER_DOT, 1), new State(Status.SLASH, 1), new State(Status.STAR, 1),
                new State(Status.PLUS, 1), new State(Status.MINUS, 1), new State(Status.PERCENT, 1),
                new State(Status.NOT, 1), new State(Status.EQUAL, 1), new State(Status.OPEN_ROUND_BRACKET, 1),
                new State(Status.CLOSE_ROUND_BRACKET, 1), new State(Status.OPEN_CURLY_BRACKET, 1),
                new State(Status.CLOSE_CURLY_BRACKET, 1), new State(Status.COMMA, 1), new State(Status.SEMICOLON, 1),
                new State(Status.AND, 1), new State(Status.OR, 1), new State(Status.MORE, 1), new State(Status.LESS, 1),
                new State(Status.ERROR, 0)
            },
            new[]
            {
                new State(Status.START, 1), new State(Status.START, 1), new State(Status.IDENT, 1),
                new State(Status.NUMBER, 1), new State(Status.STRING, 1), new State(Status.ERROR, 0),
                new State(Status.REALNUMBER_DOT, 1), new State(Status.SLASH, 1), new State(Status.STAR, 1),
                new State(Status.PLUS, 1), new State(Status.MINUS, 1), new State(Status.PERCENT, 1),
                new State(Status.NOT, 1), new State(Status.EQUAL, 1), new State(Status.OPEN_ROUND_BRACKET, 1),
                new State(Status.CLOSE_ROUND_BRACKET, 1), new State(Status.OPEN_CURLY_BRACKET, 1),
                new State(Status.CLOSE_CURLY_BRACKET, 1), new State(Status.COMMA, 1), new State(Status.SEMICOLON, 1),
                new State(Status.AND, 1), new State(Status.OR, 1), new State(Status.MORE, 1), new State(Status.LESS, 1),
                new State(Status.ERROR, 0)
            },
            new[]
            {
                new State(Status.START, 1), new State(Status.START, 1), new State(Status.IDENT, 1),
                new State(Status.NUMBER, 1), new State(Status.STRING, 1), new State(Status.ERROR, 0),
                new State(Status.REALNUMBER_DOT, 1), new State(Status.SLASH, 1), new State(Status.STAR, 1),
                new State(Status.PLUS, 1), new State(Status.MINUS, 1), new State(Status.PERCENT, 1),
                new State(Status.NOT, 1), new State(Status.EQUAL, 1), new State(Status.OPEN_ROUND_BRACKET, 1),
                new State(Status.CLOSE_ROUND_BRACKET, 1), new State(Status.OPEN_CURLY_BRACKET, 1),
                new State(Status.CLOSE_CURLY_BRACKET, 1), new State(Status.COMMA, 1), new State(Status.SEMICOLON, 1),
                new State(Status.AND, 1), new State(Status.OR, 1), new State(Status.MORE, 1), new State(Status.LESS, 1),
                new State(Status.ERROR, 0)
            },
            new[]
            {
                new State(Status.START, 1), new State(Status.START, 1), new State(Status.IDENT, 1),
                new State(Status.NUMBER, 1), new State(Status.STRING, 1), new State(Status.ERROR, 0),
                new State(Status.REALNUMBER_DOT, 1), new State(Status.SLASH, 1), new State(Status.STAR, 1),
                new State(Status.PLUS, 1), new State(Status.MINUS, 1), new State(Status.PERCENT, 1),
                new State(Status.NOT, 1), new State(Status.EQUAL, 1), new State(Status.OPEN_ROUND_BRACKET, 1),
                new State(Status.CLOSE_ROUND_BRACKET, 1), new State(Status.OPEN_CURLY_BRACKET, 1),
                new State(Status.CLOSE_CURLY_BRACKET, 1), new State(Status.COMMA, 1), new State(Status.SEMICOLON, 1),
                new State(Status.AND, 1), new State(Status.OR, 1), new State(Status.MORE, 1), new State(Status.LESS, 1),
                new State(Status.ERROR, 0)
            },
            new[]
            {
                new State(Status.START, 1), new State(Status.START, 1), new State(Status.IDENT, 1),
                new State(Status.NUMBER, 1), new State(Status.STRING, 1), new State(Status.ERROR, 0),
                new State(Status.REALNUMBER_DOT, 1), new State(Status.SLASH, 1), new State(Status.STAR, 1),
                new State(Status.PLUS, 1), new State(Status.MINUS, 1), new State(Status.PERCENT, 1),
                new State(Status.NOT, 1), new State(Status.NOT_TWO_EQUAL, 0), new State(Status.OPEN_ROUND_BRACKET, 1),
                new State(Status.CLOSE_ROUND_BRACKET, 1), new State(Status.OPEN_CURLY_BRACKET, 1),
                new State(Status.CLOSE_CURLY_BRACKET, 1), new State(Status.COMMA, 1), new State(Status.SEMICOLON, 1),
                new State(Status.AND, 1), new State(Status.OR, 1), new State(Status.MORE, 1), new State(Status.LESS, 1),
                new State(Status.ERROR, 0)
            },
            new[]
            {
                new State(Status.START, 1), new State(Status.START, 1), new State(Status.IDENT, 1),
                new State(Status.NUMBER, 1), new State(Status.STRING, 1), new State(Status.ERROR, 0),
                new State(Status.REALNUMBER_DOT, 1), new State(Status.SLASH, 1), new State(Status.STAR, 1),
                new State(Status.PLUS, 1), new State(Status.MINUS, 1), new State(Status.PERCENT, 1),
                new State(Status.NOT, 1), new State(Status.EQUAL, 1), new State(Status.OPEN_ROUND_BRACKET, 1),
                new State(Status.CLOSE_ROUND_BRACKET, 1), new State(Status.OPEN_CURLY_BRACKET, 1),
                new State(Status.CLOSE_CURLY_BRACKET, 1), new State(Status.COMMA, 1), new State(Status.SEMICOLON, 1),
                new State(Status.AND, 1), new State(Status.OR, 1), new State(Status.MORE, 1), new State(Status.LESS, 1),
                new State(Status.ERROR, 0)
            },
            new[]
            {
                new State(Status.START, 1), new State(Status.START, 1), new State(Status.IDENT, 1),
                new State(Status.NUMBER, 1), new State(Status.STRING, 1), new State(Status.ERROR, 0),
                new State(Status.REALNUMBER_DOT, 1), new State(Status.SLASH, 1), new State(Status.STAR, 1),
                new State(Status.PLUS, 1), new State(Status.MINUS, 1), new State(Status.PERCENT, 1),
                new State(Status.NOT, 1), new State(Status.EQUAL, 1), new State(Status.OPEN_ROUND_BRACKET, 1),
                new State(Status.CLOSE_ROUND_BRACKET, 1), new State(Status.OPEN_CURLY_BRACKET, 1),
                new State(Status.CLOSE_CURLY_BRACKET, 1), new State(Status.COMMA, 1), new State(Status.SEMICOLON, 1),
                new State(Status.AND, 1), new State(Status.OR, 1), new State(Status.MORE, 1), new State(Status.LESS, 1),
                new State(Status.ERROR, 0)
            },
            new[]
            {
                new State(Status.START, 1), new State(Status.START, 1), new State(Status.IDENT, 1),
                new State(Status.NUMBER, 1), new State(Status.STRING, 1), new State(Status.ERROR, 0),
                new State(Status.REALNUMBER_DOT, 1), new State(Status.SLASH, 1), new State(Status.STAR, 1),
                new State(Status.PLUS, 1), new State(Status.MINUS, 1), new State(Status.PERCENT, 1),
                new State(Status.NOT, 1), new State(Status.EQUAL, 1), new State(Status.OPEN_ROUND_BRACKET, 1),
                new State(Status.CLOSE_ROUND_BRACKET, 1), new State(Status.OPEN_CURLY_BRACKET, 1),
                new State(Status.CLOSE_CURLY_BRACKET, 1), new State(Status.COMMA, 1), new State(Status.SEMICOLON, 1),
                new State(Status.AND, 1), new State(Status.OR, 1), new State(Status.MORE, 1), new State(Status.LESS, 1),
                new State(Status.ERROR, 0)
            },
            new[]
            {
                new State(Status.START, 1), new State(Status.START, 1), new State(Status.IDENT, 1),
                new State(Status.NUMBER, 1), new State(Status.STRING, 1), new State(Status.ERROR, 0),
                new State(Status.REALNUMBER_DOT, 1), new State(Status.SLASH, 1), new State(Status.STAR, 1),
                new State(Status.PLUS, 1), new State(Status.MINUS, 1), new State(Status.PERCENT, 1),
                new State(Status.NOT, 1), new State(Status.EQUAL, 1), new State(Status.OPEN_ROUND_BRACKET, 1),
                new State(Status.CLOSE_ROUND_BRACKET, 1), new State(Status.OPEN_CURLY_BRACKET, 1),
                new State(Status.CLOSE_CURLY_BRACKET, 1), new State(Status.COMMA, 1), new State(Status.SEMICOLON, 1),
                new State(Status.AND, 1), new State(Status.OR, 1), new State(Status.MORE, 1), new State(Status.LESS, 1),
                new State(Status.ERROR, 0)
            },
            new[]
            {
                new State(Status.START, 1), new State(Status.START, 1), new State(Status.IDENT, 1),
                new State(Status.NUMBER, 1), new State(Status.STRING, 1), new State(Status.ERROR, 0),
                new State(Status.REALNUMBER_DOT, 1), new State(Status.SLASH, 1), new State(Status.STAR, 1),
                new State(Status.PLUS, 1), new State(Status.MINUS, 1), new State(Status.PERCENT, 1),
                new State(Status.NOT, 1), new State(Status.EQUAL, 1), new State(Status.OPEN_ROUND_BRACKET, 1),
                new State(Status.CLOSE_ROUND_BRACKET, 1), new State(Status.OPEN_CURLY_BRACKET, 1),
                new State(Status.CLOSE_CURLY_BRACKET, 1), new State(Status.COMMA, 1), new State(Status.SEMICOLON, 1),
                new State(Status.AND, 1), new State(Status.OR, 1), new State(Status.MORE, 1), new State(Status.LESS, 1),
                new State(Status.ERROR, 0)
            },
            new[]
            {
                new State(Status.START, 1), new State(Status.START, 1), new State(Status.IDENT, 1),
                new State(Status.NUMBER, 1), new State(Status.STRING, 1), new State(Status.ERROR, 0),
                new State(Status.REALNUMBER_DOT, 1), new State(Status.SLASH, 1), new State(Status.STAR, 1),
                new State(Status.PLUS, 1), new State(Status.MINUS, 1), new State(Status.PERCENT, 1),
                new State(Status.NOT, 1), new State(Status.EQUAL, 1), new State(Status.OPEN_ROUND_BRACKET, 1),
                new State(Status.CLOSE_ROUND_BRACKET, 1), new State(Status.OPEN_CURLY_BRACKET, 1),
                new State(Status.CLOSE_CURLY_BRACKET, 1), new State(Status.COMMA, 1), new State(Status.SEMICOLON, 1),
                new State(Status.AND, 1), new State(Status.OR, 1), new State(Status.MORE, 1), new State(Status.LESS, 1),
                new State(Status.ERROR, 0)
            },
            new[]
            {
                new State(Status.START, 1), new State(Status.START, 1), new State(Status.IDENT, 1),
                new State(Status.NUMBER, 1), new State(Status.STRING, 1), new State(Status.ERROR, 0),
                new State(Status.REALNUMBER_DOT, 1), new State(Status.SLASH, 1), new State(Status.STAR, 1),
                new State(Status.PLUS, 1), new State(Status.MINUS, 1), new State(Status.PERCENT, 1),
                new State(Status.NOT, 1), new State(Status.EQUAL, 1), new State(Status.OPEN_ROUND_BRACKET, 1),
                new State(Status.CLOSE_ROUND_BRACKET, 1), new State(Status.OPEN_CURLY_BRACKET, 1),
                new State(Status.CLOSE_CURLY_BRACKET, 1), new State(Status.COMMA, 1), new State(Status.SEMICOLON, 1),
                new State(Status.AND, 1), new State(Status.OR, 1), new State(Status.MORE, 1), new State(Status.LESS, 1),
                new State(Status.ERROR, 0)
            },
            new[]
            {
                new State(Status.START, 1), new State(Status.START, 1), new State(Status.IDENT, 1),
                new State(Status.NUMBER, 1), new State(Status.STRING, 1), new State(Status.ERROR, 0),
                new State(Status.REALNUMBER_DOT, 1), new State(Status.SLASH, 1), new State(Status.STAR, 1),
                new State(Status.PLUS, 1), new State(Status.MINUS, 1), new State(Status.PERCENT, 1),
                new State(Status.NOT, 1), new State(Status.ANDEQUAL, 0), new State(Status.OPEN_ROUND_BRACKET, 1),
                new State(Status.CLOSE_ROUND_BRACKET, 1), new State(Status.OPEN_CURLY_BRACKET, 1),
                new State(Status.CLOSE_CURLY_BRACKET, 1), new State(Status.COMMA, 1), new State(Status.SEMICOLON, 1),
                new State(Status.ANDAND, 0), new State(Status.OR, 1), new State(Status.MORE, 1),
                new State(Status.LESS, 1), new State(Status.ERROR, 0)
            },
            new[]
            {
                new State(Status.START, 1), new State(Status.START, 1), new State(Status.IDENT, 1),
                new State(Status.NUMBER, 1), new State(Status.STRING, 1), new State(Status.ERROR, 0),
                new State(Status.REALNUMBER_DOT, 1), new State(Status.SLASH, 1), new State(Status.STAR, 1),
                new State(Status.PLUS, 1), new State(Status.MINUS, 1), new State(Status.PERCENT, 1),
                new State(Status.NOT, 1), new State(Status.EQUAL, 1), new State(Status.OPEN_ROUND_BRACKET, 1),
                new State(Status.CLOSE_ROUND_BRACKET, 1), new State(Status.OPEN_CURLY_BRACKET, 1),
                new State(Status.CLOSE_CURLY_BRACKET, 1), new State(Status.COMMA, 1), new State(Status.SEMICOLON, 1),
                new State(Status.AND, 1), new State(Status.OR, 1), new State(Status.MORE, 1), new State(Status.LESS, 1),
                new State(Status.ERROR, 0)
            },
            new[]
            {
                new State(Status.START, 1), new State(Status.START, 1), new State(Status.IDENT, 1),
                new State(Status.NUMBER, 1), new State(Status.STRING, 1), new State(Status.ERROR, 0),
                new State(Status.REALNUMBER_DOT, 1), new State(Status.SLASH, 1), new State(Status.STAR, 1),
                new State(Status.PLUS, 1), new State(Status.MINUS, 1), new State(Status.PERCENT, 1),
                new State(Status.NOT, 1), new State(Status.EQUAL, 1), new State(Status.OPEN_ROUND_BRACKET, 1),
                new State(Status.CLOSE_ROUND_BRACKET, 1), new State(Status.OPEN_CURLY_BRACKET, 1),
                new State(Status.CLOSE_CURLY_BRACKET, 1), new State(Status.COMMA, 1), new State(Status.SEMICOLON, 1),
                new State(Status.AND, 1), new State(Status.OR, 1), new State(Status.MORE, 1), new State(Status.LESS, 1),
                new State(Status.ERROR, 0)
            },
            new[]
            {
                new State(Status.START, 1), new State(Status.START, 1), new State(Status.IDENT, 1),
                new State(Status.NUMBER, 1), new State(Status.STRING, 1), new State(Status.ERROR, 0),
                new State(Status.REALNUMBER_DOT, 1), new State(Status.SLASH, 1), new State(Status.STAR, 1),
                new State(Status.PLUS, 1), new State(Status.MINUS, 1), new State(Status.PERCENT, 1),
                new State(Status.NOT, 1), new State(Status.OREQUAL, 0), new State(Status.OPEN_ROUND_BRACKET, 1),
                new State(Status.CLOSE_ROUND_BRACKET, 1), new State(Status.OPEN_CURLY_BRACKET, 1),
                new State(Status.CLOSE_CURLY_BRACKET, 1), new State(Status.COMMA, 1), new State(Status.SEMICOLON, 1),
                new State(Status.AND, 1), new State(Status.OROR, 0), new State(Status.MORE, 1),
                new State(Status.LESS, 1), new State(Status.ERROR, 0)
            },
            new[]
            {
                new State(Status.START, 1), new State(Status.START, 1), new State(Status.IDENT, 1),
                new State(Status.NUMBER, 1), new State(Status.STRING, 1), new State(Status.ERROR, 0),
                new State(Status.REALNUMBER_DOT, 1), new State(Status.SLASH, 1), new State(Status.STAR, 1),
                new State(Status.PLUS, 1), new State(Status.MINUS, 1), new State(Status.PERCENT, 1),
                new State(Status.NOT, 1), new State(Status.EQUAL, 1), new State(Status.OPEN_ROUND_BRACKET, 1),
                new State(Status.CLOSE_ROUND_BRACKET, 1), new State(Status.OPEN_CURLY_BRACKET, 1),
                new State(Status.CLOSE_CURLY_BRACKET, 1), new State(Status.COMMA, 1), new State(Status.SEMICOLON, 1),
                new State(Status.AND, 1), new State(Status.OR, 1), new State(Status.MORE, 1), new State(Status.LESS, 1),
                new State(Status.ERROR, 0)
            },
            new[]
            {
                new State(Status.START, 1), new State(Status.START, 1), new State(Status.IDENT, 1),
                new State(Status.NUMBER, 1), new State(Status.STRING, 1), new State(Status.ERROR, 0),
                new State(Status.REALNUMBER_DOT, 1), new State(Status.SLASH, 1), new State(Status.STAR, 1),
                new State(Status.PLUS, 1), new State(Status.MINUS, 1), new State(Status.PERCENT, 1),
                new State(Status.NOT, 1), new State(Status.EQUAL, 1), new State(Status.OPEN_ROUND_BRACKET, 1),
                new State(Status.CLOSE_ROUND_BRACKET, 1), new State(Status.OPEN_CURLY_BRACKET, 1),
                new State(Status.CLOSE_CURLY_BRACKET, 1), new State(Status.COMMA, 1), new State(Status.SEMICOLON, 1),
                new State(Status.AND, 1), new State(Status.OR, 1), new State(Status.MORE, 1), new State(Status.LESS, 1),
                new State(Status.ERROR, 0)
            },
            new[]
            {
                new State(Status.START, 1), new State(Status.START, 1), new State(Status.IDENT, 1),
                new State(Status.NUMBER, 1), new State(Status.STRING, 1), new State(Status.ERROR, 0),
                new State(Status.REALNUMBER_DOT, 1), new State(Status.SLASH, 1), new State(Status.STAR, 1),
                new State(Status.PLUS, 1), new State(Status.MINUS, 1), new State(Status.PERCENT, 1),
                new State(Status.NOT, 1), new State(Status.MOREOREQUAL, 0), new State(Status.OPEN_ROUND_BRACKET, 1),
                new State(Status.CLOSE_ROUND_BRACKET, 1), new State(Status.OPEN_CURLY_BRACKET, 1),
                new State(Status.CLOSE_CURLY_BRACKET, 1), new State(Status.COMMA, 1), new State(Status.SEMICOLON, 1),
                new State(Status.AND, 1), new State(Status.OR, 1), new State(Status.MORE, 1), new State(Status.LESS, 1),
                new State(Status.ERROR, 0)
            },
            new[]
            {
                new State(Status.START, 1), new State(Status.START, 1), new State(Status.IDENT, 1),
                new State(Status.NUMBER, 1), new State(Status.STRING, 1), new State(Status.ERROR, 0),
                new State(Status.REALNUMBER_DOT, 1), new State(Status.SLASH, 1), new State(Status.STAR, 1),
                new State(Status.PLUS, 1), new State(Status.MINUS, 1), new State(Status.PERCENT, 1),
                new State(Status.NOT, 1), new State(Status.EQUAL, 1), new State(Status.OPEN_ROUND_BRACKET, 1),
                new State(Status.CLOSE_ROUND_BRACKET, 1), new State(Status.OPEN_CURLY_BRACKET, 1),
                new State(Status.CLOSE_CURLY_BRACKET, 1), new State(Status.COMMA, 1), new State(Status.SEMICOLON, 1),
                new State(Status.AND, 1), new State(Status.OR, 1), new State(Status.MORE, 1), new State(Status.LESS, 1),
                new State(Status.ERROR, 0)
            },
            new[]
            {
                new State(Status.START, 1), new State(Status.START, 1), new State(Status.IDENT, 1),
                new State(Status.NUMBER, 1), new State(Status.STRING, 1), new State(Status.ERROR, 0),
                new State(Status.REALNUMBER_DOT, 1), new State(Status.SLASH, 1), new State(Status.STAR, 1),
                new State(Status.PLUS, 1), new State(Status.MINUS, 1), new State(Status.PERCENT, 1),
                new State(Status.NOT, 1), new State(Status.LESSOREQUAL, 0), new State(Status.OPEN_ROUND_BRACKET, 1),
                new State(Status.CLOSE_ROUND_BRACKET, 1), new State(Status.OPEN_CURLY_BRACKET, 1),
                new State(Status.CLOSE_CURLY_BRACKET, 1), new State(Status.COMMA, 1), new State(Status.SEMICOLON, 1),
                new State(Status.AND, 1), new State(Status.OR, 1), new State(Status.MORE, 1), new State(Status.LESS, 1),
                new State(Status.ERROR, 0)
            },
            new[]
            {
                new State(Status.START, 1), new State(Status.START, 1), new State(Status.IDENT, 1),
                new State(Status.NUMBER, 1), new State(Status.STRING, 1), new State(Status.ERROR, 0),
                new State(Status.REALNUMBER_DOT, 1), new State(Status.SLASH, 1), new State(Status.STAR, 1),
                new State(Status.PLUS, 1), new State(Status.MINUS, 1), new State(Status.PERCENT, 1),
                new State(Status.NOT, 1), new State(Status.EQUAL, 1), new State(Status.OPEN_ROUND_BRACKET, 1),
                new State(Status.CLOSE_ROUND_BRACKET, 1), new State(Status.OPEN_CURLY_BRACKET, 1),
                new State(Status.CLOSE_CURLY_BRACKET, 1), new State(Status.COMMA, 1), new State(Status.SEMICOLON, 1),
                new State(Status.AND, 1), new State(Status.OR, 1), new State(Status.MORE, 1), new State(Status.LESS, 1),
                new State(Status.ERROR, 0)
            }
        };

        private readonly string[] _regexes =
        {
            "\\n",
            "\\s",
            "[a-zA-Z]",
            "[0-9]",
            "\"",
            "\\\\",
            "\\.",
            "\\/",
            "\\*",
            "\\+",
            "\\-",
            "\\%",
            "\\!",
            "\\=",
            "\\(",
            "\\)",
            "\\{",
            "\\}",
            "\\,",
            "\\;",
            "\\&",
            "\\|",
            "\\>",
            "\\<",
            "."
        };

        private readonly SortedSet<string> _keywords = new SortedSet<string>
        {
            "var",
            "let",
            "for",
            "while",
            "if",
            "in",
            "else",
            "func",
        };

        private readonly SortedSet<string> _reservedNames = new SortedSet<string>
        {
            "string",
            "substring",
            "character",
            "bool",
            "array",
            "int",
            "double",
            "float",
            "true",
            "false",
            "nil"
        };

        private readonly SortedSet<int> _skip = new SortedSet<int>
        {
            (int) Status.MULTI_LC_END,
            (int) Status.ONE_LC
        };

        int _status = (int) Status.START;
        int _oldStatus = (int) Status.START;
        int _action;
        string _str = "";

        public List<Token> Parse(string filename)
        {
            string dirName = AppDomain.CurrentDomain.BaseDirectory; // Starting Dir
            FileInfo fileInfo = new FileInfo(dirName);
            DirectoryInfo parentDir = fileInfo.Directory.Parent;
            string parentDirName = parentDir.FullName;
            parentDirName = parentDirName.Remove(parentDirName.Length - 9, 9);
            parentDirName = parentDirName + filename;

            StreamReader sr = new StreamReader(parentDirName);

            List<Token> tokens = new List<Token>();


            int line = 0;

            while (!sr.EndOfStream)
            {
                line++;
                string? tmp = sr.ReadLine();

                tmp += '\n';

                for (int i = 0, length = tmp.Length; i < length; i++)
                {
                    string str = tmp[i] + "";
                    CheckSymbol(ref tokens, str, line, i);
                }
            }

            sr.Close();

            return tokens;
        }

        int GetIndex(string s)
        {
            for (int i = 0; i < b; i++)
            {
                Regex regex = new Regex(_regexes[i]);

                MatchCollection matches = regex.Matches(s);
                if (matches.Count > 0)
                {
                    return i;
                }
            }

            return -1;
        }

        void CheckSymbol(ref List<Token> tokens, string s, int line, int position)
        {
            int index = GetIndex(s);

            if (index == -1)
            {
                int size = position - _str.Length + 1;
                Console.WriteLine($"[status: ERROR] [{line}:{size}] >> {s}\n");
                Environment.Exit(0);
            }

            _oldStatus = _status;
            _action = Table[_status][index].GetAction();
            _status = (int) Table[_status][index].GetStatus();

            if (_status == (int) Status.ERROR)
            {
                int size = position - _str.Length + 1;
                Console.WriteLine($"[status: ERROR] [{line}:{size}] >> {s}\n");
                Environment.Exit(0);
            }

            if (_action == 0)
            {
                _str += s;
            }
            else
            {
                if (_status == (int) Status.START)
                {
                    if (_oldStatus != (int) Status.START)
                    {
                        CheckWord(ref tokens, _str, line, position);
                    }

                    _str = "";
                }
                else
                {
                    CheckWord(ref tokens, _str, line, position);
                    _str = s;
                }
            }
        }

        void CheckWord(ref List<Token> tokens, string str, int line, int position)
        {
            if (_skip.Contains(_oldStatus))
            {
                return;
            }

            int tmp = position - str.Length + 1;
            position = (tmp < 1 ? 1 : tmp);

            if (_keywords.Contains(str))
            {
                Token tok = new Token()
                {
                    status = "KEYWORD",
                    value = str,
                    line = line,
                    position = position
                };

                tokens.Add(tok);
                return;
            }

            if (_reservedNames.Contains(str))
            {
                Token tok = new Token()
                {
                    status = "RESERVED_NAME",
                    value = str,
                    line = line,
                    position = position
                };

                tokens.Add(tok);
                return;
            }

            Token tok1 = new Token()
            {
                status = _statusNames[_oldStatus],
                value = str,
                line = line,
                position = position
            };

            tokens.Add(tok1);
        }
    }
}