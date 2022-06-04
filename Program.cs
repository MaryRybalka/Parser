namespace parser
{
    internal class Program
    {
        static void Main()
        {
            var lexer = new Lexer();
            var tokens = lexer.Parse("input.swift");
            Console.WriteLine($"tokens.size() {tokens.Count}");
            // foreach (Token token in tokens) Console.WriteLine($"[{token.status}][{token.line}:{token.position}] {token.value}");

            var parser = new Parser();
            parser.Parse(tokens);

            var tree = parser.MainParseTree;

            var semantic = new Semantic(tree);
            semantic.CheckLogic();
        }
    }
}