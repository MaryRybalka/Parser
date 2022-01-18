namespace parser;

public class ParseTree
{
    public struct Node
    {
        public int level { get; set; }
        public string name { get; set; }
        public List<Node> child { get; set; }

        public Node(int level = 0, string name = "underfound")
        {
            this.level = level;
            this.name = name;
            child = new List<Node>();
        }
    }

    public List<Node> Nodes { get; set; }

    public ParseTree()
    {
        Nodes = new List<Node>();
    }
}