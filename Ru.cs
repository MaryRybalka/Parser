namespace parser;

using static parser.Grammar;
public interface Rule
{
    public nu getLeftPart();
    public nu[] getRightPart();
}