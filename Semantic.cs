namespace parser;

public class Semantic
{
    struct Let
    {
        public int LetId { get; }
        public string Name { get; }

        public Let(int varId, string name)
        {
            LetId = varId;
            Name = name;
        }
    }

    private ParseTree _tree;
    private List<string> _vars;
    private List<string> _funcs;
    private List<string> _fors;
    private List<string> _whiles;
    private List<string> _ifs;
    private List<Let> _lets;

    public Semantic(ParseTree tree)
    {
        _tree = tree;
        _vars = new List<string>();
        _funcs = new List<string>();
        _fors = new List<string>();
        _whiles = new List<string>();
        _ifs = new List<string>();
        _lets = new List<Let>();
    }

    public bool notFound(int sit)
    {
        return sit != 1 && sit != 2 && sit != 3 && sit != 4 && sit != 5 && sit != 6;
    }

    public bool CheckLogic()
    {
        return RunThrough();
    }

    bool RunThrough()
    {
        bool res = false;
        if (_tree.Nodes.Count > 0)
        {
            ParseTree.Node cur = _tree.Nodes[0];
            int envId = 0;
            res = res || CheckChild(cur, envId);
        }

        if (res)
            Console.WriteLine("Everything fine");

        return res;
    }

    bool CheckChild(ParseTree.Node cur, int id)
    {
        bool result = true;
        int i = cur.Child.Count - 1;
        bool envLvl = false;
        while (i >= 0)
        {
            if (cur.Child[i].Name == "var")
            {
                string ident = findIdn(cur.Child[i - 1].Child[0].Child[1]);
                int sit = SearchInEnv(ident, id);
                if (notFound(sit))
                    _vars.Add(ident);
                else
                {
                    Console.WriteLine("Name repeating is not acceptable");
                    return false;
                }

                i = -1;
            }
            else if (cur.Child[i].Name == "let")
            {
                string ident = findIdn(cur.Child[i - 1].Child[0].Child[1]);
                int sit = SearchInEnv(ident, id);
                if (notFound(sit))
                    _lets.Add(new Let(id, ident));
                else
                {
                    Console.WriteLine("Name repeating is not acceptable");
                    return false;
                }

                i = -1;
            }
            else if (cur.Child[i].Name == "func")
            {
                string ident = findIdn(cur.Child[i - 1]);
                int sit = SearchInEnv(ident, id);
                if (notFound(sit))
                    _funcs.Add(ident);
                else
                {
                    Console.WriteLine("Name repeating is not acceptable");
                    return false;
                }

                i = -1;
            }
            else if (cur.Child[i].Name == "for")
            {
                string ident = findIdn(cur.Child[i - 1]);
                int sit = SearchInEnv(ident, id);
                if (notFound(sit))
                    _fors.Add(ident);
                else
                {
                    Console.WriteLine("Name repeating is not acceptable");
                    return false;
                }

                i = -1;
            }
            // else if (cur.Child[i].Name == "if")
            // {
            //     string ident = findIdn(cur.Child[i - 1]);
            //     int sit = SearchInEnv(ident, id);
            //     if (notFound(sit))
            //         _ifs.Add(ident);
            //     else
            //     {
            //         Console.WriteLine("Name repeating is not acceptable");
            //         return false;
            //     }
            //
            //     i = -1;
            // }
            // else if (cur.Child[i].Name == "while")
            // {
            //     string ident = findIdn(cur.Child[i - 1]);
            //     int sit = SearchInEnv(ident, id);
            //     if (notFound(sit))
            //         _whiles.Add(ident);
            //     else
            //     {
            //         Console.WriteLine("Name repeating is not acceptable");
            //         return false;
            //     }
            //
            //     i = -1;
            // }
            else
            {
                if (cur.Child[i].Name == "Identifier")
                {
                    int sit = SearchInEnv(cur.Child[i].Child[0].Name, id);

                    if (sit == 1 && cur.Name == "Operand" && i == cur.Child.Count - 1)
                    {
                        Console.WriteLine("Reassign of const [" + cur.Child[i].Child[0].Name + "] is not acceptable");
                        return false;
                    }

                    if (sit < 0)
                    {
                        Console.WriteLine(
                            "[" + id + "]" + "Variable [" + cur.Child[i].Child[0].Name + "] is not define");
                        return false;
                    }
                }
                else if (cur.Child[i].Name == "Program" || cur.Child[i].Name == "CodeBlock")
                {
                    envLvl = true;
                    id++;
                }

                result = result && CheckChild(cur.Child[i], id);
                i = (result) ? i - 1 : -1;
            }
        }

        if (envLvl)
        {
            for (int j = 0; j < _lets.Count; j++)
            {
                if (_lets[j].LetId >= id)
                    _lets.Remove(_lets[j]);
            }
        }

        return result;
    }

    string findIdn(ParseTree.Node cur)
    {
        string res = "";
        if (cur.Name == "Identifier")
        {
            return cur.Child[0].Name;
        }
        else
        {
            return cur.Name;
        }

        int i = cur.Child.Count - 1;
        while (i >= 0)
        {
            res = findIdn(cur.Child[i]);
            i = (res != "") ? -1 : i - 1;
        }

        return res;
    }

    int SearchInEnv(string name, int envId)
    {
        int res = -1;
        foreach (var varEl in _vars)
        {
            if (varEl == name)
                res = 1;
        }

        foreach (var funcEl in _funcs)
        {
            if (funcEl == name)
                res = 3;
        }

        foreach (var forEl in _fors)
        {
            if (forEl == name)
                res = 4;
        }

        foreach (var whileEl in _whiles)
        {
            if (whileEl == name)
                res = 5;
        }

        foreach (var ifEl in _ifs)
        {
            if (ifEl == name)
                res = 6;
        }

        if (res < 0)
        {
            foreach (var letEl in _lets)
            {
                if (letEl.Name == name && letEl.LetId <= envId)
                    res = 2;
            }
        }

        return res;
    }
}