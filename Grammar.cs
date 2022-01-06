namespace parser;

public class Grammar
{
    private enum nu
    {
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

    struct nnRule
    {
        public nu leftPart;
        public nu[] rightPart;

        public nnRule(nu left, nu[] right)
        {
            this.leftPart = left;
            this.rightPart = right;
        }
    }

    struct nsRule
    {
        public nu leftPart;
        public string[] rightPart;

        public nsRule(nu left, string[] right)
        {
            this.leftPart = left;
            this.rightPart = right;
        }
    }

    struct mixRule
    {
        public nu leftPart;
        public nu[] rightPart;

        public mixRule(nu left, nu[] right)
        {
            this.leftPart = left;
            this.rightPart = right;
        }
    }

    private nu axiom = nu.Program;

    private Dictionary<nu, string> sigma;

    public Grammar()
    {
        sigma = new Dictionary<nu, string>
        {
            {nu.SigmaLetters, "[a-zA-Z]"},
            {nu.SigmaNumbers, "[0-9]"},
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

        nnRule[] nnRules =
        {
            new nnRule(nu.Program, new[] {nu.Sentences}),

            new nnRule(nu.Sentences, new[] {nu.Sentence}),
            new nnRule(nu.Sentences, new[] {nu.Sentence, nu.Sentences}),

            new nnRule(nu.Sentence, new[] {nu.Expressions}),
            new nnRule(nu.Sentence, new[] {nu.Definitions}),
            new nnRule(nu.Sentence, new[] {nu.Cycle}),
            new nnRule(nu.Sentence, new[] {nu.IfBranching}),

            new nnRule(nu.CodeBlock, new[] {nu.Sentences}),

            new nnRule(nu.Expression, new[] {nu.Operand, nu.BinaryOperator, nu.Operand}),
            new nnRule(nu.Expression, new[] {nu.UnaryOperator, nu.Operand}),
            new nnRule(nu.Expression, new[] {nu.Operand, nu.UnaryOperator}),
            new nnRule(nu.Operand, new[] {nu.Identifier}),
            new nnRule(nu.Operand, new[] {nu.Literal}),
            new nnRule(nu.Operand, new[] {nu.Identifier, nu.FunctionCall}),
            new nnRule(nu.Operand, new[] {nu.Literal, nu.FunctionCall}),

            new nnRule(nu.ArgumentsList, new[] {nu.Argument}),
            new nnRule(nu.Argument, new[] {nu.Expression}),
            new nnRule(nu.Expressions, new[] {nu.Expression}),
            new nnRule(nu.Expressions, new[] {nu.Expression, nu.Expressions}),

            // new nnRule(nu.BinaryOperator, new[] {nu.BinaryOperatorStart, nu.OperatorSymbols}),
            // new nnRule(nu.BinaryOperator, new[] {nu.DotOperatorStart, nu.DotOperatorSymbols}),
            //
            // new nnRule(nu.DotOperatorSymbols, new[] {nu.DotOperatorSymbol, nu.DotOperatorSymbols}),

            new nnRule(nu.Definitions, new[] {nu.Definition}),
            new nnRule(nu.Definitions, new[] {nu.Definition, nu.Definitions}),

            new nnRule(nu.InitialisationListPattern, new[] {nu.PatternInitialisator}),
            new nnRule(nu.InitialisationListPattern, new[] {nu.PatternInitialisator, nu.InitialisationListPattern}),

            new nnRule(nu.PatternInitialisator, new[] {nu.Identifier, nu.Initialisator}),
            new nnRule(nu.PatternInitialisator, new[] {nu.Identifier, nu.TypeAnnotation, nu.Initialisator}),

            // new nnRule(nu.Cycle, new[] {nu.ForInCycle}),
            // new nnRule(nu.Cycle, new[] {nu.WhileCycle}),

            new nnRule(nu.Condition, new[] {nu.Expression}),
            // new nnRule(nu.Condition, new[] {nu.TransformationOptional}),

            // new nnRule(nu.Identifier, new[] {nu.IdentificatorsStart}),
            // new nnRule(nu.Identifier, new[] {nu.IdentificatorsStart, nu.IdentificatorsSymbols}),
            // new nnRule(nu.IdentificatorSymbol, new[] {nu.IdentificatorsStart}),
            // new nnRule(nu.IdentificatorsSymbols, new[] {nu.IdentificatorsStart}),
        };
        nsRule[] nsRules =
        {
            new nsRule(nu.Program, new[] {sigma[nu.SigmaLambda]}),

            new nsRule(nu.FunctionCall, new[] {sigma[nu.SigmaOpenRound] + sigma[nu.SigmaCloseRound]}),

            new nsRule(nu.BinaryOperator, new[] {sigma[nu.SigmaSlash]}),
            new nsRule(nu.BinaryOperator, new[] {sigma[nu.SigmaEqual]}),
            new nsRule(nu.BinaryOperator, new[] {sigma[nu.SigmaMinus]}),
            new nsRule(nu.BinaryOperator, new[] {sigma[nu.SigmaPlus]}),
            new nsRule(nu.BinaryOperator, new[] {sigma[nu.SigmaStar]}),
            new nsRule(nu.BinaryOperator, new[] {sigma[nu.SigmaPercent]}),
            new nsRule(nu.BinaryOperator, new[] {sigma[nu.SigmaMore]}),
            new nsRule(nu.BinaryOperator, new[] {sigma[nu.SigmaLess]}),
            new nsRule(nu.BinaryOperator, new[] {sigma[nu.SigmaAnd] + sigma[nu.SigmaAnd]}),
            new nsRule(nu.BinaryOperator, new[] {sigma[nu.SigmaOr] + sigma[nu.SigmaOr]}),
            new nsRule(nu.BinaryOperator, new[] {sigma[nu.SigmaMore] + sigma[nu.SigmaEqual]}),
            new nsRule(nu.BinaryOperator, new[] {sigma[nu.SigmaLess] + sigma[nu.SigmaEqual]}),
            new nsRule(nu.BinaryOperator, new[] {sigma[nu.SigmaEqual] + sigma[nu.SigmaEqual]}),
            new nsRule(nu.BinaryOperator, new[] {sigma[nu.SigmaNot] + sigma[nu.SigmaEqual]}),
            new nsRule(nu.BinaryOperator, new[] {sigma[nu.SigmaDot]}),

            new nsRule(nu.UnaryOperator, new[] {sigma[nu.SigmaNot]}),
            new nsRule(nu.UnaryOperator, new[] {sigma[nu.SigmaMinus] + sigma[nu.SigmaMinus]}),
            new nsRule(nu.UnaryOperator, new[] {sigma[nu.SigmaPlus] + sigma[nu.SigmaPlus]}),

            // new nsRule(nu.BinaryOperator, new[] {sigma[nu.SigmaLambda]}),

            // new nsRule(nu.OperatorSymbol, new[] {sigma[nu.SigmaSlash]}),
            // new nsRule(nu.OperatorSymbol, new[] {sigma[nu.SigmaEqual]}),
            // new nsRule(nu.OperatorSymbol, new[] {sigma[nu.SigmaMinus]}),
            // new nsRule(nu.OperatorSymbol, new[] {sigma[nu.SigmaPlus]}),
            // new nsRule(nu.OperatorSymbol, new[] {sigma[nu.SigmaNot]}),
            // new nsRule(nu.OperatorSymbol, new[] {sigma[nu.SigmaStar]}),
            // new nsRule(nu.OperatorSymbol, new[] {sigma[nu.SigmaPercent]}),
            // new nsRule(nu.OperatorSymbol, new[] {sigma[nu.SigmaMore]}),
            // new nsRule(nu.OperatorSymbol, new[] {sigma[nu.SigmaLess]}),
            // new nsRule(nu.OperatorSymbol, new[] {sigma[nu.SigmaAnd] + sigma[nu.SigmaAnd]}),
            // new nsRule(nu.OperatorSymbol, new[] {sigma[nu.SigmaOr] + sigma[nu.SigmaOr]}),
            // new nsRule(nu.OperatorSymbol, new[] {sigma[nu.SigmaMore] + sigma[nu.SigmaEqual]}),
            // new nsRule(nu.OperatorSymbol, new[] {sigma[nu.SigmaLess] + sigma[nu.SigmaEqual]}),
            // new nsRule(nu.OperatorSymbol, new[] {sigma[nu.SigmaEqual] + sigma[nu.SigmaEqual]}),
            // new nsRule(nu.OperatorSymbol, new[] {sigma[nu.SigmaNot] + sigma[nu.SigmaEqual]}),
            // new nsRule(nu.OperatorSymbol, new[] {sigma[nu.SigmaDot]}),

            // new nsRule(nu.BinaryOperatorStart, new[] {sigma[nu.SigmaSlash]}),
            // new nsRule(nu.BinaryOperatorStart, new[] {sigma[nu.SigmaEqual]}),
            // new nsRule(nu.BinaryOperatorStart, new[] {sigma[nu.SigmaMinus]}),
            // new nsRule(nu.BinaryOperatorStart, new[] {sigma[nu.SigmaPlus]}),
            // new nsRule(nu.BinaryOperatorStart, new[] {sigma[nu.SigmaNot]}),
            // new nsRule(nu.BinaryOperatorStart, new[] {sigma[nu.SigmaStar]}),
            // new nsRule(nu.BinaryOperatorStart, new[] {sigma[nu.SigmaPercent]}),
            // new nsRule(nu.BinaryOperatorStart, new[] {sigma[nu.SigmaMore]}),
            // new nsRule(nu.BinaryOperatorStart, new[] {sigma[nu.SigmaLess]}),
            // new nsRule(nu.BinaryOperatorStart, new[] {sigma[nu.SigmaAnd]}),
            // new nsRule(nu.BinaryOperatorStart, new[] {sigma[nu.SigmaOr]}),
            //
            // new nsRule(nu.DotOperatorStart, new[] {sigma[nu.SigmaDot]}),
            // new nsRule(nu.DotOperatorSymbol, new[] {sigma[nu.SigmaDot]}),
            // new nsRule(nu.DotOperatorSymbols, new[] {sigma[nu.SigmaLambda]}),

            new nsRule(nu.Sentence, new[] {sigma[nu.SigmaBreak]}),
            new nsRule(nu.Sentence, new[] {sigma[nu.SigmaContinue]}),

            new nsRule(nu.BoolLiteral, new[] {sigma[nu.SigmaTrue]}),
            new nsRule(nu.BoolLiteral, new[] {sigma[nu.SigmaFalse]}),
        };
        mixRule[] mixRules =
        {
            new mixRule(nu.FunctionCall, new[] {nu.SigmaOpenRound, nu.ArgumentsList, nu.SigmaCloseRound}),
            new mixRule(nu.ArgumentsList, new[] {nu.Argument, nu.SigmaComma, nu.ArgumentsList}),
            new mixRule(nu.Argument, new[] {nu.Identifier, nu.SigmaDoubleDot, nu.Expression}),

            new mixRule(nu.Definition, new[] {nu.SigmaLet, nu.InitialisationListPattern}),
            new mixRule(nu.Definition, new[] {nu.SigmaVar, nu.InitialisationListPattern}),

            new mixRule(nu.TypeAnnotation, new[] {nu.SigmaDoubleDot, nu.Identifier}),
            new mixRule(nu.TypeAnnotation, new[] {nu.SigmaDoubleDot, nu.Identifier, nu.SigmaQuest}),

            new mixRule(nu.Initialisator, new[] {nu.SigmaEqual, nu.Expression}),
            new mixRule(nu.Initialisator, new[] {nu.SigmaEqual, nu.Operand}),

            new mixRule(nu.Cycle, new[] {nu.SigmaFor, nu.Pattern, nu.SigmaIn, nu.Expression, nu.CodeBlock}),
            new mixRule(nu.Cycle, new[] {nu.SigmaWhile, nu.Condition, nu.CodeBlock}),

            new mixRule(nu.Condition, new[] {nu.SigmaLet, nu.Pattern, nu.Initialisator}),
            new mixRule(nu.Condition, new[] {nu.SigmaVar, nu.Pattern, nu.Initialisator}),

            new mixRule(nu.IfBranching, new[] {nu.SigmaIf, nu.Condition, nu.CodeBlock, nu.ElseBlock}),
            new mixRule(nu.IfBranching, new[] {nu.SigmaIf, nu.Condition, nu.CodeBlock}),
            new mixRule(nu.ElseBlock, new[] {nu.SigmaElse, nu.CodeBlock}),
            new mixRule(nu.ElseBlock, new[] {nu.SigmaElse, nu.IfBranching}),
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
}