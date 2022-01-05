namespace parser
{
    public class LexerTypes
    {
        public const int a = 46;
        public const int b = 25;
        
        public struct Token
        {
            public string status;
            public string value;
            public int line;
            public int position;
        };
        
        public enum Status
        {
            START = 0,
            IDENT,
            NUMBER,
            REALNUMBER_ONE,
            REALNUMBER_DOT,
            STRING,
            STRING_END,
            STRING_SLASH,
            SLASH,
            ONE_LC,
            MULTI_LC,
            MULTI_LC_PRE_END,
            MULTI_LC_END,
            STAR,
            PLUS,
            PLUSPLUS,
            MINUS,
            MINUSMINUS,
            PERCENT,
            NOT,
            EQUAL,
            TWO_EQUAL,
            THREE_EQUAL,
            SLASHEQUAL,
            STAREQUAL,
            PLUSEQUAL,
            MINUSEQUAL,
            PERCENTEQUAL,
            NOTEQUAL,
            NOT_TWO_EQUAL,
            OPEN_ROUND_BRACKET,
            CLOSE_ROUND_BRACKET,
            OPEN_CURLY_BRACKET,
            CLOSE_CURLY_BRACKET,
            COMMA,
            SEMICOLON,
            AND,
            ANDEQUAL,
            ANDAND,
            OR,
            OREQUAL,
            OROR,
            MORE,
            MOREOREQUAL,
            LESS,
            LESSOREQUAL,
            ERROR
        };
        
        public struct State
        {
            private Status status;
            private int action;

            public State(Status stat, int act)
            {
                status = stat;
                action = act;
            }
            
            public Status GetStatus()
            {
                return status;
            }
            public int GetAction()
            {
                return action;
            }
        };
    }
}