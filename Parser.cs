using System.Security.Cryptography;
using System.Xml.Xsl;
using Microsoft.VisualBasic;

namespace parser;

using static parser.LexerTypes;
using static parser.Grammar;

enum Precedence : byte
{
    EQUAL = 1,
    OROR = 2,
    ANDAND = 3,
    LESS = 7,
    MORE = 7,
    LESSOREQUAL = 7,
    MOREOREQUAL = 7,
    TWO_EQUAL = 7,
    NOTEQUAL = 7,
    PLUS = 10,
    MINUS = 10,
    STAR = 20,
    SLASH = 20,
    PERCENT = 20,
}

public class Parser
{
    private Grammar Grammar;
    private Dictionary<string, nu> TokensDictionary;

    Parser()
    {
        Grammar = new Grammar();
        TokensDictionary = new Dictionary<string, nu>()
        {
            {"IDENT", nu.Identifier},
            {"EQUAL", nu.SigmaEqual},
            {"NUMBER", nu.NumberLiteral},
            {"REALNUMBER_ONE", nu.SigmaEqual},
            {"REALNUMBER_DOT", nu.SigmaEqual},
            {"STRING", nu.SigmaEqual},
            {"STRING_END", nu.SigmaEqual},
            {"STRING_SLASH", nu.SigmaEqual},
            {"SLASH", nu.SigmaEqual},
            {"ONE_LC", nu.SigmaEqual},
            {"MULTI_LC", nu.SigmaEqual},
            {"MULTI_LC_PRE_END", nu.SigmaEqual},
            {"MULTI_LC_END", nu.SigmaEqual},
            {"STAR", nu.SigmaEqual},
            {"PLUS", nu.SigmaEqual},
            {"PLUSPLUS", nu.SigmaEqual},
            {"MINUS", nu.SigmaEqual},
            {"MINUSMINUS", nu.SigmaEqual},
            {"PERCENT", nu.SigmaEqual},
            {"NOT", nu.SigmaEqual},
            {"TWO_EQUAL", nu.SigmaEqual},
            {"THREE_EQUAL", nu.SigmaEqual},
            {"SLASHEQUAL", nu.SigmaEqual},
            {"STAREQUAL", nu.SigmaEqual},
            {"PLUSEQUAL", nu.SigmaEqual},
            {"MINUSEQUAL", nu.SigmaEqual},
            {"PERCENTEQUAL", nu.SigmaEqual},
            {"NOTEQUAL", nu.SigmaEqual},
            {"NOT_TWO_EQUAL", nu.SigmaEqual},
            {"OPEN_ROUND_BRACKET", nu.SigmaEqual},
            {"CLOSE_ROUND_BRACKET", nu.SigmaEqual},
            {"OPEN_CURLY_BRACKET", nu.SigmaEqual},
            {"CLOSE_CURLY_BRACKET", nu.SigmaEqual},
            {"COMMA", nu.SigmaEqual},
            {"SEMICOLON", nu.SigmaEqual},
            {"AND", nu.SigmaEqual},
            {"ANDEQUAL", nu.SigmaEqual},
            {"ANDAND", nu.SigmaEqual},
            {"OR", nu.SigmaEqual},
            {"OREQUAL", nu.SigmaEqual},
            {"OROR", nu.SigmaEqual},
            {"MORE", nu.SigmaEqual},
            {"MOREOREQUAL", nu.SigmaEqual},
            {"LESS", nu.SigmaEqual},
            {"LESSOREQUAL", nu.SigmaEqual},
            {"ERROR", nu.SigmaEqual},
        };
    }

    string Parse(List<Token> tokenList)
    {
        List<List<string>> D;
        // D.Add(nnRule);
        foreach (Token token in tokenList)
        {
        }

        return "ok";
    }
}