using Microsoft.VisualBasic;

namespace parser;

using static parser.LexerTypes;

enum Precedence : byte
{
        EQ = 1,
        OR = 2,
        AND = 3,
        LESS = 7, MORE = 7, Less_AND_EQ= 7, MORE_AND_EQ= 7, EQ_EQ = 7, NOT_EQ = 7,
        PLUS = 10, MINUS = 10,
        MUL = 20, DIV = 20, MOD = 20,
}
    
public class Parser
{
    
    string ParseToplevel(List<Token> tokens)
    {
        List<string> prog = new List<string>();
        foreach (Token token in tokens)
        {
            prog.Add(ParseExpression());
        }

        return "{\n type: \"prog\",\n prog: " + prog + "}";
    }

    string ParseFunction()
    {
        return "{\n type: \"function\",\n vars: " +
               ParseVarnames() +
               "\",\n body: \" " +
               ParseBody() +
               "\"}";
    }

    string ParseIf()
    {
        string cond = ParseExpression();
        if (!is_punc("{")) skip_kw("then");
        string then = ParseExpression();
        string ret = "{\n type: \"if\",\n cond: " + cond +
                     "\",\n then: \" " + then + "\"}";
        if (is_kw("else"))
        {
            string _else = ParseExpression();
            ret = ret + ",\n else: " + _else;
        }

        return ret;
    }

    string ParseAtom()
    {
        if (is_punc("("))
        {
            var exp = ParseExpression();
            skip_punc(")");
            return exp;
        }

        if (is_punc("{")) return ParseProg();
        if (is_kw("if")) return ParseIf();
        if (is_kw("true") || is_kw("false")) return ParseBool();
        if (is_kw("func") || is_kw("λ"))
        {
            return ParseFunction();
        }

        var tok = input.next();
        if (tok.type == "var" || tok.type == "num" || tok.type == "str")
            return tok;
        unexpected();
    }
    
    string ParseExpression() {
            return MaybeBinary(parse_atom(), 0);
    }
    
    function maybe_binary(left, my_prec) {
        var tok = is_op();
        if (tok) {
            var his_prec = PRECEDENCE[tok.value];
            if (his_prec > my_prec) {
                input.next();
                var right = maybe_binary(parse_atom(), his_prec) // (*);
                var binary = {
                    type     : tok.value == "=" ? "assign" : "binary",
                    operator : tok.value,
                    left     : left,
                    right    : right
                };
                return maybe_binary(binary, my_prec);
            }
        }
        return left;
    }
}