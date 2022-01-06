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
        Branching, //ветвление
        ControlSentence, //контрольное предложение
        Sentences, //предложения
        CodeBlock, //код-блок
        BinaryOperator, //бинарный оператор
        BinaryOperatorStart, //бинарный оператор начало
        UnaryOperator,
        DotOperatorStart,
        DotOperatorSymbols,
        DotOperatorSymbol,
        OperatorSymbols, // оператор символы
        OperatorSymbol, // оператор символ
        Operand, //опреанд
        Identifier, //идентификатор
        Literal, //литерал
        FunctionCall, //вызов функции
        ArgumentsList, //список аргументов
        Argument, //аргумент
        Expressions, //вырфжения
        ConstantDefinition, //определение константы
        VariableDefinition, //определение переменной
        TypeAnnotation,
        Definitions, //определения
        InitialisationListPattern, //список инициализации паттерн
        PatternInitialisator, // паттерн инициализатор
        Pattern, //паттерн
        Initialisator, //инициализатор
        ForInCycle,
        WhileCycle,
        Condition, //условие
        TransformationOptional, //преобразование optional
        IfBranching,
        ElseBlock,
        IdentificatorsStart,
        IdentificatorsSymbols, //символы идентификатора
        IdentificatorSymbol, //идентификатор символ
        NumberLiteral, //численный литерал
        StringLiteral,
        BoolLiteral,
        IntNumberLiteral,
        FloatNumberLiteral,
        DecimalNumber,
        DecimalSymbols,
        DecimalSymbol,
        FloatPart,
        Exp, //експонента
        Sign, //знак

        SigmaLetters,
        SigmaNumbers,
        SigmaOpenRound,
        SigmaCloseRound,
        SigmaOpenCurl,
        SigmaCloseCurl,
        SigmaEqual,
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

    private readonly SortedSet<string> _reservedNames = new SortedSet<string>
    {
        "string",
        "substring",
        "character",
        "bool",
        "array",
        "int",
        "double",
        "float",
        "true",
        "false",
        "nil"
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
            {nu.SigmaOr, "|"},
            {nu.SigmaOpenRound, "("},
            {nu.SigmaCloseRound, ")"},
            {nu.SigmaOpenCurl, "{"},
            {nu.SigmaCloseCurl, "}"},
            {nu.SigmaEqual, "="},
            {nu.SigmaSlash, "/"},
            {nu.SigmaStar, "*"},
            {nu.SigmaPlus, "+"},
            {nu.SigmaMinus, "-"},
            {nu.SigmaPercent, "%"},
            {nu.SigmaNot, "!"},
            {nu.SigmaQuest, "?"},
            {nu.SigmaMore, ">"},
            {nu.SigmaLess, "<"},
            {nu.SigmaDot, "."},
            {nu.SigmaDoubleDot, ":"},
            {nu.SigmaComma, ","},
            {nu.SigmaTrue, "true"},
            {nu.SigmaFalse, "false"},
            {nu.SigmaLambda, "lambda"},
            {nu.SigmaBreak, "break"},
            {nu.SigmaContinue, "continue"}
        };

        Rules = new Rule[]
        {
            new Rule(nu.Program, new[] {nu.Sentences}, ruleType.nn),
            new Rule(nu.Sentences, new[] {nu.Sentence}, ruleType.nn),
            new Rule(nu.Sentences, new[] {nu.Sentence, nu.Sentences}, ruleType.nn),
            new Rule(nu.Sentence, new[] {nu.Expressions}, ruleType.nn),
            new Rule(nu.Sentence, new[] {nu.Definitions}, ruleType.nn),
            new Rule(nu.Sentence, new[] {nu.Cycle}, ruleType.nn),
            new Rule(nu.Sentence, new[] {nu.IfBranching}, ruleType.nn),
            new Rule(nu.CodeBlock, new[] {nu.Sentences}, ruleType.nn),
            new Rule(nu.Expression, new[] {nu.Operand, nu.BinaryOperator, nu.Operand}, ruleType.nn),
            new Rule(nu.Expression, new[] {nu.UnaryOperator, nu.Operand}, ruleType.nn),
            new Rule(nu.Expression, new[] {nu.Operand, nu.UnaryOperator}, ruleType.nn),
            new Rule(nu.Operand, new[] {nu.Identifier}, ruleType.nn),
            new Rule(nu.Operand, new[] {nu.Literal}, ruleType.nn),
            new Rule(nu.Operand, new[] {nu.Identifier, nu.FunctionCall}, ruleType.nn),
            new Rule(nu.Operand, new[] {nu.Literal, nu.FunctionCall}, ruleType.nn),
            new Rule(nu.ArgumentsList, new[] {nu.Argument}, ruleType.nn),
            new Rule(nu.Argument, new[] {nu.Expression}, ruleType.nn),
            new Rule(nu.Expressions, new[] {nu.Expression}, ruleType.nn),
            new Rule(nu.Expressions, new[] {nu.Expression, nu.Expressions}, ruleType.nn),
            new Rule(nu.Definitions, new[] {nu.Definition}, ruleType.nn),
            new Rule(nu.Definitions, new[] {nu.Definition, nu.Definitions}, ruleType.nn),
            new Rule(nu.InitialisationListPattern, new[] {nu.PatternInitialisator}, ruleType.nn),
            new Rule(nu.InitialisationListPattern, new[] {nu.PatternInitialisator, nu.InitialisationListPattern},
                ruleType.nn),
            new Rule(nu.PatternInitialisator, new[] {nu.Identifier, nu.Initialisator}, ruleType.nn),
            new Rule(nu.PatternInitialisator, new[] {nu.Identifier, nu.TypeAnnotation, nu.Initialisator}, ruleType.nn),
            new Rule(nu.Condition, new[] {nu.Expression}, ruleType.nn),
            new Rule(nu.Literal, new[] {nu.NumberLiteral}, ruleType.nn),
            new Rule(nu.Literal, new[] {nu.StringLiteral}, ruleType.nn),
            new Rule(nu.Literal, new[] {nu.BoolLiteral}, ruleType.nn),

            new Rule(nu.Program, new[] {nu.SigmaLambda}, ruleType.ns),
            new Rule(nu.FunctionCall, new[] {nu.SigmaOpenRound, nu.SigmaCloseRound}, ruleType.ns),
            new Rule(nu.BinaryOperator, new[] {nu.SigmaSlash}, ruleType.ns),
            new Rule(nu.BinaryOperator, new[] {nu.SigmaEqual}, ruleType.ns),
            new Rule(nu.BinaryOperator, new[] {nu.SigmaMinus}, ruleType.ns),
            new Rule(nu.BinaryOperator, new[] {nu.SigmaPlus}, ruleType.ns),
            new Rule(nu.BinaryOperator, new[] {nu.SigmaStar}, ruleType.ns),
            new Rule(nu.BinaryOperator, new[] {nu.SigmaPercent}, ruleType.ns),
            new Rule(nu.BinaryOperator, new[] {nu.SigmaMore}, ruleType.ns),
            new Rule(nu.BinaryOperator, new[] {nu.SigmaLess}, ruleType.ns),
            new Rule(nu.BinaryOperator, new[] {nu.SigmaAnd, nu.SigmaAnd}, ruleType.ns),
            new Rule(nu.BinaryOperator, new[] {nu.SigmaOr, nu.SigmaOr}, ruleType.ns),
            new Rule(nu.BinaryOperator, new[] {nu.SigmaMore, nu.SigmaEqual}, ruleType.ns),
            new Rule(nu.BinaryOperator, new[] {nu.SigmaLess, nu.SigmaEqual}, ruleType.ns),
            new Rule(nu.BinaryOperator, new[] {nu.SigmaEqual, nu.SigmaEqual}, ruleType.ns),
            new Rule(nu.BinaryOperator, new[] {nu.SigmaNot, nu.SigmaEqual}, ruleType.ns),
            new Rule(nu.BinaryOperator, new[] {nu.SigmaDot}, ruleType.ns),
            new Rule(nu.UnaryOperator, new[] {nu.SigmaNot}, ruleType.ns),
            new Rule(nu.UnaryOperator, new[] {nu.SigmaMinus, nu.SigmaMinus}, ruleType.ns),
            new Rule(nu.UnaryOperator, new[] {nu.SigmaPlus, nu.SigmaPlus}, ruleType.ns),
            new Rule(nu.Sentence, new[] {nu.SigmaBreak}, ruleType.ns),
            new Rule(nu.Sentence, new[] {nu.SigmaContinue}, ruleType.ns),
            new Rule(nu.BoolLiteral, new[] {nu.SigmaTrue}, ruleType.ns),
            new Rule(nu.BoolLiteral, new[] {nu.SigmaFalse}, ruleType.ns),

            new Rule(nu.FunctionCall, new[] {nu.SigmaOpenRound, nu.ArgumentsList, nu.SigmaCloseRound}, ruleType.mix),
            new Rule(nu.ArgumentsList, new[] {nu.Argument, nu.SigmaComma, nu.ArgumentsList}, ruleType.mix),
            new Rule(nu.Argument, new[] {nu.Identifier, nu.SigmaDoubleDot, nu.Expression}, ruleType.mix),
            new Rule(nu.Definition, new[] {nu.SigmaLet, nu.InitialisationListPattern}, ruleType.mix),
            new Rule(nu.Definition, new[] {nu.SigmaVar, nu.InitialisationListPattern}, ruleType.mix),
            new Rule(nu.TypeAnnotation, new[] {nu.SigmaDoubleDot, nu.Identifier}, ruleType.mix),
            new Rule(nu.TypeAnnotation, new[] {nu.SigmaDoubleDot, nu.Identifier, nu.SigmaQuest}, ruleType.mix),
            new Rule(nu.Initialisator, new[] {nu.SigmaEqual, nu.Expression}, ruleType.mix),
            new Rule(nu.Initialisator, new[] {nu.SigmaEqual, nu.Operand}, ruleType.mix),
            new Rule(nu.Cycle, new[] {nu.SigmaFor, nu.Pattern, nu.SigmaIn, nu.Expression, nu.CodeBlock}, ruleType.mix),
            new Rule(nu.Cycle, new[] {nu.SigmaWhile, nu.Condition, nu.CodeBlock}, ruleType.mix),
            new Rule(nu.Condition, new[] {nu.SigmaLet, nu.Pattern, nu.Initialisator}, ruleType.mix),
            new Rule(nu.Condition, new[] {nu.SigmaVar, nu.Pattern, nu.Initialisator}, ruleType.mix),
            new Rule(nu.IfBranching, new[] {nu.SigmaIf, nu.Condition, nu.CodeBlock, nu.ElseBlock}, ruleType.mix),
            new Rule(nu.IfBranching, new[] {nu.SigmaIf, nu.Condition, nu.CodeBlock}, ruleType.mix),
            new Rule(nu.ElseBlock, new[] {nu.SigmaElse, nu.CodeBlock}, ruleType.mix),
            new Rule(nu.ElseBlock, new[] {nu.SigmaElse, nu.IfBranching}, ruleType.mix),
        };
    }

    public SortedSet<string> GetKeywords()
    {
        return _keywords;
    }

    public SortedSet<string> GetReservedNames()
    {
        return _reservedNames;
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