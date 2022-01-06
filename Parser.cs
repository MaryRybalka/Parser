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
    private Dictionary<nu, string> ntDic;
    private Grammar Grammar;
    private Dictionary<string, nu> TokensDictionary;

    public Parser()
    {
        ntDic = new Dictionary<nu, string>
        {
            {nu.Helper, "Helper"},
            {nu.Program, "Program"},
            {nu.Sentence, "Sentence"},
            {nu.Expression, "Expression"},
            {nu.Definition, "Definition"},
            {nu.Cycle, "Cycle"},
            {nu.Sentences, "Sentences"},
            {nu.CodeBlock, "CodeBlock"},
            {nu.BinaryOperator, "BinaryOperator"},
            {nu.UnaryOperator, "UnaryOperator"},
            {nu.Operand, "Operand"},
            {nu.Identifier, "Identifier"},
            {nu.Literal, "Literal"},
            {nu.FunctionCall, "FunctionCall"},
            {nu.ArgumentsList, "ArgumentsList"},
            {nu.Argument, "Argument"},
            {nu.Expressions, "Expressions"},
            {nu.TypeAnnotation, "TypeAnnotation"},
            {nu.Definitions, "Definitions"},
            {nu.InitialisationListPattern, "InitialisationListPattern"},
            {nu.PatternInitialisator, "PatternInitialisator"},
            {nu.Pattern, "Pattern"},
            {nu.Initialisator, "Initialisator"},
            {nu.Condition, "Condition"},
            {nu.IfBranching, "IfBranching"},
            {nu.ElseBlock, "ElseBlock"},
            {nu.NumberLiteral, "NumberLiteral"},
            {nu.StringLiteral, "StringLiteral"},
            {nu.BoolLiteral, "BoolLiteral"},
        };

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

        public state(Rule _rule, int _ind, int _meta = 0)
        {
            rule = _rule;
            ind = _ind;
            meta = _meta;
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

    public string Parse(List<Token> tokenList)
    {
        List<state>[] D = new List<state>[tokenList.Capacity + 1];
        for (int i = 0; i < D.Length; i++)
        {
            D[i] = new List<state>();
        }

        state startState = new state(new Rule(nu.Helper, new[] {Grammar.GetAxioma()}, ruleType.nn), 0);
        D[0].Add(new state(new Rule(nu.Helper, new[] {Grammar.GetAxioma()}, ruleType.nn), 0));

        foreach (Token token in tokenList)
        {
            bool changed = false || tokenList.IndexOf(token) == 0;

            if (Scan(ref D, tokenList.IndexOf(token), token, ref changed) < 0)
            {
                Console.WriteLine($"ERROR in {token.line} line: no such name {token.value}\n");
            }

            while (changed)
            {
                Complete(ref D, tokenList.IndexOf(token), ref changed);
                Predict(ref D, tokenList.IndexOf(token), ref changed);
            }
        }

        string res = "";
        if (D[tokenList.Capacity].Contains(startState))
        {
            foreach (var d in D)
            {
                foreach (var state in d)
                {
                    res = res + "<" + ntDic[state.GetRule().getLeftPart()] + "> -> ";
                    foreach (var rightPart in state.GetRule().getRightPart())
                    {
                        if (state.GetRule().getType() == ruleType.nn) res = res + "<" + ntDic[rightPart] + "> ";
                        else if (state.GetRule().getType() == ruleType.ns) res = res + Grammar.GetSigma()[rightPart];
                        else
                        {
                            if (Grammar.GetSigma().ContainsKey(rightPart)) res = res + Grammar.GetSigma()[rightPart];
                            else res = res + "<" + ntDic[rightPart] + "> ";
                        }
                    }

                    res += "\n";
                }
            }
        }
        else
        {
            res = "Program contains mistakes\n";
            foreach (var d in D)
            {
                foreach (var state in d)
                {
                    res = res + "<" + ntDic[state.GetRule().getLeftPart()] + "> -> ";
                    foreach (var rightPart in state.GetRule().getRightPart())
                    {
                        if (state.GetRule().getType() == ruleType.nn) res = res + "<" + ntDic[rightPart] + "> ";
                        else if (state.GetRule().getType() == ruleType.ns) res = res + Grammar.GetSigma()[rightPart];
                        else
                        {
                            if (Grammar.GetSigma().ContainsKey(rightPart)) res = res + Grammar.GetSigma()[rightPart];
                            else res = res + "<" + ntDic[rightPart] + "> ";
                        }
                    }

                    res += "\n";
                }
            }
        }

        return res;
    }

    int Scan(ref List<state>[] D, int j, Token token, ref bool changed)
    {
        if (j != 0)
        {
            bool flag = false;
            foreach (var state in D[j - 1])
            {
                if ((state.GetRule().getType() == ruleType.mix || state.GetRule().getType() == ruleType.ns) &&
                    state.GetMeta() < state.GetRule().getRightPart().Length &&
                    Grammar.GetSigma().ContainsKey(state.GetRule().getRightPart()[state.GetMeta()]) &&
                    Grammar.GetSigma()[state.GetRule().getRightPart()[state.GetMeta()]] == token.value)
                {
                    D[j].Add(new state(
                        new Rule(state.GetRule().getLeftPart(), state.GetRule().getRightPart(),
                            state.GetRule().getType()),
                        state.GetInd(),
                        state.GetMeta() + 1
                    ));
                    changed = true;
                }

                flag = flag ||
                       ((state.GetRule().getType() == ruleType.mix || state.GetRule().getType() == ruleType.ns) &&
                        !(Grammar.GetSigma().ContainsKey(state.GetRule().getRightPart()[state.GetMeta()])));
            }

            return (flag) ? -1 : 0;
        }

        return 0;
    }

    void Complete(ref List<state>[] D, int j, ref bool changed)
    {
        bool localCh = false;
        List<state> DJCopy = new List<state>(D[j]);
        foreach (var stateJ in DJCopy)
        {
            if (stateJ.GetMeta() == stateJ.GetRule().getRightPart().Length)
            {
                int i = stateJ.GetInd();
                foreach (var stateI in D[i])
                {
                    if ((stateI.GetRule().getType() == ruleType.mix || stateI.GetRule().getType() == ruleType.nn) &&
                        stateI.GetMeta() < stateI.GetRule().getRightPart().Length &&
                        stateI.GetRule().getRightPart()[stateI.GetMeta()] == stateJ.GetRule().getLeftPart() &&
                        stateI.GetInd() == j)
                    {
                        D[j].Add(new state(
                            new Rule(stateI.GetRule().getLeftPart(), stateI.GetRule().getRightPart(),
                                stateI.GetRule().getType()),
                            stateI.GetInd(),
                            stateI.GetMeta() + 1
                        ));
                        localCh = true;
                    }
                }
            }
        }

        changed = localCh;
    }

    void Predict(ref List<state>[] D, int j, ref bool changed)
    {
        bool localCh = false;
        List<state> DJCopy = new List<state>(D[j]);
        foreach (var state in DJCopy)
        {
            if ((state.GetRule().getType() == ruleType.mix || state.GetRule().getType() == ruleType.nn) &&
                state.GetMeta() < state.GetRule().getRightPart().Length &&
                !Grammar.GetSigma().ContainsKey(state.GetRule().getRightPart()[state.GetMeta()]))
            {
                foreach (var rule in Grammar.GetRules())
                {
                    if (rule.getLeftPart() == state.GetRule().getRightPart()[state.GetMeta()])
                    {
                        D[j].Add(new state(rule, j));
                        localCh = true;
                    }
                }
            }
        }

        changed = localCh;
    }
}