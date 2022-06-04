namespace parser;

public class ParseTree
{
    public struct Node
    {
        public string Name { get; }
        public List<Node> Child { get; }

        public Node(string name = "lambda")
        {
            this.Name = name;
            Child = new List<Node>();
        }
    }

    public List<Node> Nodes { get; set; }

    public ParseTree()
    {
        Nodes = new List<Node>();
    }
}