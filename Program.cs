using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static parser.LexerTypes;
using static parser.Grammar;

namespace parser
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Lexer lexer = new Lexer();
            List<Token> tokens = lexer.Parse("input.swift");
            Console.WriteLine($"tokens.size() {tokens.Count}");
            // foreach (Token token in tokens) Console.WriteLine($"[{token.status}][{token.line}:{token.position}] {token.value}");

            Parser parser = new Parser();
            parser.Parse(tokens);

            var tree = parser.MainParseTree;

            Semantic semantic = new Semantic(tree);
            semantic.checkLogic();
        }
    }
}