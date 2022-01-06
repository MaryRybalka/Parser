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
            {"NUMBER", nu.NumberLiteral},
            {"STRING", nu.StringLiteral},

            // {"EQUAL", nu.SigmaEqual},
            // {"SLASH", nu.SigmaSlash},
            // {"STAR", nu.SigmaStar},
            // {"PLUS", nu.SigmaPlus},
            // {"MINUS", nu.SigmaMinus},
            // {"PERCENT", nu.SigmaPercent},
            // {"NOT", nu.SigmaNot},
        };
    }

    private struct state
    {
        private nnRule rule;
        private int ind;

        public state(nnRule _rule, int _ind)
        {
            rule = _rule;
            ind = _ind;
        }
    }

    string Parse(List<Token> tokenList)
    {
        List<state>[] D = new List<state>[tokenList.Capacity];
        D[0].Add(new state(new nnRule(nu.Helper, new[] {Grammar.GetAxioma()}), 0));

        foreach (Token token in tokenList)
        {
        }

        return "ok";
    }
}