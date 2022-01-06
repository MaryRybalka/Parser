namespace parser;

using static parser.Grammar;
public class Rule
{
    private int T;
    nu leftPart;

    public Rule(nu left, int t)
    {
        leftPart = left;
        T = t;
    }

    public nu getLeftPart()
    {
        return leftPart;
    }

    public int getType()
    {
        return T;
    }
}