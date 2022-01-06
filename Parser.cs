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

    Parser()
    {
        ntDic = new Dictionary<nu, string>
        {
            Helper, // вспомогательный нетерминал
            Program, // программа
            Sentence, //предложение
            Expression, //выражение
            Definition, //определение
            Cycle, //цикл
            Sentences, //предложения
            CodeBlock, //код-блок
            BinaryOperator, //бинарный оператор
            UnaryOperator,
            Operand, //опреанд
            Identifier, //идентификатор
            Literal, //литерал
            FunctionCall, //вызов функции
            ArgumentsList, //список аргументов
            Argument, //аргумент
            Expressions, //вырфжения
            TypeAnnotation,
            Definitions, //определения
            InitialisationListPattern, //список инициализации паттерн
            PatternInitialisator, // паттерн инициализатор
            Pattern, //паттерн
            Initialisator, //инициализатор
            Condition, //условие
            IfBranching,
            ElseBlock,

            NumberLiteral, //численный литерал
            StringLiteral,
            BoolLiteral,
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

    string Parse(List<Token> tokenList)
    {
        List<state>[] D = new List<state>[tokenList.Capacity + 1];
        state startState = new state(new Rule(nu.Helper, new[] {Grammar.GetAxioma()}, ruleType.nn), 0);
        D[0].Add(startState);

        foreach (Token token in tokenList)
        {
            bool changed = false;

            if (Scan(ref D, tokenList.IndexOf(token), token, ref changed) < 0)
            {
                Console.WriteLine($"ERROR in {token.line} line]: no such name {token.value}\n");
            }

            if (changed)
            {
                Complete(ref D, tokenList.IndexOf(token));
                Predict(ref D, tokenList.IndexOf(token));
            }
        }

        if (D[tokenList.Capacity].Contains(startState))
        {
            string res = "";
            foreach (var d in D)
            {
                foreach (var state in d)
                {
                    res = res + state.GetRule().getLeftPart().ToString()
                }
            }
        }
        else
        {
        }

        return "ok";
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

                flag = flag || !(Grammar.GetSigma().ContainsKey(state.GetRule().getRightPart()[state.GetMeta()]));
            }

            return (flag) ? -1 : 0;
        }

        return 0;
    }

    void Complete(ref List<state>[] D, int j)
    {
        foreach (var stateJ in D[j])
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
                    }
                }
            }
        }
    }

    void Predict(ref List<state>[] D, int j)
    {
        foreach (var state in D[j])
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
                    }
                }
            }
        }
    }
}