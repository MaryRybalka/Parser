namespace parser;

public class Semantic
{
    struct Env
    {
        private int id { get; set; }
        private List<string> lets { get; set; }

        public Env(int _id)
        {
            id = _id;
            lets = new List<string>();
        }

        void addLet(string let)
        {
            lets.Add(let);
        }
    }

    private ParseTree tree;
    private Grammar grammar;
    private List<Env> envs;
    private List<string> vars;

    public Semantic(ParseTree _tree)
    {
        grammar = new Grammar();
        tree = _tree;
        envs = new List<Env>();
        vars = new List<string>();
    }

    public bool checkLogic()
    {
        return searchIdent() && blockVar();
    }

    bool searchIdent()
    {
        bool res = false;
        ParseTree.Node cur = tree.Nodes[0];
        res = res && checkChild(cur);

        return false;
    }

    bool checkChild(ParseTree.Node cur)
    {
        int i = 0;
        while (i < cur.child.Count)
        {
            if (cur.child[i].name == "var")
            {
                vars.Add(cur.child[i + 1].name);
            }
            else if (cur.child[i].name == "let")
            {
            }
            else
            {
            }
        }

        return true;
    }

    bool blockVar()
    {
        return false;
    }
}