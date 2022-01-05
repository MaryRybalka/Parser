using Microsoft.VisualBasic;

namespace parser;

using static parser.LexerTypes;

enum Precedence : byte
{
    EQUAL = 1,
    OROR = 2,
    ANDAND = 3,
    LESS = 7, MORE = 7, LESSOREQUAL= 7, MOREOREQUAL= 7, TWO_EQUAL = 7, NOTEQUAL = 7,
    PLUS = 10, MINUS = 10,
    STAR = 20, SLASH = 20, PERCENT = 20,
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
            return MaybeBinary(ParseAtom(), 0);
    }
    
    string MaybeBinary(string left, int prec) {
        string tok = is_op();
        if (tok) {
            byte his_prec = (Precedence)tok.value;
            if (his_prec > prec) {
                input.next();
                string right = MaybeBinary(ParseAtom(), his_prec); // (*);
                string binary = "{\n type     : " + tok.value == "=" ? "assign" : "binary" + 
                    ",\n operator :  " + tok.value + 
                    ",\n left     : " + left +
                    ",\n right    : " + right + "}";
                return MaybeBinary(binary, prec);
            }
        }
        return left;
    }
}