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
        D[0].Add(startState);

        for (int ind = 0; ind <= tokenList.Count; ind++)
        {
            bool changed = false || ind == 0;
            Scan(ref D, ind, tokenList, ref changed);
            while (changed)
            {
                bool comCh = false;
                bool predCh = false;
                Complete(ref D, ind, ref comCh);
                Predict(ref D, ind, ref predCh);
                changed = comCh || predCh;
            }

            // Console.WriteLine($"D[{ind}]");
            // foreach (var state in D[ind])
            // {
            //     Console.Write($"{state.GetRule().getLeftPart()} -> ");
            //     for (int i = 0; i < state.GetRule().getRightPart().Length; i++)
            //     {
            //         if (i == state.GetMeta())
            //             Console.Write("*");
            //         if (state.GetRule().getType() == ruleType.ns)
            //             Console.Write($"{Grammar.GetSigma()[state.GetRule().getRightPart()[i]]} ");
            //         else
            //         {
            //             if (Grammar.GetSigma().ContainsKey(state.GetRule().getRightPart()[i]))
            //                 Console.Write($"{Grammar.GetSigma()[state.GetRule().getRightPart()[i]]}");
            //             else Console.Write($"{state.GetRule().getRightPart()[i]} ");
            //         }
            //     }
            //
            //     if (state.GetMeta() == state.GetRule().getRightPart().Length)
            //         Console.Write("*");
            //     Console.WriteLine($", meta: {state.GetMeta()}, ind: {state.GetInd()}");
            // }
        }

        string res = "";
        if (tokenList.Count > 0) startState.SetMeta(1);
        if (D[tokenList.Count].Contains(startState))
        {
            Console.Write("\nProgram is ok\n");

            string right = "";
            Right(D, startState, tokenList.Count, ref res);

            for (var ind = 0; ind < Grammar.GetRules().Length; ind++)
            {
                Console.Write($"{ind}: {Grammar.GetRules()[ind].getLeftPart()} -> ");
                for (int i = 0; i < Grammar.GetRules()[ind].getRightPart().Length; i++)
                {
                    if (Grammar.GetRules()[ind].getType() == ruleType.ns)
                        Console.Write($"{Grammar.GetSigma()[Grammar.GetRules()[ind].getRightPart()[i]]} ");
                    else
                    {
                        if (Grammar.GetSigma().ContainsKey(Grammar.GetRules()[ind].getRightPart()[i]))
                            Console.Write($"{Grammar.GetSigma()[Grammar.GetRules()[ind].getRightPart()[i]]}");
                        else Console.Write($"{Grammar.GetRules()[ind].getRightPart()[i]} ");
                    }
                }

                Console.Write("\n");
            }

            // foreach (var d in D)
            // {
            //     foreach (var state in d)
            //     {
            //         res = res + "<" + ntDic[state.GetRule().getLeftPart()] + "> -> ";
            //         foreach (var rightPart in state.GetRule().getRightPart())
            //         {
            //             if (state.GetRule().getType() == ruleType.nn) res = res + "<" + ntDic[rightPart] + "> ";
            //             else if (state.GetRule().getType() == ruleType.ns) res = res + Grammar.GetSigma()[rightPart];
            //             else
            //             {
            //                 if (Grammar.GetSigma().ContainsKey(rightPart)) res = res + Grammar.GetSigma()[rightPart];
            //                 else res = res + "<" + ntDic[rightPart] + "> ";
            //             }
            //         }
            //
            //         res += "\n";
            //     }
            // }
        }
        else
        {
            Console.Write("\nParser - Program contains mistakes\n");
            res = "";
            // foreach (var d in D)
            // {
            //     foreach (var state in d)
            //     {
            //         res = res + "<" + ntDic[state.GetRule().getLeftPart()] + "> -> ";
            //         foreach (var rightPart in state.GetRule().getRightPart())
            //         {
            //             if (state.GetRule().getType() == ruleType.nn) res = res + "<" + ntDic[rightPart] + "> ";
            //             else if (state.GetRule().getType() == ruleType.ns) res = res + Grammar.GetSigma()[rightPart];
            //             else
            //             {
            //                 if (Grammar.GetSigma().ContainsKey(rightPart)) res = res + Grammar.GetSigma()[rightPart];
            //                 else res = res + "<" + ntDic[rightPart] + "> ";
            //             }
            //         }
            //
            //         res += "\n";
            //     }
            // }
        }

        return res;
    }

    int Scan(ref List<state>[] D, int j, List<Token> tokens, ref bool changed)
    {
        if (j != 0)
        {
            foreach (var state in D[j - 1])
            {
                bool includes = false;

                if (tokens[j - 1].status == "IDENT" || tokens[j - 1].status == "NUMBER" ||
                    tokens[j - 1].status == "STRING_END")
                {
                    if ((state.GetRule().getType() == ruleType.mix || state.GetRule().getType() == ruleType.ns) &&
                        state.GetMeta() < state.GetRule().getRightPart().Length &&
                        Grammar.GetSigma().ContainsKey(state.GetRule().getRightPart()[state.GetMeta()]) &&
                        (Grammar.GetSigma()[state.GetRule().getRightPart()[state.GetMeta()]] == tokens[j - 1].status))
                    {
                        D[j].Add(new state(
                            new Rule(state.GetRule().getLeftPart(), state.GetRule().getRightPart(),
                                state.GetRule().getType()),
                            state.GetInd(),
                            state.GetMeta() + 1
                        ));
                        changed = true;
                    }
                }
                else if (tokens[j - 1].status == "RESERVED_NAME")
                {
                    if ((state.GetRule().getType() == ruleType.mix || state.GetRule().getType() == ruleType.ns) &&
                        state.GetMeta() < state.GetRule().getRightPart().Length &&
                        Grammar.GetTypes().Contains(tokens[j - 1].value))
                    {
                        D[j].Add(new state(
                            new Rule(state.GetRule().getLeftPart(), state.GetRule().getRightPart(),
                                state.GetRule().getType()),
                            state.GetInd(),
                            state.GetMeta() + 1
                        ));
                        changed = true;
                    }
                }
                else
                {
                    if ((state.GetRule().getType() == ruleType.mix || state.GetRule().getType() == ruleType.ns) &&
                        state.GetMeta() < state.GetRule().getRightPart().Length &&
                        Grammar.GetSigma().ContainsKey(state.GetRule().getRightPart()[state.GetMeta()]) &&
                        (Grammar.GetSigma()[state.GetRule().getRightPart()[state.GetMeta()]] == tokens[j - 1].value))
                    {
                        D[j].Add(new state(
                            new Rule(state.GetRule().getLeftPart(), state.GetRule().getRightPart(),
                                state.GetRule().getType()),
                            state.GetInd(),
                            state.GetMeta() + 1
                        ));
                        changed = true;
                    }
                }
            }
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
                foreach (var stateK in D[i])
                {
                    if ((stateK.GetRule().getType() == ruleType.mix || stateK.GetRule().getType() == ruleType.nn) &&
                        stateK.GetMeta() < stateK.GetRule().getRightPart().Length &&
                        stateK.GetRule().getRightPart()[stateK.GetMeta()] == stateJ.GetRule().getLeftPart() &&
                        !D[j].Contains(new state(
                            new Rule(stateK.GetRule().getLeftPart(), stateK.GetRule().getRightPart(),
                                stateK.GetRule().getType()),
                            stateK.GetInd(),
                            stateK.GetMeta() + 1
                        )))
                    {
                        D[j].Add(new state(
                            new Rule(stateK.GetRule().getLeftPart(), stateK.GetRule().getRightPart(),
                                stateK.GetRule().getType()),
                            stateK.GetInd(),
                            stateK.GetMeta() + 1
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
                (state.GetRule().getType() != ruleType.mix ||
                 !Grammar.GetSigma().ContainsKey(state.GetRule().getRightPart()[state.GetMeta()])))
            {
                foreach (var rule in Grammar.GetRules())
                {
                    if (rule.getLeftPart() == state.GetRule().getRightPart()[state.GetMeta()] &&
                        !D[j].Contains(new state(rule, j)))
                    {
                        D[j].Add(new state(rule, j));
                        localCh = true;
                    }
                }
            }
        }

        changed = localCh;
    }

    string Right(List<state>[] D, state state, int counter, ref string res)
    {
        var rules = Grammar.GetRules();
        for (var i = 0; i < rules.Length; i++)
        {
            if (rules[i].getLeftPart() == state.GetRule().getLeftPart() &&
                rules[i].getRightPart() == state.GetRule().getRightPart() &&
                rules[i].getType() == state.GetRule().getType())
                res = res + ", " + i.ToString();
        }

        var k = state.GetRule().getRightPart().Length;
        var c = counter;
        if (k > 0)
        {
            if (Grammar.GetSigma().ContainsKey(state.GetRule().getRightPart()[k - 1]))
            {
                k--;
                c--;
            }
            else
            {
                foreach (var situation in D[c])
                {
                    if (k > 0 && situation.GetRule().getLeftPart() == state.GetRule().getRightPart()[k - 1] &&
                        situation.GetMeta() == situation.GetRule().getRightPart().Length)
                    {
                        foreach (var innerSit in D[situation.GetInd()])
                        {
                            if (innerSit.GetMeta() < innerSit.GetRule().getRightPart().Length &&
                                innerSit.GetRule().getRightPart()[innerSit.GetMeta()] ==
                                situation.GetRule().getLeftPart())
                            {
                                Right(D, situation, c, ref res);
                                k--;
                                c = situation.GetInd();
                            }
                        }
                    }
                }
            }
        }

        return res;
    }
}