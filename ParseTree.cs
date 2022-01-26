namespace parser;

public class ParseTree
{
    public struct Node
    {
        public string name { get; set; }
        public List<Node> child { get; set; }

        public Node(string name = "lambda")
        {
            this.name = name;
            child = new List<Node>();
        }
    }

    public int numOfLevels { get; set; }
    public List<Node> Nodes { get; set; }

    public ParseTree()
    {
        Nodes = new List<Node>();
        numOfLevels = 0;
    }

    public bool FindNode(int lvl, string name, Node node, ref Node res)
    {
        Console.WriteLine("node - " + node.name);
        bool bolRes = false;

        // if (node.child.Count == 0) return false;
        foreach (var child in node.child)
        {
            bolRes = bolRes || FindNode(lvl, name, child, ref res);
        }

        if (res.name == null)
        {
            res = new Node();
            res.child = new List<Node>();
        }

        return bolRes;
    }

    public void FindChildNode(int lvl, string name, List<Node> childs, ref Node node)
    {
        Console.WriteLine("node - " + node.name);

        for (var i = 0; i < node.child.Count; i++)
        {
            Node child = node.child[i];
            FindChildNode(lvl, name, childs, ref child);
        }

        // if (res.name == null)
        // {
        //     res = new Node();
        //     res.child = new List<Node>();
        // }
        return;
    }
}