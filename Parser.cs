using System.Dynamic;
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
        private Rule rule;
        private int ind;
        private int meta;

        public state(Rule _rule, int _ind)
        {
            rule = _rule;
            ind = _ind;
            meta = 0;
        }

        public void SetInd(int _ind)
        {
            ind = _ind;
        }

        public void SetMeta(int _meta)
        {
            meta = _meta;
        }

        public int GetMeta()
        {
            return meta;
        }

        public int GetInd()
        {
            return ind;
        }

        public Rule GetRule()
        {
            return rule;
        }
    }

    string Parse(List<Token> tokenList)
    {
        List<state>[] D = new List<state>[tokenList.Capacity];
        D[0].Add(new state(new Rule(nu.Helper, new[] {Grammar.GetAxioma()}, ruleType.nn), 0));

        foreach (Token token in tokenList)
        {
        }

        return "ok";
    }

    void Scan(ref List<state>[] D, int j, Token token)
    {
        if (j != 0)
        {
            foreach (var state in D[j - 1])
            {
                if (state.GetRule().getType() == ruleType.mix &&
                    state.GetMeta() < state.GetRule().getRightPart().Length &&
                    Grammar.GetSigma().ContainsKey(state.GetRule().getRightPart()[state.GetMeta()]) &&
                    Grammar.GetSigma()[state.GetRule().getRightPart()[state.GetMeta()]] == token.value)
                {
                    D[j].Add(new state(
                        new Rule(state.GetRule().getLeftPart(), state.GetRule().getRightPart(), ruleType.mix), 0));
                }
            }
        }
    }

    void Complete(ref List<state> Di, int j)
    {
    }

    void Predict(ref List<state> Di, int j)
    {
    }
}