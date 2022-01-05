using Microsoft.VisualBasic;

namespace parser;

using static parser.LexerTypes;

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
    private enum nu
    {
        Program,
        Sentence,
        Expression,
        Definition,
        Cycle,
        Branching,
        ControlSentence,
        Sentences,
        CodeBlock,
        BinaryOperator,
        Operand,
        Identifier,
        Literal,
        FunctionCall,
        ArgumentsList,
        Argument,
        Expressions,
        ConstantDefinition,
        VariableDefinition,
        Definitions,
        InitialisationListPattern,
        PatternInitialisator,
        Pattern,
        Initialisator,
        ForInCycle,
        WhileCycle,
        ConditionsList,
        Condition,
        TransformationOptional,
        IfBranching,
        ElseBlock,
        IdentificatorsStart,
        IdentificatorsSymbols,
        IdentificatorSymbol,
        NumberLiteral,
        StringLiteral,
        BoolLiteral,
        IntNumberLiteral,
        FloatNumberLiteral,
        DecimalNumber,
        DecimalSymbols,
        DecimalSymbol,
        FloatPart,
        Exp,
        Sign
    };

    private readonly string[] sigma =
    {
        "[a-zA-Z]",
        "[0-9]",
        "(",
        ")",
        "{",
        "}",
        "=",
        "/",
        "*",
        "+",
        "-",
        "%",
        "!",
        "?",
        ">",
        "<",
        "."
    };

    private readonly string[] _regexes =
    {
        "\\n",
        "\\s",
        "[a-zA-Z]",
        "[0-9]",
        "\"",
        "\\\\",
        "\\.",
        "\\/",
        "\\*",
        "\\+",
        "\\-",
        "\\%",
        "\\!",
        "\\=",
        "\\(",
        "\\)",
        "\\{",
        "\\}",
        "\\,",
        "\\;",
        "\\&",
        "\\|",
        "\\>",
        "\\<",
        "."
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
}