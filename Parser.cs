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

    Parser()
    {
        Grammar = new Grammar();
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