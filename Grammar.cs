namespace parser;

public class Grammar
{
    public enum ruleType
    {
        nn,
        ns,
        mix
    }

    public enum nu
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
        Type,
        FuncCodeBlock,

        SigmaOpenRound,
        SigmaCloseRound,
        SigmaOpenCurl,
        SigmaCloseCurl,
        SigmaEqual,
        SigmaDoubleEqual,
        SigmaSlash,
        SigmaStar,
        SigmaPlus,
        SigmaMinus,
        SigmaPercent,
        SigmaNot,
        SigmaQuest,
        SigmaMore,
        SigmaLess,
        SigmaDot,
        SigmaDoubleDot,
        SigmaComma,
        SigmaLambda,
        SigmaAnd,
        SigmaOr,
        SigmaFor,
        SigmaIf,
        SigmaIn,
        SigmaLet,
        SigmaVar,
        SigmaWhile,
        SigmaElse,
        SigmaBreak,
        SigmaContinue,
        SigmaTrue,
        SigmaFalse,
        SigmaString,
        SigmaNumber,
        SigmaIdent,
        SigmaAndAnd,
        SigmaOrOr,
        SigmaMoreEq,
        SigmaLessEq,
        SigmaNotEqual,
        SigmaMinusMinus,
        SigmaPlusPlus,
        SigmaPlusEqual,
        SigmaMinusEqual,
        SigmaStarEqual,
        SigmaSlashEqual,
        SigmaPercentEqual,
        SigmaFunc,
        SigmaArrow,
        SigmaReturn,
        SigmaType,
    };

    private readonly SortedSet<string> _keywords = new SortedSet<string>
    {
        "var",
        "let",
        "for",
        "while",
        "if",
        "in",
        "else"
    };

    private readonly SortedSet<string> _types = new SortedSet<string>
    {
        "String",
        "Character",
        "Bool",
        "Int",
        "Double",
        "Float"
    };

    public struct Rule
    {
        nu leftPart;
        nu[] rightPart;
        ruleType type;

        public Rule(nu left, nu[] right, ruleType _type)
        {
            leftPart = left;
            rightPart = right;
            type = _type;
        }

        public nu getLeftPart()
        {
            return leftPart;
        }

        public nu[] getRightPart()
        {
            return rightPart;
        }

        public ruleType getType()
        {
            return type;
        }
    }

    private nu axioma = nu.Program;
    private Dictionary<nu, string> sigma;
    private Rule[] Rules;

    public Grammar()
    {
        sigma = new Dictionary<nu, string>
        {
            {nu.SigmaFor, "for"},
            {nu.SigmaIf, "if"},
            {nu.SigmaIn, "in"},
            {nu.SigmaLet, "let"},
            {nu.SigmaVar, "var"},
            {nu.SigmaWhile, "while"},
            {nu.SigmaElse, "else"},
            {nu.SigmaAnd, "&"},
            {nu.SigmaAndAnd, "&&"},
            {nu.SigmaOr, "|"},
            {nu.SigmaOrOr, "||"},
            {nu.SigmaOpenRound, "("},
            {nu.SigmaCloseRound, ")"},
            {nu.SigmaOpenCurl, "{"},
            {nu.SigmaCloseCurl, "}"},
            {nu.SigmaEqual, "="},
            {nu.SigmaDoubleEqual, "=="},
            {nu.SigmaSlash, "/"},
            {nu.SigmaStar, "*"},
            {nu.SigmaPlus, "+"},
            {nu.SigmaPlusPlus, "++"},
            {nu.SigmaPlusEqual, "+="},
            {nu.SigmaMinus, "-"},
            {nu.SigmaMinusMinus, "--"},
            {nu.SigmaMinusEqual, "-="},
            {nu.SigmaPercent, "%"},
            {nu.SigmaNot, "!"},
            {nu.SigmaNotEqual, "!="},
            {nu.SigmaStarEqual, "*="},
            {nu.SigmaSlashEqual, "/="},
            {nu.SigmaPercentEqual, "%="},
            {nu.SigmaQuest, "?"},
            {nu.SigmaMore, ">"},
            {nu.SigmaMoreEq, ">="},
            {nu.SigmaLess, "<"},
            {nu.SigmaLessEq, "<="},
            {nu.SigmaDot, "."},
            {nu.SigmaDoubleDot, ":"},
            {nu.SigmaFunc, "func"},
            {nu.SigmaArrow, "->"},
            {nu.SigmaComma, ","},
            {nu.SigmaTrue, "true"},
            {nu.SigmaFalse, "false"},
            {nu.SigmaLambda, "lambda"},
            {nu.SigmaBreak, "break"},
            {nu.SigmaContinue, "continue"},
            {nu.SigmaReturn, "return"},
            {nu.SigmaString, "STRING_END"},
            {nu.SigmaNumber, "NUMBER"},
            {nu.SigmaIdent, "IDENT"},
            {nu.SigmaType, "TYPE"},
        };

        Rules = new Rule[]
        {
            new Rule(nu.Helper, new[] {nu.Program}, ruleType.nn),
            new Rule(nu.Program, new[] {nu.Sentences}, ruleType.nn),
            new Rule(nu.Sentences, new[] {nu.Sentence}, ruleType.nn),
            new Rule(nu.Sentences, new[] {nu.Sentence, nu.Sentences}, ruleType.nn),
            new Rule(nu.Sentence, new[] {nu.Expressions}, ruleType.nn),
            new Rule(nu.Sentence, new[] {nu.Definitions}, ruleType.nn),
            new Rule(nu.Sentence, new[] {nu.Cycle}, ruleType.nn),
            new Rule(nu.Sentence, new[] {nu.IfBranching}, ruleType.nn),
            new Rule(nu.Expression, new[] {nu.Operand, nu.BinaryOperator, nu.Operand}, ruleType.nn),
            new Rule(nu.Expression, new[] {nu.Operand, nu.BinaryOperator, nu.Expression}, ruleType.nn),
            new Rule(nu.Expression, new[] {nu.UnaryOperator, nu.Operand}, ruleType.nn),
            new Rule(nu.Expression, new[] {nu.UnaryOperator, nu.Expression}, ruleType.nn),
            new Rule(nu.Expression, new[] {nu.Operand, nu.UnaryOperator}, ruleType.nn),
            new Rule(nu.Expression, new[] {nu.Operand, nu.FunctionCall}, ruleType.nn),
            new Rule(nu.Operand, new[] {nu.Identifier}, ruleType.nn),
            new Rule(nu.Operand, new[] {nu.Literal}, ruleType.nn),
            new Rule(nu.ArgumentsList, new[] {nu.Argument}, ruleType.nn),
            new Rule(nu.Argument, new[] {nu.Expression}, ruleType.nn),
            new Rule(nu.Expressions, new[] {nu.Expression}, ruleType.nn),
            new Rule(nu.Expressions, new[] {nu.Expression, nu.Expressions}, ruleType.nn),
            new Rule(nu.Definitions, new[] {nu.Definition}, ruleType.nn),
            // new Rule(nu.Definitions, new[] {nu.Definition, nu.Definitions}, ruleType.nn),
            new Rule(nu.InitialisationListPattern, new[] {nu.PatternInitialisator}, ruleType.nn),
            new Rule(nu.PatternInitialisator, new[] {nu.Identifier, nu.Initialisator}, ruleType.nn),
            new Rule(nu.PatternInitialisator, new[] {nu.Identifier, nu.TypeAnnotation, nu.Initialisator}, ruleType.nn),
            new Rule(nu.Condition, new[] {nu.Expression}, ruleType.nn),
            new Rule(nu.Literal, new[] {nu.NumberLiteral}, ruleType.nn),
            new Rule(nu.Literal, new[] {nu.StringLiteral}, ruleType.nn),
            new Rule(nu.Literal, new[] {nu.BoolLiteral}, ruleType.nn),
            new Rule(nu.TypeAnnotation, new[] {nu.Type}, ruleType.nn),
            new Rule(nu.Argument, new[] {nu.Identifier, nu.TypeAnnotation}, ruleType.nn),
            new Rule(nu.Argument, new[] {nu.Identifier}, ruleType.nn),

            // new Rule(nu.Program, new[] {nu.SigmaLambda}, ruleType.ns),
            // new Rule(nu.Sentences, new[] {nu.SigmaLambda}, ruleType.ns),
            new Rule(nu.FunctionCall, new[] {nu.SigmaOpenRound, nu.SigmaCloseRound}, ruleType.ns),
            new Rule(nu.BinaryOperator, new[] {nu.SigmaSlash}, ruleType.ns),
            new Rule(nu.BinaryOperator, new[] {nu.SigmaEqual}, ruleType.ns),
            new Rule(nu.BinaryOperator, new[] {nu.SigmaMinus}, ruleType.ns),
            new Rule(nu.BinaryOperator, new[] {nu.SigmaPlus}, ruleType.ns),
            new Rule(nu.BinaryOperator, new[] {nu.SigmaStar}, ruleType.ns),
            new Rule(nu.BinaryOperator, new[] {nu.SigmaPercent}, ruleType.ns),
            new Rule(nu.BinaryOperator, new[] {nu.SigmaMore}, ruleType.ns),
            new Rule(nu.BinaryOperator, new[] {nu.SigmaLess}, ruleType.ns),
            new Rule(nu.BinaryOperator, new[] {nu.SigmaAndAnd}, ruleType.ns),
            new Rule(nu.BinaryOperator, new[] {nu.SigmaOrOr}, ruleType.ns),
            new Rule(nu.BinaryOperator, new[] {nu.SigmaMoreEq}, ruleType.ns),
            new Rule(nu.BinaryOperator, new[] {nu.SigmaLessEq}, ruleType.ns),
            new Rule(nu.BinaryOperator, new[] {nu.SigmaDoubleEqual}, ruleType.ns),
            new Rule(nu.BinaryOperator, new[] {nu.SigmaNotEqual}, ruleType.ns),
            new Rule(nu.BinaryOperator, new[] {nu.SigmaDot}, ruleType.ns),
            new Rule(nu.BinaryOperator, new[] {nu.SigmaPlusEqual}, ruleType.ns),
            new Rule(nu.BinaryOperator, new[] {nu.SigmaMinusEqual}, ruleType.ns),
            new Rule(nu.BinaryOperator, new[] {nu.SigmaStarEqual}, ruleType.ns),
            new Rule(nu.BinaryOperator, new[] {nu.SigmaSlashEqual}, ruleType.ns),
            new Rule(nu.BinaryOperator, new[] {nu.SigmaPercentEqual}, ruleType.ns),
            new Rule(nu.UnaryOperator, new[] {nu.SigmaNot}, ruleType.ns),
            new Rule(nu.UnaryOperator, new[] {nu.SigmaMinusMinus}, ruleType.ns),
            new Rule(nu.UnaryOperator, new[] {nu.SigmaPlusPlus}, ruleType.ns),
            new Rule(nu.Sentence, new[] {nu.SigmaBreak}, ruleType.ns),
            new Rule(nu.Sentence, new[] {nu.SigmaContinue}, ruleType.ns),
            new Rule(nu.BoolLiteral, new[] {nu.SigmaTrue}, ruleType.ns),
            new Rule(nu.BoolLiteral, new[] {nu.SigmaFalse}, ruleType.ns),
            new Rule(nu.Identifier, new[] {nu.SigmaIdent}, ruleType.ns),
            new Rule(nu.NumberLiteral, new[] {nu.SigmaNumber}, ruleType.ns),
            new Rule(nu.StringLiteral, new[] {nu.SigmaString}, ruleType.ns),
            new Rule(nu.Type, new[] {nu.SigmaType}, ruleType.ns),
            new Rule(nu.CodeBlock, new[] {nu.SigmaOpenCurl, nu.SigmaCloseCurl}, ruleType.ns),

            new Rule(nu.FunctionCall, new[] {nu.SigmaOpenRound, nu.ArgumentsList, nu.SigmaCloseRound}, ruleType.mix),
            new Rule(nu.ArgumentsList, new[] {nu.Argument, nu.SigmaComma, nu.ArgumentsList}, ruleType.mix),
            new Rule(nu.Definition, new[] {nu.SigmaLet, nu.InitialisationListPattern}, ruleType.mix),
            new Rule(nu.Definition, new[] {nu.SigmaVar, nu.InitialisationListPattern}, ruleType.mix),
            new Rule(nu.Definition,
                new[]
                {
                    nu.SigmaFunc, nu.Identifier, nu.FunctionCall, nu.SigmaMinus, nu.SigmaMore, nu.TypeAnnotation,
                    nu.FuncCodeBlock
                }, ruleType.mix),
            new Rule(nu.Definition,
                new[]
                {
                    nu.SigmaFunc, nu.Identifier, nu.FunctionCall, nu.SigmaMinus, nu.SigmaMore, nu.TypeAnnotation,
                    nu.CodeBlock
                }, ruleType.mix),
            new Rule(nu.Definition, new[] {nu.SigmaFunc, nu.Identifier, nu.FunctionCall, nu.CodeBlock}, ruleType.mix),
            new Rule(nu.InitialisationListPattern,
                new[] {nu.PatternInitialisator, nu.SigmaComma, nu.InitialisationListPattern}, ruleType.mix),
            new Rule(nu.TypeAnnotation, new[] {nu.SigmaDoubleDot, nu.Type}, ruleType.mix),
            new Rule(nu.TypeAnnotation, new[] {nu.SigmaDoubleDot, nu.Type, nu.SigmaQuest}, ruleType.mix),
            new Rule(nu.Initialisator, new[] {nu.SigmaEqual, nu.Expression}, ruleType.mix),
            new Rule(nu.Initialisator, new[] {nu.SigmaEqual, nu.Operand}, ruleType.mix),
            new Rule(nu.CodeBlock, new[] {nu.SigmaOpenCurl, nu.Sentences, nu.SigmaCloseCurl}, ruleType.mix),
            new Rule(nu.FuncCodeBlock,
                new[] {nu.SigmaOpenCurl, nu.Sentences, nu.SigmaReturn, nu.Identifier, nu.SigmaCloseCurl}, ruleType.mix),
            new Rule(nu.Cycle, new[] {nu.SigmaFor, nu.Identifier, nu.SigmaIn, nu.Operand, nu.CodeBlock},
                ruleType.mix),
            new Rule(nu.Cycle, new[] {nu.SigmaWhile, nu.Condition, nu.CodeBlock}, ruleType.mix),
            new Rule(nu.Condition, new[] {nu.SigmaLet, nu.Identifier, nu.Initialisator}, ruleType.mix),
            new Rule(nu.Condition, new[] {nu.SigmaVar, nu.Identifier, nu.Initialisator}, ruleType.mix),
            new Rule(nu.IfBranching, new[] {nu.SigmaIf, nu.Condition, nu.CodeBlock, nu.ElseBlock}, ruleType.mix),
            new Rule(nu.IfBranching, new[] {nu.SigmaIf, nu.Condition, nu.CodeBlock}, ruleType.mix),
            new Rule(nu.ElseBlock, new[] {nu.SigmaElse, nu.CodeBlock}, ruleType.mix),
            new Rule(nu.ElseBlock, new[] {nu.SigmaElse, nu.IfBranching}, ruleType.mix),
            new Rule(nu.Expression,
                new[] {nu.Operand, nu.BinaryOperator, nu.SigmaOpenRound, nu.Expression, nu.SigmaCloseRound},
                ruleType.mix),
            new Rule(nu.Expression, new[] {nu.UnaryOperator, nu.SigmaOpenRound, nu.Expression, nu.SigmaCloseRound},
                ruleType.mix),
            new Rule(nu.Expression, new[] {nu.UnaryOperator, nu.SigmaOpenRound, nu.Operand, nu.SigmaCloseRound},
                ruleType.mix),
        };
    }

    public SortedSet<string> GetKeywords()
    {
        return _keywords;
    }

    public SortedSet<string> GetTypes()
    {
        return _types;
    }

    public Rule[] GetRules()
    {
        return Rules;
    }

    public nu GetAxioma()
    {
        return axioma;
    }

    public Dictionary<nu, string> GetSigma()
    {
        return sigma;
    }
}