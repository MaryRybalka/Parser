namespace parser;

public class Semantic
{
    struct Let
    {
        public int letId { get; set; }
        public string name { get; set; }

        public Let(int _varId, string _name)
        {
            letId = _varId;
            name = _name;
        }
    }

    private ParseTree tree;
    private Grammar grammar;
    private List<string> vars;
    private List<Let> lets;

    public Semantic(ParseTree _tree)
    {
        grammar = new Grammar();
        tree = _tree;
        vars = new List<string>();
        lets = new List<Let>();
    }

    public bool checkLogic()
    {
        // return searchIdent() && blockVar();
        return runThrough();
    }

    bool runThrough()
    {
        bool res = false;
        if (tree.Nodes.Count > 0)
        {
            ParseTree.Node cur = tree.Nodes[0];
            int envId = 0;
            res = res || checkChild(cur, envId);
        }

        if (res)
            Console.WriteLine("Everything fine");

        return res;
    }

    bool checkChild(ParseTree.Node cur, int id)
    {
        bool result = true;
        int i = cur.child.Count - 1;
        bool envLvl = false;
        while (i >= 0)
        {
            if (cur.child[i].name == "var")
            {
                string ident = findIdn(cur.child[i - 1]);
                int sit = searchInEnv(ident, id);
                if (sit != 2)
                    vars.Add(ident);
                else
                {
                    Console.WriteLine("Name repeating is not acceptable");
                    return false;
                }

                i = -1;
            }
            else if (cur.child[i].name == "let")
            {
                string ident = findIdn(cur.child[i - 1]);
                int sit = searchInEnv(ident, id);
                if (sit != 1)
                    lets.Add(new Let(id, ident));
                else
                {
                    Console.WriteLine("Name repeating is not acceptable");
                    return false;
                }

                i = -1;
            }
            else
            {
                if (cur.child[i].name == "Identifier")
                {
                    int sit = searchInEnv(cur.child[i].child[0].name, id);
                    if (sit == 1 && cur.name == "Operand" && i == cur.child.Count - 1)
                    {
                        Console.WriteLine("Reassign of const [" + cur.child[i].child[0].name + "] is not acceptable");
                        return false;
                    }
                    else if (sit < 0)
                    {
                        Console.WriteLine("Variable [" + cur.child[i].child[0].name + "] is not define");
                        return false;
                    }
                }
                else if (cur.child[i].name == "Program" || cur.child[i].name == "CodeBlock")
                {
                    envLvl = true;
                    id++;
                }

                result = result && checkChild(cur.child[i], id);
                i = (result) ? i - 1 : -1;
            }
        }

        if (envLvl)
        {
            for (int j = 0; j < lets.Count; j++)
            {
                if (lets[j].letId >= id)
                    lets.Remove(lets[j]);
            }
        }

        return result;
    }

    string findIdn(ParseTree.Node cur)
    {
        string res = "";
        if (cur.name == "Identifier")
        {
            return cur.child[0].name;
        }

        int i = cur.child.Count - 1;
        while (i >= 0)
        {
            res = findIdn(cur.child[i]);
            i = (res != "") ? -1 : i - 1;
        }

        return res;
    }

    int searchInEnv(string name, int envId)
    {
        int res = -1;
        foreach (var varEl in vars)
        {
            if (varEl == name)
                res = 1;
        }

        if (res < 0)
        {
            foreach (var letEl in lets)
            {
                if (letEl.name == name && letEl.letId <= envId)
                    res = 2;
            }
        }

        return res;
    }

    bool blockVar()
    {
        return false;
    }
}