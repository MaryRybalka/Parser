namespace parser;

public class Semantic
{
    public bool checkLogic(ParseTree tree)
    {
        return searchIdent() && blockVar();
    }

    public bool searchIdent()
    {
        return false;
    }

    public bool blockVar()
    {
        return false;
    }
}