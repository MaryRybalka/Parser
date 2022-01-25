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

    public ParseTree MainParseTree;

    // private Rule[] rules;
    private List<state> rules;

    public Parser()
    {
        MainParseTree = new ParseTree();

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
            {nu.Type, "Type"},
            {nu.FuncCodeBlock, "FuncCodeBlock"},
        };

        Grammar = new Grammar();
        // rules = Grammar?.GetRules();
        rules = new List<state>();
        foreach (var rule in Grammar.GetRules())
        {
            Rule newRule = new Rule(rule.getLeftPart(), rule.getRightPart(), rule.getType());
            rules.Add(new state(newRule, 0));
        }

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

        public void SetRule(Rule _rule)
        {
            rule = _rule;
        }
    }

    public string Parse(List<Token> tokenList)
    {
        List<state>[] D = new List<state>[tokenList.Count + 1];
        for (int i = 0; i < D.Length; i++)
        {
            D[i] = new List<state>();
        }

        state startState = new state(new Rule(nu.Helper, new[] {Grammar.GetAxioma()}, ruleType.nn), 0);
        D[0].Add(startState);

        var counterOfD = 0;
        var lineNum = 0;

        // List<>

        for (int ind = 0; ind <= tokenList.Count; ind++)
        {
            bool changed = false || ind == 0;
            Scan(ref D, ind, tokenList, ref changed);

            if (changed)
            {
                counterOfD++;
                if (ind < tokenList.Count) lineNum = tokenList[ind].line;
            }

            while (changed)
            {
                bool comCh = false;
                bool predCh = false;
                Complete(ref D, ind, ref comCh);
                Predict(ref D, ind, ref predCh);
                changed = comCh || predCh;
            }

            // ---------------- D tables printing
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
            string tree = "";
            int level = 0;
            int indTok = 0;
            Right(D, startState, tokenList.Count, tokenList, indTok, ref res, ref tree, level);

            // ---------------------- Write all rules with numbers
            // for (var ind = 0; ind < Grammar.GetRules().Length; ind++)
            // {
            //     Console.Write($"{ind}: {Grammar.GetRules()[ind].getLeftPart()} -> ");
            //     for (int i = 0; i < Grammar.GetRules()[ind].getRightPart().Length; i++)
            //     {
            //         if (Grammar.GetRules()[ind].getType() == ruleType.ns)
            //             Console.Write($"{Grammar.GetSigma()[Grammar.GetRules()[ind].getRightPart()[i]]} ");
            //         else
            //         {
            //             if (Grammar.GetSigma().ContainsKey(Grammar.GetRules()[ind].getRightPart()[i]))
            //                 Console.Write($"{Grammar.GetSigma()[Grammar.GetRules()[ind].getRightPart()[i]]}");
            //             else Console.Write($"{Grammar.GetRules()[ind].getRightPart()[i]} ");
            //         }
            //     }
            //
            //     Console.Write("\n");
            // }
        }
        else
        {
            Console.Write("\nParser - Program contains mistakes\n");
            res = "";
            var lowestInd = tokenList.Capacity;
            var lowestIndCounter = 0;
            var stateInd = 0;
            string vars = "[";

            if (!Grammar.GetSigma().ContainsKey(D[counterOfD - 1][0].GetRule().getRightPart()[0]))
            {
                foreach (var state in D[counterOfD - 1])
                {
                    if (state.GetInd() <= lowestInd)
                    {
                        lowestInd = state.GetInd();
                        if (lowestInd == state.GetInd())
                        {
                            var ind = (state.GetMeta() == state.GetRule().getRightPart().Length)
                                ? state.GetMeta() - 1
                                : state.GetMeta();
                            string newWord = Grammar.GetSigma().ContainsKey(state.GetRule().getRightPart()[ind])
                                ? Grammar.GetSigma()[state.GetRule().getRightPart()[ind]]
                                : ntDic[state.GetRule().getRightPart()[ind]];
                            if (!vars.Contains(newWord))
                            {
                                vars += newWord + "] OR [";
                            }

                            lowestIndCounter++;
                        }

                        stateInd = D[counterOfD - 1].IndexOf(state);
                    }
                }
            }
            else
            {
                lowestIndCounter = 1;
                vars = Grammar.GetSigma()[D[counterOfD - 1][0].GetRule().getRightPart()[0]];
            }


            if (lowestIndCounter == 0)
            {
                if (D[counterOfD - 1][stateInd].GetMeta() < D[counterOfD - 1][stateInd].GetRule().getRightPart().Length)
                {
                    res = "ERROR after [";
                    var metaInd = D[counterOfD - 1][stateInd].GetMeta();
                    var rightPartAfterMeta = D[counterOfD - 1][stateInd].GetRule().getRightPart()[metaInd];
                    var rightPartZero = D[counterOfD - 1][stateInd].GetRule().getRightPart()[0];

                    if (D[counterOfD - 1][stateInd].GetRule().getType() == ruleType.ns)
                        res = res + Grammar.GetSigma()[rightPartZero];
                    else
                    {
                        if (Grammar.GetSigma().ContainsKey(rightPartZero))
                            res = res + Grammar.GetSigma()[rightPartZero];
                        else res = res + ntDic[rightPartZero];
                    }

                    if (Grammar.GetSigma().ContainsKey(rightPartAfterMeta))
                    {
                        res += "] in " + lineNum + " line, expected : " + Grammar.GetSigma()[rightPartAfterMeta];
                    }
                    else
                    {
                        var lastInd = D[counterOfD - 1].Count;
                        var el = D[counterOfD - 1][lastInd - 1].GetRule().getRightPart()[0];
                        res += "] in " + lineNum + " line, expected : " + Grammar.GetSigma()[el];
                    }
                }
                else
                {
                    res = "ERROR is not define in " + lineNum + " line";
                }
            }
            else
            {
                if (vars[0] == '[') vars = vars.Substring(0, vars.Length - 5);
                res = "ERROR in " + lineNum + " line." + " Expect: " + vars;
            }
        }

        return (res.Length > 2 && D[tokenList.Count].Contains(startState)) ? res.Substring(2) : res;
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
                        D[j].Add(new state(
                            new Rule(state.GetRule().getRightPart()[state.GetMeta()], new[] {Grammar.nu.IdentIdent},
                                state.GetRule().getType()),
                            (j - 1),
                            0
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

    string Right(List<state>[] D, state state, int counter, List<Token> tokens, int TokInd, ref string res,
        ref string tree, int level,
        int parentInd = -1)
    {
        if (D.Length > 1)
        {
            bool found = false;
            var i = 0;

            while (!found && i < rules.Count)
            {
                if (rules[i].GetRule().getLeftPart() == state.GetRule().getLeftPart() &&
                    rules[i].GetRule().getType() == state.GetRule().getType())
                {
                    var flag = true;
                    // foreach (var rule in rules[i].getRightPart())
                    flag = rules[i].GetRule().getRightPart() == state.GetRule().getRightPart();
                    if (flag)
                    {
                        res = res + ", " + i.ToString();

                        ParseTree.Node newNode = new ParseTree.Node(level - 1, ntDic[rules[i].GetRule().getLeftPart()]);
                        if (MainParseTree.Nodes.Count > 0) newNode.parentName = MainParseTree.Nodes[parentInd].name;
                        newNode.index = i;

                        tree += "{\n(ind = " + i + ")\n  parent: ";
                        tree += (MainParseTree.Nodes.Count > 0)
                            ? MainParseTree.Nodes[parentInd].name
                            : "Helper";
                        tree += ",\n  node: " + ntDic[rules[i].GetRule().getLeftPart()] + ",\n";

                        parentInd++;

                        foreach (var rigthPart in rules[i].GetRule().getRightPart())
                        {
                            // -------------------- Print JSON view of tree
                            if (rules[i].GetRule().getType() == ruleType.nn)
                            {
                                tree += "  child:{\n" + "    level:" + level + ",\n    node: " + ntDic[rigthPart] +
                                        "\n    }\n";
                            }
                            else if (rules[i].GetRule().getType() == ruleType.ns)
                            {
                                var val = "";
                                if ((rigthPart == nu.SigmaString || rigthPart == nu.SigmaNumber ||
                                     rigthPart == nu.SigmaIdent || rigthPart == nu.SigmaType))
                                {
                                    while (Grammar.GetSigma().ContainsValue(tokens[TokInd].value))
                                    {
                                        TokInd++;
                                    }

                                    if (TokInd < tokens.Count) val = tokens[TokInd].value;
                                }

                                tree += "  child:{\n" + "    level:" + level + ",\n    node - " + val + ": " +
                                        Grammar.GetSigma()[rigthPart] + "\n    }\n";
                            }
                            else
                            {
                                if (Grammar.GetSigma().ContainsKey(rigthPart))
                                {
                                    tree += "  child:{\n" + "      level:" + level + ",\n" + "      node: " +
                                            Grammar.GetSigma()[rigthPart] + "\n" + "    }\n";
                                }
                                else
                                    tree += "  child:{\n" + "      level:" + level + ",\n" + "      node: " +
                                            ntDic[rigthPart] + "\n" + "    }\n";
                            }

                            if (Grammar.GetSigma().ContainsKey(rigthPart))
                            {
                                if ((rigthPart == nu.SigmaString || rigthPart == nu.SigmaNumber ||
                                     rigthPart == nu.SigmaIdent || rigthPart == nu.SigmaType))
                                {
                                    if (TokInd < tokens.Count)
                                        newNode.child.Add(new ParseTree.Node(level, tokens[TokInd].value,
                                            ntDic[rules[i].GetRule().getLeftPart()], i + 1, true));
                                }
                                else
                                {
                                    newNode.child.Add(new ParseTree.Node(level, Grammar.GetSigma()[rigthPart],
                                        ntDic[rules[i].GetRule().getLeftPart()], i + 1));
                                }

                                TokInd++;
                            }
                            else
                                newNode.child.Add(new ParseTree.Node(level, ntDic[rigthPart],
                                    ntDic[rules[i].GetRule().getLeftPart()], i + 1));
                        }

                        tree += "}\n";
                        MainParseTree.Nodes.Add(newNode);
                        found = true;
                    }
                }

                i++;
            }

            var k = state.GetRule().getRightPart().Length;
            var c = counter;
            while (k > 0)
            {
                if (Grammar.GetSigma().ContainsKey(state.GetRule().getRightPart()[k - 1]))
                {
                    k--;
                    c--;
                }
                else
                {
                    bool situationIcNotFound = true;
                    int sitIcInd = 0;
                    while (situationIcNotFound && sitIcInd < D[c].Count)
                    {
                        var IcSit = D[c][sitIcInd];
                        if (k > 0 && IcSit.GetRule().getLeftPart() == state.GetRule().getRightPart()[k - 1] &&
                            IcSit.GetMeta() == IcSit.GetRule().getRightPart().Length)
                        {
                            var ind = 0;
                            while ((ind < D[IcSit.GetInd()].Count) && (k > 0))
                            {
                                var innerSit = D[IcSit.GetInd()][ind];
                                if (innerSit.GetMeta() < innerSit.GetRule().getRightPart().Length &&
                                    innerSit.GetRule().getRightPart()[innerSit.GetMeta()] ==
                                    IcSit.GetRule().getLeftPart() &&
                                    state.GetRule().getLeftPart() == innerSit.GetRule().getLeftPart())
                                {
                                    situationIcNotFound = false;

                                    level++;

                                    Right(D, IcSit, c, tokens, TokInd, ref res, ref tree, level, parentInd);
                                    k--;
                                    ind = D[IcSit.GetInd()].Count;
                                    c = IcSit.GetInd();
                                }

                                ind++;
                            }
                        }

                        sitIcInd++;
                    }
                }
            }

            MainParseTree.numOfLevels = level + 1;
        }
        else
        {
            MainParseTree.Nodes.Add(new ParseTree.Node());
            MainParseTree.numOfLevels = 1;
        }

        Console.Write(tree);
        return res;
    }
}