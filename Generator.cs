using System.Linq.Expressions;

namespace parser;

public class Generator
{
    private string _dstname;
    private string _srcname;
    private string _bodyText;
    private List<List<LexerTypes.Token>> _tokens;
    private Optimizator _optimizator;

    private readonly List<string> _keywordsInSwift = new()
    {
        "var",
        "let",
        "func",
    };

    private Dictionary<string, string> _keywordsInCpp = new()
    {
        {"var", "const auto"},
        {"let", "auto"},
        {"func", "function"},
    };

    private List<string> _addsInCpp = new()
    {
        {"for"},
        {"while"},
        {"if"},
    };

    public Generator(List<LexerTypes.Token> tokens)
    {
        _dstname = "result.cpp";
        _srcname = "input.swift";
        _bodyText = "";
        _optimizator = new Optimizator();

        _tokens = new List<List<LexerTypes.Token>>();
        int curLine = 0;
        var tmpTokensList = new List<LexerTypes.Token>();
        foreach (var token in tokens)
        {
            if (curLine != token.line - 1)
            {
                _tokens.Add(tmpTokensList);
                tmpTokensList = new List<LexerTypes.Token>();
            }

            tmpTokensList.Add(token);
            curLine = token.line - 1;
        }

        _tokens.Add(tmpTokensList);
    }

    bool GenerateBodyBak(ParseTree.Node curNode)
    {
        if (curNode.Child != null && curNode.Child.Count > 0)
        {
            foreach (var child in curNode.Child)
            {
                GenerateBodyBak(child);
            }

            return false;
        }
        else
        {
            var withSpace = curNode.Name + ' ';
            if (_keywordsInSwift.Contains(curNode.Name))
                _bodyText = '\n' + withSpace + _bodyText;
            else
                _bodyText = (curNode.Name == "}" || curNode.Name == "{")
                    ? '\n' + curNode.Name + '\n' + _bodyText
                    : withSpace + _bodyText;
            return true;
        }
    }

    bool GenerateBody(StreamReader stream)
    {
        string prevLine = "";
        int lineCounter = 0;
        int numberOfTabs = 1;

        while (!stream.EndOfStream)
        {
            string? tmp = stream.ReadLine();
            tmp += '\n';
            bool needSemicolon = true;

            if (prevLine != tmp)
            {
                if (tmp == "\n")
                    _bodyText += '\n';
                else
                {
                    _bodyText += string.Concat(Enumerable.Repeat("  ", numberOfTabs));
                    var roundWasOpen = false;
                    if (_addsInCpp.Contains(_tokens[lineCounter][0].value))
                    {
                        _bodyText += _tokens[lineCounter][0].value + " ( ";
                        roundWasOpen = true;
                        if (_tokens[lineCounter][0].value == "for")
                        {
                            needSemicolon = false;
                            numberOfTabs++;
                            _bodyText += "auto ";

                            string ident = "";
                            var i = 1;
                            while (i < _tokens[lineCounter].Count)
                            {
                                if (_tokens[lineCounter][i].value == "in" &&
                                    _tokens[lineCounter][i + 1].value.Contains("..."))
                                {
                                    var values = _tokens[lineCounter][i + 1].value.Split("...");
                                    _bodyText += "=" + values[0] + "; " +
                                                 ident + "<=" + values[1] + "; " +
                                                 ident + "++ ) {";
                                    i = _tokens[lineCounter].Count;
                                }
                                else if (_tokens[lineCounter][i].value == "in")
                                {
                                    _bodyText += ": " + _tokens[lineCounter][i + 1].value + " ) " +
                                                 _tokens[lineCounter][i + 2].value + ' ';
                                    i = _tokens[lineCounter].Count;
                                }
                                else
                                {
                                    ident = _tokens[lineCounter][i].value;
                                    _bodyText += _tokens[lineCounter][i].value;
                                }

                                i++;
                            }
                        }
                        else
                            for (var i = 1; i < _tokens[lineCounter].Count; i++)
                            {
                                if (_tokens[lineCounter][i].value == "{" && roundWasOpen)
                                {
                                    needSemicolon = false;
                                    numberOfTabs++;
                                    roundWasOpen = false;
                                    _bodyText += ") " + _tokens[lineCounter][i].value + ' ';
                                }
                                else
                                {
                                    _bodyText += _tokens[lineCounter][i].value + ' ';
                                }
                            }
                    }
                    else
                    {
                        if (_keywordsInSwift.Contains(_tokens[lineCounter][0].value))
                            _bodyText += _keywordsInCpp[_tokens[lineCounter][0].value] + ' ';
                        else
                        {
                            if (_tokens[lineCounter][0].value == "}")
                            {
                                needSemicolon = false;
                                numberOfTabs--;
                                _bodyText = _bodyText.Substring(0, _bodyText.Length - 2 * numberOfTabs);
                                _bodyText += _tokens[lineCounter][0].value;
                            }
                            else
                                _bodyText += _tokens[lineCounter][0].value + ' ';
                        }

                        for (var i = 1; i < _tokens[lineCounter].Count; i++)
                        {
                            if (_tokens[lineCounter][i].value == "{")
                            {
                                needSemicolon = false;
                                numberOfTabs++;
                                _bodyText += _tokens[lineCounter][i].value;
                            }
                            else
                                _bodyText += _tokens[lineCounter][i].value + ' ';
                        }
                    }

                    _bodyText += needSemicolon ? ";\n" : '\n';
                }
            }

            prevLine = tmp;
            if (tmp != "\n")
                lineCounter++;
        }

        return true;
    }

    void GenerateStartMain(StreamWriter s)
    {
        s.WriteLine("int main() {");
    }

    void GenerateEndMain(StreamWriter s)
    {
        s.WriteLine("return 0;");
        s.WriteLine("}");
    }

    public void Generate()
    {
        string dirName = AppDomain.CurrentDomain.BaseDirectory;
        FileInfo fileInfo = new FileInfo(dirName);
        DirectoryInfo parentDir = fileInfo.Directory.Parent;
        string parentDstDirName = parentDir!.FullName.Remove(parentDir!.FullName.Length - 9, 9) + _dstname;
        string parentSrcDirName = parentDir!.FullName.Remove(parentDir!.FullName.Length - 9, 9) + _srcname;

        StreamWriter sw = new StreamWriter(parentDstDirName);
        StreamReader sr = new StreamReader(parentSrcDirName);

        GenerateStartMain(sw);
        GenerateBody(sr);
        sw.WriteLine(_bodyText);
        GenerateEndMain(sw);

        sw.Close();
        sr.Close();
    }
}