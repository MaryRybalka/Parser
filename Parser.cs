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

    public ParseTree MainParseTree;

    private Rule[] rules;

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
        rules = Grammar?.GetRules();
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

    public void Parse(List<Token> tokenList)
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
            //     if ((state.GetInd() != ind || ind == 0) && state.GetMeta() == state.GetRule().getRightPart().Length)
            //     {
            //         Console.Write($"{state.GetRule().getLeftPart()} -> ");
            //         for (int i = 0; i < state.GetRule().getRightPart().Length; i++)
            //         {
            //             if (i == state.GetMeta())
            //                 Console.Write("*");
            //
            //             if (state.GetRule().getType() == ruleType.ns)
            //             {
            //                 Console.Write($"{Grammar.GetSigma()[state.GetRule().getRightPart()[i]]} ");
            //             }
            //             else
            //             {
            //                 Console.Write(Grammar.GetSigma().ContainsKey(state.GetRule().getRightPart()[i])
            //                     ? $"{Grammar.GetSigma()[state.GetRule().getRightPart()[i]]}"
            //                     : $"{state.GetRule().getRightPart()[i]} ");
            //             }
            //         }
            //
            //         if (state.GetMeta() == state.GetRule().getRightPart().Length)
            //             Console.Write("*");
            //         Console.WriteLine($", meta: {state.GetMeta()}, ind: {state.GetInd()}");
            //     }
            // }
        }

        List<int> res = new List<int>();

        if (tokenList.Count > 0) startState.SetMeta(1);
        if (D[tokenList.Count].Contains(startState))
        {
            Console.WriteLine("\nProgram is ok\n");

            Razb(D, tokenList, res);

            // Right(D, startState, tokenList.Count, tokenList, ref indTok, ref res, ref tree, level);

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
            var lowestInd = tokenList.Capacity;
            var lowestIndCounter = 0;
            var stateInd = 0;
            string vars = "[";
            string error = "";

            if (D[counterOfD - 1][0].GetMeta() < D[counterOfD - 1][0].GetRule().getRightPart().Length && !Grammar
                    .GetSigma()
                    .ContainsKey(D[counterOfD - 1][0].GetRule().getRightPart()[D[counterOfD - 1][0].GetMeta()]))
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
                if (D[counterOfD - 1][0].GetMeta() < D[counterOfD - 1][0].GetRule().getRightPart().Length)
                    vars = Grammar.GetSigma()[
                        D[counterOfD - 1][0].GetRule().getRightPart()[D[counterOfD - 1][0].GetMeta()]];
                else
                    vars = "something else";
            }


            if (lowestIndCounter == 0)
            {
                if (D[counterOfD - 1][stateInd].GetMeta() < D[counterOfD - 1][stateInd].GetRule().getRightPart().Length)
                {
                    error += "ERROR after [";
                    var metaInd = D[counterOfD - 1][stateInd].GetMeta();
                    var rightPartAfterMeta = D[counterOfD - 1][stateInd].GetRule().getRightPart()[metaInd];
                    var rightPartZero = D[counterOfD - 1][stateInd].GetRule().getRightPart()[0];

                    if (D[counterOfD - 1][stateInd].GetRule().getType() == ruleType.ns)
                        error += Grammar.GetSigma()[rightPartZero];
                    else
                    {
                        error += Grammar.GetSigma().ContainsKey(rightPartZero)
                            ? Grammar.GetSigma()[rightPartZero]
                            : ntDic[rightPartZero];
                    }

                    if (Grammar.GetSigma().ContainsKey(rightPartAfterMeta))
                    {
                        error += "] in " + lineNum + " line, expected : " + Grammar.GetSigma()[rightPartAfterMeta];
                    }
                    else
                    {
                        var lastInd = D[counterOfD - 1].Count;
                        var el = D[counterOfD - 1][lastInd - 1].GetRule().getRightPart()[0];
                        error += "] in " + lineNum + " line, expected : " + Grammar.GetSigma()[el];
                    }
                }
                else
                {
                    error += "ERROR is not define in " + lineNum + " line";
                }
            }
            else
            {
                if (vars[0] == '[')
                    vars = vars.Substring(0, vars.Length - 5);
                error += "ERROR in " + lineNum + " line." + " Expect: " + vars;
            }

            Console.WriteLine(error);
        }

        return;
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

    List<int> Razb(List<state>[] D, List<Token> tokens, List<int> res)
    {
        List<int> pi = new List<int>();
        List<state> valuable = new List<state>();
        List<bool> watched = new List<bool>();

        for (var ind = D.Length - 1; ind >= 0; ind--)
        {
            for (var j = D[ind].Count - 1; j >= 0; j--)
            {
                if ((D[ind][j].GetInd() != ind || ind == 0) &&
                    D[ind][j].GetMeta() == D[ind][j].GetRule().getRightPart().Length
                   ) //!valuable.Contains(D[ind][j])
                {
                    state exm = new state();

                    if (valuable.Count == 0 || (
                            valuable[valuable.Count - 1].GetRule().getLeftPart() != D[ind][j].GetRule().getLeftPart() &&
                            valuable[valuable.Count - 1].GetRule().getRightPart() != D[ind][j].GetRule().getRightPart()
                        ))
                    {
                        if ((!valuable.Contains(D[ind][j]) || D[ind][j].GetRule().getLeftPart() != nu.Helper) &&
                            (!valuable.Contains(D[ind][j]) || D[ind][j].GetRule().getLeftPart() != nu.Program))
                        {
                            // if (D[ind][j].GetRule().getLeftPart() != nu.Sentences || ind == D.Length - 1)
                            // {
                            valuable.Add(D[ind][j]);
                            watched.Add(false);
                            // }
                            // else if (D[ind][j].GetRule().getRightPart()
                            //              [D[ind][j].GetRule().getRightPart().Length - 1] ==
                            //          nu.Sentence)
                            // {
                            //     valuable.Add(D[ind][j]);
                            //     watched.Add(false);
                            // }
                        }
                    }
                }
            }
        }

        // foreach (var state in valuable)
        // {
        //     Console.Write($"{state.GetRule().getLeftPart()} -> ");
        //     for (int i = 0; i < state.GetRule().getRightPart().Length; i++)
        //     {
        //         if (i == state.GetMeta())
        //             Console.Write("*");
        //
        //         if (state.GetRule().getType() == ruleType.ns)
        //         {
        //             Console.Write($"{Grammar.GetSigma()[state.GetRule().getRightPart()[i]]} ");
        //         }
        //         else
        //         {
        //             Console.Write(Grammar.GetSigma().ContainsKey(state.GetRule().getRightPart()[i])
        //                 ? $"{Grammar.GetSigma()[state.GetRule().getRightPart()[i]]}"
        //                 : $"{state.GetRule().getRightPart()[i]} ");
        //         }
        //     }
        //
        //     if (state.GetMeta() == state.GetRule().getRightPart().Length)
        //         Console.Write("*");
        //     Console.WriteLine($", meta: {state.GetMeta()}, ind: {state.GetInd()}");
        // }

        ParseTree newTree = new ParseTree();

        int counter = 0;
        if (valuable.Count > 0)
            newTree.Nodes.Add(R(valuable[0], valuable, ref watched, ref counter, tokens));
        // newTree.Nodes.Add(R(valuable[valuable.Count - 1], valuable, ref watched, ref counter, tokens));

        MainParseTree = newTree;
        return pi;
    }

    ParseTree.Node R(state state, List<state> states, ref List<bool> watched, ref int counter, List<Token> tokens)
    {
        ParseTree.Node newNode = new ParseTree.Node(ntDic[state.GetRule().getLeftPart()]);
        int tail = state.GetRule().getRightPart().Length - 1;

        while (tail >= 0)
        {
            state foundState = new state();

            int iter = 0;

            while (foundState.GetRule().getRightPart() == null && iter < states.Count)
            {
                if (!Grammar.GetSigma().ContainsKey(state.GetRule().getRightPart()[tail]) &&
                    state.GetRule().getRightPart()[tail] == states[iter].GetRule().getLeftPart() &&
                    !watched[iter])
                {
                    foundState = states[iter];
                    watched[iter] = true;
                }

                iter++;

                // iter = states.GetRange(iter + 1, states.Count - iter - 1).IndexOf(state) + iter + 1;
            }

            // int iter = states.IndexOf(state)-1;
            // while (iter < states.Count && foundState.GetRule().getRightPart() == null)
            // {
            //     if (!Grammar.GetSigma().ContainsKey(state.GetRule().getRightPart()[tail]) &&
            //         states[iter].GetRule().getLeftPart() == state.GetRule().getRightPart()[tail])
            //         foundState = states[iter];
            //     iter++;
            // }

            if (foundState.GetRule().getRightPart() != null)
                newNode.child.Add(R(foundState, states, ref watched, ref counter, tokens));
            else
            {
                newNode.child.Add(new ParseTree.Node(tokens[tokens.Count - counter - 1].value));
                counter++;
            }

            tail--;

            if (counter >= tokens.Count) return newNode;
        }

        return newNode;
    }
}