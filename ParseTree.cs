namespace parser;

public class ParseTree
{
    public struct Node
    {
        public int level { get; set; }
        public string name { get; set; }
        public string parentName { get; set; }
        public List<Node> child { get; set; }

        public Node(int level = 0, string name = "lambda", string parentName = "Helper")
        {
            this.level = level;
            this.name = name;
            this.parentName = parentName;
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
        if (lvl == node.level && name == node.name)
        {
            res = node;
            // res.child = node.child;
            return true;
        }

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

        if (lvl == node.level && name == node.name)
        {
            node.child = childs;
            return;
        }

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